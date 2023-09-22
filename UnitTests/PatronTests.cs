using Domain.Abstract;
using Domain.Entities.Patron;
using FluentAssertions;

namespace UnitTests
{
    public class PatronTests
    {
        [Fact]
        public async void Patron_FirstName_CannotBeTooLong()
        {
            string longName = new('a', Patron.PATRON_NAME_MAXLENGTH + 1);
            Result<Patron> patronResult = await Patron.Create(Guid.NewGuid(), longName, "Last", 18);

            patronResult.Should().BeOfType<DomainValidationResult<Patron>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async void Patron_LastName_CannotBeTooLong()
        {
            string longName = new('a', Patron.PATRON_NAME_MAXLENGTH + 1);
            Result<Patron> patronResult = await Patron.Create(Guid.NewGuid(), "First", longName, 18);

            patronResult.Should().BeOfType<DomainValidationResult<Patron>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(Patron.MINIMUM_AGE - 1)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public async void Patron_CannotBeTooOldOrYoung(int age)
        {
            Result<Patron> patronResult = await Patron.Create(Guid.NewGuid(), "First", "Last", age);

            patronResult.Should().BeOfType<DomainValidationResult<Patron>>()
                .Which.ValidationErrors.Should().HaveCount(1)
                .And.Subject.First().Message.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData(4)]
        [InlineData(Patron.ADULT_AGE_CUTOFF)]

        public async void Patron_CanHaveValidAge(int age)
        {
            Result<Patron> patronResult = await Patron.Create(Guid.NewGuid(), "First", "Last", age);

            patronResult.IsSuccess.Should().BeTrue();
            patronResult.Value.Age.Should().Be(age);
        }

        [Theory]
        [InlineData(10, false)]
        [InlineData(Patron.ADULT_AGE_CUTOFF, true)]
        [InlineData(Patron.ADULT_AGE_CUTOFF + 10, true)]
        public async void PatronIsConsideredAdultCorrectly(int age, bool adultIfOldEnough)
        {
            Result<Patron> patronResult = await Patron.Create(Guid.NewGuid(), "First", "Last", age);

            patronResult.IsSuccess.Should().BeTrue();
            patronResult.Value.IsAdult.Should().Be(adultIfOldEnough);
        }
    }
}
