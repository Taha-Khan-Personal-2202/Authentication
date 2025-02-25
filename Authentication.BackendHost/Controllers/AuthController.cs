using Authentication.BackendHost.CustomServices;
using Authentication.BackendHost.DataBase;
using Authentication.Shared.Model;
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
        public UserManager<User> UserManager { get; }
        public SignInManager<User> SignInManager { get; set; }
        public ApplicationDbContext ApplicationDb { get; }

        public RoleManager<IdentityRole> RoleManager { get; set; }

        public AuthController(JwtService jwtService, UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext applicationDb, RoleManager<IdentityRole> roleManager)
        {
            JwtService = jwtService;
            UserManager = userManager;
            SignInManager = signInManager;
            ApplicationDb = applicationDb;
            RoleManager = roleManager;
        }

        [Authorize]
        //[Authorize(Policy = Permission.ManageUser)]
        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            Console.WriteLine($"🔍 Incoming Token: {authHeader}");


            var users = await ApplicationDb.Users.ToListAsync();

            var result = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await UserManager.GetRolesAsync(user);
                result.Add(new UserViewModel
                {
                    UserName = user.Email,
                    Email = user.Email,
                    Role = roles != null && roles.Any() ? roles.FirstOrDefault()?.ToString()! : string.Empty,
                });
            }

            return Ok(result);
        }


        [HttpGet("/GetByEmail/{email}")]
        public async Task<IActionResult> GetByEmailId(string email)
        {
            var ExistUser = await UserManager.FindByEmailAsync(email);
            if (ExistUser == null) return BadRequest("Not Found.");

            var roles = await UserManager.GetRolesAsync(ExistUser);

            var obj = new UserViewModel()
            {
                UserId = ExistUser.Id,
                UserName = ExistUser.Email,
                Email = ExistUser.Email,
                Role = roles.FirstOrDefault() ?? string.Empty
            };

            return Ok(obj);
        }


        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] UserViewModel user)
        {
            var identityUser = new User { UserName = user.Email, Email = user.Email };

            var result = await UserManager.CreateAsync(identityUser, user.Password);
            if (!result.Succeeded) return Ok(result.Errors.FirstOrDefault()?.Description);

            if (!string.IsNullOrEmpty(user.Role))
            {
                var roleExist = await RoleManager.RoleExistsAsync(user.Role);
                if (!roleExist)
                {
                    await RoleManager.CreateAsync(new IdentityRole(user.Role));
                }
                await UserManager.AddToRoleAsync(identityUser, user.Role);
            }

            return Ok(result);
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] UserViewModel user)
        {
            var AddedUser = await UserManager.FindByEmailAsync(user.Email);
            if (AddedUser == null) return BadRequest("Invalid User Name Or Password.");

            var result = await SignInManager.CheckPasswordSignInAsync(AddedUser, user.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid User Name Or Password.");

            var role = await UserManager.GetRolesAsync(AddedUser);
            if (role == null || !role.Any()) return Unauthorized("Role is not selected please select a role to login");

            // Step 2: Get User Permissions
            var userClaims = await UserManager.GetClaimsAsync(AddedUser);
            var permissions = userClaims
                .Where(c => c.Type == "Permission")
                .Select(c => c.Value)
                .ToList();

            var token = JwtService.GenerateToken(AddedUser.Id, AddedUser.UserName, role.FirstOrDefault(), permissions);

            user.Role = role.FirstOrDefault();
            user.token = token;
            user.Permissions = permissions;
            return Ok(user);
        }

        [HttpPost("/add")]
        public async Task<IActionResult> AddUser([FromBody] UserViewModel user)
        {
            var identityUser = new User { UserName = user.Email, Email = user.Email };

            var result = await UserManager.CreateAsync(identityUser, user.Password);
            if (!result.Succeeded) return Ok(result.Errors.FirstOrDefault()?.Description);

            if (!string.IsNullOrEmpty(user.Role))
            {
                var roleExist = await RoleManager.RoleExistsAsync(user.Role);
                if (!roleExist)
                {
                    await RoleManager.CreateAsync(new IdentityRole(user.Role));
                }
                await UserManager.AddToRoleAsync(identityUser, user.Role);

                if (RolePermissions.RolePermissionMaping.ContainsKey(user.Role))
                {
                    foreach (var permission in RolePermissions.RolePermissionMaping[user.Role])
                    {
                        await UserManager.AddClaimAsync(identityUser, new System.Security.Claims.Claim("Permission", permission));
                    }
                }
            }

            return Ok(result);
        }

        [HttpPut("/update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel user)
        {
            var existingUser = await UserManager.FindByIdAsync(user.UserId);
            if (existingUser == null) return NotFound("User not found.");

            existingUser.UserName = user.Email;
            existingUser.Email = user.Email;


            var result = await UserManager.UpdateAsync(existingUser);
            if (!result.Succeeded)
                return BadRequest(result.Errors.First().Description);

            if (!string.IsNullOrEmpty(user.ConfirmedPassword))
            {
                var changePassword = await UserManager.ChangePasswordAsync(existingUser, user.Password, user.ConfirmedPassword);
                if (!changePassword.Succeeded) return Ok(result.Errors.First().Description);
            }

            if (!string.IsNullOrEmpty(user.Role))
            {
                var roleExist = await RoleManager.RoleExistsAsync(user.Role);
                if (!roleExist)
                {
                    await RoleManager.CreateAsync(new IdentityRole(user.Role));
                }
                await UserManager.AddToRoleAsync(existingUser, user.Role);
            }

            return Ok(result);
        }


    }
}