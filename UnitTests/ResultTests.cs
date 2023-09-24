using Domain.Abstract;
using FluentAssertions;

namespace UnitTests
{
    public class ResultTests
    {
        [Fact]
        public void DomainValidationResultT_SuccessShouldBeNoHighLevelError()
        {
            DomainValidationResult<int> validationResult = DomainValidationResult<int>.SuccessfulValidation(1);

            validationResult.HighLevelErrorCode.Should().BeNull();
        }

        [Fact]
        public void DomainValidationResultT_ErrorCodeCannotBeSetToNull()
        {
            DomainValidationResult<int> validationResult = DomainValidationResult<int>.SuccessfulValidation(1);

            Action a = () => validationResult.HighLevelErrorCode = null;

            a.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void DomainValidationResultT_CanGetErrorCodeFromFirstError()
        {
            ValidationError error = new(TestReferenceHelper.TEST_CODE, "message");
            DomainValidationResult<int> validationResult = DomainValidationResult<int>.WithErrors(new[] { error });
            validationResult.HighLevelErrorCode = TestReferenceHelper.TEST_CODE;

            validationResult.HighLevelErrorCode.Should().Be(TestReferenceHelper.TEST_CODE);
            validationResult.ValidationErrors.Should().HaveCount(1);
        }

        [Fact]
        public void DomainValidationResultT_CannotHaveEmptyArray()
        {
            Action a = () => DomainValidationResult<int>.WithErrors(Array.Empty<ValidationError>());

            a.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void DomainValidationResult_CanBeSuccessful()
        {
            DomainValidationResult result = DomainValidationResult.Success();
            DomainValidationResult successfulValidationResult = DomainValidationResult.SuccessfulValidation();

            result.Error.Should().BeEquivalentTo(Error.None);
            result.ValidationErrors.Should().HaveCount(0);
            successfulValidationResult.Error.Should().BeEquivalentTo(Error.None);
            successfulValidationResult.ValidationErrors.Should().HaveCount(0);
        }

        [Fact]
        public void DomainValidationResult_CanHaveErrors()
        {
            ValidationError error = new(TestReferenceHelper.TEST_CODE, "message");
            DomainValidationResult result = DomainValidationResult.WithErrors(new[] { error });

            result.HighLevelErrorCode.Should().Be(TestReferenceHelper.TEST_CODE);
            result.ValidationErrors.Should().HaveCount(1);
        }

        [Fact]
        public void DomainValidationResult_CannotHaveEmptyArray()
        {
            Action a = () => DomainValidationResult.WithErrors(Array.Empty<ValidationError>());

            a.Should().Throw<IndexOutOfRangeException>();
        }

        [Fact]
        public void DomainValidationResult_HighLevelErrorCannotBeSetToNull()
        {
            ValidationError error = new(TestReferenceHelper.TEST_CODE, "message");
            DomainValidationResult result = DomainValidationResult.WithErrors(new[] { error });
            Action a = () => result.HighLevelErrorCode = null;

            a.Should().Throw<InvalidOperationException>();
        }
    }
}
