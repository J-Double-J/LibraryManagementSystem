using Domain.Abstract;

namespace Infrastructure.Errors
{
    public class InfrastructureErrors
    {
        public static class BookRepositoryErrors
        {
            public const string BOOK_GETALL_ERROR = "BookRepository.GetAll";
            public const string BOOK_ADD_ERROR = "BookRepository.AddError";
            public const string BOOK_DELETION_ERROR = "BookRepository.DeletionError";
            public const string BOOK_ID_NOT_FOUND = "BookRepository.IDNotFound";
            public const string BOOK_DB_ERROR = "BookRepository.DbError";
        }
    }
}
