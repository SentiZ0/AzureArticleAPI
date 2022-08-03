using MediatR;

namespace AzureArticleAPI.Features.Command.Delete
{
    public class DeleteArticleCommand : IRequest<DeleteArticleCommandResult>
    {
        public int Id { get; set; }

        public DeleteArticleCommand(int id)
        {
            Id = id;
        }
    }
}
