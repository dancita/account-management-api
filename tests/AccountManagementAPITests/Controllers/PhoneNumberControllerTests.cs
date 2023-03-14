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
    public class PhoneNumberControllerTests
    {
        private readonly Mock<IManagementService> _managementService = new();

        [Fact]
        public async Task GetPhoneNumbersByAccountId_AccountIdAssignedToPhoneNumber_MustReturnOk()
        {
            // Arrange           
            _managementService.Setup(x => x.GetPhoneNumbersByAccountId(4))
                .ReturnsAsync(new List<PhoneNumber>()
                {
                    new PhoneNumber()
                    {
                        AccountId = 1,
                        Id = 2,
                        Number = "555-4321"
                    }
                });

            var phoneNumberController = new PhoneNumberController(_managementService.Object);

            // Act
            var result = await phoneNumberController.GetPhoneNumbersByAccountId(4);

            //Assert
            var actionResult = Assert
                .IsType<ActionResult<IEnumerable<PhoneNumber>>>(result);
            Assert.IsAssignableFrom<IEnumerable<PhoneNumber>>(
                ((OkObjectResult)actionResult.Result).Value);
        }

        [Fact]
        public async Task GetPhoneNumbersByAccountId_NoAccountIdAssignedToPhoneNumber_MustReturnNotFound()
        {
            // Arrange
            _managementService.Setup(x => x.GetPhoneNumbersByAccountId(3))
                .ReturnsAsync(new List<PhoneNumber>());
            var phoneNumberController = new PhoneNumberController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => phoneNumberController.GetPhoneNumbersByAccountId(3));

            //Assert
            ex.Message.ShouldBe("No phone numbers were found by account id : 3");
        }

        [Fact]
        public async Task DeletePhoneNumber_ValidId_MustReturnNoContent()
        {
            var phoneNumber = new PhoneNumber()
            {
                AccountId = 1,
                Id = 2,
                Number = "555-4321"
            };
            // Arrange
            _managementService.Setup(x => x.GetPhoneNumber(2)).ReturnsAsync(phoneNumber);

            _managementService.Setup(x => x.DeletePhoneNumber(phoneNumber)).ReturnsAsync(true);

            var phoneNumberController = new PhoneNumberController(_managementService.Object);

            // Act
            var result = await phoneNumberController.DeletePhoneNumber(2);

            //Assert
            Assert.IsAssignableFrom<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePhoneNumber_InvalidId_MustThrowNotFound()
        {
            var phoneNumberController = new PhoneNumberController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => phoneNumberController.DeletePhoneNumber(1));

            //Assert
            ex.Message.ShouldBe("Phone number wasn't found for account id : 1");
        }

        [Fact]
        public async Task DeletePhoneNumber_UnsuccessfulDelete_MustThrowAccountManagementAPIException()
        {
            var phoneNumber = new PhoneNumber()
            {
                AccountId = 1,
                Id = 2,
                Number = "555-4321"
            };
            // Arrange
            _managementService.Setup(x => x.GetPhoneNumber(2)).ReturnsAsync(phoneNumber);

            _managementService.Setup(x => x.DeletePhoneNumber(phoneNumber)).ReturnsAsync(false);

            var phoneNumberController = new PhoneNumberController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<AccountManagementAPIException>(() => phoneNumberController.DeletePhoneNumber(2));

            //Assert
            ex.Message.ShouldBe("An error occured while trying to delete phone number with id : 2");
        }

        [Fact]
        public async Task AssignPhoneNumberToAccount_ValidPhoneNumber_ValidAccount_ReturnsNoContent()
        {
            // Arrange
            var phoneNumber = new PhoneNumber()
            {
                AccountId = 12,
                Id = 2,
                Number = "555-4321"
            };

            _managementService.Setup(x => x.GetAccount(12)).ReturnsAsync(new Account()
            {
                Id = 12,
                AccountHolderName = "Test",
                IsActive = true
            });

            _managementService.Setup(x => x.AssignAccount(phoneNumber, 12)).ReturnsAsync(new PhoneNumber()
            {
                AccountId = 12,
                Id = 2,
                Number = "555-4321"
            });

            var phoneNumberController = new PhoneNumberController(_managementService.Object);

            // Act
            var result = await phoneNumberController.AssignPhoneNumberToAccount(2, phoneNumber);

            //Assert
            Assert.IsAssignableFrom<NoContentResult>(result);
        }

        [Fact]
        public async Task AssignPhoneNumberToAccount_IdInParameterDoesntMatchIdInPhoneNumber_ThrowsBadRequestException()
        {
            // Arrange
            var phoneNumber = new PhoneNumber()
            {
                AccountId = 12,
                Id = 2,
                Number = "555-4321"
            };

            var phoneNumberController = new PhoneNumberController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => phoneNumberController.AssignPhoneNumberToAccount(5, phoneNumber));

            //Assert
            ex.Message.ShouldBe("Id given in parameter : 5 must match id given in body: 2");
        }

        [Fact]
        public async Task AssignPhoneNumberToAccount_InvalidAccountId_ThrowsNotFoundException()
        {
            // Arrange
            var phoneNumber = new PhoneNumber()
            {
                AccountId = 12,
                Id = 2,
                Number = "555-4321"
            };

            var phoneNumberController = new PhoneNumberController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => phoneNumberController.AssignPhoneNumberToAccount(2, phoneNumber));

            //Assert
            ex.Message.ShouldBe("Account with id : 12 wasn't found");
        }

        [Fact]
        public async Task AssignPhoneNumberToAccount_InactiveAccount_ThrowsAccountManagementApiException()
        {
            // Arrange
            var phoneNumber = new PhoneNumber()
            {
                AccountId = 12,
                Id = 2,
                Number = "555-4321"
            };

            _managementService.Setup(x => x.GetAccount(12)).ReturnsAsync(new Account()
            {
                Id = 12,
                AccountHolderName = "Test",
                IsActive = false
            });

            var phoneNumberController = new PhoneNumberController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<AccountManagementAPIException>(() => phoneNumberController.AssignPhoneNumberToAccount(2, phoneNumber));

            //Assert
            ex.Message.ShouldBe("Account with id : 12 is inactive");
        }

        [Fact]
        public async Task AssignPhoneNumberToAccount_UnsuccessfulUpdate_ThrowsAccountManagementApiException()
        {
            // Arrange
            var phoneNumber = new PhoneNumber()
            {
                AccountId = 12,
                Id = 2,
                Number = "555-4321"
            };

            _managementService.Setup(x => x.GetAccount(12)).ReturnsAsync(new Account()
            {
                Id = 12,
                AccountHolderName = "Test",
                IsActive = true
            });

            var phoneNumberController = new PhoneNumberController(_managementService.Object);

            // Act
            var ex = await Assert.ThrowsAsync<AccountManagementAPIException>(() => phoneNumberController.AssignPhoneNumberToAccount(2, phoneNumber));

            //Assert
            ex.Message.ShouldBe("An error occured while trying to assign account id : 12 to phone number with id : 2");
        }
    }
}
