using DemoServerRest.Data.Models;
using DemoServerRest.Dtos;
using DemoServerRest.Dtos.Users;
using DemoServerRest.Helpers;
using DemoServerRest.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoServerRest.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IUsersService usersService;
        private readonly JwtService jwtService;

        public AuthController(IUsersService usersService, JwtService jwtService)
        {
            this.usersService = usersService;
            this.jwtService = jwtService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            if (this.usersService.Exists(dto.Email))
            {
                return this.BadRequest(new { Message = "User with that email already exists!" });
            }

            var user = new ApplicationUser
            {
                UserName = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            };

            var result = this.usersService.Create(user);

            var jwt = this.jwtService.Generate(result.Id);

            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true,
            });

            return this.Ok(new
            {
                message = "success",
            });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = this.usersService.GetByEmail(dto.Email);

            if (user == null)
            {
                return this.BadRequest(new { Message = "Invalid Credentials!" });
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return this.BadRequest(new { Message = "Invalid Credentials!" });
            }

            var jwt = this.jwtService.Generate(user.Id);

            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true,
            });

            return this.Ok(user);
        }

        [HttpGet("user")]
        public IActionResult GetCurrentUser()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = this.jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var user = this.usersService.GetById(userId);

                return this.Ok(user);
            }
            catch (Exception ex)
            {
                return this.Ok(new ApplicationUser { Id = 0, Email = "unauthorized", UserName = "invalid", Password = "pass" });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");

            return this.Ok(new
            {
                message = "success",
            });
        }
    }
}
