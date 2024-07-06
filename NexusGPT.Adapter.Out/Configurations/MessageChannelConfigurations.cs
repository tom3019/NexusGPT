using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusGPT.Entities;

namespace NexusGPT.Adapter.Out.Configurations;

public class MessageChannelConfigurations : IEntityTypeConfiguration<MessageChannel>
{
    public void Configure(EntityTypeBuilder<MessageChannel> builder)
    {
        builder.ToTable("MessageChannels");
        
        builder.Property(x=>x.Id)
            .HasColumnName("Id")
            .HasConversion(x=>x.Value,x=>x)
            .IsRequired();
        
        builder.HasIndex(x=>x.Id, "IX_MessageChannel_Id").IsUnique();

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CreateTime)
            .IsRequired();

        builder.Property(x => x.MemberId)
            .HasConversion(x => x.Value, x => x)
            .IsRequired();
        
        builder.Property(x => x.TotalQuestionTokenCount)
            .IsRequired();

        builder.Property(x => x.TotalAnswerTokenCount)
            .IsRequired();

        builder.Property(x => x.TotalTokenCount)
            .IsRequired();

        builder.OwnsMany(x => x.Messages, a =>
        {
            a.ToTable("Messages");
            
            a.Property(x=>x.Id)
                .HasColumnName("Id")
                .HasConversion(x=>x.Value,x=>x)
                .IsRequired();
            a.HasIndex(c=>c.Id).HasDatabaseName("IX_Message_Id").IsUnique();
            
            a.Property(x => x.Answer).IsRequired().HasMaxLength(2000);
            a.Property(x => x.Question).IsRequired().HasMaxLength(2000);
            a.Property(x => x.AnswerTokenCount).IsRequired();
            a.Property(x => x.QuestionTokenCount).IsRequired();
            a.Property(x => x.TotalTokenCount).IsRequired();
            a.Property(x => x.CreateTime).IsRequired();
        });
    }
}