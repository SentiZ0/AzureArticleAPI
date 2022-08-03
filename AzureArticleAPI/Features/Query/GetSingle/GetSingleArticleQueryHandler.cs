using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using AzureArticleAPI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureArticleAPI.Features.Query.GetSingle
{
    public class GetSingleArticleQueryHandler : IRequestHandler<GetSingleArticleQuery, GetSingleArticleQueryResult>
    {
        private readonly ArticleDBContext _context;

        private readonly string _storageConnectionString;

        private readonly string _storageAccountKey;

        public GetSingleArticleQueryHandler(ArticleDBContext context, IConfiguration configuration)
        {
            _context = context;
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
            _storageAccountKey = configuration.GetConnectionString("jkstorageaccount");
        }

        public async Task<GetSingleArticleQueryResult> Handle(GetSingleArticleQuery request, CancellationToken cancellationToken)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);

            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("file-container");

            var singleArticle = await _context.Articles.Where(x => x.Id == request.Id).FirstOrDefaultAsync();


            if (singleArticle == null)
            {
                return null;
            }

            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                BlobName = singleArticle.ImagePath,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddMinutes(5),
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential("jkstorageaccount", _storageAccountKey)).ToString();

            var article = await _context.Articles.Where(x => x.Id == request.Id).Select(x => new GetSingleArticleQueryResult.ArticleDTO
            {
                Title = x.Title,
                Description = x.Description,
                FileUri = string.IsNullOrEmpty(x.ImagePath) ? null : Convert.ToString(container.Uri) + "/" + x.ImagePath + "?" + sasToken,

            }).FirstOrDefaultAsync();

            if (article == null)
            {
                return null;
            }

            return new GetSingleArticleQueryResult
            {
                Article = article
            };
        }
    }
}
