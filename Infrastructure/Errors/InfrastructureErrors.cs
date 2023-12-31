﻿using Domain.Abstract;

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

        public static class PatronRepositoryErrors
        {
            public static readonly ErrorCode PATRON_ID_NOT_FOUND = new("Database", "PatronRepository", "IDNotFound");
            public static readonly ErrorCode PATRON_DB_ERROR = new("Database", "PatronRepository", "DbError");
        }

        public static class CheckoutRepositoryErrors
        {
            public static readonly ErrorCode CHECKOUT_DB_ERROR = new("Database", "CheckoutRepository", "DbError");
            public static readonly ErrorCode CHECKED_OUT_BOOK_ID_NOT_FOUND = new("Database", "CheckoutRepository", "CheckedOutBookNotFound");
            public static readonly ErrorCode CHECKOUT_TRANSACTION_NOT_FOUND = new("Database", "CheckoutRepository", "CheckoutTransactionNotFound");
        }
    }
}
