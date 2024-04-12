using System.Threading.Tasks.Dataflow;

namespace TPLDataFlowDemo
{
    public interface INamesProcessor
    {
        Task<string> ProcessName(string name);
    }
    public class NamesProcessor : INamesProcessor
    {

        //https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/walkthrough-creating-a-dataflow-pipeline
        //https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library
        private readonly IValidatorsFactory _validatorsFactory;
        private readonly INamesDb _namesDb;

        public NamesProcessor(IValidatorsFactory validatorsFactory, INamesDb namesDb)
        {
            _validatorsFactory = validatorsFactory;
            _namesDb = namesDb;
        }

        public async Task<string> ProcessName(string name)
        {
            string _nameId = null;
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var nameValidator = _validatorsFactory.CreateNameValidator();

            var blockedNamesValidator = _validatorsFactory.CreateBlockedNamesValidator();
            var errorMessageBlock = CreateSendErrorMessageBlock();
            var saveNameBlock = new TransformBlock<(string name, bool isValid), string>(async input =>
            {
                await _namesDb.SaveName(input.name);
                return input.name;
            });

            var emailOnSaveBlock = new ActionBlock<string>(input =>
            {
                Console.WriteLine($"The name {input} was saved");
                _nameId = Guid.NewGuid().ToString();
            });

            //nameValidator.LinkTo(blockedNamesValidator, linkOptions,
            //                                            predicate: result => result.isValid);

            //nameValidator.LinkTo(errorMessageBlock, linkOptions,
            //    predicate: result => !result.isValid);

            nameValidator.LinkWithPredicate(linkOptions, blockedNamesValidator,
                errorMessageBlock, result => result.isValid);

            blockedNamesValidator.LinkWithPredicate(linkOptions, saveNameBlock,
                errorMessageBlock, result => result.isValid);

            saveNameBlock.LinkTo(emailOnSaveBlock, linkOptions);


            nameValidator.Post(name);
            nameValidator.Complete();
            await emailOnSaveBlock.Completion;

            return _nameId;
        }

        private ActionBlock<(string name, bool isValid)> CreateSendErrorMessageBlock()
        {
            return new ActionBlock<(string name, bool isValid)>(input =>
              {
                  //send an email in real-life
                  Console.WriteLine($"The name {input.name} is not valid!");
              });
        }


    }
}
