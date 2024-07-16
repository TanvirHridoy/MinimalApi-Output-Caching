namespace MinimalApi.Repository;
using Microsoft.EntityFrameworkCore;
using MinimalApi.DTO;
public class EmployeeRepository
{
    private readonly EmployeeDbContext _context;

    public EmployeeRepository(EmployeeDbContext context)
    {
        _context = context;
    }

    // Add a new employee
    public async Task<Employee> AddEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    // Get an employee by ID
    public async Task<Employee> GetEmployeeById(int id)
    {
        return await _context.Employees.FindAsync(id);
    }

    // Get all employees
    public async Task<IEnumerable<Employee>> GetAllEmployees()
    {
        return await _context.Employees.ToListAsync();
    }

    // Update an existing employee
    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        _context.Entry(employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return employee;
    }

    // Delete an employee
    public async Task DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
    }
}
