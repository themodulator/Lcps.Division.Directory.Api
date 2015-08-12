using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Lcps.Division.Directory.Repository;

namespace Lcps.Division.Directory.API.Areas.Manage.Models
{
    public class MembershipScopeModel
    {
        public List<MembershipScopeEditorItem> MembershipScopes { get; set; }

        public DirectoryMemberInfo Person { get; set; }
        
    }
}