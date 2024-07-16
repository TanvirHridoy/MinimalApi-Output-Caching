using Microsoft.EntityFrameworkCore;
using MinimalApi.DTO;
using MinimalApi.Interfaces;

namespace MinimalApi.Services;


public class TokenService : ITokenService
{
    private readonly EmployeeDbContext _context;

    public TokenService(EmployeeDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ConsumeTokenAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null || user.TokenBalance <= 0)
            return false;

        user.TokenBalance--;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetBalanceAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        return user?.TokenBalance ?? 0;
    }

    public async Task PurchaseTokensAsync(string username, int amount)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        if (user == null)
            throw new ArgumentException("User not found", nameof(username));

        user.TokenBalance += amount;
        await _context.SaveChangesAsync();
    }
}
