using Authentication.BackendHost.CustomServices;
using Authentication.BackendHost.DataBase;
using Authentication.Shared.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Authentication.BackendHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        public CustomMethods CustomMethods { get; }
        public ApplicationDbContext ApplicationDb { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        public RoleController(RoleManager<IdentityRole> roleManager, CustomMethods customMethods, ApplicationDbContext applicationDb)
        {
            RoleManager = roleManager;
            CustomMethods = customMethods;
            ApplicationDb = applicationDb;
        }

        [HttpGet("/GetAllRoles")]
        public IActionResult Get()
        {
            var roles = RoleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpGet("/GetRoleById/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            RoleViewModel roleViewModel = new RoleViewModel();

            roleViewModel.Id = role.Id;
            roleViewModel.Name = role.Name;
            roleViewModel.ConcurrencyStamp = role.ConcurrencyStamp;
            roleViewModel.NormalizedName = role.NormalizedName;
            roleViewModel.PermissionIds = await CustomMethods.GetAssignPermission(role.Id);

            return Ok(roleViewModel);
        }

        [HttpPost("/AddRoles")]
        public async Task<IActionResult> Post([FromBody] RoleViewModel model)
        {
            var isExist = await RoleManager.RoleExistsAsync(model.Name);
            if (isExist) return BadRequest("Role is already added.");

            var identityRole = new IdentityRole(model.Name);

            var result = await RoleManager.CreateAsync(identityRole);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            foreach (var permissionId in model.PermissionIds)
            {
                ApplicationDb.RolePermissions.Add(new RolePermissionModel
                {
                    RoleId = identityRole.Id,
                    PermissionId = permissionId
                });
            }

            await ApplicationDb.SaveChangesAsync();
            return Ok(result);
        }

        [HttpPut("/Update/{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] RoleViewModel model)
        {
            var isExist = await RoleManager.FindByIdAsync(id);
            if (isExist == null) return BadRequest($"{model.Name} Role does not exist.");

            isExist.Name = model.Name;

            var result = await RoleManager.UpdateAsync(isExist);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var alreadyAddedPermission = ApplicationDb.RolePermissions.Where(w => w.RoleId == isExist.Id).ToList();
            ApplicationDb.RolePermissions.RemoveRange(alreadyAddedPermission);

            foreach (var permissionId in model.PermissionIds)
            {
                ApplicationDb.RolePermissions.Add(new RolePermissionModel
                {
                    RoleId = isExist.Id,
                    PermissionId = permissionId
                });
            }

            await ApplicationDb.SaveChangesAsync();

            return Ok(result);
        }

        [HttpDelete("/Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound($"Role with ID {id} does not exist.");

            try
            {
                var result = await RoleManager.DeleteAsync(role);
                if (!result.Succeeded)
                    return BadRequest("Failed to delete the role.");

                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"An error occurred while deleting the role: {e.Message}");
            }
        }

        [HttpGet("/GetAllPermissions")]
        public IActionResult GetAllPermissions()
        {
            var permission = ApplicationDb.Permissions.ToList();
            return Ok(permission);
        }


    }
}
