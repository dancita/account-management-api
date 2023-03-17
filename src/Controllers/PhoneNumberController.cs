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

        /// <summary>
        /// Retrieves specific phone numbers by the account id they assigned to
        /// </summary>
        /// <param name="accountId">The assigned account id of the phone numbers to retrieve</param>
        /// <returns>A list of the phone numbers assigned to a specific account id</returns>
        /// <exception cref="NotFoundException"></exception>
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

        /// <summary>
        /// Deletes a phone number from the database
        /// </summary>
        /// <param name="id">The id of the phonenumber to delete</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="AccountManagementAPIException"></exception>
        [HttpDelete("{id}")]
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

        /// <summary>
        /// Updates an existing phone number in the database with a new assinged account
        /// </summary>
        /// <param name="id">The id of the phone number to update</param>
        /// <param name="phoneNumber">The updated phone number</param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="AccountManagementAPIException"></exception>
        [HttpPut("{id}")]
        public async Task<IActionResult> AssignPhoneNumberToAccount(int id, [FromBody]PhoneNumber phoneNumber)
        {
            if (id != phoneNumber.Id)
            {
                throw new BadRequestException($"Id given in parameter : {id} must match id given in body: {phoneNumber.Id}");
            }

            var account = await _managementService.GetAccount(phoneNumber.AccountId);

            if (account == null)
            {
                throw new NotFoundException($"Account with id : {phoneNumber.AccountId} wasn't found");
            }

            if (!account.IsActive)
            {
                throw new AccountManagementAPIException($"Account with id : {phoneNumber.AccountId} is inactive");
            }

            var updatedPhoneNumber = await _managementService.AssignAccount(phoneNumber, account.Id);

            if (updatedPhoneNumber == null)
            {
                throw new AccountManagementAPIException($"An error occured while trying to assign account id : {account.Id} to phone number with id : {phoneNumber.Id}");
            }
            
            return NoContent();
        }
    }
}
