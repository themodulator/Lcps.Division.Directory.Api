using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Lcps.Division.Directory.Infrastructure;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Lcps.Division.Directory.API.Infrastructure
{
    public class LcpsApiRepositoryContext : LcpsRepositoryContext
    {
        public LcpsApiRepositoryContext()
            : base(GetConnectionString())
        {
        }

        public static string GetConnectionString()
        {
            string defaultCnxn = System.Configuration.ConfigurationManager.AppSettings["CurrentConnectionString"];
            return System.Configuration.ConfigurationManager.ConnectionStrings[defaultCnxn].ConnectionString;
        }

    }
}