using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.DTO;
using MinimalApi.Models;
using MinimalApi.Repository;
using MinimalApi.Services;
using System.Security.Claims;

namespace MinimalApi.Endpoints.GroupExtensions;

public static class AuthGroupExtention
{
    public static RouteGroupBuilder MapAuthGroup(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (UserRegistrationModel model, UserManager<ApplicationUser> userManager) =>
        {

            try
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    CreatedDate = DateTime.UtcNow,

                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return Results.BadRequest(result.Errors);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
                throw;
            }

        }).AllowAnonymous();

        group.MapPost("/login", async (UserLoginModel model, JwtTokenService jwtTokenService, UserManager<ApplicationUser> userManager ,IConfiguration _configuration) =>
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                return Results.Unauthorized();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("FullName", user.FullName)
            };

            var accessToken = jwtTokenService.GenerateAccessToken(claims);

            #region Refresh Token
            var refreshToken = jwtTokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenValidityInDays);
            await userManager.UpdateAsync(user);
            #endregion

            return Results.Ok(new RefreshTokenModel
            {
                AccessToken = accessToken,
                RefreshToken= refreshToken
            });
        }).AllowAnonymous();

        group.MapPost("/refresh-token", async (RefreshTokenModel tokenModel, IConfiguration _configuration, JwtTokenService jwtTokenService, UserManager<ApplicationUser> userManager) =>
        {
            if (tokenModel is null)
            {
                return Results.BadRequest("Invalid client request");
            }

            string? accessToken = tokenModel.AccessToken;
            string? refreshToken = tokenModel.RefreshToken;

            var principal = jwtTokenService.GetPrincipalFromExpiredToken(accessToken);
            if (principal == null && principal.Identity != null)
            {
                return Results.BadRequest("Invalid access token or refresh token");
            }
            string username = principal.Identity.Name;
            var user = await userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Results.BadRequest("Invalid access token or refresh token");
            }

            var newAccessToken = jwtTokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = jwtTokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await userManager.UpdateAsync(user);

            return Results.Ok(new RefreshTokenModel
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }).AllowAnonymous();

        group.MapPost("/revoke/{username}", async (string username, UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null) return Results.BadRequest("Invalid user name");

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await userManager.UpdateAsync(user);

            return Results.NoContent();

        }).RequireAuthorization();

        group.MapPost("/revoke-all", async (UserManager<ApplicationUser> userManager) =>
        {
            var users = userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await userManager.UpdateAsync(user);
            }
            return Results.NoContent();

        }).RequireAuthorization();


        return group;
    }
}
