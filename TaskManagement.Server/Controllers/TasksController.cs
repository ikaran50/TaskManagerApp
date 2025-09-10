using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TaskManagement.Server.Api.Models;
using TaskManagement.Server.Data;
using System.Collections.Generic;

namespace TaskManagement.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IMemoryCache _cache;
    private HashSet<string> cacheKeys;
    public TasksController(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _cache = cache;
        cacheKeys = new HashSet<string>();
    }

    // GET: api/tasks?query=&status=all|open|done&sort=due|created
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks(
        [FromQuery] string? query, [FromQuery] string status = "all", [FromQuery] string sort = "created")
    {
        string cacheKey = $"tasks_{query}_{status}_{sort}";
        if (string.IsNullOrWhiteSpace(query))
        {
            cacheKey = $"tasks_{status}_{sort}";
        }

        if (!_cache.TryGetValue(cacheKey, out List<TaskItem>? cachedDataTasks))
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

            cachedDataTasks = await q.ToListAsync();
            // Cache for 30 seconds if query is null
            if (string.IsNullOrWhiteSpace(query))
            {
                _cache.Set(cacheKey, cachedDataTasks, TimeSpan.FromSeconds(30));
            }
            else
            {
                _cache.Set(cacheKey, cachedDataTasks, TimeSpan.FromSeconds(5));
            }
            cacheKeys.Add(cacheKey);
        }
        return Ok(cachedDataTasks);
    }

    // GET: api/tasks/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> GetTask(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        } 

        return Ok(task);
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
        // Invalidate cache
        string cacheKey1 = $"tasks_all_created";
        string cacheKey2 = $"tasks_open_created";
        string cacheKey3 = $"tasks_done_created";

        _cache.Remove(cacheKey1);
        _cache.Remove(cacheKey2);
        _cache.Remove(cacheKey3);

        cacheKeys.Remove(cacheKey1);
        cacheKeys.Remove(cacheKey2);
        cacheKeys.Remove(cacheKey3);

        await _db.SaveChangesAsync();

        cacheKeys.Clear();
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
        // Invalidate cache
        string cacheKey1 = $"tasks_all_created";
        string cacheKey2 = $"tasks_open_created";
        string cacheKey3 = $"tasks_done_created";

        _cache.Remove(cacheKey1);
        _cache.Remove(cacheKey2);
        _cache.Remove(cacheKey3);

        cacheKeys.Remove(cacheKey1);
        cacheKeys.Remove(cacheKey2);
        cacheKeys.Remove(cacheKey3);

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
        // Invalidate cache
        string cacheKey1 = $"tasks_all_created";
        string cacheKey2 = $"tasks_open_created";
        string cacheKey3 = $"tasks_done_created";

        _cache.Remove(cacheKey1);
        _cache.Remove(cacheKey2);
        _cache.Remove(cacheKey3);

        cacheKeys.Remove(cacheKey1);
        cacheKeys.Remove(cacheKey2);
        cacheKeys.Remove(cacheKey3);

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
        // Invalidate cache
        string cacheKey1 = $"tasks_all_created";
        string cacheKey2 = $"tasks_open_created";
        string cacheKey3 = $"tasks_done_created";

        _cache.Remove(cacheKey1);
        _cache.Remove(cacheKey2);
        _cache.Remove(cacheKey3);

        cacheKeys.Remove(cacheKey1);
        cacheKeys.Remove(cacheKey2);
        cacheKeys.Remove(cacheKey3);

        await _db.SaveChangesAsync();
        return NoContent();
    }
}