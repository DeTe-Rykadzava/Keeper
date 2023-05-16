using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KeeperAPI.DataBase;
using KeeperAPI.Models;

namespace KeeperAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataBaseKeeperContext _context;

        public UserController(DataBaseKeeperContext context)
        {
            _context = context;
        }

        // GET: api/User/5
        [HttpPost("{login}&{password}")]
        public async Task<ActionResult<int>> AuthUser(string login, string password)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            password = await CreateMD5(password);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == login && x.Password == password);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.Id);
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<int>> RegisterUser(UserRegModel model)
        {
            if (_context.Users == null)
            {
              return Problem("Entity set 'DataBaseKeeperContext.Users'  is null.");
            }
            
            var user = new User()
            {
                Login = model.Login,
                Password = await CreateMD5(model.Password)            
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user.Id);
        }
        
        private async Task<string> CreateMD5(string inputStr)
        {
            using(System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(inputStr);
                byte[] hashBytes = await md5.ComputeHashAsync(new MemoryStream(inputBytes));

                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
