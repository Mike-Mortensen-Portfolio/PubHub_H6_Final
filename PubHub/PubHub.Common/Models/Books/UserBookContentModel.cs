using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubHub.Common.Models.Books
{
    public class UserBookContentModel : BookContentModel
    {
        public required Guid AccessTypeId { get; init; }
        public required float ProgressInProcent { get; init; }
        public required DateTime AcquireDate { get; init; }
    }
}
