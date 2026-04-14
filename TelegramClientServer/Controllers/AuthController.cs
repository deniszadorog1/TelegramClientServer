using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TelegramClientServer.Interfaces;
using TelegramLib.MainClasses;
using TelegramLib.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private IHashPassword _hash;

        public AuthController(IConfiguration config, IHashPassword toHash)
        {
            _config = config;
            _hash = toHash;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] GetTokenDTO request)
        {
            var user = DbService.GetUserModelByLogin(request.Login);

            if (user is null || !_hash.Verfy(request.Password, user.Password))
            {
                return Unauthorized("Incorrect Login or Password");
            }

            var token = GenerateJwtToken(user);

            return Ok(new { Token = token });
        }

        public record GetTokenDTO
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        [HttpPut("AddUser")]
        [AllowAnonymous]
        public bool RegisterUser([FromBody] AddUserDTO newUser)
        {
            if (DbService.IsUserExist(newUser.Login)) return false;

            string hashPassword = _hash.CreateHash(newUser.Password);

            DbService.AddUser(newUser.Name, newUser.Surname, newUser.PhoneNumber,
                newUser.BirthDate, newUser.Login, hashPassword);

            return true;
        }

        [HttpPost("UpdateUser")]
        public void UpdateUser([FromBody] UserDTO user)
        {
            DbService.UpdateUser(user.User);
        }
        public class UserDTO()
        {
            public User User { get; set; }
        }

        [HttpPut("AddUserSettings")]
        [AllowAnonymous]
        public void AddUserSettings([FromBody] UserIdDTO userId)
        {
            DbService.AddSettings(userId.UserId);
        }
        public class UserIdDTO()
        {
            public int UserId { get; set; }
        };

        [HttpPut("AddUserBasicColor")]
        [AllowAnonymous]
        public void AddUserBasicColor([FromBody] UserIdDTO userId)
        {
            DbService.AddUserBasicColor(userId.UserId);
        }

        [HttpPut("AddTellSystem")] //this better
        public bool AddNewUserSystem([FromBody] AddUserDTO newUser)
        {
            if (DbService.IsUserExist(newUser.Login)) return false;

            string pasHash = _hash.CreateHash(newUser.Password);

            DbService.AddUser(newUser.Name,
                              newUser.Surname,
                              newUser.PhoneNumber,
                              newUser.BirthDate,
                              newUser.Login,
                              pasHash);
            return true;
        }

        public class AddUserDTO()
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string? Name { get; set; }
            public string? Surname { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime? BirthDate { get; set; }
        }

        [HttpGet("GetUser")]
        [AllowAnonymous]
        public User GetUser(string login, string password)
        {
            var user = DbService.GetUserModelByLogin(login);
            if (user is null) return null;

            if (user is null || !_hash.Verfy(password, user.Password)) return null;

            return DbService.GetUserByLoginAndPassword(login, user.Password);
        }
        public class GetUserParams()
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        [Authorize]
        [HttpGet("GetTelSystem")]
        public ActionResult<TelSystem> GetTellSystemByUser(/*string login, string password*/)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            // Теперь в DbService тебе нужен метод, который ищет по ID, а не по паре логин/пароль
            var system = DbService.GetTelSystem(userId);

            if (system == null) return NotFound();

            return Ok(system);
        }


        [HttpGet("GetUserByLoginPassword")]
        public User GetUserByLoginPassword(string login, string password)
        {
            return DbService.GetUserByLoginPass(login, password);
        }

        [HttpGet("IsRegistrationParamsAreExist")]
        [AllowAnonymous]
        public bool IsRegistrationParamsAreExist(string login, string phoneNumber)
        {
            return DbService.IsRegistrationParamsareExist(login, phoneNumber);
        }

        private string GenerateJwtToken(TelegramLib.Models.User user)
        {
            var keyString = _config["Jwt:Key"];

            // Если упадет здесь, значит конфигурация реально не видит ключ
            if (string.IsNullOrEmpty(keyString))
                throw new Exception("JWT Key is missing in appsettings.json!");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Токен будет жить неделю
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
