using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MockDoor.Data.Models;

public class QueryParameter
{
    [Key]
    public int Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [MaxLength(64)]
    [Column(TypeName = "varchar(64)")]
    public string Name { get; set; }

    [Required(AllowEmptyStrings = true)]
    [MaxLength(1000)]
    [Column(TypeName = "varchar(1000)")]
    public string Value { get; set; }

    public int OrderIndex { get; set; }

    public int ServiceRequestId { get; set; }

    public ServiceRequest ServiceRequest { get; set; }
}