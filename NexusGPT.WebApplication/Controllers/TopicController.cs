using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NexusGPT.UseCase.Port.In;
using NexusGPT.WebApplication.Hubs;
using NexusGPT.WebApplication.Infrastructure.ExceptionFilters;
using NexusGPT.WebApplication.Models.Parameters;
using NexusGPT.WebApplication.Models.ResultViewModel;

namespace NexusGPT.WebApplication.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
[Consumes("application/json")]
[MessageChannelNotFoundExceptionFilter]
public class TopicController : ControllerBase
{
    private readonly ICreateTopicService _createTopicService;
    private readonly IChangeTitleService _changeTitleService;
    private readonly IDeleteTopicService _deleteTopicService;
    private readonly ITopicQueryService _topicQueryService;
    private readonly IHubContext<TopicHub> _hubContext;
    private readonly IImportTopicService _importTopicService;

    public TopicController(ICreateTopicService createTopicService,
        IChangeTitleService changeTitleService,
        IDeleteTopicService deleteTopicService,
        ITopicQueryService topicQueryService,
        IHubContext<TopicHub> hubContext,
        IImportTopicService importTopicService)
    {
        _createTopicService = createTopicService;
        _changeTitleService = changeTitleService;
        _deleteTopicService = deleteTopicService;
        _topicQueryService = topicQueryService;
        _hubContext = hubContext;
        _importTopicService = importTopicService;
    }

    /// <summary>
    /// 建立聊天室
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    [HttpPost]
    [ProducesResponseType<ResultViewModel<Guid>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ResultViewModel<Guid>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ResultViewModel<Guid>>(StatusCodes.Status404NotFound)]
    [MessageChannelMaxCountExceptionFilter]
    public async Task<IActionResult> CreateAsync([FromBody] TopicParameter parameter)
    {
        var memberId = new Guid("E4727ED6-52E8-4C4C-AF92-2ED42ECF1D59");
        var topicId = await _createTopicService.HandlerAsync(memberId, parameter.Title);
        if (topicId == Guid.Empty)
        {
            return BadRequest(new ResultViewModel<Guid>
            {
                StatuesCode = 400,
                StatusMessage = "建立失敗",
                Data = Guid.Empty
            });
        }

        await _hubContext.Clients.User(memberId.ToString()).SendAsync("TopicCreateResult",
            new ResultViewModel<object>
            {
                StatuesCode = 200,
                StatusMessage = "OK",
                Data = new
                {
                    TopicId = topicId,
                    Title = parameter.Title
                }
            });

        return Ok(new ResultViewModel<Guid>
        {
            StatuesCode = 200,
            StatusMessage = "OK",
            Data = topicId
        });
    }

    /// <summary>
    /// 變更聊天室名稱
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="parameter">The parameter.</param>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType<ResultViewModel<bool>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTitleAsync([FromRoute] Guid id, [FromBody] TopicParameter parameter)
    {
        var memberId = new Guid("E4727ED6-52E8-4C4C-AF92-2ED42ECF1D59");
        var success = await _changeTitleService.HandleAsync(id, memberId, parameter.Title);
        if (!success)
        {
            return BadRequest(new ResultViewModel<bool>
            {
                StatuesCode = 400,
                StatusMessage = "變更失敗",
                Data = false
            });
        }

        await _hubContext.Clients.User(memberId.ToString()).SendAsync("TopicTitleUpdateResult",
            new ResultViewModel<object>
            {
                StatuesCode = 200,
                StatusMessage = "OK",
                Data = new
                {
                    TopicId = id,
                    Title = parameter.Title
                }
            });

        return Ok(new ResultViewModel<bool>
        {
            StatuesCode = 200,
            StatusMessage = "OK",
            Data = true
        });
    }

    /// <summary>
    /// 刪除聊天室
    /// </summary>
    /// <param name="id">The identifier.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType<ResultViewModel<bool>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        var memberId = new Guid("E4727ED6-52E8-4C4C-AF92-2ED42ECF1D59");
        var success = await _deleteTopicService.HandleAsync(id, memberId);

        if (!success)
        {
            return BadRequest(new ResultViewModel<bool>
            {
                StatuesCode = 400,
                StatusMessage = "刪除失敗",
                Data = false
            });
        }

        await _hubContext.Clients.User(memberId.ToString()).SendAsync("TopicDeleteResult",
            new ResultViewModel<object>
            {
                StatuesCode = 200,
                StatusMessage = "OK",
                Data = new
                {
                    TopicId = id,
                }
            });

        return Ok(new ResultViewModel<bool>
        {
            StatuesCode = 200,
            StatusMessage = "OK",
            Data = true
        });
    }

    /// <summary>
    /// 取得聊天室列表
    /// </summary>
    [HttpGet]
    [ProducesResponseType<ResultViewModel<IEnumerable<TopicResultViewModel>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListAsync()
    {
        var memberId = new Guid("E4727ED6-52E8-4C4C-AF92-2ED42ECF1D59");
        var messageChannels = await _topicQueryService.GetListAsync(memberId);

        var viewModel = messageChannels.Select(x =>
            new TopicResultViewModel
            {
                Id = x.Id,
                Title = x.Title,
                CreateTime = x.CreateTime,
                LastMessageCreateTime = x.LastMessageCreateTime
            }).OrderByDescending(x => x.LastMessageCreateTime);

        return Ok(new ResultViewModel<IEnumerable<TopicResultViewModel>>
        {
            StatuesCode = 200,
            StatusMessage = "OK",
            Data = viewModel
        });
    }

    /// <summary>
    /// 取得聊天室、下載聊天室
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<ResultViewModel<TopicDetailViewModel>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDetailAsync(Guid id)
    {
        var memberId = new Guid("E4727ED6-52E8-4C4C-AF92-2ED42ECF1D59");
        var messageChannel = await _topicQueryService.GetDetailAsync(id, memberId);

        var viewModel = new TopicDetailViewModel
        {
            TopicId = messageChannel.Id,
            Title = messageChannel.Title,
            Messages = messageChannel.Messages.Select(x => new TopicMessageViewModel
            {
                MessageId = x.Id,
                Question = x.Question,
                Answer = x.Answer,
                QuestionTokenCount = x.QuestionTokenCount,
                AnswerTokenCount = x.AnswerTokenCount,
                TotalTokenCount = x.TotalTokenCount,
                CreateTime = x.CreateTime.DateTime
            }).OrderByDescending(x => x.CreateTime),
            CreateTime = messageChannel.CreateTime.DateTime
        };

        return Ok(new ResultViewModel<TopicDetailViewModel>
        {
            StatuesCode = 200,
            StatusMessage = "OK",
            Data = viewModel
        });
    }

    /// <summary>
    /// 上傳聊天室
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [HttpPost("import")]
    [ProducesResponseType<ResultViewModel<Guid>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> ImportAsync(ImportTopicParameter parameter)
    {
        var memberId = new Guid("E4727ED6-52E8-4C4C-AF92-2ED42ECF1D59");
        var channelId = await _importTopicService.HandlerAsync(parameter.TopicId, memberId);
        if (channelId == Guid.Empty)
        {
            return BadRequest(new ResultViewModel<Guid>
            {
                StatuesCode = 400,
                StatusMessage = "建立失敗",
                Data = Guid.Empty
            });
        }

        await _hubContext.Clients.User(memberId.ToString()).SendAsync("TopicImportResult",
            new ResultViewModel<object>
            {
                StatuesCode = 200,
                StatusMessage = "OK",
                Data = new
                {
                    TopicId = Guid.NewGuid(),
                    Title = parameter.Title
                }
            });

        return Ok(new ResultViewModel<Guid>
        {
            StatuesCode = 200,
            StatusMessage = "OK",
            Data = Guid.NewGuid()
        });
    }

    /// <summary>
    /// 搜尋聊天室
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns></returns>
    [HttpGet("search")]
    [ProducesResponseType<ResultViewModel<IEnumerable<SearchTopicViewModel>>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchAsync([FromQuery]SearchTopicParameter parameter)
    {
        var memberId = new Guid("E4727ED6-52E8-4C4C-AF92-2ED42ECF1D59");
        var searchMessageChannelDataModels =
            await _topicQueryService.SearchTopicAsync(memberId, parameter.Keyword);

        var viewModels =
            searchMessageChannelDataModels.Select(x =>
                new SearchTopicViewModel
                {
                    TopicId = x.Id,
                    MessageId = x.MessageIds
                });
        
        return Ok(new ResultViewModel<IEnumerable<SearchTopicViewModel>>
        {
            StatuesCode = 200,
            StatusMessage = "OK",
            Data = viewModels
        });
    }
}