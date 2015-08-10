using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Lcps.Division.Directory.API.Infrastructure;
using Lcps.Division.Directory.Repository;
using Lcps.Division.Directory.Infrastructure;


namespace Lcps.Division.Directory.API.Areas.API.Controllers
{
    public class MembershipScopesController : ApiController
    {
        private DirectoryContext db = new DirectoryContext();

        // GET: api/MembershipScopeApi
        public IEnumerable<MembershipScope> GetMembershipScopes()
        {
            return db.MembershipScopes.Get().ToList();
        }

        // GET: api/MembershipScopeApi/5
        [ResponseType(typeof(MembershipScope))]
        public IHttpActionResult GetMembershipScope(Guid id)
        {
            MembershipScope membershipScope = db.MembershipScopes.GetByID(id);
            if (membershipScope == null)
            {
                return NotFound();
            }

            return Ok(membershipScope);
        }

        // PUT: api/MembershipScopeApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMembershipScope(Guid id, MembershipScope membershipScope)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != membershipScope.MembershipScopeId)
            {
                return BadRequest();
            }



            try
            {
                db.MembershipScopes.Update(membershipScope);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MembershipScopeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/MembershipScopeApi
        [ResponseType(typeof(MembershipScope))]
        public IHttpActionResult PostMembershipScope(MembershipScope membershipScope)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                db.MembershipScopes.Insert(membershipScope);
            }
            catch (DbUpdateException)
            {
                if (MembershipScopeExists(membershipScope.MembershipScopeId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = membershipScope.MembershipScopeId }, membershipScope);
        }

        // DELETE: api/MembershipScopeApi/5
        [ResponseType(typeof(MembershipScope))]
        public IHttpActionResult DeleteMembershipScope(Guid id)
        {
            MembershipScope membershipScope = db.MembershipScopes.GetByID(id);
            if (membershipScope == null)
            {
                return NotFound();
            }

            db.MembershipScopes.Delete(membershipScope);


            return Ok(membershipScope);
        }


        private bool MembershipScopeExists(Guid id)
        {
            return db.MembershipScopes.Get(e => e.MembershipScopeId == id).Count() > 0;
        }
    }
}