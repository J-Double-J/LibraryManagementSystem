using Domain.Abstract;
using Domain.Primitives;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Patron
{
    public class Patron : Entity
    {
        public const int PATRON_NAME_MAXLENGTH = 50;
        public const int MINIMUM_AGE = 4;
        public const int ADULT_AGE_CUTOFF = 18;

        private Patron(Guid id, string firstName, string lastName, int age)
            : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }

        [NotMapped]
        public bool IsAdult { get { return Age >= ADULT_AGE_CUTOFF; } }

        public static async Task<Result<Patron>> Create(Guid guid, string firstName, string lastName, int age)
        {
            Patron patron = new(guid, firstName, lastName, age);

            PatronValidator validator = new();

            DomainValidationResult<Patron> validationResult = await validator.ValidateAsyncGetDomainResult(patron);

            return validationResult;
        }
    }
}
