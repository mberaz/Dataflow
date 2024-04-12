using System.Threading.Tasks.Dataflow;

namespace TPLDataFlowDemo;

public interface IValidatorsFactory
{
    TransformBlock<string, (string name, bool isValid)> CreateNameValidator();

    TransformBlock<(string name, bool isValid), (string name, bool isValid)> CreateBlockedNamesValidator();
}