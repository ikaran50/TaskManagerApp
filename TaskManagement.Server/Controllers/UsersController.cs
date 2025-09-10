using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Server.Api.Models;
using TaskManagement.Server.Data;

namespace TaskManagement.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;


    public UsersController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers([FromQuery] string status = "all")
    {
        IQueryable<User> q = _db.Users.AsNoTracking();

        q = status switch
        {
            "inactive" => q.Where(t => !t.IsActive),
            "active" => q.Where(t => t.IsActive),
            _ => q
        };

        q.OrderByDescending(t => t.CreatedAtDate);

        return Ok(await q.ToListAsync());
    }

    // GET: api/users/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> GetUser(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User dto)
    {
        var entity = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            IsActive = true,
        };

        _db.Users.Add(entity);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUser), new { id = entity.Id }, entity);
    }

    // PATCH: api/users/5/toggle
    [HttpPatch("{id:int}/toggle")]
    public async Task<ActionResult<TaskItem>> ToggleActive(int id)
    {
        var entity = await _db.Users.FindAsync(id);
        if (entity is null) return NotFound();

        entity.IsActive = !entity.IsActive;
        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    // DELETE: api/users/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var entity = await _db.Users.FindAsync(id);
        if (entity is null) return NotFound();
        _db.Users.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

