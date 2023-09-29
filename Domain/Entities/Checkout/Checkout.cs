using Domain.Abstract;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Checkout
    {
        /// <summary>
        /// Constructor for EFCore only.
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Checkout() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private Checkout(DateOnly checkoutDate,
                        DateOnly dueDate,
                        int renewalsLeft,
                        bool isReturned,
                        Patron patron,
                        Book book)
        {
            CheckoutDate = checkoutDate;
            DueDate = dueDate;
            RenewalsLeft = renewalsLeft;
            IsReturned = isReturned;

            PatronId = patron.Id;
            Patron = patron;

            BookId = book.Id;
            Book = book;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CheckoutId { get; set; }
        public DateOnly CheckoutDate { get; set; }
        public DateOnly DueDate { get; set; }
        public DateOnly? DateReturned { get; set; }
        public int RenewalsLeft { get; set; } = 1;
        public bool IsReturned { get; set; } = false;

        public Guid PatronId { get; set; }
        public Patron Patron { get; set; }

        public Guid BookId { get; set; }
        public Book Book { get; set; }

        public static async Task<Result<Checkout>> Create(DateOnly checkoutDate,
                                                DateOnly dueDate,
                                                int renewalsLeft,
                                                bool isReturned,
                                                Patron patron,
                                                Book book)
        {
            Checkout checkout = new(checkoutDate, dueDate, renewalsLeft, isReturned, patron, book);

            CheckoutValidator validator = new();

            DomainValidationResult<Checkout> result = await validator.ValidateAsyncGetDomainResult(checkout);

            return result;
        }

        public void ReturnBook()
        {
            IsReturned = true;
            Book.IsCheckedOut = false;
            DateReturned = DateOnly.FromDateTime(DateTime.UtcNow);
        }
    }
}
