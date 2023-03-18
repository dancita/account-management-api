using System.ComponentModel.DataAnnotations;

namespace AccountManagementAPI.Models
{
    public class Account
    {
        /// <summary>
        /// Id of the account
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Name of the account holder
        /// </summary>
        public string? AccountHolderName { get; set; }

        /// <summary>
        /// Status of the account
        /// </summary>
        public bool IsActive { get; set; }
    }
}
