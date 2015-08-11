
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PagedList;

using Lcps.Division.Directory.Providers;


using Lcps.Division.Directory.Repository;

namespace Lcps.Division.Directory.API.Areas.Manage.Models
{
    public class DirectoryMemberListModel
    {
        public bool AllowEdit { get; set; }

        public PagedList<DirectoryMember> Members { get; set; }

        public MvcRouteDefinition SourceRoute { get; set; }
    }
}