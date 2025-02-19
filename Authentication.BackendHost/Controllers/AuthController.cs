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

        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
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

            var token = JwtService.GenerateToken(user.Name, "Admin");
            return Ok(token);
        }


        //[Authorize]
        [HttpGet("/GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await ApplicationDb.Users.ToListAsync());
        }


    }
}
