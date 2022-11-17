using api.primerparcial.Dto;

namespace api.parcial1.Interfaces;

public interface IUserService
{
    Task<User> GetByCredentials(string username, string password);
}