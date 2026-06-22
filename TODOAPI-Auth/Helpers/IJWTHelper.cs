using TODOAPI_Auth.Models;

namespace TODOAPI_Auth.Helpers
{
    public interface IJWTHelper
    {
        string GenerateToken(User user);
    }
}
