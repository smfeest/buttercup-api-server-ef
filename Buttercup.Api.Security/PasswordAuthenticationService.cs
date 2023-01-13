using Buttercup.Api.DbModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Buttercup.Api.Security;

public sealed class PasswordAuthenticationService : IPasswordAuthenticationService
{
    private readonly IDbContextFactory<AppDbContext> dbContextFactory;
    private readonly ILogger<PasswordAuthenticationService> logger;
    private readonly IPasswordHasher<User> passwordHasher;

    public PasswordAuthenticationService(
        IDbContextFactory<AppDbContext> dbContextFactory,
        ILogger<PasswordAuthenticationService> logger,
        IPasswordHasher<User> passwordHasher)
    {
        this.dbContextFactory = dbContextFactory;
        this.logger = logger;
        this.passwordHasher = passwordHasher;
    }

    public async Task<User?> Authenticate(string email, string password)
    {
        using var dbContext = await this.dbContextFactory.CreateDbContextAsync();

        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);

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
    }
}
