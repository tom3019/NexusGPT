using Microsoft.EntityFrameworkCore;
using NexusGPT.Adapter.Out.Configurations;
using NexusGPT.Entities;

namespace NexusGPT.Adapter.Out;

public class MessageChannelDbContext : DbContext
{
    public MessageChannelDbContext(DbContextOptions<MessageChannelDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.ApplyConfiguration(new MessageChannelConfigurations());
    }


    /// <summary>
    /// Gets or sets the MessageChannels.
    /// </summary>
    /// <value>
    /// The MessageChannels.
    /// </value>
    public virtual DbSet<MessageChannel> MessageChannels { get; set; }
}