using backend.dto.response;
using backend.entity;


namespace backend.service;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokens(User user);
    Task<TokenResponse> RefreshToken(string refreshToken);
    Task RevokeToken(string refreshToken);
}