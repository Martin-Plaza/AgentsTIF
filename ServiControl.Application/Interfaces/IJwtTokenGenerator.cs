using ServiControl.Domain.Entities;

namespace ServiControl.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Usuario usuario);
}
