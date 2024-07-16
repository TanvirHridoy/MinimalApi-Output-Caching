using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinimalApi.DTO;

public class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int EmployeeId { get; set; }

    [Required]
    [StringLength(250)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public string Address { get; set; }

    [Required]
    [StringLength(100)]
    public string Designation { get; set; }


}
