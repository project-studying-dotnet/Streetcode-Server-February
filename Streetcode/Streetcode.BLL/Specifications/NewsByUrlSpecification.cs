using Ardalis.Specification;
using Streetcode.DAL.Entities.News;

namespace Streetcode.BLL.Specifications;

public class NewsByUrlSpecification : Specification<News>
{
    public NewsByUrlSpecification(string url)
    {
        Query.Where(news => news.URL == url)
             .Include(news => news.Image);
    }
}