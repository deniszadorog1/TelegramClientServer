using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
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
        public User GetUser([FromBody] GetUserParams userParams)
        {
            return DbService.GetUserByLoginAndPassword(
                userParams.Login, userParams.Password);
        }
        public class GetUserParams()
        {
            public string Login { get; set; }
            public string Password { get; set; }
        }

        [HttpGet("GetTelSystem")]
        public TelSystem GetTellSystemByUser([FromBody] GetUserParams userParam)
        {
            return DbService.GetTelSystem(userParam.Login, userParam.Password);
        }

        //Update social
        //Get contact by phone number
        //Update user contacts (Add new contact)
        


        //Update system
        //Updates all settings types
        //Add chosen contacts in privacy 








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
