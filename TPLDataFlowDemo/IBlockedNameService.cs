namespace TPLDataFlowDemo;

public interface IBlockedNameService
{
    Task<(bool isBlocked, string reason)> IsBlocked(string name);
}