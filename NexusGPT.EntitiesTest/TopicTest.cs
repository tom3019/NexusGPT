using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NexusGPT.Entities;
using NexusGPT.Entities.Exceptions;

namespace NexusGPT.EntitiesTest;

public class TopicTest
{
    [Fact]
    public void CreateTopicTest_InputIdAndMemberIdAndTitle_ShouldCreateTopic()
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
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void CreateTopicTest_InputNullTitle_ShouldThrowTopicDomainException(string title)
    {
        // Arrange
        var id = Guid.NewGuid();
        var memberId = new MemberId(Guid.NewGuid());
        var timeProvider = new FakeTimeProvider();
        var localDateTimeNow = new DateTimeOffset(new DateTime(2024, 05, 15));
        timeProvider.SetUtcNow(localDateTimeNow);
        
        // Act
        Action act = () => new Topic(id, memberId, title,timeProvider);

        // Assert
        act.Should().Throw<TopicDomainException>();
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
    
    [Fact]
    public void ChangeTitleTest_InputNewTitle_ShouldChangeTitle()
    {
        // Arrange
        var id = Guid.NewGuid();
        var memberId = new MemberId(Guid.NewGuid());
        var timeProvider = new FakeTimeProvider();
        var localDateTimeNow = new DateTimeOffset(new DateTime(2024, 05, 15));
        timeProvider.SetUtcNow(localDateTimeNow);
        var title = "title";

        var messageChannel = new Topic(id, memberId, title,timeProvider);
        var newTitle = "newTitle";

        // Act
        messageChannel.ChangeTitle(newTitle);

        // Assert
        messageChannel.Title.Should().Be(newTitle);
    }
}