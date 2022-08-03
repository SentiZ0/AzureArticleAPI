using AzureArticleAPI.Features.Command.Create;
using AzureArticleAPI.Features.Command.Delete;
using AzureArticleAPI.Features.Command.Update;
using AzureArticleAPI.Features.Query.GetAll;
using AzureArticleAPI.Features.Query.GetSingle;
using AzureArticleAPI.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AzureArticleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ArticleController(IMediator mediator)
        {
            _mediator = mediator;
        }    

        [HttpPost]
        public async Task<ActionResult> CreateArticle([FromForm]CreateArticleCommand command)
        {
            var response = await _mediator.Send(command);

            if(response == null)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetSingleArticle(int id)
        {
            var query = new GetSingleArticleQuery(id);
            var response = await _mediator.Send(query);

            if(response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllArticles()
        {
            var response = await _mediator.Send(new GetAllArticlesQuery());

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteArticle(int id)
        {
            var query = new DeleteArticleCommand(id);
            var response = await _mediator.Send(query);

            if (response == null)
            {
                return BadRequest();
            }

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult> ModifyArticle([FromForm]UpdateArticleCommand command)
        {
            var repsonse = await _mediator.Send(command);

            if (repsonse == null)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
