using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Identity.Models
{
    public class UserListModel
    {
        public IEnumerable<UserSummaryModel> Users { get; set; }
    }
}
