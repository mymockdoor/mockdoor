using MockDoor.Client.Helpers;
using Xunit;

namespace MockDoor.Client.Tests.Helpers
{
    public class HelperMethodsExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void SafePrintSafelyPrintsWhenValueNullOrWhiteSpace(string value)
        {
            var defaultValue = "someDefault";

            var output = value.SafePrint(defaultValue);

            Assert.Equal(defaultValue, output);
        }

        [Fact]
        public void SafePrintPrintsValueWhenProvided()
        {
            var defaultValue = "someDefault";

            string value = "value";

            var output = value.SafePrint(defaultValue);

            Assert.Equal(value, output);
        }

        [Fact]
        public void SafePrintPrintsSafelyWhenBothNull()
        {
            var output = ((string)null).SafePrint();

            Assert.NotNull(output);
        }
    }
}
