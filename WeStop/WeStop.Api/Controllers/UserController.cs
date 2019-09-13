using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeStop.Api.Domain;
using WeStop.Api.Dtos;
using WeStop.Api.Infra.Storages.Interfaces;

namespace WeStop.Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserStorage _users;

        public UserController(IUserStorage userStorage)
        {
            _users = userStorage;
        }

        [Route("api/users.create"), HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody]CreateUserDto dto)
        {
            User user = new User(dto.UserName);
            await _users.CreateAsync(user);

            return Ok(new 
            {
                ok = true,
                user
            });
        }
    }
}