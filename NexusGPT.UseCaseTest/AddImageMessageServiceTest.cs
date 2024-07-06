using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NexusGPT.Entities;
using NexusGPT.SeedWork;
using NexusGPT.UseCase;
using NexusGPT.UseCase.Exceptions;
using NexusGPT.UseCase.Port.In.AddMessage;
using NexusGPT.UseCase.Port.Out;
using NSubstitute;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels.ResponseModels;
using OpenAI.ObjectModels.ResponseModels.ImageResponseModel;

namespace NexusGPT.UseCaseTest;

public class AddImageMessageServiceTest
{
    private readonly IOpenAIService _openAiService;
    private readonly ITopicOutPort _topicOutPort;
    private readonly IMessageOutPort _messageOutPort;
    private readonly IImageStorageOutPort _imageStorageOutPort;
    private readonly IDomainEventBus _domainEventBus;
    private readonly FakeTimeProvider _timeProvider;

    public AddImageMessageServiceTest()
    {
        _openAiService = Substitute.For<IOpenAIService>();
        _topicOutPort = Substitute.For<ITopicOutPort>();
        _messageOutPort = Substitute.For<IMessageOutPort>();
        _imageStorageOutPort = Substitute.For<IImageStorageOutPort>();
        _domainEventBus = Substitute.For<IDomainEventBus>();
        _timeProvider = new FakeTimeProvider();
        
    }

    private IAddImageMessageService GetSystemUnderTest()
    {
        return new AddImageMessageService(_openAiService, 
            _topicOutPort, 
            _messageOutPort,
            _imageStorageOutPort, 
            _domainEventBus,
            _timeProvider);
    }
    
    [Fact]
    public async Task HandlerAsync_WhenMessageChannelIsNull_ThrowMessageChannelNotFoundException()
    {
        // Arrange
        var input = new AddImageMessageInput
        {
            TopicId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            Message = "Test"
        };
        _topicOutPort.GetAsync(input.TopicId, input.MemberId).Returns(Topic.Null);
        var service = GetSystemUnderTest();
        
        // Act
        Func<Task> act = async () => await service.HandlerAsync(input);
        
        // Assert
        await act.Should().ThrowAsync<TopicNotFoundException>();
    }
    
    [Fact]
    public async Task HandlerAsync_WhenImageCreateResponseIsNotSuccessful_ThrowImageCreateException()
    {
        // Arrange
        var input = new AddImageMessageInput
        {
            TopicId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            Message = "Test"
        };
        var messageChannel = new Topic(input.TopicId, input.MemberId, "title",_timeProvider);
        _topicOutPort.GetAsync(input.TopicId, input.MemberId).Returns(messageChannel);
        var imageCreateResponse = new ImageCreateResponse
        {
            Error = new Error()
        };
        _openAiService.Image.CreateImage(Arg.Any<ImageCreateRequest>()).Returns(imageCreateResponse);
        var service = GetSystemUnderTest();
        
        // Act
        Func<Task> act = async () => await service.HandlerAsync(input);
        
        // Assert
        await act.Should().ThrowAsync<ImageCreateException>();
    }
    
    [Fact]
    public async Task HandlerAsync_WhenImageCreateResponseIsSuccessful_ReturnMarkDownFilePath()
    {
        // Arrange
        var input = new AddImageMessageInput
        {
            TopicId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            Message = "Test"
        };
        var messageChannel = new Topic(input.TopicId, input.MemberId, "title",_timeProvider);
        _topicOutPort.GetAsync(input.TopicId, input.MemberId).Returns(messageChannel);
        var imageCreateResponse = new ImageCreateResponse
        {
            Results = new List<ImageCreateResponse.ImageDataResult>
            {
               new ImageCreateResponse.ImageDataResult
               {
                   B64 = "imageSteam"
               }
            }
        };
        _openAiService.Image.CreateImage(Arg.Any<ImageCreateRequest>()).Returns(imageCreateResponse);

        var imageUrl = "https://image.com";
        _imageStorageOutPort.SaveObjectAsync("imageSteam").Returns(imageUrl);
        _messageOutPort.GenerateIdAsync().Returns(Guid.NewGuid());
        _topicOutPort.UpdateAsync(messageChannel).Returns(true);
        
        var service = GetSystemUnderTest();
        
        // Act
        var act = await service.HandlerAsync(input);
        
        // Assert
        var mdUrl = $"![Test]({imageUrl})";
        act.Should().Be(mdUrl);
        _domainEventBus.Received(1).DispatchDomainEventsAsync(Arg.Any<Topic>());
        
    }
    
    [Fact]
    public async Task HandlerAsync_WhenUpdateMessageChannelFailed_ThrowCreateMessageErrorException()
    {
        // Arrange
        var input = new AddImageMessageInput
        {
            TopicId = Guid.NewGuid(),
            MemberId = Guid.NewGuid(),
            Message = "Test"
        };
        var messageChannel = new Topic(input.TopicId, input.MemberId, "title",_timeProvider);
        _topicOutPort.GetAsync(input.TopicId, input.MemberId).Returns(messageChannel);
        var imageCreateResponse = new ImageCreateResponse
        {
            Results = new List<ImageCreateResponse.ImageDataResult>
            {
                new ImageCreateResponse.ImageDataResult
                {
                    B64 = "imageSteam"
                }
            }
        };
        _openAiService.Image.CreateImage(Arg.Any<ImageCreateRequest>()).Returns(imageCreateResponse);

        var imageUrl = "https://image.com";
        _imageStorageOutPort.SaveObjectAsync("imageSteam").Returns(imageUrl);
        _messageOutPort.GenerateIdAsync().Returns(Guid.NewGuid());
        _topicOutPort.UpdateAsync(messageChannel).Returns(false);
        
        var service = GetSystemUnderTest();
        
        // Act
        Func<Task> act = async () => await service.HandlerAsync(input);
        
        // Assert
        await act.Should().ThrowAsync<CreateMessageErrorException>();
    }

}