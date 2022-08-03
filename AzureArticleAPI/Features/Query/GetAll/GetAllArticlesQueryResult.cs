namespace AzureArticleAPI.Features.Query.GetAll
{
    public class GetAllArticlesQueryResult
    {
        public List<ArticleDTO> Article { get; set; }

        public class ArticleDTO
        {
            public string Title { get; set; }

            public string Description { get; set; }

            public string FileUri { get; set; }
        }
    }
}
