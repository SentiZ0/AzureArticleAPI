using Azure.Storage;
using Azure.Storage.Sas;
using AzureArticleAPI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AzureArticleAPI.Features.Query.GetAll
{
    public class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, GetAllArticlesQueryResult>
    {
        private readonly ArticleDBContext _context;
        private readonly string _storageConnectionString;
        private readonly string _storageAccountKey;

        public GetAllArticlesQueryHandler(ArticleDBContext context, IConfiguration configuration)
        {
            _context = context;
            _storageConnectionString = configuration.GetConnectionString("AzureStorage");
            _storageAccountKey = configuration.GetConnectionString("jkstorageaccount");
        }

        public async Task<GetAllArticlesQueryResult> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
        {

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(_storageConnectionString);

            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference("file-container");

            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                //BlobName = singleArticle.ImagePath,
                Resource = "c",
                StartsOn = DateTimeOffset.UtcNow,
                ExpiresOn = DateTime.UtcNow.AddMinutes(5),
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential("jkstorageaccount", _storageAccountKey)).ToString();

            var articles = await _context.Articles.Select(x => new GetAllArticlesQueryResult.ArticleDTO
            {
                Title = x.Title,
                Description = x.Description,
                FileUri = string.IsNullOrEmpty(x.ImagePath) ? null : Convert.ToString(container.Uri) + "/" + x.ImagePath + "?" + sasToken,
            }).ToListAsync();

            if (articles == null)
            {
                return null;
            }

            return new GetAllArticlesQueryResult
            {
                Article = articles
            };
        }
    }
}
