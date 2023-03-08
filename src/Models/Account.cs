using System.ComponentModel.DataAnnotations;

namespace AccountManagementAPI.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public string? AccountHolderName { get; set; }
        public bool IsActive { get; set; }
    }
}
