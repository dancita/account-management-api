using AccountManagementAPI.Controllers;
using AccountManagementAPI.Exceptions;
using AccountManagementAPI.Models;
using AccountManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;

namespace AccountManagementAPITests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IManagementService> _managementService = new();

        [Fact]
        public async Task GetAccounts_ThereIsOneAccountAtLeast_MustReturnOk()
        {
            // Arrange
            var testAccount = new List<Account>()
                {
                    new Account()
                    {
                        AccountHolderName = "Test Name",
                        Id = 1,
                        IsActive = true
                    }
            };
            _managementService.Setup(x => x.GetAccounts())
                .ReturnsAsync(testAccount);

            var accountController = new AccountController(_managementService.Object);

            // Act
            var result = await accountController.GetAccounts();

            // Assert
            var actionResult = Assert
                .IsType<ActionResult<IEnumerable<Account>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var accounts = Assert.IsAssignableFrom<IEnumerable<Account>>
                (okObjectResult.Value);
            Assert.Single(accounts);
            Assert.Equal(accounts, testAccount);
        }


        [Fact]
        public async Task GetAccounts_NoAccounts_ThrowNotFoundException()
        {
            // Arrange
            var accountController = new AccountController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => accountController.GetAccounts());

            // Assert
            ex.Message.ShouldBe("No accounts were found");
        }

        [Fact]
        public async Task AddAccount_Successful_MustReturnCreatedAtAction()
        {
            // Arrange
            var testAccount = new Account()
            {
                Id = 5,
                AccountHolderName = "Test Test",
                IsActive = true
            };
            _managementService.Setup(x => x.AddAccount(testAccount)).ReturnsAsync(testAccount);
            _managementService.Setup(x => x.GetAccount(testAccount.Id)).ReturnsAsync(testAccount);
            var accountController = new AccountController(_managementService.Object);

            // Act
            var result = await accountController.AddAccount(testAccount);

            // Assert
            var actionResult = Assert
                .IsType<ActionResult<Account>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var accountResult = Assert.IsAssignableFrom<Account>
                (createdAtActionResult.Value);
            Assert.Equal(accountResult, testAccount);
            Assert.Equal("Test Test", accountResult.AccountHolderName);
            Assert.Equal(5, accountResult.Id);
            Assert.True(accountResult.IsActive);
        }

        [Fact]
        public async Task AddAccount_Unsuccessful_ThrowsAccountManagementAPIException()
        {
            // Arrange
            var testAccount = new Account()
            {
                Id = 5,
                AccountHolderName = "Test Test",
                IsActive = true
            };
            var accountController = new AccountController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<AccountManagementAPIException>(() => accountController.AddAccount(testAccount));

            // Assert
            ex.Message.ShouldBe("An error occured while trying to add an account");
        }

        [Fact]
        public async Task GetAccount_ValidAccountId_MustReturnOk()
        {
            // Arrange
            var testAccount = new Account()
            {
                Id = 5,
                AccountHolderName = "Test Account Holder",
                IsActive = true
            };
            _managementService.Setup(x => x.GetAccount(5)).ReturnsAsync(testAccount);
            var accountController = new AccountController(_managementService.Object);

            // Act
            var result = await accountController.GetAccount(5);

            // Assert
            var actionResult = Assert
                .IsAssignableFrom<ActionResult>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            var accountResult = Assert.IsAssignableFrom<Account>
                (okObjectResult.Value);
            Assert.Equal(accountResult, testAccount);
            Assert.Equal("Test Account Holder", accountResult.AccountHolderName);
            Assert.Equal(5, accountResult.Id);
            Assert.True(accountResult.IsActive);
        }

        [Fact]
        public async Task GetAccount_InvalidAccountId_ThrowsAccountManagementAPIException()
        {
            // Arrange
            var accountController = new AccountController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<AccountManagementAPIException>(() => accountController.GetAccount(5));

            // Assert
            ex.Message.ShouldBe("An error occured while trying to get an account by id : 5");
        }

        [Fact]
        public async Task ToggleAccountState_Successful_MustReturnNoContent()
        {
            // Arrange
            var testAccount = new Account()
            {
                Id = 5,
                AccountHolderName = "Test Account Holder",
                IsActive = true
            };
            _managementService.Setup(x => x.GetAccount(5)).ReturnsAsync(testAccount);
            _managementService.Setup(x => x.ToggleAccountStatus(testAccount)).ReturnsAsync(new Account()
            {
                Id = 5,
                AccountHolderName = "Test Account Holder",
                IsActive = false
            });
            var accountController = new AccountController(_managementService.Object);

            // Act
            var result = await accountController.ToggleAccountState(5);

            // Assert
            Assert.IsAssignableFrom<NoContentResult>(result);
        }

        [Fact]
        public async Task ToggleAccountState_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            var accountController = new AccountController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => accountController.ToggleAccountState(6));

            // Assert
            ex.Message.ShouldBe("Account wasn't found with id : 6");
        }

        [Fact]
        public async Task ToggleAccountState_ValidIdUnsuccessfulUpdate_ThrowsAccountManagementAPIException()
        {
            // Arrange
            var testAccount = new Account()
            {
                Id = 5,
                AccountHolderName = "Test Account Holder",
                IsActive = true
            };
            _managementService.Setup(x => x.GetAccount(5)).ReturnsAsync(testAccount);
            var accountController = new AccountController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<AccountManagementAPIException>(() => accountController.ToggleAccountState(5));

            // Assert
            ex.Message.ShouldBe("An error occured while toggling the account status");
        }
    }
}
