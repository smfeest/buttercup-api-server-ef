using Buttercup.Api.DbModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Buttercup.Api.Security;

public sealed class PasswordAuthenticationService : IPasswordAuthenticationService
{
    private readonly IDbContextFactory<AppDbContext> dbContextFactory;
    private readonly ILogger<PasswordAuthenticationService> logger;
    private readonly IPasswordAuthenticationQueries passwordAuthenticationQueries;
    private readonly IPasswordHasher<User> passwordHasher;

    public PasswordAuthenticationService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<PasswordAuthenticationService> logger,
        IPasswordAuthenticationQueries passwordAuthenticationQueries,
        IPasswordHasher<User> passwordHasher)
    {
        this.dbContextFactory = dbContextFactory;
        this.logger = logger;
        this.passwordAuthenticationQueries = passwordAuthenticationQueries;
        this.passwordHasher = passwordHasher;
    }

    public async Task<User?> Authenticate(string email, string password)
    {
        using var dbContext = this.dbContextFactory.CreateDbContext();

        var user = await this.passwordAuthenticationQueries.FindUserByEmail(dbContext, email);

        if (user == null)
        {
            AuthenticateLogMessages.UnrecognizedEmail(this.logger, email, null);

            return null;
        }

        if (user.PasswordHash == null)
        {
            AuthenticateLogMessages.NoPasswordSet(this.logger, user.Id, null);

            return null;
        }

        var verificationResult = this.passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            AuthenticateLogMessages.IncorrectPassword(this.logger, user.Id, null);

            return null;
        }

        AuthenticateLogMessages.Success(this.logger, user.Id, null);

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            var newHash = this.passwordHasher.HashPassword(user, password);

            if (await this.passwordAuthenticationQueries.SaveUpgradedPasswordHash(
                dbContext, user.Id, user.PasswordHash, newHash))
            {
                AuthenticateLogMessages.UpgradedPasswordHash(this.logger, user.Id, null);

                user.PasswordHash = newHash;
            }
        }

        return user;
    }

    private static class AuthenticateLogMessages
    {
        public static readonly Action<ILogger, long, Exception?> IncorrectPassword =
            LoggerMessage.Define<long>(
                LogLevel.Information,
                new(1, nameof(Authenticate)),
                "Authentication failed; incorrect password for user {UserId}");

        public static readonly Action<ILogger, long, Exception?> NoPasswordSet =
            LoggerMessage.Define<long>(
                LogLevel.Information,
                new(2, nameof(Authenticate)),
                "Authentication failed; no password set for user {UserId}");

        public static readonly Action<ILogger, long, Exception?> Success =
            LoggerMessage.Define<long>(
                LogLevel.Information,
                new(3, nameof(Authenticate)),
                "User {UserId} successfully authenticated");

        public static readonly Action<ILogger, string, Exception?> UnrecognizedEmail =
            LoggerMessage.Define<string>(
                LogLevel.Information,
                new(4, nameof(Authenticate)),
                "Authentication failed; no user with email {Email}");

        public static readonly Action<ILogger, long, Exception?> UpgradedPasswordHash =
            LoggerMessage.Define<long>(
                LogLevel.Debug,
                new(5, nameof(Authenticate)),
                "Upgraded password hash for user {UserId}");
    }
}
