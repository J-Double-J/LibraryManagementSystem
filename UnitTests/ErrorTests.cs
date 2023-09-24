using Domain.Abstract;
using FluentAssertions;

namespace UnitTests
{
    public class ErrorTests
    {
        [Fact]
        public void ErrorToStringIsCode()
        {
            Error error = TestReferenceHelper.TEST_ERROR;
            string errorImplicitString = error;

            errorImplicitString.ToString().Should().BeEquivalentTo("UnitTestError.Testing.AnExpectedError");
        }

        [Fact]
        public void ErrorIsEqual_IsCorrect()
        {
            Error error = TestReferenceHelper.TEST_ERROR;
            Error constructedError = new(new ErrorCode("UnitTestError", "Testing", "AnExpectedError"), "Some arbitrary message");

            bool equalSignIsCorrect = error == constructedError;
            bool notEqualSignIsCorrect = error != constructedError;
            bool equalMethodIsCorrect = error.Equals(constructedError);
            bool hashCodeIsCorrect = error.GetHashCode() == constructedError.GetHashCode();

            equalSignIsCorrect.Should().BeTrue();
            notEqualSignIsCorrect.Should().BeFalse();
            equalMethodIsCorrect.Should().BeTrue(); 
            hashCodeIsCorrect.Should().BeTrue();
        }

        [Fact]
        public void NullEquivalenceIsFalse()
        {
            Error error = TestReferenceHelper.TEST_ERROR;
            Error? constructedError = null;

            bool equalSignIsCorrect = error == constructedError;
            bool equalMethodIsCorrect = error.Equals(constructedError);

            equalSignIsCorrect.Should().BeFalse();
            equalMethodIsCorrect.Should().BeFalse();
        }

        [Fact]
        public void ConstructErrorCodeFromString()
        {
            string errorString = "UnitTestError.Testing.AnExpectedError";

            ErrorCode code = ErrorCode.ConstructFromStringRepresentation(errorString);

            code.Should().BeEquivalentTo(TestReferenceHelper.TEST_CODE);
        }

        [Fact]
        public void ConstructFromTypeDomainString()
        {
            string typeDomainString = "UnitTestError.Testing";
            string errorSpecification = "AnExpectedError";

            ErrorCode code = ErrorCode.ConstructFromCombinedTypeDomain(typeDomainString, errorSpecification);

            code.Should().BeEquivalentTo(TestReferenceHelper.TEST_CODE);
        }

        [Theory]
        [InlineData("UnitTestError.Testing")]
        [InlineData("UnitTestError.Testing.")]
        public void CannotConstructFromInvalidString(string code)
        {
            Action action = () => ErrorCode.ConstructFromStringRepresentation(code);

            action.Should().Throw<InvalidOperationException>();
        }

        [Theory]
        [InlineData("UnitTestError")]
        [InlineData("UnitTestError.")]
        public void CannotConstuctFromIncompleteTypeDomainString(string code)
        {
            Action action = () => ErrorCode.ConstructFromCombinedTypeDomain(code, "specification");

            action.Should().Throw<InvalidOperationException>();
        }
    }
}
