using AwesomeAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OLT.Core;
using OLT.Logging.Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;


namespace OLT.AspNetCore.Serilog.Tests.General
{
    public class UnitTestsMiddleware
    {
        [Fact]
        public async Task Returns500StatusCode()
        {
            var expectedMsg = "An error has occurred.";
            var overrideMsg = "Override Error Message";
            var expectedException = new ArgumentNullException();
            RequestDelegate next = (HttpContext hc) => Task.FromException(expectedException);

            var response = await InvokeMiddlewareAsync(GetOptions(true), next, HttpStatusCode.InternalServerError);
            Assert.Equal(expectedMsg, response?.Message);
            Assert.NotNull(response?.ErrorUid);
            Assert.NotEmpty(response.Errors);


            response = await InvokeMiddlewareAsync(GetOptions(true, overrideMsg), next, HttpStatusCode.InternalServerError);
            Assert.Equal(overrideMsg, response?.Message);
            Assert.NotNull(response?.ErrorUid);
            Assert.NotEmpty(response.Errors);

            response = await InvokeMiddlewareAsync(GetOptions(), next, HttpStatusCode.InternalServerError);
            Assert.Equal(expectedMsg, response?.Message);
            Assert.NotNull(response?.ErrorUid);
            Assert.Empty(response.Errors);

            response = await InvokeMiddlewareAsync(GetOptions(false, overrideMsg), next, HttpStatusCode.InternalServerError);
            Assert.Equal(overrideMsg, response?.Message);
            Assert.NotNull(response?.ErrorUid);
            Assert.Empty(response.Errors);

        }


        [Fact]
        public async Task OltBadRequestException()
        {
            //arrange
            var expectedException = new OltBadRequestException("Test Bad Request");
            RequestDelegate next = (HttpContext hc) => Task.FromException(expectedException);
            var response = await InvokeMiddlewareAsync(GetOptions(true), next, HttpStatusCode.BadRequest);

            Assert.Equal(expectedException.Message, response?.Message);
            Assert.NotNull(response?.ErrorUid);
            Assert.Empty(response.Errors);
        }

        [Fact]
        public async Task OltValidationException()
        {
            var expectedException = new OltValidationException(new List<IOltValidationError> { new OltValidationError("Test Validation") });
            RequestDelegate next = (HttpContext hc) => Task.FromException(expectedException);
            var response = await InvokeMiddlewareAsync(GetOptions(true), next, HttpStatusCode.BadRequest);

            Assert.Equal("Please correct the validation errors", response?.Message);
            Assert.NotNull(response?.ErrorUid);
            Assert.NotEmpty(response.Errors);
            Assert.Collection(response.Errors, item => Assert.Equal("Test Validation", item));
        }

        [Fact]
        public async Task OltRecordNotFoundException()
        {
            var expectedException = new OltRecordNotFoundException("Person");
            RequestDelegate next = (HttpContext hc) => Task.FromException(expectedException);
            var response = await InvokeMiddlewareAsync(GetOptions(true), next, HttpStatusCode.BadRequest);
            Assert.Equal(expectedException.Message, response?.Message);
            Assert.NotNull(response?.ErrorUid);
            Assert.Empty(response.Errors);
        }


        [Fact]
        public async Task Completed()
        {
            var dto = new PersonModel
            {
                Name = Faker.Name.FullName()
            };

            var json = JsonConvert.SerializeObject(dto);

            RequestDelegate next = (HttpContext hc) =>
            {
                return hc.Response.WriteAsync(json);
            };

            var response = await InvokeMiddlewareAsync<PersonModel>(GetOptions(true), next, HttpStatusCode.OK);
            response.Should().BeEquivalentTo(dto);
        }


        private static IOptions<OltSerilogOptions> GetOptions(bool showExceptionDetails = false, string? errorMsg = null)
        {
            var settings = new OltSerilogOptions
            {
                ShowExceptionDetails = showExceptionDetails,
            };

            if (errorMsg != null)
            {
                settings.ErrorMessage = errorMsg;
            }
            return Options.Create(settings);
        }

        private static async Task<OltErrorHttpSerilog?> InvokeMiddlewareAsync(IOptions<OltSerilogOptions> options, RequestDelegate next, HttpStatusCode expectedStatusCode)
        {
            return await InvokeMiddlewareAsync<OltErrorHttpSerilog>(options, next, expectedStatusCode);
        }

        private static async Task<T?> InvokeMiddlewareAsync<T>(IOptions<OltSerilogOptions> options, RequestDelegate next, HttpStatusCode expectedStatusCode)
        {
            var exceptionHandlingMiddleware = new OltMiddlewarePayload(options);
            var bodyStream = new MemoryStream();
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = bodyStream;
            httpContext.Request.Path = "/testing";

            //act
            await exceptionHandlingMiddleware.InvokeAsync(httpContext, next);

            T? response;
            bodyStream.Seek(0, SeekOrigin.Begin);
            using (var sr = new StreamReader(bodyStream))
            {
                response = JsonConvert.DeserializeObject<T>(await sr.ReadToEndAsync());
            }

            Assert.Equal(expectedStatusCode, (HttpStatusCode)httpContext.Response.StatusCode);

            return response;
        }
    }
}