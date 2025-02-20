using Authentication.BackendHost.CustomServices;
using Authentication.BackendHost.DataBase;
using Authentication.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Authentication.BackendHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public JwtService JwtService { get; }
        public UserManager<IdentityUser> UserManager { get; }
        public SignInManager<IdentityUser> SignInManager { get; set; }
        public ApplicationDbContext ApplicationDb { get; }

        public AuthController(JwtService jwtService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext applicationDb)
        {
            JwtService = jwtService;
            UserManager = userManager;
            SignInManager = signInManager;
            ApplicationDb = applicationDb;
        }

        //[Authorize]
        [HttpGet("/GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await ApplicationDb.Users.ToListAsync();
            var result = users.Select(user => new User
            {
                UserName = user.Email,
                Email = user.Email,
            });
            
            return Ok(result);
        }

        [HttpGet("/GetByEmail/{email}")]
        public async Task<IActionResult> GetByEmailId(string email)
        {
            var ExistUser = await UserManager.FindByEmailAsync(email);
            if (ExistUser == null) return BadRequest("Not Found.");

            var obj = new User()
            {
                UserId = ExistUser.Id,
                UserName = ExistUser.Email,
                Email = ExistUser.Email,
            };

            return Ok(obj);
        }


        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var identityUser = new IdentityUser { UserName = user.Email, Email = user.Email };
            var result = await UserManager.CreateAsync(identityUser, user.Password);

            if (result.Succeeded) return Ok(result);

            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var AddedUser = await UserManager.FindByEmailAsync(user.Email);
            if (AddedUser == null) return BadRequest("Invalid User Name Or Password.");

            var result = await SignInManager.CheckPasswordSignInAsync(AddedUser, user.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid User Name Or Password.");

            var token = JwtService.GenerateToken(AddedUser.UserName, "Admin");
            return Ok(token);
        }

        [HttpPost("/add")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            var identityUser = new IdentityUser { UserName = user.Email, Email = user.Email };
            var result = await UserManager.CreateAsync(identityUser, user.Password);

            if (result.Succeeded) return Ok(result);

            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }

        [HttpPut("/update")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            var existingUser = await UserManager.FindByIdAsync(user.UserId);
            if (existingUser == null) return NotFound("User not found.");

            existingUser.UserName = user.Email;
            existingUser.Email = user.Email;

            var result = await UserManager.UpdateAsync(existingUser);
            if (result.Succeeded) return Ok(result);

            return BadRequest(result.Errors.FirstOrDefault()?.Description);
        }


    }
}
