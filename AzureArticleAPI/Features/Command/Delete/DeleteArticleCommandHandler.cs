using AzureArticleAPI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureArticleAPI.Features.Command.Delete
{
    public class DeleteArticleCommandHandler : IRequestHandler<DeleteArticleCommand, DeleteArticleCommandResult>
    {
        private readonly ArticleDBContext _context;
        private readonly string _storageConnectionString;

        public DeleteArticleCommandHandler(ArticleDBContext context, IConfiguration configuration)
        {
            _context = context;
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
        }
        public async Task<DeleteArticleCommandResult> Handle(DeleteArticleCommand request, CancellationToken cancellationToken)
        {
            var article = await _context.Articles.Where(x => x.Id == request.Id).FirstOrDefaultAsync(x => x.Id == request.Id);

            if (article == null)
            {
                return null;
            }

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);

            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("file-container");

            var blob = container.GetBlobReference(article.ImagePath);
            await blob.DeleteIfExistsAsync();

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return new DeleteArticleCommandResult();
        }
    }
}
