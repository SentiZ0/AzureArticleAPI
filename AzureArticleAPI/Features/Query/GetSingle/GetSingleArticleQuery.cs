using MediatR;

namespace AzureArticleAPI.Features.Query.GetSingle
{
    public class GetSingleArticleQuery : IRequest<GetSingleArticleQueryResult>
    {
        public int Id { get; set; }

        public GetSingleArticleQuery(int id)
        {
            Id = id;
        }
    }
}
