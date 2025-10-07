using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using KpaComplianceTracker.Data;
using KpaComplianceTracker.Entities;

namespace KpaComplianceTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly KpaDbContext _db;
    private readonly ILogger<TasksController> _logger;

    public TasksController(KpaDbContext db, ILogger<TasksController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET /api/tasks
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var tasks = await _db.Tasks
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} tasks at {Time}", tasks.Count, DateTime.UtcNow);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch tasks");
            return Problem("Failed to fetch tasks", statusCode: 500);
        }
    }

    // PUT /api/tasks/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ComplianceTask input)
    {
        // validate request
        if (input is null)
        {
            _logger.LogWarning("Update rejected: body was null for id {Id}", id);
            return BadRequest("Request body is required.");
        }

        if (input.Id != Guid.Empty && input.Id != id)
        {
            _logger.LogWarning("Update rejected: route id {RouteId} != body id {BodyId}", id, input.Id);
            return BadRequest("Route id and body id must match.");
        }

        if (string.IsNullOrWhiteSpace(input.Title))
        {
            _logger.LogWarning("Update rejected: missing title for id {Id}", id);
            return BadRequest("Title is required.");
        }

        try
        {
            var t = await _db.Tasks.FindAsync(id);
            if (t is null)
            {
                _logger.LogInformation("Task not found for id {Id}", id);
                return NotFound();
            }

            // Map all fields except S3 key
            t.Title = input.Title.Trim();
            t.Category = input.Category;
            t.Site = input.Site;
            t.Owner = input.Owner;
            t.DueDate = input.DueDate;
            t.Status = input.Status;
            t.UpdatedAt = DateTimeOffset.UtcNow;

            try
            {
                await _db.SaveChangesAsync();
                _logger.LogInformation("Task {Id} updated at {Time}", id, DateTime.UtcNow);
                return Ok(t);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating task {Id}", id);
                return Problem("The task was modified by someone else. Please refresh and try again.", statusCode: 409);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB error updating task {Id}", id);
                return Problem("A database error occurred while saving the task.", statusCode: 500);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error updating task {Id}", id);
            return Problem("Unexpected error.", statusCode: 500);
        }
    }
}
