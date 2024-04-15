using System.Threading.Tasks.Dataflow;

namespace TPLDataFlowDemo
{
    public interface INamesProcessor
    {
        Task<int> ProcessName(string name);
    }
    public class NamesProcessor : INamesProcessor
    {
        //https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/walkthrough-creating-a-dataflow-pipeline
        //https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library

        DataflowLinkOptions linkOptions = new() { PropagateCompletion = true };
        private readonly IValidatorsFactory _validatorsFactory;
        private readonly INamesDb _namesDb;

        public NamesProcessor(IValidatorsFactory validatorsFactory, INamesDb namesDb)
        {
            _validatorsFactory = validatorsFactory;
            _namesDb = namesDb;
        }

        public async Task<int> ProcessName(string name)
        {
            var nameValidator = _validatorsFactory.CreateNameValidator();
            var blockedNamesValidator = _validatorsFactory.CreateBlockedNamesValidator();
            var errorMessageBlock = CreateSendErrorMessageBlock();
            var saveNameBlock = new TransformBlock<(string name, bool isValid),
                                                    (string name, int nameId)>(async input =>
            {
                await _namesDb.SaveName(input.name);
                return (input.name, new Random().Next(100, 99999));
            });

            var emailOnSaveBlock = new TransformBlock<(string name, int nameId), int>(input =>
            {
                Console.WriteLine($"The name {input.name} was saved");
                return input.nameId;
            });
 
            nameValidator.LinkTo(blockedNamesValidator, linkOptions);

            blockedNamesValidator.LinkWithPredicate(linkOptions, saveNameBlock,
                errorMessageBlock, result => result.isValid);

            saveNameBlock.LinkTo(emailOnSaveBlock, linkOptions);

            nameValidator.Post(name);
            nameValidator.Complete();

            try
            {
                blockedNamesValidator.Completion.Wait();
                errorMessageBlock.Completion.Wait();
            }
            catch (AggregateException ex)
            {
                throw new Exception(ex.InnerException.Message);
            }

            return await emailOnSaveBlock.ReceiveAsync();
        }

        private ActionBlock<(string name, bool isValid)> CreateSendErrorMessageBlock()
        {
            return new ActionBlock<(string name, bool isValid)>(input =>
              {
                  //send an email in real-life
                  Console.WriteLine($"The name {input.name} is not valid!");
                  throw new Exception($"The name {input.name} is not valid!");
              });
        }


    }
}
