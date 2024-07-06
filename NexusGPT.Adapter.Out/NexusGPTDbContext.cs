using Microsoft.EntityFrameworkCore;
using NexusGPT.Adapter.Out.Configurations;
using NexusGPT.Entities;

namespace NexusGPT.Adapter.Out;

/// <summary>
/// NexusGPTDbContext
/// </summary>
public class NexusGptDbContext : DbContext
{
    public NexusGptDbContext(DbContextOptions<NexusGptDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// OnModelCreating
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.ApplyConfiguration(new TopicConfigurations());
    }


    /// <summary>
    /// Gets or sets the MessageChannels.
    /// </summary>
    /// <value>
    /// The MessageChannels.
    /// </value>
    public virtual DbSet<Topic> Topics { get; set; }
}