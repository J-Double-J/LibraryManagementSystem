using Domain.Abstract;

namespace Infrastructure.Errors
{
    public class InfrastructureErrors
    {
        public static class BookRepositoryErrors
        {
            public static readonly ErrorCode BOOK_GETALL_ERROR = new("Database", "BookRepository", "GetAll");
            public static readonly ErrorCode BOOK_ADD_ERROR = new("Database", "BookRepository", "AddError");
            public static readonly ErrorCode BOOK_DELETION_ERROR = new("Database", "BookRepository", "DeletionError");
            public static readonly ErrorCode BOOK_ID_NOT_FOUND = new("Database", "BookRepository", "IDNotFound");
            public static readonly ErrorCode BOOK_DB_ERROR = new("Database", "BookRepository", "DbError");
        }
    }
}
