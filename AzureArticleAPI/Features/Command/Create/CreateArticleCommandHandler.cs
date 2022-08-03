using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using AzureArticleAPI.Models;
using MediatR;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;

namespace AzureArticleAPI.Features.Command.Create
{
    public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, CreateArticleCommandResult>
    {

        private readonly ArticleDBContext _context;
        private readonly string _storageConnectionString;

        public CreateArticleCommandHandler(ArticleDBContext context, IConfiguration configuration)
        {
            _context = context;
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task<CreateArticleCommandResult> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            var article = new Article();

            article.Title = request.Title;
            article.Description = request.Description;
            
            if (request.File != null)
            {
                var checkType = request.File.ContentType;

                if(checkType.Contains("image"))
                {
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);

                    CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

                    CloudBlobContainer container = blobClient.GetContainerReference("file-container");

                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(request.File.FileName);

                    using (var stream = request.File.OpenReadStream())
                    {
                        await blockBlob.UploadFromStreamAsync(stream);
                    }

                    article.ImagePath = request.File.FileName;
                }
                else
                {
                    return null;
                }
            }

            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return new CreateArticleCommandResult();
        }
    }
}
