using SmartQuiz.Components;
using SmartQuiz.Services;
using SmartQuiz.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add Razor + Blazor support
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ✅ Register database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// ✅ Add your services
builder.Services.AddSingleton<QuizService>();

var app = builder.Build();

// ✅ Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
    app.UseHttpsRedirection(); // 👈 HTTPS only used in production
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ✅ Ensure database is created and migrate schema
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        // Check if Role column exists, if not add it
        try
        {
            var connection = context.Database.GetDbConnection();
            var wasOpen = connection.State == System.Data.ConnectionState.Open;
            if (!wasOpen)
            {
                connection.Open();
            }

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM pragma_table_info('Users') WHERE name='Role'";
            var result = command.ExecuteScalar();

            if (result != null && Convert.ToInt32(result) == 0)
            {
                // Role column doesn't exist, add it
                command.CommandText = "ALTER TABLE Users ADD COLUMN Role TEXT DEFAULT 'Student'";
                command.ExecuteNonQuery();
                Console.WriteLine("Added Role column to Users table.");
            }

            if (!wasOpen)
            {
                connection.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Could not check/update schema: {ex.Message}");

            // If schema update fails, recreate database in development
            if (app.Environment.IsDevelopment())
            {
                Console.WriteLine("Recreating database in development mode...");
                try
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    Console.WriteLine("Database recreated successfully.");
                }
                catch (Exception recreateEx)
                {
                    Console.WriteLine($"Error recreating database: {recreateEx.Message}");
                }
            }
        }

        Console.WriteLine("Database initialized successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error initializing database: {ex.Message}");
    }
}

app.Run();
