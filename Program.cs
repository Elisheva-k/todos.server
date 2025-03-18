using Microsoft.EntityFrameworkCore;

using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"), 
    new MySqlServerVersion(new Version(8, 0, 41)))); // ודא שאתה משתמש בגרסה הנכונה של MySQL

var app = builder.Build();

app.UseCors("AllowAllOrigins");

app.MapGet("", () => 
{
    return "שלום";
});


app.MapGet("/Item", async (ToDoDbContext db) => 
{
    return await db.Items.ToListAsync();
});

app.MapGet("/Item/{id}", async (int id,ToDoDbContext db) => 
{
    return await db.Items.FindAsync(id);
});

app.MapPost("/Item", async (Item newItem, ToDoDbContext db) => 
{
    db.Items.Add(newItem);
    await db.SaveChangesAsync();
    return "Item created successfully.";
});

app.MapPut("/Item/{id}", async (int id, Item updatedItem, ToDoDbContext db) => 
{
    var existingItem = await db.Items.FindAsync(id);
    if (existingItem == null)
    {
        return "Item not found.";
    }

    existingItem.IsComplete = updatedItem.IsComplete;
    await db.SaveChangesAsync();
    return $"Item {id} updated successfully.";
});

app.MapDelete("/Item/{id}", async (int id, ToDoDbContext db) => 
{
    var existingItem = await db.Items.FindAsync(id);
    if (existingItem == null)
    {
        return "Item not found.";
    }

    db.Items.Remove(existingItem);
    await db.SaveChangesAsync();
    return $"Item {id} deleted successfully.";
});


app.Run();
