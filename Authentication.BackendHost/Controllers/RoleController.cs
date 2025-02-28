using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Authentication.BackendHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        public RoleManager<IdentityRole> RoleManager { get; }

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            RoleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var roles = RoleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            var isExist = await RoleManager.RoleExistsAsync(value);
            if (isExist) return BadRequest("Role is already added.");

            var result = await RoleManager.CreateAsync(new IdentityRole(value));
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] string value)
        {
            var isExist = await RoleManager.FindByIdAsync(id);
            if (isExist == null) return BadRequest($"{value} Role does not exist.");

            var result = await RoleManager.UpdateAsync(new IdentityRole(value));
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await RoleManager.FindByIdAsync(id.ToString());
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

    }
}
