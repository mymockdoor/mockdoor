using System.ComponentModel.DataAnnotations;

namespace MockDoor.Data.Models.Headers
{
    public abstract class BaseHeader
    {
        [Key]
        // ReSharper disable once InconsistentNaming
        public int ID { get; set; }

        [MaxLength(150)]
        public string Name { get; set; }
    }
}
