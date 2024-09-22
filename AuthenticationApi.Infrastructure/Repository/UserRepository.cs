using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductCatalog.SharedLibrary.Responses;
using AuthenticationApi.Application.Interface;
using ProductCatalog.SharedLibrary.Logs;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AuthenticationApi.Infrastructure.Repository
{
    public class UserRepository(AuthenticationDbContext _dbContext, IConfiguration _config) : IUser
    {
        private async Task<AppUsers> GetAppUserByEmail(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user is null ? null! : user!;
        }

        public async Task<GetUserDTO> GetUserById(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            return user is not null ? new GetUserDTO(
                user.Id,
                user.Name,
                user.Address,
                user.Phonenumber,
                user.Email,
                user.Role
                ) : null!;
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            try
            {
                var getUser = await GetAppUserByEmail(loginDTO.Email);
                if (getUser is null)
                    return new Response(false, "Invalid Login Details");

                bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password, getUser.Password);

                if (!verifyPassword)

                    return new Response(false, "Password Invalid");
                string token = GenerateToken(getUser);
                return new Response(true, token);
            }
            catch (Exception ex)
            {
                LogExceptions.LogException(ex);
                return new Response(false, "Invalid Login Details");
            }
        }

        private string GenerateToken(AppUsers getUser)
        {
            var key = Encoding.UTF8.GetBytes(_config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, getUser.Name),
                new (ClaimTypes.Email, getUser.Email),
                new (ClaimTypes.Role, getUser.Role)
            };

            //Assign defualt role to user
            if (string.IsNullOrEmpty(getUser.Role) || !Equals("string", getUser.Role))
                claims.Add(new(ClaimTypes.Role, getUser.Role!));
            var token = new JwtSecurityToken(
                issuer: _config["Autjentication:Issure"],
                audience: _config["Authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<Response> Register(AppUsersDTO appUsersDTO)
        {
            var _getUser = await GetAppUserByEmail(appUsersDTO.Email);
            if (_getUser is not null)
                return new Response(false, $"this Email {_getUser.Email} Already Exist");

            var result = _dbContext.Users.Add(new AppUsers()
            {
                Name = appUsersDTO.Name,
                Email = appUsersDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUsersDTO.Password),
                Address = appUsersDTO.Address,
                Role = appUsersDTO.Role,
                Phonenumber = appUsersDTO.PhoneNumber,
            });
            await _dbContext.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response(true, "Registered Successfully") :
                new Response(false, "Invalid Inputs please try again");
        }
    }
}