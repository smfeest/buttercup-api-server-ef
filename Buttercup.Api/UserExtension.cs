using Buttercup.Api.DbModel;

namespace Buttercup.Api;

[ExtendObjectType(typeof(User), IgnoreProperties = new[] { nameof(User.PasswordHash) })]
public class UserExtension
{
}
