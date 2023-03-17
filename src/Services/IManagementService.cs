using AccountManagementAPI.Models;

namespace AccountManagementAPI.Services
{
    public interface IManagementService
    {
        Task<Account?> ToggleAccountStatus(Account account);
        Task<Account?> GetAccount(int? accountId);    
        Task<Account?> AddAccount(Account account);
        Task<List<Account>> GetAccounts();
        Task<List<PhoneNumber>> GetPhoneNumbersByAccountId(int accountId);

        Task<PhoneNumber?> GetPhoneNumber(int? phoneNumberId);
        Task<bool> DeletePhoneNumber(PhoneNumber phoneNumber);
        Task<PhoneNumber?> AssignAccount(PhoneNumber phoneNumber, int accountId);

    }
}
