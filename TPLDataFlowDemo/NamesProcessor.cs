using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TPLDataFlowDemo
{
    public interface INamesProcessor
    {
        Task ProcessName(string name);
    }
    public class NamesProcessor : INamesProcessor
    {
        //https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/walkthrough-creating-a-dataflow-pipeline
        //https://learn.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library
        private readonly IValidatorFactory _validatorFactory;

        public NamesProcessor(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory;
        }

        public async Task ProcessName(string name)
        {
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            var nameValidator = _validatorFactory.CreateNameValidator();

            var blockedNamesValidator = _validatorFactory.CreateBlockedNamesValidator();
            var errorMessageBlock = CreateSendErrorMessageBlock();



            nameValidator.LinkTo(blockedNamesValidator, linkOptions,
                                                        predicate: result => result.isValid);

            nameValidator.LinkTo(errorMessageBlock, linkOptions,
                predicate: result => !result.isValid);
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
