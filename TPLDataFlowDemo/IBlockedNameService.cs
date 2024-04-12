namespace TPLDataFlowDemo;

public interface IBlockedNameService
{
    Task<(bool isBlocked, string reason)> IsBlocked(string name);
}

public class BlockedNameService : IBlockedNameService
{
    private readonly List<string> _blockedNames = ["hitler", "fuck", "bad"];

    public Task<(bool isBlocked, string reason)> IsBlocked(string name)
    {
        var result = _blockedNames.Contains(name) ?
              (true, $"The name {name} is in the blocked list") : (false, null);

        return Task.FromResult(result);
    }
}
