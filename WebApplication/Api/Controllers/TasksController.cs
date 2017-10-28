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
using WebApplication.Models;
using WebApplication.Models.CodeFirst;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Web.Http.ModelBinding;
using WebApplication.Api.Utils;

namespace WebApplication.Api.Controllers
{
    [Authorize]
    public class TasksController : ApiController
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> manager;
        public TasksController()
        {
            db = new ApplicationDbContext();
            manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        // GET: api/Tasks
        [ActionName("GetSyncTasks")]
        public IHttpActionResult GetSyncTasks(DateTimeOffset? date = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var currentUser = manager.FindById(User.Identity.GetUserId());
            if (currentUser == null || !currentUser.EmailConfirmed)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return ResponseMessage(response);

            }


            List<SyncEntityInfo> SyncObjects = new List<SyncEntityInfo>();


            List<WebApplication.Models.CodeFirst.Task> entities;
            if (date == null)
            {
                IQueryable<WebApplication.Models.CodeFirst.Task> tasks = db.Tasks.Where(b => (b.ApplicationUserId == currentUser.Id));
                entities = tasks.ToList();
            }
            else
            {
                IQueryable<WebApplication.Models.CodeFirst.Task> tasks = db.Tasks.Where(b => (b.ApplicationUserId == currentUser.Id && b.LastMod > date));
                entities = tasks.ToList();
            }
            foreach (WebApplication.Models.CodeFirst.Task entity in entities)
            {
                SyncObjects.Add(new SyncEntityInfo(entity));
            }

            return Ok(new EntityListResponse(GetEntityListResult.SUCCESS, SyncObjects.ToArray(), DateTime.UtcNow));
        }

        // GET: api/Tasks/5
        [ResponseType(typeof(WebApplication.Models.CodeFirst.Task))]
        [ActionName("GetTasks")]
        public IHttpActionResult GetTasks(bool includeDependentEntities, [ModelBinder(typeof(CommaDelimitedArrayModelBinder))] long[] ids)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var currentUser = manager.FindById(User.Identity.GetUserId());
            if (currentUser == null || !currentUser.EmailConfirmed)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return ResponseMessage(response);
                //return Unauthorized(null/*params AuthenticationHeaderValue[] challenges*/);
            }

            WebApplication.Models.CodeFirst.Task[] entityArray = db.Tasks
                    .Where(b => ids.Contains(b.ID)).ToArray();
            TaskDto[] listItemList = new TaskDto[entityArray.Count()];
            for (int i = 0; i < entityArray.Count(); i++)
            {
                listItemList[i] = new TaskDto(entityArray[i]);
            }

            return Ok(new TasksListResponse(listItemList.Count() == ids.Count() ? GetEntityListResult.SUCCESS : GetEntityListResult.NOT_ALL_ENTITIES_ARE_FOUND, listItemList, DateTime.UtcNow));
        }

        //// PUT: api/Tasks/5
        //[ResponseType(typeof(void))]
        //public async Simple<IHttpActionResult> PutTask(int id, WebApplication.Models.CodeFirst.Simple task)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != task.ID)
        //    {
        //        return BadRequest();
        //    }

        //    db.Entry(task).State = EntityState.Modified;

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TaskExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // POST: api/Tasks
        [ResponseType(typeof(TaskDto))]
        public async Task<IHttpActionResult> PostLabel(TaskDto label)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // now the user is authenticated

            Label newEntity = null;
            if (label.Id != 0)
            {
                return BadRequest(ModelState);
            }
            else
            {
                newEntity = new Label();
                db.Labels.Add(newEntity);
            }

            Label parentEntity = null;
            if (label.ParentId != 0)
            {
                parentEntity = db.Labels.Where(b => b.ID == label.ParentId).SingleOrDefault();

                if (parentEntity == null)
                {
                    //dbTransaction.Rollback();
                    //     return new SetEntityResponse(SetEntityResult.ParentEntityIsNotFound, null,/*"The requested parent remind was not found on the server",*/ entityDTO.ID, null, null, DateTime.UtcNow);
                    return BadRequest(ModelState);
                }

                //if (parentEntity.Deleted && !label.Deleted)
                //{
                //    //dbTransaction.Rollback();
                //    return BadRequest(ModelState);
                //    //return new SetEntityResponse(SetEntityResult.ParentEntityIsDeleted, null,/*"The requested parent remind was deleted on the server",*/ entityDTO.ID, null, null, DateTime.UtcNow);
                //}
            }

            // verify field consistency

            //if (label.ID == 0 && label.Deleted)
            //{
            //    // dbTransaction.Rollback();
            //    //return new SetEntityResponse(SetEntityResult.EntityHasIncorrectData, "entityDTO.ID == 0 && entityDTO.Deleted", 0, null, null, DateTime.UtcNow);
            //    return BadRequest(ModelState);
            //}

            //if (label.UserName == null || label.UserName.Equals(""))
            //{
            //    //      return new SetEntityResponse(SetEntityResult.EntityHasIncorrectData, "entityDTO.UserName == null || entityDTO.UserName.Equals(\"\")", 0, null, null, DateTime.UtcNow);
            //    return BadRequest(ModelState);
            //}

            // verify child parent relation consistency
            if (label.Id != 0 && label.ParentId != 0 && label.Id == label.ParentId)
            {
                //     dbTransaction.Rollback();
                //      return new SetEntityResponse(SetEntityResult.EntityHasIncorrectData, "Parent remind is set to the remind itself", 0, null, null, DateTime.UtcNow);
                return BadRequest(ModelState);
            }

            newEntity.ID = label.Id.Value;
            //   newEntity.Created = label.Created;
            newEntity.Text = label.Name;
            //newEntity.UserName = label.UserName;
            //      newEntity.Description = entityDTO.Description;
            newEntity.ParentID = label.ParentId;
            newEntity.SortOrder = label.SortOrder;
            //   newEntity.LastMod = label.LastMod;
            //newEntity.Deleted = label.Deleted;
            //     newEntity.CompanyID = entityDTO.CompanyID;
            //newEntity.OriginalID = entityDTO.OriginalID;
            //    newEntity.IsCompany = entityDTO.IsCompany;
            //    newEntity.IsSection = entityDTO.IsSection;

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
            //    Tasks remind = db.Tasks.Where(b => b.ID == entityId && b.UserProfile.Username.Equals(entityDTO.UserName)).SingleOrDefault();
            //    if (remind != null)
            //    {
            //        newEntity.TaskList.Add(remind);
            //    }
            //    else
            //    {
            //        NotFoundRelatedEntities.Add(new EntityData(remind.ID, EntityType.Simple));
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

            return CreatedAtRoute("DefaultApi", new { id = label.Id }, label);
        }

        private SetTaskResponse xfunc(TaskDto entityDTO, string ApplicationUserId)
        {
            using (var dbTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    WebApplication.Models.CodeFirst.Task newEntity = null;

                    if (entityDTO.Id != null)
                    {
                        //verify incoming entity
                        //
                        newEntity = db.Tasks.Where(b => (b.ID == entityDTO.Id && b.ApplicationUserId == ApplicationUserId)).SingleOrDefault();
                        if (newEntity == null)
                        {
                            dbTransaction.Rollback();
                            return new SetTaskResponse(SetEntityResult.ENTITY_IS_NOT_FOUND, null, entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                        }
                        if (!(newEntity.ApplicationUserId.Equals(ApplicationUserId)))
                        {
                            dbTransaction.Rollback();
                            return new SetTaskResponse(SetEntityResult.ACCESS_DENIED, null, entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                        }
                        if (newEntity.Deleted)
                        {
                            dbTransaction.Rollback();
                            return new SetTaskResponse(SetEntityResult.ENTITY_IS_DELETED, null, entityDTO.Id, null, null, newEntity.LastMod.UtcDateTime, null, DateTime.UtcNow);
                        }
                        // Decide < or <=
                        //
                        // entityDTO.LastMod < localEntity.LastMod
                        // or 
                        // entityDTO.LastMod <= localEntity.LastMod
                        //
                        //Here are various syncronization scenarios when
                        //server or phone records or don't the entity if 
                        //entityDTO.LastMod == localEntity.LastMod
                        //
                        //1. + (Syncronization works) (Syncronization scenario: first the server tries to write to the phone, then the phone tries to write to the server)
                        //phone1 <--record--- server
                        //phone1 ---record--> server
                        //                    server ---record--> phone2
                        //                    server <--record--- phone2
                        //2. - (Syncronization doesn't work) (Syncronization scenario: first the phone tries to write to the server, then the server tries to write to the phone)
                        //phone1 ---record--> server
                        //phone1 <--record--- server
                        //                    server <--record--- phone2
                        //                    server ---record--> phone2
                        //3. +
                        //phone1 <--record--- server
                        //phone1 ---don't---> server
                        //                    server ---record--> phone2				  
                        //                    server <--don't---- phone2
                        //4. +
                        //phone1 ---don't---> server
                        //phone1 <--record--- server
                        //                    server <--don't---- phone2
                        //                    server ---record--> phone2
                        //5. -
                        //phone1 ---record--> server
                        //phone1 <--don't---- server
                        //                    server <--record--- phone2
                        //                    server ---don't---> phone2
                        //6. -
                        //phone1 <--don't---- server
                        //phone1 ---record--> server
                        //                    server ---don't---> phone2
                        //                    server <--record--- phone2
                        //7. -
                        //phone1 <--don't---- server
                        //phone1 ---don't---> server
                        //                    server ---don't---> phone2
                        //                    server <--don't---- phone2
                        //8. -
                        //phone1 ---don't---> server
                        //phone1 <--don't---- server
                        //                    server <--don't---- phone2
                        //                    server ---don't---> phone2
                        //
                        // Detected we have to record the entity to the phone if 
                        // entityDTO_FromTheServer.LastMod >= localEntityOnThePhone.LastMod
                        // And decided to record the entity from the phone to the server only if 
                        // entityDTO_FromThePhone.LastMod > localEntityOnTheServer.LastMod
                        // That way we are independent from the order in which the synchronization is performed:
                        // From the phone to the server first and from the server to phone next or 
                        // from the server to the phone first and from the phone to the server next

                        if (entityDTO.LastMod <= Helper.ToUnixTime(newEntity.LastMod))
                        {
                            dbTransaction.Rollback();
                            return new SetTaskResponse(SetEntityResult.ENTITY_ON_THE_SERVER_IS_NEWER, null, entityDTO.Id, null, null, newEntity.LastMod.UtcDateTime, null, DateTime.UtcNow);
                        }
                        // checks from android code
                        // verify task and reminder consistency
                        foreach (ReminderDto reminder in entityDTO.Reminders)
                        {
                            if (reminder.ReminderDateTime < 0)
                            {
                                return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "reminder.ReminderDateTime < 0", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                            }
                            if (entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.ONE_TIME))
                            {
                                if (entityDTO.StartDateTime == null)
                                {
                                    if (!reminder.ReminderTimeMode.Equals(ReminderTimeMode.ABSOLUTE_TIME))
                                    {
                                        return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.ONE_TIME) && entityDTO.StartDateTime == null && !reminder.ReminderMode.Equals(ReminderMode.ABSOLUTE_TIME)", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                                    }
                                }
                            }
                        }
                        // verify task and repetitions consistency
                        switch (entityDTO.RecurrenceInterval)
                        {
                            case RecurrenceInterval.ONE_TIME:
                                if (entityDTO.TaskOccurrences.Count > 0)
                                {
                                    return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.ONE_TIME) && entityDTO.TaskRecurrences.Count > 0", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                                }
                                break;
                            case RecurrenceInterval.HOURS:
                            case RecurrenceInterval.MINUTES:
                            case RecurrenceInterval.DAYS:
                                if (entityDTO.TaskOccurrences.Count == 0)
                                {
                                    return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.HOURS || MINUTES || DAYS) && entityDTO.TaskRecurrences.Count == 0 && entityDTO.TimeUnitCount > 1", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                                }
                                foreach (TaskOccurrenceDto taskRecurrence in entityDTO.TaskOccurrences)
                                {
                                    if (taskRecurrence.OrdinalNumber < 1
                                            || taskRecurrence.OrdinalNumber > entityDTO.TimeUnitsCount)
                                    {
                                        return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.HOURS || MINUTES || DAYS) && (taskRecurrence.Recurrence < 1 || taskRecurrence.Recurrence > entityDTO.TimeUnitCount)", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                                    }
                                }
                                break;
                            case RecurrenceInterval.WEEKS:
                                if (entityDTO.TaskOccurrences.Count == 0)
                                {
                                    return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.WEEKS) && entityDTO.TaskRecurrences.Count == 0", entityDTO.Id, null, null, null, null, DateTime.UtcNow);

                                }
                                foreach (TaskOccurrenceDto taskRecurrence in entityDTO.TaskOccurrences)
                                {
                                    if (taskRecurrence.OrdinalNumber < 1
                                            || taskRecurrence.OrdinalNumber > /* entityWithDependents
																	 * .task
																	 * .getTimeUnitCount
																	 * () * */7)
                                    {
                                        return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.WEEKS) && (taskRecurrence.Recurrence < 1 || taskRecurrence.Recurrence > 7)", entityDTO.Id, null, null, null, null, DateTime.UtcNow);

                                    }
                                }
                                break;
                            case RecurrenceInterval.MONTHS_ON_DATE:
                                if (entityDTO.TaskOccurrences.Count == 0)
                                {
                                    return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                        "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.MONTHS_ON_DATE) && entityDTO.TaskRecurrences.Count == 0", entityDTO.Id, null, null, null, null, DateTime.UtcNow);

                                }
                                foreach (TaskOccurrenceDto taskRecurrence in entityDTO.TaskOccurrences)
                                {
                                    if (taskRecurrence.OrdinalNumber < 1
                                            || taskRecurrence.OrdinalNumber > 31)
                                    {
                                        return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.MONTHS_ON_DATE) && (taskRecurrence.Recurrence < 1 || taskRecurrence.Recurrence > 31)", entityDTO.Id, null, null, null, null, DateTime.UtcNow);

                                    }
                                }
                                break;
                            case RecurrenceInterval.MONTHS_ON_NTH_WEEK_DAY:
                                if (entityDTO.TaskOccurrences.Count == 1 || entityDTO.TaskOccurrences.Count == 0)
                                {
                                    return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                        "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.MONTHS_ON_NTH_WEEK_DAY) && entityDTO.TaskRecurrences.Count == 1 || entityDTO.TaskRecurrences.Count == 0", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                                }
                                TaskOccurrenceDto[] TaskRecurrenceDtoArray = entityDTO.TaskOccurrences.ToArray();
                                for (int i = 0; i < TaskRecurrenceDtoArray.Count() - 2; i++)
                                {
                                    TaskOccurrenceDto taskRecurrence = TaskRecurrenceDtoArray[i];
                                    if (taskRecurrence.OrdinalNumber < 1
                                            || taskRecurrence.OrdinalNumber > 7)
                                    {
                                        return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                            "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.MONTHS_ON_NTH_WEEK_DAY) && (taskRecurrence.Recurrence < 1 || taskRecurrence.Recurrence > 7)", entityDTO.Id, null, null, null, null, DateTime.UtcNow);

                                    }
                                }
                                int location = TaskRecurrenceDtoArray.Count() - 1;
                                int weekCode = TaskRecurrenceDtoArray[location]
                                       .OrdinalNumber;
                                if (weekCode == 0 || weekCode < -1 || weekCode > 4)
                                {
                                    return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                        "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.MONTHS_ON_NTH_WEEK_DAY) && (weekCode == 0 || weekCode < -1 || weekCode > 4)", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                                }
                                break;
                            case RecurrenceInterval.YEARS:
                                if (entityDTO.TaskOccurrences.Count == 0)
                                {
                                    return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                        "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.YEARS) && entityDTO.TaskRecurrences.Count == 0", entityDTO.Id, null, null, null, null, DateTime.UtcNow);

                                }
                                foreach (TaskOccurrenceDto taskRecurrence in entityDTO.TaskOccurrences)
                                {
                                    if (taskRecurrence.OrdinalNumber < 1
                                            || taskRecurrence.OrdinalNumber > 367)
                                    {
                                        return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.YEARS) && (taskRecurrence.Recurrence < 1 || taskRecurrence.Recurrence > 367)", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                                    }
                                }
                                break;
                                //default:
                                //    return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "", entityDTO.Id, null, null, null, null, DateTime.UtcNow);

                        }
                        // verify field consistency                      
                        if (entityDTO.RecurrenceInterval
                                .Equals(RecurrenceInterval.ONE_TIME))
                        {
                            if (entityDTO.RepetitionsEndDateTime != null)
                            {
                                return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA, "entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.ONE_TIME) && entityDTO.RepetitionsEndDateTime != null", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                            }
                        }
                        if (entityDTO.TimeUnitsCount == null)
                        {
                            if (!entityDTO.RecurrenceInterval
                                .Equals(RecurrenceInterval.ONE_TIME))
                            {
                                return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                    "entityDTO.TimeUnitCount == null && !entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.ONE_TIME)", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                            }
                            if (entityDTO.RepetitionsMaxCount != null)
                            {
                                return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                       "entityDTO.TimeUnitCount == null && entityDTO.RepetitionsMaxCount != null", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                            }
                        }
                        else
                        {
                            if (entityDTO.TimeUnitsCount <= 0)
                            {
                                return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                      "entityDTO.TimeUnitCount != null && entityDTO.TimeUnitCount <= 0", entityDTO.Id, null, null, null, null, DateTime.UtcNow);

                            }
                            if (entityDTO.RecurrenceInterval
                                .Equals(RecurrenceInterval.ONE_TIME))
                            {
                                return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                         "entityDTO.TimeUnitCount != null && entityDTO.RecurrenceInterval.Equals(RecurrenceInterval.ONE_TIME)", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                            }
                            if (entityDTO.StartDateTime == null)
                            {
                                return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                         "entityDTO.TimeUnitCount != null && entityDTO.StartDateTime == null", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                            }
                            if (entityDTO.EndDateTime == null)
                            {
                                return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                          "entityDTO.TimeUnitCount != null && entityDTO.EndDateTime == null", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                            }

                        }
                        // TODO verify parent child relation consistency
                        if (entityDTO.Id != null
                                && entityDTO.ParentId != null
                                && entityDTO.Id.Equals(entityDTO.ParentId))
                        {
                            return new SetTaskResponse(SetEntityResult.ENTITY_HAS_INCORRECT_DATA,
                                         "entityDTO.Id != null && entityDTO.ParentId != null && entityDTO.Id.Equals(entityDTO.ParentId)", entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                        }

                    }
                    else
                    {
                        newEntity = new WebApplication.Models.CodeFirst.Task();
                        newEntity.ApplicationUserId = ApplicationUserId;
                        newEntity.Name = entityDTO.Name;
                        db.Tasks.Add(newEntity);
                        db.SaveChanges();
                    }

                    bool isPreviousStateCompleted = newEntity.IsCompleted;

                    WebApplication.Models.CodeFirst.Task parentEntity = null;

                    //TODO remove the code below. 
                    //if (entityDTO.ParentID == 0)
                    //{
                    //    entityDTO.ParentID = null;
                    //}

                    if (entityDTO.ParentId != null)
                    {
                        parentEntity = db.Tasks.Where(b => (b.ID == entityDTO.ParentId && b.ApplicationUserId == ApplicationUserId)).SingleOrDefault();

                        if (parentEntity == null)
                        {
                            dbTransaction.Rollback();
                            return new SetTaskResponse(SetEntityResult.PARENT_ENTITY_IS_NOT_FOUND, null,/*"The requested parent remind was not found on the server",*/ entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                        }

                        if (parentEntity.Deleted /*&& !entityDTO.Deleted*/)
                        {
                            dbTransaction.Rollback();
                            return new SetTaskResponse(SetEntityResult.PARENT_ENTITY_IS_DELETED, null,/*"The requested parent remind was deleted on the server",*/ entityDTO.Id, null, null, null, null, DateTime.UtcNow);
                        }
                    }

                    List<EntityData> NotFoundRelatedEntities = new List<EntityData>();

                    if (entityDTO.Reminders == null)
                    {
                        entityDTO.Reminders = new List<ReminderDto>();
                    }

                    if (entityDTO.TaskOccurrences == null)
                    {
                        entityDTO.TaskOccurrences = new List<TaskOccurrenceDto>();
                    }

                    if (entityDTO.Id != null)
                    {
                        db.Entry(newEntity).Collection(p => p.Reminders).Load();
                        ////newEntity.RemindList.Clear();
                        //while (newEntity.Reminders.Count > 0)
                        //{
                        //    db.Reminders.Remove(newEntity.Reminders.First());
                        //}                       

                        List<Reminder> reminds = newEntity.Reminders.ToList();
                        foreach (Reminder oldReminder in reminds)
                        {
                            bool includedInNewReminderList = false;
                            foreach (ReminderDto newReminder in entityDTO.Reminders)
                            {
                                if (oldReminder.ID == newReminder.Id)
                                {
                                    includedInNewReminderList = true;
                                    break;
                                }
                            }
                            if (!includedInNewReminderList)
                            {
                                db.Reminds.Remove(oldReminder);
                            }
                        }
                        //
                        db.Entry(newEntity).Collection(p => p.TaskRecurrences).Load();
                        List<TaskOccurrence> dailyRepetitions = newEntity.TaskRecurrences.ToList();
                        foreach (TaskOccurrence oldDailyRepetition in dailyRepetitions)
                        {
                            db.DailyRepetitions.Remove(oldDailyRepetition);
                        }
                        //

                        //reminds.ForEach(x => db.Reminders.Remove(x));

                        //db.Entry(newEntity).Collection(p => p.LabelList).Load();
                        // newEntity.LabelList.Clear();
                        //while (newEntity.LabelList.Count > 0)
                        //{
                        //    db.Labels.Remove(newEntity.LabelList.First());
                        //}

                        //db.Entry(newEntity).Collection(p => p.ContactList).Load();
                        //// newEntity.ContactList.Clear();
                        //while (newEntity.ContactList.Count > 0)
                        //{
                        //    db.Contacts.Remove(newEntity.ContactList.First());
                        //}

                        //db.Entry(newEntity).Collection(p => p.FileList).Load();
                        ////newEntity.FileList.Clear();
                        //while (newEntity.FileList.Count > 0)
                        //{
                        //    db.Files.Remove(newEntity.FileList.First());
                        //}
                    }

                    //if (entityDTO.Created.Kind == DateTimeKind.Unspecified)
                    //{
                    //    entityDTO.Created = new DateTime(entityDTO.Created.Ticks, DateTimeKind.Utc);
                    //}

                    newEntity.ApplicationUserId = ApplicationUserId;
                    newEntity.Name = entityDTO.Name;
                    newEntity.Description = entityDTO.Description;
                    newEntity.Location = entityDTO.Location;
                    //newEntity.Type = entityDTO.Type;
                    newEntity.ParentID = entityDTO.ParentId;
                    newEntity.StartDateTime = Helper.FromUnixTime(entityDTO.StartDateTime);
                    newEntity.EndDateTime = Helper.FromUnixTime(entityDTO.EndDateTime);
                    newEntity.Length = entityDTO.RequiredLength;
                    newEntity.ActualLength = entityDTO.ActualLength;
                    newEntity.TimeUnitCount = entityDTO.TimeUnitsCount;
                    newEntity.RecurrenceInterval = entityDTO.RecurrenceInterval;
                    newEntity.Priority = entityDTO.Priority;
                    newEntity.Color = entityDTO.Color;
                    newEntity.IsCompleted = entityDTO.IsCompleted.HasValue ? entityDTO.IsCompleted.Value : false;
                    newEntity.PercentOfCompletion = entityDTO.PercentOfCompletion;
                    newEntity.CompletedTime = Helper.FromUnixTime(entityDTO.CompletedDateTime);
                    //newEntity.Deleted = entityDTO.Deleted;
                    newEntity.SortOrder = entityDTO.SortOrder;
                    newEntity.RepetitionsMaxCount = entityDTO.RepetitionsMaxCount;
                    newEntity.RepetitionsEndDateTime = Helper.FromUnixTime(entityDTO.RepetitionsEndDateTime);
                    DateTimeOffset utcNow = DateTimeOffset.UtcNow;
                    long unixTime = Helper.ToUnixTime(utcNow);
                    if (unixTime < entityDTO.LastMod)
                    {
                        newEntity.LastMod = Helper.FromUnixTime(unixTime).Value;
                    }
                    else
                    {
                        newEntity.LastMod = Helper.FromUnixTime(entityDTO.LastMod).Value;
                    }
                    //newEntity.EndTime = 0;
                    //newEntity.StartTime = 0;
                    //newEntity.RepeatEndTime = 0;

                    newEntity.MaxAutomaticSnoozeCount = entityDTO.AutomaticSnoozesMaxCount;
                    newEntity.AutomaticSnoozeTime = entityDTO.AutomaticSnoozeDuration;
                    newEntity.PlayingTime = entityDTO.PlayingTime;
                    newEntity.Vibrate = entityDTO.Vibrate;
                    newEntity.Led = entityDTO.Led;

                    newEntity.VibratePattern = entityDTO.VibratePattern;
                    newEntity.LedPattern = entityDTO.LedPattern;
                    newEntity.LedColor = entityDTO.LedColor;

                    long?[] entityDtoReminderIds = new long?[entityDTO.Reminders.Count];

                    if (entityDTO.Reminders.Count > 0)
                    {
                        if (newEntity.Reminders == null)
                        {
                            newEntity.Reminders = new List<Reminder>();
                        }
                        Reminder reminder;
                        for (int i = 0; i < entityDTO.Reminders.Count; i++)
                        {
                            ReminderDto reminderDTO = entityDTO.Reminders.ElementAt(i);

                            if (reminderDTO.Id != null)
                            {
                                reminder = db.Reminds.Where(b => (b.ID == reminderDTO.Id)).SingleOrDefault();
                                if (reminder == null)
                                {
                                    NotFoundRelatedEntities.Add(new EntityData(reminderDTO.Id.Value, EntityType.REMINDER));
                                    entityDtoReminderIds[i] = null;
                                    continue;
                                }
                            }
                            else
                            {
                                reminder = new Reminder();
                            }
                            // remind.ID = remindDTO.ID;
                            reminder.ReminderDateTime = reminderDTO.ReminderDateTime;
                            reminder.Text = reminderDTO.Text;
                            reminder.Enabled = reminderDTO.Enabled;
                            reminder.ReminderMode = reminderDTO.ReminderTimeMode;
                            reminder.IsAlarm = reminderDTO.IsAlarm;
                            reminder.TaskID = newEntity.ID;
                            reminder.MaxAutomaticSnoozeCount = reminderDTO.AutomaticSnoozesMaxCount;
                            reminder.AutomaticSnoozeTime = reminderDTO.AutomaticSnoozeDuration;
                            reminder.PlayingTime = reminderDTO.PlayingTime;
                            reminder.Vibrate = reminderDTO.Vibrate;
                            reminder.Led = reminderDTO.Led;
                            reminder.VibratePattern = reminderDTO.VibratePattern;
                            reminder.LedPattern = reminderDTO.LedPattern;
                            reminder.LedColor = reminderDTO.LedColor;

                            if (reminder.ID == 0)
                            {
                                db.Reminds.Add(reminder);
                                db.SaveChanges();
                            }
                            else
                            {
                                // newEntity.Reminders.Add(remind);
                            }
                            entityDtoReminderIds[i] = reminder.ID;
                        }
                    }

                    long?[] entityDtoDailyRepetitionIds = new long?[entityDTO.TaskOccurrences.Count];

                    if (entityDTO.TaskOccurrences.Count > 0)
                    {
                        if (newEntity.TaskRecurrences == null)
                        {
                            newEntity.TaskRecurrences = new List<TaskOccurrence>();
                        }
                        TaskOccurrence dailyRepetition;
                        for (int i = 0; i < entityDTO.TaskOccurrences.Count; i++)
                        {
                            TaskOccurrenceDto dailyRepetitionDTO = entityDTO.TaskOccurrences.ElementAt(i);

                            if (dailyRepetitionDTO.Id != null)
                            {
                                dailyRepetition = db.DailyRepetitions.Where(b => (b.ID == dailyRepetitionDTO.Id)).SingleOrDefault();
                                if (dailyRepetition == null)
                                {
                                    // NotFoundRelatedEntities.Add(new EntityData(dailyRepetitionDTO.ID.Value, EntityType.DailyRepetition));
                                    entityDtoDailyRepetitionIds[i] = null;
                                    continue;
                                }
                            }
                            else
                            {
                                dailyRepetition = new TaskOccurrence();
                            }
                            // remind.ID = remindDTO.ID;
                            dailyRepetition.TaskID = newEntity.ID;
                            dailyRepetition.OrdinalNumber = dailyRepetitionDTO.OrdinalNumber;
                            //  remind.Date = newEntity.Date;

                            if (dailyRepetition.ID == 0)
                            {
                                db.DailyRepetitions.Add(dailyRepetition);
                                db.SaveChanges();
                            }
                            else
                            {
                                // newEntity.Reminders.Add(remind);
                            }
                            entityDtoDailyRepetitionIds[i] = dailyRepetition.ID;
                        }
                    }


                    //foreach (int entityId in entityDTO.Contacts)
                    //{
                    //    Contact remind = db.Contacts.Where(b => b.ContactID == entityId && b.UserProfile.Username.Equals(entityDTO.UserName)).SingleOrDefault();
                    //    if (remind != null)
                    //    {
                    //        newEntity.ContactList.Add(remind);
                    //    }
                    //    else
                    //    {
                    //        NotFoundRelatedEntities.Add(new EntityData(remind.ContactID, EntityType.Contact));
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

                    // adjust completed state on the subtree
                    if (entityDTO.Id != null)
                    {
                        if (entityDTO.IsCompleted.HasValue && entityDTO.IsCompleted.Value)
                        {
                            List<TaskTreeNode> taskTree = getTaskTree(entityDTO.Id.Value, true, true, true, true);

                            // If completed set completed on the nodes of the subtree

                            for (int i1 = 1; i1 < taskTree.Count; i1++)
                            {
                                if (!taskTree[i1].Node.IsCompleted)
                                {
                                    taskTree[i1].Node.IsCompleted = true;
                                    taskTree[i1].Node.LastMod = DateTimeOffset.UtcNow;
                                }
                            }
                        }
                        else
                        {
                            if (isPreviousStateCompleted)
                            {
                                long? nextId = entityDTO.ParentId;
                                while (nextId != null)
                                {
                                    WebApplication.Models.CodeFirst.Task entity2 = db.Tasks.Where(b => b.ID == nextId).Single();
                                    if (!entity2.IsCompleted)
                                    {
                                        break;
                                    }
                                    entity2.IsCompleted = false;
                                    entity2.LastMod = DateTimeOffset.UtcNow;
                                    nextId = entity2.ParentID;
                                }
                            }
                        }
                    }

                    db.SaveChanges();

                    dbTransaction.Commit();

                    return new SetTaskResponse(SetEntityResult.SAVED, null, newEntity.ID, entityDtoReminderIds, entityDtoDailyRepetitionIds, newEntity.LastMod.UtcDateTime, NotFoundRelatedEntities.Count == 0 ? null : NotFoundRelatedEntities, DateTime.UtcNow);
                }
                catch (Exception e)
                {
                    dbTransaction.Rollback();
                    throw e;
                }
            }
        }

        private void checkEntityWithDependentsRepetitionsSizeForHoursMinutesDays(TaskDto entityDTO)
        {


        }


        // POST: api/Tasks/Tasks
        // [ResponseType(typeof(void))]
        [ActionName("TaskArray")]
        [AcceptVerbs("PUT", "POST")]
        public async Task<IHttpActionResult> TaskArray(TaskDto[] entityDtoArray)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //string id = User.Identity.GetUserId();
            //var user = from b in db.Users
            //           where b.Id == id
            //           select b;

            //if (user == null)
            var currentUser = await manager.FindByIdAsync(User.Identity.GetUserId());
            if (currentUser == null || !currentUser.EmailConfirmed)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return ResponseMessage(response);
                //return Unauthorized(null/*params AuthenticationHeaderValue[] challenges*/);
            }

            SetTaskResponse[] response2 = new SetTaskResponse[entityDtoArray.Count()];
            for (int i = 0; i < entityDtoArray.Count(); i++)
            {
                response2[i] = xfunc(entityDtoArray[i], currentUser.Id);
            }
            return Json<SetTaskResponse[]>(response2);
        }
        private List<TaskTreeNode> getTaskTree(long rootId, bool includeNonDeleted, bool includeDeleted, bool includeActive, bool includeCompleted)
        {
            List<TaskTreeNode> taskTree = new List<TaskTreeNode>();

            WebApplication.Models.CodeFirst.Task root = db.Tasks.Where(b => b.ID == rootId).SingleOrDefault();
            if (root == null) return taskTree;
            taskTree.Add(new TaskTreeNode(root, null));
            appendChildTasksRecursively(taskTree, root.ID);

            return taskTree;
        }
        private void appendChildTasksRecursively(List<TaskTreeNode> list, long parentId)
        {
            //advancedCalendarEntities.Configuration.ProxyCreationEnabled = false;
            WebApplication.Models.CodeFirst.Task parent = db.Tasks.Where(b => b.ID == parentId).SingleOrDefault();
            //advancedCalendarEntities.Entry(parent).Collection(p => p.ChildTasks).Load();

            // Load one blogs and its related posts 
            //var parent1 = advancedCalendarEntities.Tasks
            //                    .Where(b => b.TaskID == parentId)
            //                    .Include(b => b.ChildTasks)
            //                    .FirstOrDefault(); 

            // ICollection<Tasks> ChildTasks = advancedCalendarEntities.Tasks.Where(i => i.ParentID == parentId).Single().ChildTasks;
            List<WebApplication.Models.CodeFirst.Task> ChildTasks2 = db.Tasks.Where(i => i.ParentID == parentId).ToList();


            foreach (WebApplication.Models.CodeFirst.Task child in ChildTasks2)
            {
                list.Add(new TaskTreeNode(child, parent));
                appendChildTasksRecursively(list, child.ID);
            }
            //foreach (Tasks child in parent.ChildTasks)
            //{
            //    appendChildTasksRecursively(list, child.TaskID);
            //}            
        }
        // DELETE: api/Tasks/5
        [ActionName("DeleteTask")]
        [ResponseType(typeof(WebApplication.Models.CodeFirst.Task))]
        public async Task<IHttpActionResult> DeleteTask(long id, long lastMod)
        {
            db.Configuration.ProxyCreationEnabled = false;

            using (var dbLabelTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //string userId = User.Identity.GetUserId();
                    //var user = from b in db.Users
                    //           where b.Id == userId
                    //           select b;
                    var currentUser = await manager.FindByIdAsync
                                                      (User.Identity.GetUserId());
                    if (currentUser == null || !currentUser.EmailConfirmed)
                    {
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                        return ResponseMessage(response);
                        //return Unauthorized(null/*params AuthenticationHeaderValue[] challenges*/);
                    }

                    WebApplication.Models.CodeFirst.Task taskToDelete = await db.Tasks.FindAsync(id);
                    if (taskToDelete == null || taskToDelete.Deleted)
                    {
                        return NotFound();
                    }
                    if (taskToDelete.ApplicationUserId != currentUser.Id)
                    {
                        return Unauthorized(null/*params AuthenticationHeaderValue[] challenges*/);
                    }

                    //WebApplication.Models.CodeFirst.Simple newEntity;

                    //newEntity = db.Tasks.Where(b => b.ID == id).Single();

                    //IQueryable<WebApplication.Models.CodeFirst.Simple> tasks =
                    //    from b in db.Tasks
                    //    where (b.ID == id && b.ApplicationUserId == currentUser.Id)
                    //    select b;

                    //WebApplication.Models.CodeFirst.Simple taskToDelete = tasks.FirstOrDefault();

                    //if (taskToDelete == null)
                    //{
                    //    return NotFound();
                    //}

                    //db.Tasks.Remove(taskToDelete);
                    //await db.SaveChangesAsync();
                    //MarkTaskAsDeleted(taskToDelete.ID);


                    //if (newEntity == null)
                    //{
                    //    return NotFound();
                    //}
                    // db.Entry(newEntity).Collection(p => p.WorkgroupList).Load();
                    //newEntity.WorkgroupList.Clear();

                    //db.Entry(taskToDelete).Collection(p => p.Reminders).Load();
                    //while (taskToDelete.Reminders.Count > 0)
                    //{
                    //    db.Reminders.Remove(taskToDelete.Reminders.First());
                    //}

                    //db.Entry(newEntity).Collection(p => p.LabelList).Load();
                    //newEntity.LabelList.Clear();

                    //advancedCalendarEntities.Entry(newEntity).Collection(p => p.ContactList).Load();
                    //newEntity.ContactList.Clear();

                    //advancedCalendarEntities.Entry(newEntity).Collection(p => p.FileList).Load();
                    //newEntity.FileList.Clear();

                    //advancedCalendarEntities.SaveChanges();

                    //get childs
                    List<WebApplication.Models.CodeFirst.Task> taskToDeleteChildTasks = db.Tasks.Where(i => i.ParentID == taskToDelete.ID).ToList();
                    foreach (WebApplication.Models.CodeFirst.Task child in taskToDeleteChildTasks)
                    {
                        child.ParentID = null;
                        //child.Parent = null;
                        // don't update LastMod on child tasks because  no logical change occured to these child nodes and corresponding tasks on phone should have priority and eventually override these tasks
                    }

                    // set deleted state on the node
                    taskToDelete.Deleted = true;

                    DateTime _epochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTimeOffset dto = _epochTime.AddMilliseconds(lastMod);
                    //next version of .Net (v4.6) 
                    //DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(1000000);

                    taskToDelete.LastMod = dto;



                    // set deleted state on the subtree

                    //List<TaskTreeNode> taskTree = getTaskTree(id, true, true, true, true);

                    //for (int i = 0; i < taskTree.Count; i++)
                    //{
                    //    long id1 = taskTree[i].Node.ID;
                    //    WebApplication.Models.CodeFirst.Simple t = db.Tasks.Where(b => b.ID == id1).Single();
                    //    t.Deleted = true;

                    //     _epochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    //     dto = _epochTime.AddMilliseconds(lastMod);
                    //    //next version of .Net (v4.6) 
                    //    //DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(1000000);

                    //    t.LastMod = dto;

                    //}

                    db.SaveChanges();
                    dbLabelTransaction.Commit();
                }
                catch (Exception e)
                {
                    dbLabelTransaction.Rollback();
                    throw e;
                }
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaskExists(int id)
        {
            return db.Tasks.Count(e => e.ID == id) > 0;
        }
    }

    public class CommaDelimitedArrayModelBinder : IModelBinder
    {
        public bool BindModel(System.Web.Http.Controllers.HttpActionContext actionContext, System.Web.Http.ModelBinding.ModelBindingContext bindingContext)
        {
            var key = bindingContext.ModelName;
            var val = bindingContext.ValueProvider.GetValue(key);
            if (val != null)
            {
                var s = val.AttemptedValue;
                if (s != null)
                {
                    var elementType = bindingContext.ModelType.GetElementType();
                    var converter = System.ComponentModel.TypeDescriptor.GetConverter(elementType);
                    var values =
                        s.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(converter.ConvertFromString).ToArray();

                    var typedValues = Array.CreateInstance(elementType, values.Length);

                    values.CopyTo(typedValues, 0);

                    bindingContext.Model = typedValues;
                }
                else
                {
                    // change this line to null if you prefer nulls to empty arrays 
                    bindingContext.Model = Array.CreateInstance(bindingContext.ModelType.GetElementType(), 0);
                }
                return true;
            }
            return false;
        }
    }

}