using AccountManagementAPI.Exceptions;
using AccountManagementAPI.Models;
using AccountManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhoneNumberController : ControllerBase
    {
        private readonly IManagementService _managementService;

        public PhoneNumberController(IManagementService managementService)
        {
            _managementService = managementService;
        }

        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<IEnumerable<PhoneNumber>>> GetPhoneNumbersByAccountId(int accountId)
        {
            var phoneNumbers = await _managementService.GetPhoneNumbersByAccountId(accountId);

            if (phoneNumbers.Count == 0)
            {
                throw new NotFoundException($"No phone numbers were found by account id : {accountId}");
            }

            return Ok(phoneNumbers);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeletePhoneNumber(int id)
        {
            var phoneNumber = await _managementService.GetPhoneNumber(id);

            if (phoneNumber == null)
            {
                throw new NotFoundException($"Phone number wasn't found for account id : {id}");
            }

            var successfulDelete = _managementService.DeletePhoneNumber(phoneNumber).Result;

            if (!successfulDelete)
            {
                throw new AccountManagementAPIException($"An error occured while trying to delete phone number with id : {phoneNumber.Id}");
            }

            return NoContent();
        }

        [HttpPut("id")]
        public async Task<IActionResult> AssignPhoneNumberToAccount(int id, [FromBody]PhoneNumber phoneNumber)
        {
            if (id != phoneNumber.Id)
            {
                throw new BadRequestException($"Id given in parameter : {id} must match id given in body: {phoneNumber.Id}");
            }

            var account = await _managementService.GetAccount(phoneNumber.AccountId);
            var phoneNumberToAssignTo = await _managementService.GetPhoneNumber(phoneNumber.Id);

            if (account == null)
            {
                throw new NotFoundException($"Account with id : {phoneNumber.AccountId} wasn't found");
            }
            
            if (phoneNumberToAssignTo == null)
            {
                throw new NotFoundException($"Phone number with id : {phoneNumber.Id} wasn't found");
            }

            if (!account.IsActive)
            {
                throw new AccountManagementAPIException($"Account with id : {id} is inactive");
            }

            var updatedPhoneNumber = await _managementService.AssignAccount(phoneNumberToAssignTo, account.Id);

            if (updatedPhoneNumber == null)
            {
                throw new AccountManagementAPIException($"An error occured while trying to assign account id : {account.Id} to phone number with id  : {phoneNumber.Id}");
            }
            
            return NoContent();
        }
    }
}
