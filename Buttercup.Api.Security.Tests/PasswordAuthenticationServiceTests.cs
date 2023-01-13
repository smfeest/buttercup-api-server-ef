using Buttercup.Api.DbModel;
using Buttercup.Api.TestUtils;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Buttercup.Api.Security;

public sealed class PasswordAuthenticationServiceTests : IDisposable
{
    private const string SuppliedEmail = "supplied-email@example.com";
    private const string SuppliedPassword = "user-password";
    private const string ExistingPasswordHash = "existing-password-hash";
    private const string UpgradedPasswordHash = "new-password-hash";

    private readonly FakeDbContextFactory dbContextFactory = new();
    private readonly ListLogger<PasswordAuthenticationService> logger = new();
    private readonly Mock<IPasswordAuthenticationServiceQueries> mockQueries = new();
    private readonly Mock<IPasswordHasher<User>> mockPasswordHasher = new();
    private readonly PasswordAuthenticationService passwordAuthenticationService;
    private readonly SampleDataFactory sampleDataFactory = new();

    public PasswordAuthenticationServiceTests() =>
        this.passwordAuthenticationService = new(
            this.dbContextFactory,
            this.logger,
            this.mockQueries.Object,
            this.mockPasswordHasher.Object);

    public void Dispose() => this.dbContextFactory.Dispose();

    private Task<User?> Authenticate() =>
        this.passwordAuthenticationService.Authenticate(SuppliedEmail, SuppliedPassword);

    private void SetupEmailNotFound() => this.SetupFindUserByEmail(null);

    private User SetupUserHasNoPassword() => this.SetupUserExists(null);

    private User SetupUserHasPassword(PasswordVerificationResult verificationResult)
    {
        var user = this.SetupUserExists(ExistingPasswordHash);

        this.mockPasswordHasher
            .Setup(x => x.VerifyHashedPassword(user, ExistingPasswordHash, SuppliedPassword))
            .Returns(verificationResult);

        return user;
    }

    private User SetupPasswordRehashNeeded(bool saveResult)
    {
        var user = this.SetupUserHasPassword(PasswordVerificationResult.SuccessRehashNeeded);

        this.mockQueries
            .Setup(x => x.SaveUpgradedPasswordHash(
                this.dbContextFactory.FakeDbContext,
                user.Id,
                ExistingPasswordHash,
                UpgradedPasswordHash))
            .ReturnsAsync(saveResult);

        this.mockPasswordHasher
            .Setup(x => x.HashPassword(user, SuppliedPassword))
            .Returns(UpgradedPasswordHash);

        return user;
    }

    private void SetupFindUserByEmail(User? user) =>
        this.mockQueries
            .Setup(x => x.FindUserByEmail(
                this.dbContextFactory.FakeDbContext, SuppliedEmail))
            .ReturnsAsync(user);

    private User SetupUserExists(string? passwordHash)
    {
        var user = this.sampleDataFactory.BuildUser() with { PasswordHash = passwordHash };

        this.SetupFindUserByEmail(user);

        return user;
    }

    private void VerifyHashPasswordCalled(User user, Times times) =>
        this.mockPasswordHasher
            .Verify(x => x.HashPassword(user, SuppliedPassword), times);

    #region Authenticate

    [Theory]
    [InlineData(PasswordVerificationResult.Success)]
    [InlineData(PasswordVerificationResult.SuccessRehashNeeded)]
    public async Task Authenticate_OnSuccess_LogsAndReturnsUser(
        PasswordVerificationResult verificationResult)
    {
        var user = this.SetupUserHasPassword(verificationResult);

        Assert.Equal(user, await this.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message == $"User {user.Id} successfully authenticated");
    }

    [Fact]
    public async Task Authenticate_WhenEmailNotFound_LogsAndReturnsNull()
    {
        this.SetupEmailNotFound();

        Assert.Null(await this.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Authentication failed; no user with email {SuppliedEmail}");
    }

    [Fact]
    public async Task Authenticate_WhenUserHasNoPassword_LogsAndReturnsNull()
    {
        var user = this.SetupUserHasNoPassword();

        Assert.Null(await this.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Authentication failed; no password set for user {user.Id}");
    }

    [Fact]
    public async Task Authenticate_WhenPasswordIsIncorrect_LogsAndReturnsNull()
    {
        var user = this.SetupUserHasPassword(PasswordVerificationResult.Failed);

        Assert.Null(await this.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Authentication failed; incorrect password for user {user.Id}");
    }

    [Fact]
    public async Task Authenticate_WhenPasswordHashNeedsUpgrading_RehashesPassword()
    {
        var user = this.SetupUserHasPassword(PasswordVerificationResult.SuccessRehashNeeded);

        await this.Authenticate();

        this.VerifyHashPasswordCalled(user, Times.Once());
    }

    [Fact]
    public async Task Authenticate_WhenPasswordHashDoesNotNeedUpgrading_DoesNotRehashPassword()
    {
        var user = this.SetupUserHasPassword(PasswordVerificationResult.Success);

        await this.Authenticate();

        this.VerifyHashPasswordCalled(user, Times.Never());
    }

    [Fact]
    public async Task Authenticate_WhenRehashedPasswordSaved_LogsAndReturnsUserWithUpgradedHash()
    {
        var user = this.SetupPasswordRehashNeeded(true);

        Assert.Equal(user with { PasswordHash = UpgradedPasswordHash }, await this.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message == $"Password hash upgraded for user {user.Id}");
    }

    [Fact]
    public async Task Authenticate_WhenRehashedPasswordNotSaved_LogsAndReturnsUserWithExistingHash()
    {
        var user = this.SetupPasswordRehashNeeded(false);

        Assert.Equal(user with { PasswordHash = ExistingPasswordHash }, await this.Authenticate());

        Assert.Contains(
            this.logger.Entries,
            entry => entry.Message ==
                $"Password hash not upgraded for user {user.Id}; concurrent change detected");
    }

    #endregion
}
