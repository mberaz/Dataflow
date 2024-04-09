using System.Threading.Tasks.Dataflow;

namespace TPLDataFlowDemo;

public interface IValidatorFactory
{
    TransformBlock<string, (string name, bool isValid)> CreateNameValidator();

    TransformBlock<(string name, bool isValid), (string name, bool isValid)> CreateBlockedNamesValidator();
}