namespace MinimalApi.Interfaces;

public interface ITokenService
{
    Task<bool> ConsumeTokenAsync(string username);
    Task<int> GetBalanceAsync(string username);
    Task PurchaseTokensAsync(string username, int amount);
}
