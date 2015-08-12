using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using PagedList;
using Lcps.Division.Directory.Repository;
using Lcps.Division.Directory.Infrastructure;
using Lcps.Division.Directory.Providers.Ldap;
using Anvil.Repository;
using Lcps.Division.Directory.Providers;
using System.DirectoryServices;

namespace Lcps.Division.Directory.API.Areas.LDAP.Controllers
{
    public class SyncController : Controller
    {
        public const string varname = "LdapSyncMembers";

        private List<DirectoryMemberInfo> all;
        private System.Type _membershipScopeEnum;

        private GenericRepository<DirectoryMemberInfo> repo = new GenericRepository<DirectoryMemberInfo>(new LcpsRepositoryContext());


        public List<DirectoryMemberInfo> AllMembers
        {
            get
            {
                if (all == null)
                    all = repo.Get().OrderBy(x => x.SurName + x.GivenName).ToList();
                return all;
            }
        }

        public System.Type MembershipScopeEnum
        {
            get
            {
                if (_membershipScopeEnum == null)
                    _membershipScopeEnum = MembershipScopeRepository.GetEnumType();

                return _membershipScopeEnum;
            }
        }

        // GET: LDAP/Sync
        public ActionResult Index(int? page, long? filter)
        {
            List<DirectoryMemberInfo> mm = null;

            page = (page == null) ? 1 : page;

            if (filter == null)
                mm = AllMembers;
            else
                mm = AllMembers.Where(x => MembershipScopeRepository.HasFlag(x.MembershipScope, filter.Value, MembershipScopeEnum)).ToList();


            DirectoryEntry domain = new DirectoryEntry("LDAP://lcps.local", "earlyms", "Lcp$-pw1");
            DirectoryEntry parentou = null;
            string adminUserName = "earlyms";
            string adminPw = "Lcp$-pw1";
            string principalDomain = "lcps.local";
            string studentOu = "LDAP://OU=LCPS-Students,OU=LCPS-Accounts,DC=lcps,DC=local";
            string staffOu = "OU=LCPS-Staff,OU=LCPS-Accounts,DC=lcps,DC=local";
            List<string> groups = new List<string>();


            List<LdapUser> ll = new List<LdapUser>();
            foreach (DirectoryMemberInfo m in mm)
            {
                List<MembershipScope> scopes = MembershipScopeRepository.GetApplicableScopes(m.MembershipScope);
                bool IsStudent = (scopes.Where(x => x.ScopeQualifier == MembershipScopeQualifier.Grade).Count() > 0);
                bool IsStaff = (scopes.Where(x => x.ScopeQualifier == MembershipScopeQualifier.Position).Count() > 0);

                if (IsStudent)
                {
                    parentou = new DirectoryEntry(studentOu, adminUserName, adminPw);
                    
                    groups.Add("LDAP://CN=Palo Alto Secondary Student Group,OU=LCPS-PaloAlto,DC=lcps,DC=local");
                    
                    if (MembershipScopeRepository.HasFlag(m.MembershipScope, "Lunenburg Middle", MembershipScopeEnum))
                        groups.Add("CN=LCPS-LMS-Student Group,OU=LCPS-Students,OU=LCPS-Accounts,DC=lcps,DC=local");

                    if (MembershipScopeRepository.HasFlag(m.MembershipScope, "Central High", MembershipScopeEnum))
                        groups.Add("CN=CHS-Students,OU=LCPS-Students,OU=LCPS-Accounts,DC=lcps,DC=local");
                }

                if(IsStudent)
                    parentou = new DirectoryEntry(staffOu, adminUserName, adminPw);


                LdapUser u = new LdapUser(parentou, domain, m)
                        {
                            AllowPasswordChange = false,
                            PrincipalDomain = principalDomain,
                            AdminPassword = adminPw,
                            AdminUserName = adminUserName,
                            IntendedGroups = groups.ToArray()
                        };

                u.GetSyncStatus();

                ll.Add(u);
            }

            ViewBag.Total = ll.Count();
            ViewBag.Page = page.Value;
            ViewBag.Insert = ll.Where(x => x.SyncStatus == ImportSyncStatus.Insert).Count();
            ViewBag.Current = ll.Where(x => x.SyncStatus == ImportSyncStatus.Current).Count();
            ViewBag.Update = ll.Where(x => x.SyncStatus == ImportSyncStatus.Update).Count();
            ViewBag.Error = ll.Where(x => x.SyncStatus == ImportSyncStatus.Error).Count();

            PagedList<LdapUser> pg = new PagedList<LdapUser>(ll, page.Value, 12);

            return View(pg);
        }
    }
}