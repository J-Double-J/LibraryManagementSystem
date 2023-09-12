using Domain.Abstract;

namespace Domain.Errors
{
    public class DomainErrors
    {
        public static class BookErrors
        {
            public static readonly Error NegativeOrZeroPages = new Error(
                "Book.NegativeOrZeroPages",
                "Book can't have negative or zero pages");
        }
    }
}
