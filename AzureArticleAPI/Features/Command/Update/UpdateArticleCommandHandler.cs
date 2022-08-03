using AzureArticleAPI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureArticleAPI.Features.Command.Update
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, UpdateArticleCommandResult>
    {

        private readonly ArticleDBContext _context;
        private readonly string _storageConnectionString;

        public UpdateArticleCommandHandler(ArticleDBContext context, IConfiguration configuration)
        {
            _context = context;
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task<UpdateArticleCommandResult> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
        {
            var article = await _context.Articles.Where(x => x.Id == request.Id).FirstOrDefaultAsync(x => x.Id == request.Id);

            if(article == null)
            {
                return null;
            }

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);

            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("file-container");

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(request.File.FileName);

            var blob = container.GetBlobReference(article.ImagePath);
            await blob.DeleteIfExistsAsync();

            using (var stream = request.File.OpenReadStream())
            {
                await blockBlob.UploadFromStreamAsync(stream);
            }

            article.Title = request.Title;
            article.Description = request.Description;
            article.ImagePath = request.File.FileName;

            await _context.SaveChangesAsync();

            return new UpdateArticleCommandResult();
        }
    }
}
