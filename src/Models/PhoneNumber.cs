using System.ComponentModel.DataAnnotations;

namespace AccountManagementAPI.Models
{
    public class PhoneNumber
    {
        /// <summary>
        /// Id of the phone number
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The phone number itself
        /// </summary>
        public string? Number { get; set; }

        /// <summary>
        /// The id of the account assigned to the phone number
        /// </summary>
        public int? AccountId { get; set; }
    }
}
