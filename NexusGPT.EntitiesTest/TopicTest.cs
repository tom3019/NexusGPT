using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NexusGPT.Entities;

namespace NexusGPT.EntitiesTest;

public class TopicTest
{
    [Fact]
    public void CreateMessageChannelTest_InputIdAndMemberId_ShouldCreateMessageChannel()
    {
        // Arrange
        var id = Guid.NewGuid();
        var memberId = new MemberId(Guid.NewGuid());
        var timeProvider = new FakeTimeProvider();
        var localDateTimeNow = new DateTimeOffset(new DateTime(2024, 05, 15));
        timeProvider.SetUtcNow(localDateTimeNow);
        
        var title = "title";
        
        // Act
        var actual = new Topic(id, memberId, title,timeProvider);

        // Assert
        actual.Id.Value.Should().Be(id);
        actual.MemberId.Value.Should().Be(memberId);
        actual.CreateTime.Should().Be(localDateTimeNow);
        actual.Title.Should().Be(title);
    }

    [Fact]
    public void AddMessageTest_InputQuestionAndAnswer_ShouldAddMessage()
    {
        // Arrange
        var id = Guid.NewGuid();
        var memberId = new MemberId(Guid.NewGuid());
        var timeProvider = new FakeTimeProvider();
        var localDateTimeNow = new DateTimeOffset(new DateTime(2024, 05, 15));
        timeProvider.SetUtcNow(localDateTimeNow);
        var title = "title";

        var messageChannel = new Topic(id, memberId, title,timeProvider);
        var messageId = new MessageId(Guid.NewGuid());


        var question = "question";
        var answer = "answer";
        var questionTokenCount = 100;
        var answerTokenCount = 100;
        var totalTokenCount = questionTokenCount + answerTokenCount;

        // Act
        messageChannel.AddMessage(messageId, question, answer, questionTokenCount, answerTokenCount,
            timeProvider);

        // Assert
        messageChannel.Messages.Should().HaveCount(1);
        messageChannel.Messages.First().Question.Should().Be(question);
        messageChannel.Messages.First().Answer.Should().Be(answer);
        messageChannel.Messages.First().Id.Value.Should().Be(messageId);
        messageChannel.Messages.First().QuestionTokenCount.Should().Be(questionTokenCount);
        messageChannel.Messages.First().AnswerTokenCount.Should().Be(answerTokenCount);
        messageChannel.Messages.First().TopicId.Should().Be(id);
        messageChannel.Messages.First().TotalTokenCount.Should().Be(totalTokenCount);

        messageChannel.TotalQuestionTokenCount.Should().Be(questionTokenCount);
        messageChannel.TotalAnswerTokenCount.Should().Be(answerTokenCount);
        messageChannel.TotalTokenCount.Should().Be(questionTokenCount + answerTokenCount);
        messageChannel.CreateTime.Should().Be(localDateTimeNow);
        messageChannel.Title.Should().Be(title);
    }
}