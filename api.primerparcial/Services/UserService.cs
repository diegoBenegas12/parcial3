using api.parcial1.Dto;
using api.parcial1.Interfaces;

namespace api.parcial1.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetByCredentials(string username, string password)
    {
        return await _userRepository.GetUserByCredentials(username, password);
    }
}