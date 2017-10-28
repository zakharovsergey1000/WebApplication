using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Security;
using WebApplication.Models;
using WebApplication.Models.CodeFirst;
using Microsoft.AspNet.Identity;

namespace WebApplication.Api.Controllers
{
    [Authorize]
    public class LabelsController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Labels
        public LabelDTO[] GetLabels()
        {
            //var user = Membership.GetUser(User.Identity.Name);
            string id = User.Identity.GetUserId();
            var contacts = from b in db.Labels
                           where b.ApplicationUserId == id
                           select b;

            Label[] entityArray = contacts.ToArray();

            LabelDTO[] listItemList = new LabelDTO[entityArray.Count()];
            for (int i = 0; i < entityArray.Count(); i++)
            {
                listItemList[i] = new LabelDTO(entityArray[i]);
            }

            return listItemList;

            //ContactDTO[] listItemList = new ContactDTO[tasks.Count()];
            //int i = 0;
            //foreach (Contact c in tasks)
            //{
            //    listItemList[i] = new ContactDTO(tasks.g[i]);
            //    i++;
            //}

            //return listItemList;
        }
        // GET: api/Labels
        public LabelDTO[] GetLabels(String date)
        {
            //var user = Membership.GetUser(User.Identity.Name);
            DateTimeOffset date2 = DateTimeOffset.Parse(date);
            string id = User.Identity.GetUserId();
            Label[] entityArray = (from b in db.Labels
                                   where (b.ApplicationUserId == id /*&& b.LastMod.UtcTicks / 10000 * 10000 > date2.UtcTicks*/)
                                  select b).ToArray();

           // Label[] entityArray = tasks.ToArray();

            LabelDTO[] listItemList = new LabelDTO[entityArray.Count()];
            for (int i = 0; i < entityArray.Count(); i++)
            {
                listItemList[i] = new LabelDTO(entityArray[i]);
            }

            return listItemList;

            //ContactDTO[] listItemList = new ContactDTO[tasks.Count()];
            //int i = 0;
            //foreach (Contact c in tasks)
            //{
            //    listItemList[i] = new ContactDTO(tasks.g[i]);
            //    i++;
            //}

            //return listItemList;
        }

        // GET: api/Labels/5
        [ResponseType(typeof(Label))]
        public async Task<IHttpActionResult> GetLabel(int id)
        {
            string userId = User.Identity.GetUserId();
            Label label = await db.Labels.FindAsync(id);
            if (label == null)
            {
                return NotFound();
            }
            if (label.ApplicationUserId != userId)
            {
                return Unauthorized(null);
            }

            return Ok(label);
        }

        // PUT: api/Labels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLabel(int id, Label label)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != label.ID)
            {
                return BadRequest();
            }

            db.Entry(label).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LabelExists(id))
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

        // POST: api/Labels
        [ResponseType(typeof(LabelDTO))]
        public async Task<IHttpActionResult> PostLabel(LabelDTO label)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // now the user is authenticated

            Label newEntity = null;
            if (label.ID != 0)
            {
                return BadRequest(ModelState);
            }
            else
            {
                newEntity = new Label();
                db.Labels.Add(newEntity);
            }

            Label parentEntity = null;
            if (label.ParentID != 0)
            {
                parentEntity = db.Labels.Where(b => b.ID == label.ParentID).SingleOrDefault();

                if (parentEntity == null)
                {
                    //dbTransaction.Rollback();
                    //     return new SetEntityResponse(SetEntityResult.ParentEntityIsNotFound, null,/*"The requested parent remind was not found on the server",*/ entityDTO.ID, null, null, DateTime.UtcNow);
                    return BadRequest(ModelState);
                }

                if (parentEntity.Deleted && !label.Deleted)
                {
                    //dbTransaction.Rollback();
                    return BadRequest(ModelState);
                    //return new SetEntityResponse(SetEntityResult.ParentEntityIsDeleted, null,/*"The requested parent remind was deleted on the server",*/ entityDTO.ID, null, null, DateTime.UtcNow);
                }
            }

            // verify field consistency

            if (label.ID == 0 && label.Deleted)
            {
                // dbTransaction.Rollback();
                //return new SetEntityResponse(SetEntityResult.EntityHasIncorrectData, "entityDTO.ID == 0 && entityDTO.Deleted", 0, null, null, DateTime.UtcNow);
                return BadRequest(ModelState);
            }

            //if (label.UserName == null || label.UserName.Equals(""))
            //{
            //    //      return new SetEntityResponse(SetEntityResult.EntityHasIncorrectData, "entityDTO.UserName == null || entityDTO.UserName.Equals(\"\")", 0, null, null, DateTime.UtcNow);
            //    return BadRequest(ModelState);
            //}

            // verify child parent relation consistency
            if (label.ID != 0 && label.ParentID != 0 && label.ID == label.ParentID)
            {
                //     dbTransaction.Rollback();
                //      return new SetEntityResponse(SetEntityResult.EntityHasIncorrectData, "Parent remind is set to the remind itself", 0, null, null, DateTime.UtcNow);
                return BadRequest(ModelState);
            }

            newEntity.ID = label.ID;
            newEntity.Created = label.Created;
            newEntity.Text = label.Text;
            //newEntity.UserName = label.UserName;
            newEntity.Description = label.Description;
            newEntity.ParentID = label.ParentID;
            newEntity.SortOrder = label.SortOrder;
            newEntity.LastMod = label.LastMod;
            newEntity.Deleted = label.Deleted;
            newEntity.CompanyID = label.CompanyID;
            //newEntity.OriginalID = entityDTO.OriginalID;
            newEntity.IsCompany = label.IsCompany;
            newEntity.IsSection = label.IsSection;

            //public virtual ICollection<Tasks> TaskList { get; set; }
            //public virtual ICollection<Contacts> ContactList { get; set; }
            //public virtual ICollection<Files> FileList { get; set; }
            //public virtual UserProfiles UserProfile { get; set; }
            //public virtual ICollection<Labels> ChildLabels { get; set; }
            //public virtual Labels ParentLabel { get; set; }

            newEntity.LastMod = DateTimeOffset.UtcNow;
            if (newEntity.ID == 0)
            {
                newEntity.Created = newEntity.LastMod;
            }

            //List<EntityData> NotFoundRelatedEntities = new List<EntityData>();
            //foreach (int entityId in entityDTO.Contacts)
            //{
            //    Contacts remind = db.Contacts.Where(b => b.ContactID == entityId && b.UserProfile.Username.Equals(entityDTO.UserName)).SingleOrDefault();
            //    if (remind != null)
            //    {
            //        newEntity.ContactList.Add(remind);
            //    }
            //    else
            //    {
            //        NotFoundRelatedEntities.Add(new EntityData(remind.ContactID, EntityType.Contact));
            //    }
            //}

            //foreach (int entityId in entityDTO.Tasks)
            //{
            //    Tasks remind = db.Tasks.Where(b => b.TaskID == entityId && b.UserProfile.Username.Equals(entityDTO.UserName)).SingleOrDefault();
            //    if (remind != null)
            //    {
            //        newEntity.TaskList.Add(remind);
            //    }
            //    else
            //    {
            //        NotFoundRelatedEntities.Add(new EntityData(remind.TaskID, EntityType.Simple));
            //    }
            //}

            //foreach (int entityId in entityDTO.Files)
            //{
            //    Files remind = db.Files.Where(b => b.FileID == entityId && b.UserProfile.Username.Equals(entityDTO.UserName)).SingleOrDefault();
            //    if (remind != null)
            //    {
            //        newEntity.FileList.Add(remind);
            //    }
            //    else
            //    {
            //        NotFoundRelatedEntities.Add(new EntityData(remind.FileID, EntityType.File));
            //    }
            //}

            // db.SaveChanges();

            //dbTransaction.Commit();
            //return new SetEntityResponse(SetEntityResult.Saved, null, newEntity.ID, newEntity.LastMod.UtcDateTime, NotFoundRelatedEntities.Count == 0 ? null : NotFoundRelatedEntities, DateTime.UtcNow);


            //db.Labels.Add(entityDTO);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = label.ID }, label);
        }

        // DELETE: api/Labels/5
        [ResponseType(typeof(Label))]
        public async Task<IHttpActionResult> DeleteLabel(int id)
        {
            Label label = await db.Labels.FindAsync(id);
            if (label == null)
            {
                return NotFound();
            }

            db.Labels.Remove(label);
            await db.SaveChangesAsync();

            return Ok(label);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LabelExists(int id)
        {
            return db.Labels.Count(e => e.ID == id) > 0;
        }
    }
}