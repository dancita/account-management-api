using AccountManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountManagementAPI.Services
{
    public class ManagementService : IManagementService
    {
        private readonly AppDbContext _appDbContext;

        public ManagementService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<PhoneNumber?> AssignAccount(PhoneNumber phoneNumber, int accountId)
        {
            try
            {
                phoneNumber.AccountId = accountId;
                _appDbContext.Entry(phoneNumber).State = EntityState.Modified;
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return null;
            }

            return phoneNumber;
        }

        public async Task<Account?> ToggleAccountStatus(Account account)
        {
            try
            {
                account.IsActive = !account.IsActive;
                _appDbContext.Entry(account).State = EntityState.Modified;
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return null;
            }

            return account;
        }

        public async Task<Account?> GetAccount(int? accountId)
        {
            return await _appDbContext.Accounts.FindAsync(accountId);
        }

        public async Task<PhoneNumber?> GetPhoneNumber(int? phoneNumberId)
        {
            return await _appDbContext.PhoneNumbers.FindAsync(phoneNumberId);
        }

        public async Task<Account?> AddAccount(Account account)
        {
            try
            {
                _appDbContext.Accounts.Add(account);
                await _appDbContext.SaveChangesAsync();
                return await GetAccount(account.Id);
            }
            catch (Exception)
            {
                return null;
            }           
        }

        public async Task<bool> DeletePhoneNumber(PhoneNumber phoneNumber)
        {
            try
            {
                _appDbContext.PhoneNumbers.Remove(phoneNumber);
                await _appDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Account>> GetAccounts()
        {
            return await _appDbContext.Accounts.ToListAsync();
        }

        public async Task<List<PhoneNumber>> GetPhoneNumbersByAccountId(int accountId)
        {
            return await _appDbContext.PhoneNumbers.Where(x => x.AccountId == accountId).ToListAsync();
        }
    }
}
