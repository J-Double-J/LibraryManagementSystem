﻿using Application;
using Application.Interfaces;
using Application.Interfaces.Configuration;
using Domain.Abstract;
using Domain.Entities;
using Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CheckoutRepository : ICheckoutRepository
    {
        IApplicationDbContext _context;
        ILibraryManagementConfiguration _configuration;

        public CheckoutRepository(IApplicationDbContext context, ILibraryManagementConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<Result> AddCheckoutTransaction(Checkout checkout)
        {
            try
            {
                _context.Checkout.Add(checkout);

                await _context.SaveChanges();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(new(InfrastructureErrors.CheckoutRepositoryErrors.CHECKOUT_DB_ERROR, ex.Message));
            }
        }

        public async Task<Result<bool>> Renew(Guid bookGuid)
        {
            try
            {
                Checkout? checkout = await _context.Checkout.Where(c => !c.IsReturned && c.Book.Id == bookGuid).FirstOrDefaultAsync();

                if (checkout is null)
                {
                    return Result.Failure(false, new(InfrastructureErrors.CheckoutRepositoryErrors.CHECKOUT_TRANSACTION_NOT_FOUND,
                                          $"Checkout transaction involving '{bookGuid}' not found."));
                }

                Result<bool> didSuccessfullyRenewResult = checkout.TryRenew(_configuration.RenewalPolicy.RenewalPeriodInDays);

                if (didSuccessfullyRenewResult.IsFailure)
                {
                    return didSuccessfullyRenewResult;
                }

                if (didSuccessfullyRenewResult.Value == true)
                {
                    await _context.SaveChanges();
                }

                return Result.Success(didSuccessfullyRenewResult.Value);
            }
            catch (Exception ex)
            {
                return Result.Failure(false, new(InfrastructureErrors.CheckoutRepositoryErrors.CHECKOUT_DB_ERROR, ex.Message));
            }
        }

        public async Task<Result> ReturnBook(Guid bookGuid)
        {
            try
            {
                Book? book = await _context.Books.Where(b => b.Id == bookGuid && b.IsCheckedOut).FirstOrDefaultAsync();

                if (book is null)
                {
                    return Result.Failure(new(InfrastructureErrors.CheckoutRepositoryErrors.CHECKED_OUT_BOOK_ID_NOT_FOUND,
                                          $"Book with ID `{bookGuid}` was either not found or was not checked out."));
                }

                Checkout? checkout = await _context.Checkout.Where(c => !c.IsReturned && c.Book.Id == bookGuid).FirstOrDefaultAsync();

                if (checkout is null)
                {
                    return Result.Failure(new(InfrastructureErrors.CheckoutRepositoryErrors.CHECKOUT_TRANSACTION_NOT_FOUND,
                                          $"Checkout transaction involving '{book.Title}' not found."));
                }

                checkout.ReturnBook();

                await _context.SaveChanges();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(false, new(InfrastructureErrors.CheckoutRepositoryErrors.CHECKOUT_DB_ERROR, ex.Message));
            }
        }
    }
}
