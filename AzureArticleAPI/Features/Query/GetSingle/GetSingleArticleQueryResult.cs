namespace AzureArticleAPI.Features.Query.GetSingle
{
    public class GetSingleArticleQueryResult
    {
        public ArticleDTO Article { get; set; }

        public class ArticleDTO
        {
            public string Title { get; set; }

            public string Description { get; set; }

            public string FileUri { get; set; }
        }
    }
}
