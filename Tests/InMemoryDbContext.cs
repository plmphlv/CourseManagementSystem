using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class SqliteTestDb : IAsyncDisposable
{
    private readonly SqliteConnection connection;
    private readonly DbContextOptions<ApplicationDbContext> options;

    public SqliteTestDb()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); // must stay open for the DB to persist

        options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;
    }

    
    public async Task EnsureCreatedAsync(ICurrentUserService currentUser, IDateTime clock)
    {
        await using ApplicationDbContext context = CreateContext(currentUser, clock);
        await context.Database.EnsureCreatedAsync();
    }

    public ApplicationDbContext CreateContext(ICurrentUserService currentUser, IDateTime clock)
    {
        return new ApplicationDbContext(currentUser, clock, options);
    }

    public async ValueTask DisposeAsync() => await connection.DisposeAsync();

    public void SeedData() 
    {

    }
}
