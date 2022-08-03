using MediatR;

namespace AzureArticleAPI.Features.Query.GetAll
{
    public class GetAllArticlesQuery : IRequest<GetAllArticlesQueryResult>
    {
    }
}
