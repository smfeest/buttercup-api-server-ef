using Buttercup.Api.DbModel;

namespace Buttercup.Api;

/// <summary>
/// Configures the GraphQL representation of the <see cref="User"/> type.
/// </summary>
[ExtendObjectType(typeof(User), IgnoreProperties = new[] { nameof(User.PasswordHash) })]
public class UserExtension
{
}
