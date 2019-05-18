using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeStop.Api.Classes;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Infra.Storages
{
    public class UserStorage : IUserStorage
    {
        private readonly ICollection<User> _users;

        public UserStorage()
        {
            _users = new List<User>();
        }

        public Task CreateAsync(User user) =>
            Task.Run(() => _users.Add(user));

        public Task<User> GetByIdAsync(Guid id) =>
            Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
    }
}