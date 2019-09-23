using System;
using System.Threading.Tasks;
using WeStop.Api.Domain;

namespace WeStop.Api.Infra.Storages.Interfaces
{
    public interface IUserStorage
    {
        Task CreateAsync(User user);
        Task<User> GetByIdAsync(Guid id);
    }
}