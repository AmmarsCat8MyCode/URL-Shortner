using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base (options) { }

    public DbSet<UrlContainer> URLs { get; set; }
}

public class UrlContainer
{
    public int Id { get; set; }
    public string code { get; set; }
    public string longUrl { get; set; }
    public DateTime dateCreated { get; set; }
    public DateTime? expire { get; set; }
    public int clickCount { get; set; }
}
