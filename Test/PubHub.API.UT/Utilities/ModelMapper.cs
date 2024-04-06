using System.Linq;
using PubHub.API.Domain.Entities;
using PubHub.Common.Models.Publishers;

namespace PubHub.API.UT.Utilities
{
    internal static class ModelMapper
    {
        #region Publisher
        public static PublisherInfoModel ToInfo(this Publisher publisher) => new()
        {
            Id = publisher.Id,
            Name = publisher.Name,
            Email = publisher.Account!.Email
        };

        public static IEnumerable<PublisherInfoModel> ToInfo(this IEnumerable<Publisher> publishers) =>
            publishers.Select(ToInfo);
        #endregion
    }
}
