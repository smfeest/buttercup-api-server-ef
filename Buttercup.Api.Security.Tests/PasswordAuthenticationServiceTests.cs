using Buttercup.Api.DbModel;
using Buttercup.Api.TestUtils;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Buttercup.Api.Security;

public sealed class PasswordAuthenticationServiceTests : IDisposable
{
    private readonly FakeDbContextFactory dbContextFactory = new();
    private readonly ListLogger<PasswordAuthenticationService> logger = new();
    private readonly Mock<IPasswordAuthenticationQueries> mockPasswordAuthenticationQueries = new();
    private readonly Mock<IPasswordHasher<User>> mockPasswordHasher = new();
    private readonly PasswordAuthenticationService passwordAuthenticationService;
    private readonly SampleDataFactory sampleDataFactory = new();

    public PasswordAuthenticationServiceTests() =>
        this.passwordAuthenticationService = new(
            this.dbContextFactory,
            this.logger,
            this.mockPasswordAuthenticationQueries.Object,
            this.mockPasswordHasher.Object);

    public void Dispose() => this.dbContextFactory.Dispose();

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

        fixture.SetupUserHasNoPassword();

        Assert.Null(await fixture.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Authentication failed; no password set for user {fixture.User.Id}");
    }

    [Fact]
    public async Task Authenticate_LogsAndReturnsNullWhenPasswordIsIncorrect()
    {
        var fixture = new AuthenticateFixture(this);

        fixture.SetupUserHasPassword(PasswordVerificationResult.Failed);

        Assert.Null(await fixture.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Authentication failed; incorrect password for user {fixture.User.Id}");
    }

    [Theory]
    [InlineData(PasswordVerificationResult.Success)]
    [InlineData(PasswordVerificationResult.SuccessRehashNeeded)]
    public async Task Authenticate_LogsAndReturnsUserOnSuccess(
        PasswordVerificationResult verificationResult)
    {
        var fixture = new AuthenticateFixture(this);

        fixture.SetupUserHasPassword(verificationResult);

        Assert.Equal(fixture.User, await fixture.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message == $"User {fixture.User.Id} successfully authenticated");
    }

    [Fact]
    public async Task Authenticate_RehashesPasswordWhenRehashNeeded()
    {
        var fixture = new AuthenticateFixture(this);

        fixture.SetupUserHasPassword(PasswordVerificationResult.SuccessRehashNeeded);

        await fixture.Authenticate();

        fixture.VerifyHashPasswordCalled(Times.Once());
    }

    [Fact]
    public async Task Authenticate_DoesNotRehashPasswordWhenRehashNotNeeded()
    {
        var fixture = new AuthenticateFixture(this);

        fixture.SetupUserHasPassword(PasswordVerificationResult.Success);

        await fixture.Authenticate();

        fixture.VerifyHashPasswordCalled(Times.Never());
    }

    [Fact]
    public async Task Authenticate_LogsReturnsUserWithUpgradedPasswordHashWhenUpgradedHashSaved()
    {
        var fixture = new AuthenticateFixture(this);

        fixture.SetupPasswordRehashNeeded(true);

        await fixture.Authenticate();

        Assert.Equal(AuthenticateFixture.UpgradedPasswordHash, fixture.User.PasswordHash);

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message == $"Upgraded password hash for user {fixture.User.Id}");
    }

    [Fact]
    public async Task Authenticate_DoesNotLogAndReturnsUserWithExistingPasswordHashWhenUpgradedHashNotSaved()
    {
        var fixture = new AuthenticateFixture(this);

        fixture.SetupPasswordRehashNeeded(false);

        await fixture.Authenticate();

        Assert.Equal(AuthenticateFixture.ExistingPasswordHash, fixture.User.PasswordHash);

        Assert.DoesNotContain(
            this.logger.Entries,
            entry => entry.Message == $"Upgraded password hash password for user {fixture.User.Id}");
    }

    private sealed class AuthenticateFixture
    {
        public const string ExistingPasswordHash = "existing-password-hash";
        public const string UpgradedPasswordHash = "new-password-hash";
        public const string Password = "user-password";
        public const string SuppliedEmail = "supplied-email@example.com";

        private readonly PasswordAuthenticationServiceTests context;

        public AuthenticateFixture(PasswordAuthenticationServiceTests context)
        {
            this.context = context;

            this.User = context.sampleDataFactory.BuildUser();

            this.context.mockPasswordHasher
                .Setup(x => x.HashPassword(this.User, Password))
                .Returns(UpgradedPasswordHash);
        }

        public User User { get; }

        public Task<User?> Authenticate() =>
            this.context.passwordAuthenticationService.Authenticate(SuppliedEmail, Password);

        public void SetupEmailNotFound() => this.SetupFindUserByEmail(null);

        public void SetupUserHasNoPassword() => this.SetupUserExists(null);

        public void SetupUserHasPassword(PasswordVerificationResult verificationResult)
        {
            this.SetupUserExists(ExistingPasswordHash);

            this.context.mockPasswordHasher
                .Setup(x => x.VerifyHashedPassword(this.User, ExistingPasswordHash, Password))
                .Returns(verificationResult);
        }

        public void SetupPasswordRehashNeeded(bool saveResult)
        {
            SetupUserHasPassword(PasswordVerificationResult.SuccessRehashNeeded);

            this.context.mockPasswordAuthenticationQueries
                .Setup(x => x.SaveUpgradedPasswordHash(
                    context.dbContextFactory.FakeDbContext,
                    this.User.Id,
                    ExistingPasswordHash,
                    UpgradedPasswordHash))
                .ReturnsAsync(saveResult);
        }

        public void VerifyHashPasswordCalled(Times times) => context.mockPasswordHasher
            .Verify(x => x.HashPassword(this.User, Password), times);

        private void SetupFindUserByEmail(User? user) =>
            this.context.mockPasswordAuthenticationQueries
                .Setup(x => x.FindUserByEmail(
                    context.dbContextFactory.FakeDbContext, SuppliedEmail))
                .ReturnsAsync(user);

        private void SetupUserExists(string? passwordHash)
        {
            this.User.PasswordHash = passwordHash;
            this.SetupFindUserByEmail(this.User);
        }
    }

    #endregion
}
