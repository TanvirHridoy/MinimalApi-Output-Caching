using Microsoft.EntityFrameworkCore;
using MinimalApi.DTO;

namespace MinimalApi.Repository;

public class ReligionRepository
{
    private readonly EmployeeDbContext _context;

    public ReligionRepository(EmployeeDbContext context)
    {
        _context = context;
    }

    // Add a new Religion
    public async Task<Religion> Add(Religion Religion)
    {
        _context.Religions.Add(Religion);
        await _context.SaveChangesAsync();
        return Religion;
    }

    // Get an Religion by ID
    public async Task<Religion?> GetById(int id)
    {
        return await _context.Religions.FindAsync(id);
    }

    // Get all Religions
    public async Task<IEnumerable<Religion>> GetAll()
    {
        return await _context.Religions.ToListAsync();
    }

    // Update an existing Religion
    public async Task<Religion> Update(Religion Religion)
    {
        _context.Entry(Religion).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Religion;
    }

    // Delete an Religion
    public async Task Delete(int id)
    {
        var Religion = await _context.Religions.FindAsync(id);
        if (Religion != null)
        {
            _context.Religions.Remove(Religion);
            await _context.SaveChangesAsync();
        }
    }
}

