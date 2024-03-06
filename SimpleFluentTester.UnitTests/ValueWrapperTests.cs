using SimpleFluentTester.Helpers;

namespace SimpleFluentTester.UnitTests
{
    public class ValueWrapperTests
    {
        [Fact]
        public void ValueWrapper_HasPrintableValue_ShouldBeValid()
        {
            // Assign
            var value = new ValueWrapper<int>(2);

            // Act
            var printedResult = value.ToString();

            // Assert
            Assert.Equal("2",printedResult);
        }
    
        [Fact]
        public void ValueWrapper_HasNullValue_ShouldBeEmpty()
        {
            // Assign
            var value = new ValueWrapper<string?>(null);

            // Act
            var printedResult = value.ToString();

            // Assert
            Assert.Empty(printedResult);
        }
    }
}