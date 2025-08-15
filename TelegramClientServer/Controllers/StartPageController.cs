using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text.Json;
using TelegramLib;
using TelegramLib.MainClasses;
using TelegramLib.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TelegramClientServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StartPageController : ControllerBase
    {
        [HttpPut("AddUser")]
        public bool RegisterUser([FromBody] AddUserDTO newUser)
        {
            if (DbService.IsUserExist(newUser.Login)) return false;

            DbService.AddUser(newUser.Name, newUser.Surname, newUser.PhoneNumber,
                newUser.BirthDate, newUser.Login, newUser.Password);

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
        public void AddUserSettings([FromBody] UserIdDTO userId)
        {
            DbService.AddSettings(userId.UserId);
        }
        public class UserIdDTO()
        {
            public int UserId { get; set; }
        };

        [HttpPut("AddUserBasicColor")]
        public void AddUserBasicColor([FromBody] UserIdDTO userId)
        {
            DbService.AddUserBasicColor(userId.UserId);
        }

        [HttpPut("AddTellSystem")] //this better
        public bool AddNewUserSystem([FromBody] AddUserDTO newUser)
        {
            if (DbService.IsUserExist(newUser.Login)) return false;

            DbService.AddUser(newUser.Name, newUser.Surname, newUser.PhoneNumber,
                newUser.BirthDate, newUser.Login, newUser.Password);

            return true;
        }

        public class AddUserDTO()
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string PhoneNumber { get; set; }
            public DateTime? BirthDate { get; set; }
        }

        [HttpGet("GetUser")]
        public User GetUser(string login, string password)
        {
            return DbService.GetUserByLoginAndPassword(
                login, password);
        }
        public class GetUserParams()
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        [HttpGet("GetTelSystem")]
        public TelSystem GetTellSystemByUser(string login, string password)
        {
            return DbService.GetTelSystem(login, password);
        }


        [HttpGet("GetUserByLoginPassword")]
        public User GetUserByLoginPassword(string login, string password)
        {
            return DbService.GetUserByLoginPass(login, password);
        }








        // GET: api/<StartPageController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<StartPageController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<StartPageController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<StartPageController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<StartPageController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
