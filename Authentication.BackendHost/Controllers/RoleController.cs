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
        public async Task<IActionResult> Get(int id)
        {
            var role = await RoleManager.FindByIdAsync(id.ToString());
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
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            var isExist = await RoleManager.RoleExistsAsync(value);
            if (!isExist) return BadRequest("Invalid role");

            var result = await RoleManager.UpdateAsync(new IdentityRole(value));
            if(!result.Succeeded) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isExist = await RoleManager.FindByIdAsync(id.ToString());
            if (!isExist) return BadRequest("Invalid role");


        }
    }
}
