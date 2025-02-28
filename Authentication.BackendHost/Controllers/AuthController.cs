using Authentication.BackendHost.CustomServices;
using Authentication.BackendHost.DataBase;
using Authentication.Shared.Constants;
using Authentication.Shared.Model;
using Authentication.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Authentication.BackendHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public JwtService JwtService { get; }
        public UserManager<User> UserManager { get; }
        public ApplicationDbContext ApplicationDb { get; }
        public CustomMethods CustomMethods { get; }

        public RoleManager<IdentityRole> RoleManager { get; set; }

        public AuthController(JwtService jwtService, UserManager<User> userManager, ApplicationDbContext applicationDb, RoleManager<IdentityRole> roleManager, CustomMethods customMethods)
        {
            JwtService = jwtService;
            UserManager = userManager;
            ApplicationDb = applicationDb;
            RoleManager = roleManager;
            CustomMethods = customMethods;
        }

        [HttpPost("/add")]
        public async Task<IActionResult> AddUser([FromBody] UserViewModel user)
        {
            // CREATING IDENTITY
            var identityUser = new User { UserName = user.FullName, Email = user.Email };

            // CREATING USER
            var result = await UserManager.CreateAsync(identityUser, user.Password);
            if (!result.Succeeded) return Ok(result.Errors.FirstOrDefault()?.Description);

            // ASSIGNING THE ROLE AND PERMISSION
            if (!string.IsNullOrEmpty(user.Role))
            {
                // CHECKING ROLE
                var isRoleAdded = await CustomMethods.HandlingRoleAddOrUpdate(identityUser, user);
                if (!isRoleAdded) return BadRequest(Constant.MessageForRole);

                // ASSINGING PERMISSION
                var isPermissionAdded = await CustomMethods.HandlingPermissionAddOrUpdate(identityUser, user);
                if (!isPermissionAdded) return BadRequest(Constant.MessageForPermissionError);

            }

            return Ok(result);
        }

        [HttpGet("/GetByEmail/{email}")]
        public async Task<IActionResult> GetByEmailId(string email)
        {
            // GETTING THE EXISTING USER BY EMAIL
            var ExistUser = await CustomMethods.FindUserByEmail(email);
            if (ExistUser == null) return BadRequest(Constant.MessageForUserNotFound);

            var ViewModel = new UserViewModel()
            {
                UserId = ExistUser.Id,
                Email = ExistUser.Email,
                FullName = ExistUser.UserName,
                Role = await CustomMethods.GetRoleByIdentityUser(ExistUser),
                Permissions = await CustomMethods.GetPermissionByIdentityUser(ExistUser)
            };

            return Ok(ViewModel);
        }

        [HttpPut("/update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel userViewModel)
        {
            // GETTING USER BY EMAIL
            var identityUser = await CustomMethods.FindUserByEmail(userViewModel.Email)!;
            if (identityUser == null) return NotFound(Constant.MessageForUserNotFound);

            // ASSIGNING NEW VALUES
            identityUser.UserName = userViewModel.FullName;
            identityUser.Email = userViewModel.Email;

            // UPDATING USER
            var result = await UserManager.UpdateAsync(identityUser);
            if (!result.Succeeded)
                return BadRequest(result.Errors.First().Description);

            // HANDLING PASSWORD
            if (!string.IsNullOrEmpty(userViewModel.NewPassword))
            {
                var passwordHasChanged = await CustomMethods.ChangePassword(identityUser, userViewModel);
                if (!passwordHasChanged) return BadRequest(Constant.MessageForPasswordUpdate);
            }

            // HANDLING ROLE
            if (!string.IsNullOrEmpty(userViewModel.Role))
            {
                var isRoleAdded = await CustomMethods.HandlingRoleAddOrUpdate(identityUser, userViewModel);
                if (!isRoleAdded) return BadRequest(Constant.MessageForRole);
            }

            // HANDLING PERMISSIONS
            var isPermissionUpdated = await CustomMethods.HandlingPermissionAddOrUpdate(identityUser, userViewModel);
            if (!isPermissionUpdated) return BadRequest(Constant.MessageForPermissionError);

            return Ok(result);
        }

        [HttpPost("/login")]
        public async Task<IActionResult> Login([FromBody] UserViewModel user)
        {
            // CHECKING FOR CURRENT EXIST
            var ExistUser = await CustomMethods.FindUserByEmail(user.Email);
            if (ExistUser == null) return BadRequest(Constant.MessageForUserNotFound);

            // CHECKING PASSWORD
            var (isPasswordCorrect, message) = await CustomMethods.CheckPassword(ExistUser, user);
            if (!isPasswordCorrect) return BadRequest(message);

            // CHECKING ROLE
            var role = await CustomMethods.GetRoleByIdentityUser(ExistUser);
            if (role == null || !role.Any()) return BadRequest(Constant.MessageForRole);

            // GETTING PERMISSION
            var permissions = await CustomMethods.GetPermissionByIdentityUser(ExistUser);

            // GENERATE JWT TOKEN 
            var token = JwtService.GenerateToken(ExistUser.Id, ExistUser.Email, ExistUser.UserName, role, permissions);

            user.FullName = ExistUser.UserName;
            user.token = token;
            user.Role = role;
            user.Permissions = permissions;

            return Ok(user);
        }


        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] UserViewModel user)
        {
            // CREATING IDENTITY
            var identityUser = new User { UserName = user.FullName, Email = user.Email };

            // CREATING USER
            var result = await UserManager.CreateAsync(identityUser, user.Password);
            if (!result.Succeeded) return Ok(result.Errors.FirstOrDefault()?.Description);

            // CHECKING ROLE
            var isRoleAdded = await CustomMethods.HandlingRoleAddOrUpdate(identityUser, user);
            if (!isRoleAdded) return BadRequest(Constant.MessageForRole);

            return Ok(result);
        }

        [Authorize]
        [Authorize(Policy = Permission.ManageUser)]
        [HttpGet("/GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await ApplicationDb.Users.ToListAsync();
            var result = new List<UserViewModel>();

            foreach (var user in users)
            {
                result.Add(new UserViewModel
                {
                    Email = user.Email,
                    FullName = user.UserName,
                    Role = await CustomMethods.GetRoleByIdentityUser(user),
                    Permissions = await CustomMethods.GetPermissionByIdentityUser(user),
                });
            }

            return Ok(result);
        }


    }
}