using AccountManagementAPI.Exceptions;
using AccountManagementAPI.Models;
using AccountManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccountManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IManagementService _managementService;

        public AccountController(IManagementService managementService)
        {
            _managementService = managementService;
        }

        /// <summary>
        /// Retrieves all accounts
        /// </summary>
        /// <returns>A list of all accounts</returns>
        /// <exception cref="NotFoundException"></exception>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccounts()
        {
            var accounts = await _managementService.GetAccounts();

            if (accounts == null) 
            {
                throw new NotFoundException($"No accounts were found");
            }

            return Ok(accounts);
        }

        /// <summary>
        /// Adds a new account to the database
        /// </summary>
        /// <param name="account">The account to add</param>
        /// <returns>The newly added account</returns>
        /// <exception cref="AccountManagementAPIException"></exception>
        [HttpPost]
        public async Task<ActionResult<Account>> AddAccount(Account account)
        {
            var dbAccount = await _managementService.AddAccount(account);

            if (dbAccount == null)
            {
                throw new AccountManagementAPIException($"An error occured while trying to add an account");
            }

            return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, account);
        }

        /// <summary>
        /// Retrieves a specific by id
        /// </summary>
        /// <param name="id">The id of the account to retrieve</param>
        /// <returns>The retrieved account by the specified id</returns>
        /// <exception cref="AccountManagementAPIException"></exception>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            var account = await _managementService.GetAccount(id);

            if (account == null)
            {
                throw new AccountManagementAPIException($"An error occured while trying to get an account by id : {id}");
            }

            return Ok(account);
        }

        /// <summary>
        /// Toggles the state of an existing account (between active and suspended)
        /// </summary>
        /// <param name="id">The id of the account to toggle the status</param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="AccountManagementAPIException"></exception>
        [HttpPut("{id}")]
        public async Task<IActionResult> ToggleAccountState(int id)
        {
            var account = await _managementService.GetAccount(id);

            if (account == null)
            {
                throw new NotFoundException($"Account wasn't found with id : {id}");
            }

            var updatedAccount = await _managementService.ToggleAccountStatus(account);

            if (updatedAccount == null)
            {
                throw new AccountManagementAPIException($"An error occured while toggling the account status");
            }

            return NoContent();
        }
    }
}
