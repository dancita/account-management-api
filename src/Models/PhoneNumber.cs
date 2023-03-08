using System.ComponentModel.DataAnnotations;

namespace AccountManagementAPI.Models
{
    public class PhoneNumber
    {
        [Key]
        public int Id { get; set; }
        public string? Number { get; set; }
        public int? AccountId { get; set; }
    }
}
