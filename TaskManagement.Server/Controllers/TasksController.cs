using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Server.Api.Models;
using TaskManagement.Server.Data;

namespace TaskManagement.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/tasks?query=&status=all|open|done&sort=due|created
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks(
        [FromQuery] string? query, [FromQuery] string status = "all", [FromQuery] string sort = "created")
    {
        IQueryable<TaskItem> q = _db.Tasks.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.ToLower();
            q = q.Where(t => t.Title.ToLower().Contains(term) || (t.Description != null && t.Description.ToLower().Contains(term)));
        }

        q = status switch
        {
            "open" => q.Where(t => !t.IsCompleted),
            "done" => q.Where(t => t.IsCompleted),
            _ => q
        };

        q = sort switch
        {
            "due" => q.OrderBy(t => t.DueDate),
            _ => q.OrderByDescending(t => t.CreatedAtDate)
        };

        return Ok(await q.ToListAsync());
    }

    // GET: api/tasks/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> GetTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        return task is null ? NotFound() : Ok(task);
    }

    // POST: api/tasks
    [HttpPost]
    public async Task<ActionResult<TaskItem>> CreateTask(TaskItem dto)
    {
        var entity = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            DueDate = dto.DueDate,
            IsCompleted = dto.IsCompleted,
            UserId = null,
        };

        var curDate = DateTime.UtcNow;

        if (entity.DueDate < curDate)
        {
            return Problem(
               statusCode: StatusCodes.Status500InternalServerError,
               title: "Invalid Due Date",
               detail: "The provided dueDate must be greater than current Date."
           );
        }

        _db.Tasks.Add(entity);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new { id = entity.Id }, entity);
    }

    // PUT: api/tasks/4
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTaskUser(int id, TaskItem dto)
    {
        var entity = await _db.Tasks.FindAsync(id);
        if (entity is null) return NotFound();
        entity.UserId = dto.UserId;
        entity.UserName = dto.UserName;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PATCH: api/tasks/5/toggle
    [HttpPatch("{id:int}/toggle")]
    public async Task<ActionResult<TaskItem>> ToggleComplete(int id)
    {
        var entity = await _db.Tasks.FindAsync(id);
        if (entity is null) return NotFound();

        entity.IsCompleted = !entity.IsCompleted;
        await _db.SaveChangesAsync();
        return Ok(entity);
    }

    // DELETE: api/tasks/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var entity = await _db.Tasks.FindAsync(id);
        if (entity is null) return NotFound();

        _db.Tasks.Remove(entity);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}