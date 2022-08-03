using MediatR;

namespace AzureArticleAPI.Features.Command.Update
{
    public class UpdateArticleCommand : IRequest<UpdateArticleCommandResult>
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile File { get; set; }
    }
}
