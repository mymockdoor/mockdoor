using System;
using Microsoft.Extensions.Logging;
using MockDoor.Abstractions.Repositories;
using MockDoor.Api.Controllers.AdminControllers;
using Moq;
using Xunit;

namespace MockDoor.Server.Tests.Controller
{
    public class MicroserviceControllerTests
    {
        [Fact]
        public void ConstructsOk()
        {
            var mockLogger = new Mock<ILogger<MicroserviceController>>();
            var mockRepository = new Mock<IMicroserviceRepository>();

            var ex = Record.Exception(() => new MicroserviceController(mockLogger.Object, mockRepository.Object));

            Assert.Null(ex);
        }

        [Fact]
        public void ThrowsExceptionOnNullLogger()
        {

            var mockRepository = new Mock<IMicroserviceRepository>();

            Assert.Throws<ArgumentNullException>(() => new MicroserviceController(null, mockRepository.Object));
        }

        [Fact]
        public void ThrowsExceptionOnNullRepository()
        {
            var mockLogger = new Mock<ILogger<MicroserviceController>>();

            Assert.Throws<ArgumentNullException>(() => new MicroserviceController(mockLogger.Object, null));
        }
    }
}
