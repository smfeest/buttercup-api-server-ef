using Buttercup.Api.DbModel;
using Microsoft.EntityFrameworkCore;

namespace Buttercup.Api.Security;

/// <summary>
/// The default implementation of <see cref="ISessionManager" />.
/// </summary>
public sealed class SessionManager : ISessionManager
{
    private readonly IClock clock;
    private readonly IDbContextFactory<AppDbContext> dbContextFactory;

    /// <summary>
    /// Initializes a new instance of the the <see cref="PasswordAuthenticationService" /> class.
    /// </summary>
    /// <param name="clock">The clock.</param>
    /// <param name="dbContextFactory">The database context factory.</param>
    public SessionManager(IClock clock, IDbContextFactory<AppDbContext> dbContextFactory)
    {
        this.clock = clock;
        this.dbContextFactory = dbContextFactory;
    }

    /// <inheritdoc/>
    public async Task<Session> CreateSession(User user)
    {
        var timestamp = this.clock.UtcNow;

        var session = new Session
        {
            User = user,
            Created = timestamp,
            CurrentTokensIssued = timestamp,
        };

        using (var dbContext = await dbContextFactory.CreateDbContextAsync())
        {
            dbContext.Sessions.Add(session);
            await dbContext.SaveChangesAsync();
        }

        return session;
    }
}
