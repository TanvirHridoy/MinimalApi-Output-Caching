using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MinimalApi.DTO;

public class Religion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }  // Primary key
    [Required]
    [StringLength(250)]
    public string Name { get; set; }
}
