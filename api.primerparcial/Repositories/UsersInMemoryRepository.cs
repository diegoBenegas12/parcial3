using api.primerparcial.Interfaces;
using api.primerparcial.Dto;

namespace api.primerparcial.Repositories;

public class UsersInMemoryRepository : IUserRepository
{
    private readonly List<User> _users = new List<User>
    {
        new()
        {
            Id       = 1,
            Fullname = "Diego Benegas",
            Username = "diegobe",
            Password = "123456"
        }
    };

    public async Task<User?> GetUserByCredentials(string username, string password)
    {
        return _users.FirstOrDefault(p => p.Username.Equals(username) && p.Password.Equals(password));
    }
}
