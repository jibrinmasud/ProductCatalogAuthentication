using AuthenticationApi.Application.DTOs;
using ProductCatalog.SharedLibrary.Responses;

namespace AuthenticationApi.Application.Interface
{
    public interface IUser
    {
        Task<Response> Register(AppUsersDTO appUsersDTO);

        Task<Response> Login(LoginDTO loginDTO);

        Task<GetUserDTO> GetUserById(int userId);
    }
}