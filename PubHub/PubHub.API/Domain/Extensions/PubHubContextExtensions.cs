using Microsoft.EntityFrameworkCore;
using PubHub.API.Domain.Entities;
using PubHub.Common.Models.Publishers;
using PubHub.Common.Models.Users;

namespace PubHub.API.Domain.Extensions
{
    public static class PubHubContextExtensions
    {
        public static async Task<UserInfoModel?> GetUserInfoAsync(this PubHubContext context, Guid id) =>
            await context.Set<User>()
                .Include(u => u.Account)
                    .ThenInclude(a => a!.AccountType)
                .Select(u => new UserInfoModel()
                {
                    Id = u.Id,
                    Email = u.Account!.Email,
                    Name = u.Name,
                    Surname = u.Surname,
                    Birthday = u.Birthday,
                    AccountType = u.Account!.AccountType!.Name
                })
                .FirstOrDefaultAsync(u => u.Id == id);

        public static async Task<PublisherInfoModel?> GetPublisherInfoAsync(this PubHubContext context, Guid id) =>
            await context.Set<Publisher>()
                .Include(p => p.Account)
                .Select(p => new PublisherInfoModel()
                {
                    Id = p.Id,
                    Email = p.Account!.Email ?? string.Empty,
                    Name = p.Name
                })
                .FirstOrDefaultAsync(p => p.Id == id);
    }
}
