using Buttercup.Api.DbModel;
using Buttercup.Api.TestUtils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Buttercup.Api.Security;

public class PasswordAuthenticationServiceTests
{
    private readonly ListLogger<PasswordAuthenticationService> logger = new();
    private readonly SampleDataFactory modelFactory = new();
    private readonly Mock<IPasswordHasher<User>> mockPasswordHasher = new();
    private readonly PasswordAuthenticationService passwordAuthenticationService;

    public PasswordAuthenticationServiceTests() =>
        this.passwordAuthenticationService = new(
            Mock.Of<IDbContextFactory<AppDbContext>>(),
            this.logger,
            this.mockPasswordHasher.Object);

    #region Authenticate

    [Fact]
    public async Task Authenticate_LogsAndReturnsNullWhenEmailNotFound()
    {
        var fixture = new AuthenticateFixture(this);

        fixture.SetupEmailNotFound();

        Assert.Null(await fixture.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Authentication failed; no user with email {AuthenticateFixture.SuppliedEmail}");
    }

    [Fact]
    public async Task Authenticate_LogsAndReturnsNullWhenUserHasNoPassword()
    {
        var fixture = new AuthenticateFixture(this);

        var user = fixture.SetupUserHasNoPassword();

        Assert.Null(await fixture.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Authentication failed; no password set for user {user.Id}");
    }

    [Fact]
    public async Task Authenticate_LogsAndReturnsNullWhenPasswordIsIncorrect()
    {
        var fixture = new AuthenticateFixture(this);

        var user = fixture.SetupUserHasPassword(PasswordVerificationResult.Failed);

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
        var fixture = new AuthenticateFixture(this);

        var user = fixture.SetupUserHasPassword(verificationResult);

        Assert.Equal(user, await fixture.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message == $"User {user.Id} successfully authenticated");
    }

    private class AuthenticateFixture
    {
        public const string Password = "user-password";
        public const string PasswordHash = "password-hash";
        public const string SuppliedEmail = "supplied-email@example.com";

        private readonly PasswordAuthenticationServiceTests context;

        public AuthenticateFixture(PasswordAuthenticationServiceTests context) =>
            this.context = context;

        public Task<User?> Authenticate() =>
            this.context.passwordAuthenticationService.Authenticate(SuppliedEmail, Password);

        public void SetupEmailNotFound() => this.SetupFindUserByEmail(null);

        public User SetupUserHasNoPassword() => this.SetupUserExists(null);

        public User SetupUserHasPassword(PasswordVerificationResult verificationResult)
        {
            var user = this.SetupUserExists(PasswordHash);

            context.mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(user, PasswordHash, Password))
                .Returns(verificationResult);

            return user;
        }

        private void SetupFindUserByEmail(User? user) =>
            throw new NotImplementedException();
        // this.context.mockUserDataProvider
        //     .Setup(x => x.FindUserByEmail(context.mySqlConnection, SuppliedEmail))
        //     .ReturnsAsync(user);

        private User SetupUserExists(string? passwordHash)
        {
            var user = this.context.modelFactory.BuildUser();
            user.PasswordHash = passwordHash;

            this.SetupFindUserByEmail(user);

            return user;
        }
    }

    #endregion
}
