using Buttercup.Api.DbModel;
using Buttercup.Api.TestUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Buttercup.Api.Security;

[Collection(nameof(SecurityTestsDbCollection))]
public class PasswordAuthenticationServiceTests
{
    private readonly SecurityTestsDbFixture dbFixture;
    private readonly ListLogger<PasswordAuthenticationService> logger = new();
    private readonly SampleDataFactory modelFactory = new();
    private readonly Mock<IPasswordHasher<User>> mockPasswordHasher = new();
    private readonly PasswordAuthenticationService passwordAuthenticationService;

    public PasswordAuthenticationServiceTests(SecurityTestsDbFixture dbFixture)
    {
        this.dbFixture = dbFixture;

        this.passwordAuthenticationService = new(
            this.dbFixture.DbContextFactory,
            this.logger,
            this.mockPasswordHasher.Object);
    }

    #region Authenticate

    [Fact]
    public async Task Authenticate_LogsAndReturnsNullWhenEmailNotFound()
    {
        await using var fixture = new AuthenticateFixture(this);

        Assert.Null(await fixture.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Authentication failed; no user with email {AuthenticateFixture.SuppliedEmail}");
    }

    [Fact]
    public async Task Authenticate_LogsAndReturnsNullWhenUserHasNoPassword()
    {
        await using var fixture = new AuthenticateFixture(this);

        var user = await fixture.SetupUserHasNoPassword();

        Assert.Null(await fixture.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Authentication failed; no password set for user {user.Id}");
    }

    [Fact]
    public async Task Authenticate_LogsAndReturnsNullWhenPasswordIsIncorrect()
    {
        await using var fixture = new AuthenticateFixture(this);

        var user = await fixture.SetupUserHasPassword(PasswordVerificationResult.Failed);

        Assert.Null(await fixture.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Authentication failed; incorrect password for user {user.Id}");
    }

    [Theory]
    [InlineData(PasswordVerificationResult.Success)]
    [InlineData(PasswordVerificationResult.SuccessRehashNeeded)]
    public async Task Authenticate_LogsAndReturnsUserOnSuccess(
        PasswordVerificationResult verificationResult)
    {
        await using var fixture = new AuthenticateFixture(this);

        var user = await fixture.SetupUserHasPassword(verificationResult);

        Assert.Equivalent(user, await fixture.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message == $"User {user.Id} successfully authenticated");
    }

    private sealed class AuthenticateFixture : IAsyncDisposable
    {
        public const string Password = "user-password";
        public const string PasswordHash = "password-hash";
        public const string SuppliedEmail = "supplied-email@example.com";

        private readonly PasswordAuthenticationServiceTests context;

        public AuthenticateFixture(PasswordAuthenticationServiceTests context) =>
            this.context = context;

        public async ValueTask DisposeAsync()
        {
            using var dbContext = this.context.dbFixture.DbContextFactory.CreateDbContext();

            await dbContext.Users.ExecuteDeleteAsync();
        }

        public Task<User?> Authenticate() =>
            this.context.passwordAuthenticationService.Authenticate(SuppliedEmail, Password);

        public Task<User> SetupUserHasNoPassword() => this.SetupUserExists(null);

        public async Task<User> SetupUserHasPassword(PasswordVerificationResult verificationResult)
        {
            var user = await this.SetupUserExists(PasswordHash);

            context.mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(It.IsAny<User>(), PasswordHash, Password)) // TODO: Maybe don't stub this?
                .Returns(verificationResult);

            return user;
        }

        private async Task<User> SetupUserExists(string? passwordHash)
        {
            var user = this.context.modelFactory.BuildUser();

            user.Email = SuppliedEmail;
            user.PasswordHash = passwordHash;

            using var dbContext = this.context.dbFixture.DbContextFactory.CreateDbContext();

            dbContext.Users.Add(user);

            await dbContext.SaveChangesAsync();

            return user;
        }
    }

    #endregion
}
