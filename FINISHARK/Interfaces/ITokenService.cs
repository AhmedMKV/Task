using FINISHARK.Models;

namespace FINISHARK.Interfaces
{
    public interface ITokenService
    {

        string CreateToken(AppUser user);
    }
}
