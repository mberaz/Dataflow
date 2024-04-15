using System.Threading.Tasks.Dataflow;

namespace TPLDataFlowDemo
{
    public class ValidatorsFactory : IValidatorsFactory
    {
        private readonly IBlockedNameService _blockedNameService;

        public ValidatorsFactory(IBlockedNameService blockedNameService)
        {
            _blockedNameService = blockedNameService;
        }

        public TransformBlock<string, (string name, bool isValid)> CreateNameValidator()
        {
            return new TransformBlock<string, (string name, bool isValid)>(name
                => (name, !string.IsNullOrWhiteSpace(name)));
        }

        public TransformBlock<(string name, bool isValid), (string name, bool isValid)> CreateBlockedNamesValidator()
        {
            return new TransformBlock<(string name, bool isValid), (string name, bool isValid)>(async input =>
            {
                if (input.isValid)
                {
                    var (isBlocked, _) = await _blockedNameService.IsBlocked(input.name);
                    return (input.name, !isBlocked);
                }

                return input;
            });
        }
    }
}
