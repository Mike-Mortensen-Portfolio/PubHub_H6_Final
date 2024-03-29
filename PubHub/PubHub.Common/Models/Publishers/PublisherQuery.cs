namespace PubHub.Common.Models.Publishers
{
    public class PublisherQuery : PaginationQuery
    {
        public OrderPublisherBy OrderBy { get; set; }
    }
}
