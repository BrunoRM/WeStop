using Microsoft.AspNetCore.Mvc;
using System;

namespace WeStop.Api.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        [Route("api/users.create"), HttpPost]
        public IActionResult CreateAsync()
        {
            return Ok(new 
            {
                id = Guid.NewGuid()
            });
        }
    }
}