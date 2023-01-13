using Buttercup.Api.DbModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Buttercup.Api.Security;

/// <summary>
/// The default implementation of <see cref="IPasswordAuthenticationService" />.
/// </summary>
public sealed class PasswordAuthenticationService : IPasswordAuthenticationService
{
    private readonly IDbContextFactory<AppDbContext> dbContextFactory;
    private readonly ILogger<PasswordAuthenticationService> logger;
    private readonly IPasswordAuthenticationServiceQueries queries;
    private readonly IPasswordHasher<User> passwordHasher;

    /// <summary>
    /// Initializes a new instance of the the <see cref="PasswordAuthenticationService" /> class.
    /// </summary>
    /// <param name="dbContextFactory">The database context factory.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="queries">The query provider.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    public PasswordAuthenticationService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<PasswordAuthenticationService> logger,
        IPasswordAuthenticationServiceQueries queries,
        IPasswordHasher<User> passwordHasher)
    {
        this.dbContextFactory = dbContextFactory;
        this.logger = logger;
        this.queries = queries;
        this.passwordHasher = passwordHasher;
    }

    /// <inheritdoc/>
    public async Task<User?> Authenticate(string email, string password)
    {
        using var dbContext = this.dbContextFactory.CreateDbContext();

        var user = await this.queries.FindUserByEmail(dbContext, email);

        if (user == null)
        {
            LogMessages.UnrecognizedEmail(this.logger, email, null);

            return null;
        }

        if (user.PasswordHash == null)
        {
            LogMessages.NoPasswordSet(this.logger, user.Id, null);

            return null;
        }

        var verificationResult = this.passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            LogMessages.IncorrectPassword(this.logger, user.Id, null);

            return null;
        }

        LogMessages.SuccessfullyAuthenticated(this.logger, user.Id, null);

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            var newHash = this.passwordHasher.HashPassword(user, password);

            if (await this.queries.SaveUpgradedPasswordHash(
                dbContext, user.Id, user.PasswordHash, newHash))
            {
                LogMessages.PasswordHashUpgraded(this.logger, user.Id, null);

                user.PasswordHash = newHash;
            }
            else
            {
                LogMessages.PasswordHashNotUpgraded(this.logger, user.Id, null);
            }
        }

        return user;
    }

    private static class LogMessages
    {
        public static readonly Action<ILogger, string, Exception?> UnrecognizedEmail =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new(1, nameof(UnrecognizedEmail)),
                "Authentication failed; no user with email {Email}");

        public static readonly Action<ILogger, long, Exception?> NoPasswordSet =
            LoggerMessage.Define<long>(
                LogLevel.Information,
                new(2, nameof(NoPasswordSet)),
                "Authentication failed; no password set for user {UserId}");

        public static readonly Action<ILogger, long, Exception?> IncorrectPassword =
            LoggerMessage.Define<long>(
                LogLevel.Information,
                new(3, nameof(IncorrectPassword)),
                "Authentication failed; incorrect password for user {UserId}");

        public static readonly Action<ILogger, long, Exception?> SuccessfullyAuthenticated =
            LoggerMessage.Define<long>(
                LogLevel.Information,
                new(4, nameof(SuccessfullyAuthenticated)),
                "User {UserId} successfully authenticated");

        public static readonly Action<ILogger, long, Exception?> PasswordHashUpgraded =
            LoggerMessage.Define<long>(
                LogLevel.Debug,
                new(5, nameof(PasswordHashUpgraded)),
                "Password hash upgraded for user {UserId}");

        public static readonly Action<ILogger, long, Exception?> PasswordHashNotUpgraded =
            LoggerMessage.Define<long>(
                LogLevel.Debug,
                new(6, nameof(PasswordHashNotUpgraded)),
                "Password hash not upgraded for user {UserId}; concurrent change detected");
    }
}
