using AccountManagementAPI.Exceptions;
using AccountManagementAPI.Middleware;
using AccountManagementAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Shouldly;
using System.Net;
using Xunit;

namespace AccountManagementAPITests.MiddlewareTests
{
    public class ExceptionHandlingMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlingMiddleware>> logger = new();

        [Fact]
        public async Task WhenAnAccountManagementAPIExceptionIsRaised_MiddlewareShouldHandleItAsExpected()
        {
            // Arrange
            RequestDelegate requestDelegate = (innerHttpContext) =>
            {
                throw new AccountManagementAPIException("Test error message!");
            };

            var middleware = new ExceptionHandlingMiddleware(requestDelegate, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();            

            //Act
            await middleware.InvokeAsync(httpContext);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(httpContext.Response.Body);
            var streamText = reader.ReadToEnd();
            var errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(streamText);

            //Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Test error message!");
            errorDetails.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
            errorDetails.Source.ShouldBe("AccountManagementAPITests");
        }

        [Fact]
        public async Task WhenANotFoundExceptionIsRaised_MiddlewareShouldHandleItAsExpected()
        {
            // Arrange
            RequestDelegate requestDelegate = (innerHttpContext) =>
            {
                throw new NotFoundException("Test id wasn't found!");
            };

            var middleware = new ExceptionHandlingMiddleware(requestDelegate, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            //Act
            await middleware.InvokeAsync(httpContext);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(httpContext.Response.Body);
            var streamText = reader.ReadToEnd();
            var errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(streamText);

            //Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Test id wasn't found!");
            errorDetails.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
            errorDetails.Source.ShouldBe("AccountManagementAPITests");
        }

        [Fact]
        public async Task WhenABadRequestExceptionIsRaised_MiddlewareShouldHandleItAsExpected()
        {
            // Arrange
            RequestDelegate requestDelegate = (innerHttpContext) =>
            {
                throw new BadRequestException("Test error message: bad request!");
            };

            var middleware = new ExceptionHandlingMiddleware(requestDelegate, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            //Act
            await middleware.InvokeAsync(httpContext);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(httpContext.Response.Body);
            var streamText = reader.ReadToEnd();
            var errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(streamText);

            //Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Test error message: bad request!");
            errorDetails.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
            errorDetails.Source.ShouldBe("AccountManagementAPITests");
        }

        [Fact]
        public async Task WhenAnyOtherExceptionIsRaised_MiddlewareShouldHandleItAsExpected()
        {
            // Arrange
            RequestDelegate requestDelegate = (innerHttpContext) =>
            {
                throw new Exception("Testing other type of exceptions");
            };

            var middleware = new ExceptionHandlingMiddleware(requestDelegate, logger.Object);

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            //Act
            await middleware.InvokeAsync(httpContext);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            var reader = new StreamReader(httpContext.Response.Body);
            var streamText = reader.ReadToEnd();
            var errorDetails = JsonConvert.DeserializeObject<ErrorDetails>(streamText);

            //Assert
            errorDetails.ShouldNotBeNull();
            errorDetails.Message.ShouldBe("Testing other type of exceptions");
            errorDetails.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
            errorDetails.Source.ShouldBe("AccountManagementAPITests");
        }
    }
}