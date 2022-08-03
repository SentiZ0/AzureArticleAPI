using MediatR;

namespace AzureArticleAPI.Features.Command.Create
{
    public class CreateArticleCommand : IRequest<CreateArticleCommandResult>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile? File { get; set; }
    }
}
