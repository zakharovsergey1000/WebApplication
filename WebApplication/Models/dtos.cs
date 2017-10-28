using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using WebApplication.Api.Utils;
using WebApplication.Models.CodeFirst;

namespace WebApplication.Models
{
    [DataContract]
    public class TaskDto
    {
        public TaskDto()
        { }
        public TaskDto(Task entity)
        {
            Id = entity.ID;
            ParentId = entity.ParentID;
            Created = Helper.ToUnixTime(entity.Created);
            LastMod = Helper.ToUnixTime(entity.LastMod);
            Name = entity.Name;
            Priority = entity.Priority;
            Color = entity.Color;
            if (entity.StartDateTime.HasValue)
            {
                StartDateTime = Helper.ToUnixTime(entity.StartDateTime.Value);                
                //if (entity.StartTime.HasValue)
                //{
                //    StartDateTime += entity.StartTime / 10000;
                //}
            }
            else
            {
                StartDateTime = null;
            }
            if (entity.EndDateTime.HasValue)
            {
                EndDateTime = Helper.ToUnixTime(entity.EndDateTime.Value);
                //if (entity.EndTime.HasValue)
                //{
                //    EndDateTime += entity.EndTime / 10000;
                //}
            }
            else
            {
                EndDateTime = null;
            }
            RequiredLength = entity.Length;
            ActualLength = entity.ActualLength;
            IsCompleted = entity.IsCompleted;
            PercentOfCompletion = entity.PercentOfCompletion;
            if (entity.CompletedTime.HasValue)
            {
                CompletedDateTime = Helper.ToUnixTime(entity.CompletedTime.Value);
            }
            else
            {
                CompletedDateTime = null;
            }
            //CompletedDateTime = entity.CompletedTime.HasValue ? entity.CompletedTime.Value.UtcDateTime.Millisecond : null;
            Deleted = entity.Deleted;
            SortOrder = entity.SortOrder;
            Description = entity.Description;
            Location = entity.Location;
            //Type = entity.Type;
            TimeUnitsCount = entity.TimeUnitCount;
            RecurrenceInterval = entity.RecurrenceInterval;
            RepetitionsMaxCount = entity.RepetitionsMaxCount;

            if (entity.RepetitionsEndDateTime.HasValue)
            {
                RepetitionsEndDateTime = Helper.ToUnixTime(entity.RepetitionsEndDateTime.Value);
                //if (entity.RepeatEndTime.HasValue)
                //{
                //    RepetitionsEndDateTime += entity.RepeatEndTime / 10000;
                //}
            }
            else
            {
                RepetitionsEndDateTime = null;
            }

            AutomaticSnoozeDuration = entity.AutomaticSnoozeTime;
            AutomaticSnoozesMaxCount = entity.MaxAutomaticSnoozeCount;
            PlayingTime = entity.PlayingTime;
            Vibrate = entity.Vibrate;
            VibratePattern = entity.VibratePattern;
            Led = entity.Led;
            LedPattern = entity.LedPattern;
            LedColor = entity.LedColor;

            Reminders = new List<ReminderDto>();
            foreach (Reminder entity2 in entity.Reminders)
            {
                Reminders.Add(new ReminderDto(entity2));
            }

            TaskOccurrences = new List<TaskOccurrenceDto>();
            foreach (TaskOccurrence entity2 in entity.TaskRecurrences)
            {
                TaskOccurrences.Add(new TaskOccurrenceDto(entity2));
            }


            //Files = new List<long>();
            //foreach (File remind in remind.Files)
            //{
            //    Files.Add(remind.ID);
            //}
        }

        [DataMember]
        public long? Id { get; set; }
        [DataMember]
        public long? ParentId { get; set; }
        [DataMember]
        public long Created
        {
            get;
            set;
            //get { return this._Created.Kind == DateTimeKind.Utc ? this._Created : this._Created.ToUniversalTime(); }
            //set { this._Created = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime(); }
            //get { return this._Created; }
            //set { this._Created = new DateTime(value.Ticks, DateTimeKind.Utc); }
        }
        [DataMember]
        public long LastMod
        {
            get;
            set;
            //get { System.DateTime LastMod2 = this._LastMod.Kind == DateTimeKind.Utc ? this._LastMod : this._LastMod.ToUniversalTime(); return LastMod2; }
            //set { System.DateTime LastMod2 = value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime(); this._LastMod = LastMod2; }
            //get { return this._LastMod; }
            //set { this._LastMod = new DateTime(value.Ticks, DateTimeKind.Utc); }
            //set { this._LastMod = value; }
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public TaskPriority Priority { get; set; }
        [DataMember]
        public int? Color { get; set; }
        [DataMember]
        public long? StartDateTime
        {
            get;
            set;
            //get { return this._StartDate.HasValue ? (this._StartDate.Value.Kind == DateTimeKind.Utc ? this._StartDate : this._StartDate.Value.ToUniversalTime()) : null; }
            //set { this._StartDate = value.HasValue ? (value.Value.Kind == DateTimeKind.Utc ? value : value.Value.ToUniversalTime()) : null; }
            //get { return this._StartDate; }
            //set { this._StartDate = value.HasValue ? (new Nullable<System.DateTime>(new DateTime(value.Value.Ticks, DateTimeKind.Utc))) : null; }
        }

        [DataMember]
        public long? EndDateTime
        {
            get;
            set;
            //get { return this._EndDate.HasValue ? (this._EndDate.Value.Kind == DateTimeKind.Utc ? this._EndDate : this._EndDate.Value.ToUniversalTime()) : null; }
            //set { this._EndDate = value.HasValue ? (value.Value.Kind == DateTimeKind.Utc ? value : value.Value.ToUniversalTime()) : null; }
            //get { return this._EndDate; }
            //set { this._EndDate = value.HasValue ? (new Nullable<System.DateTime>(new DateTime(value.Value.Ticks, DateTimeKind.Utc))) : null; }
        }
        [DataMember]
        public int? RequiredLength { get; set; }
        [DataMember]
        public int? ActualLength { get; set; }
        [DataMember]
        public bool? IsCompleted { get; set; }
        [DataMember]
        public short PercentOfCompletion { get; set; }
        [DataMember]
        public long? CompletedDateTime
        {
            get;
            set;
            //get { return this._CompletedTime.HasValue ? (this._CompletedTime.Value.Kind == DateTimeKind.Utc ? this._CompletedTime : this._CompletedTime.Value.ToUniversalTime()) : null; }
            //set { this._CompletedTime = value.HasValue ? (value.Value.Kind == DateTimeKind.Utc ? value : value.Value.ToUniversalTime()) : null; }
            //get { return this._CompletedTime; }
            //set { this._CompletedTime = value.HasValue ? (new Nullable<System.DateTime>(new DateTime(value.Value.Ticks, DateTimeKind.Utc))) : null; }
        }
        [DataMember]
        public bool Deleted { get; set; }
        [DataMember]
        public int SortOrder { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Location { get; set; }
        //[DataMember]
        //public TaskType Type { get; set; }
        [DataMember]
        public int? TimeUnitsCount { get; set; }
        [DataMember]
        public RecurrenceInterval RecurrenceInterval { get; set; }
        [DataMember]
        public int? RepetitionsMaxCount { get; set; }
        [DataMember]
        public long? RepetitionsEndDateTime { get; set; }
        [DataMember]
        public int? AutomaticSnoozeDuration { get; set; }
        [DataMember]
        public int? AutomaticSnoozesMaxCount { get; set; }
        [DataMember]
        public int? PlayingTime { get; set; }
        [DataMember]
        public bool? Vibrate { get; set; }
        [DataMember]
        public string VibratePattern { get; set; }
        [DataMember]
        public bool? Led { get; set; }
        [DataMember]
        public string LedPattern { get; set; }
        [DataMember]
        public int? LedColor { get; set; }
        [DataMember]
        public ICollection<ReminderDto> Reminders { get; set; }
        [DataMember]
        public ICollection<TaskOccurrenceDto> TaskOccurrences { get; set; }
    }

    [DataContract]
    public partial class ReminderDto
    {
        public ReminderDto() { }
        public ReminderDto(Reminder entity)
        {
            Id = entity.ID;
            TaskId = entity.TaskID;
            ReminderDateTime = entity.ReminderDateTime;
            ReminderTimeMode = entity.ReminderMode;
            Text = entity.Text;
            Enabled = entity.Enabled;
            IsAlarm = entity.IsAlarm;
            AutomaticSnoozeDuration = entity.AutomaticSnoozeTime;
            AutomaticSnoozesMaxCount = entity.MaxAutomaticSnoozeCount;
            PlayingTime = entity.PlayingTime;
            Vibrate = entity.Vibrate;
            VibratePattern = entity.VibratePattern;
            Led = entity.Led;
            LedPattern = entity.LedPattern;
            LedColor = entity.LedColor;
        }
        [DataMember]
        public long? Id { get; set; }
        [DataMember]
        public long TaskId { get; set; }
        [DataMember]
        public long ReminderDateTime { get; set; }
        [DataMember]
        public ReminderTimeMode ReminderTimeMode { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public bool IsAlarm { get; set; }
        [DataMember]
        public int? AutomaticSnoozeDuration { get; set; }
        [DataMember]
        public int? AutomaticSnoozesMaxCount { get; set; }
        [DataMember]
        public int? PlayingTime { get; set; }
        [DataMember]
        public bool? Vibrate { get; set; }
        [DataMember]
        public string VibratePattern { get; set; }
        [DataMember]
        public bool? Led { get; set; }
        [DataMember]
        public string LedPattern { get; set; }
        [DataMember]
        public int? LedColor { get; set; }
    }

    [DataContract]
    public partial class TaskOccurrenceDto
    {
        public TaskOccurrenceDto() { }
        public TaskOccurrenceDto(TaskOccurrence entity)
        {
            Id = entity.ID;
            TaskId = entity.TaskID;
            OrdinalNumber = entity.OrdinalNumber;
        }
        [DataMember]
        public long? Id { get; set; }
        [DataMember]
        public long TaskId { get; set; }
        [DataMember]
        public int OrdinalNumber { get; set; }
    }

    //[DataContract]
    //public class GetTaskResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public GetEntityResult Result;
    //    [DataMember]
    //    public TaskDTO Simple;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }

    //    public GetTaskResponse(GetEntityResult Result, TaskDTO Simple, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.Simple = Simple;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    [DataContract]
    public class SetEntityResponse : BaseServiceResponse
    {
        //private Nullable<System.DateTime> _LastMod;
        [DataMember]
        public SetEntityResult Result;
        [DataMember]
        public String ErrorMessage;
        [DataMember]
        public long? EntityId;
        [DataMember]
        public Nullable<System.DateTime> LastMod { get; set; }
        [DataMember]
        public List<EntityData> NotFoundRelatedEntities { get; set; }
        [DataMember]
        public DateTime ServerTime { get; set; }

        public SetEntityResponse(SetEntityResult Result, String ErrorMessage, long? EntityId, Nullable<System.DateTime> LastMod, List<EntityData> NotFoundRelatedEntities, DateTime ServerTime)
        {
            this.Result = Result;
            this.ErrorMessage = ErrorMessage;
            this.EntityId = EntityId;
            this.LastMod = LastMod;
            this.NotFoundRelatedEntities = NotFoundRelatedEntities;
            this.ServerTime = ServerTime;
        }
    }
    [DataContract]
    public class SetTaskResponse : SetEntityResponse
    {
        [DataMember]
        public long?[] ReminderIds { get; set; }
        [DataMember]
        public long?[] DailyRepetitionIds { get; set; }

        public SetTaskResponse(SetEntityResult Result, String ErrorMessage, long? EntityId, long?[] ReminderIds, long?[] DailyRepetitionIds, Nullable<System.DateTime> LastMod, List<EntityData> NotFoundRelatedEntities, DateTime ServerTime)
            : base(Result, ErrorMessage, EntityId, LastMod, NotFoundRelatedEntities, ServerTime)
        {
            this.ReminderIds = ReminderIds;
            this.DailyRepetitionIds = DailyRepetitionIds;
        }
    }

    //[DataContract]
    //public partial class ContactDTO
    //{
    //    public ContactDTO(Contact remind)
    //    {
    //        ID = remind.ID;
    //        UserName = remind.UserName;
    //        ContactName = remind.ContactName;
    //        Description = remind.Description;
    //        Photo = remind.Photo;
    //        SortOrder = remind.SortOrder;
    //        Deleted = remind.Deleted;
    //        TmProfileID = remind.TmProfileID;
    //        BirthDate = remind.BirthDate;
    //        BirthMonth = remind.BirthMonth;
    //        BirthYear = remind.BirthYear;
    //        CorpStatus = remind.CorpStatus;
    //        Created = remind.Created.UtcDateTime;
    //        LastMod = remind.LastMod.UtcDateTime;

    //        Data = new List<ContactDataDTO>();
    //        foreach (ContactData remind in remind.ContactDatas)
    //        {
    //            Data.Add(new ContactDataDTO(remind));
    //        }

    //        Tasks = new List<long>();
    //        foreach (Simple remind in remind.Tasks)
    //        {
    //            Tasks.Add(remind.ID);
    //        }

    //        Labels = new List<long>();
    //        foreach (Label remind in remind.Labels)
    //        {
    //            Labels.Add(remind.ID);
    //        }

    //        Files = new List<long>();
    //        foreach (File remind in remind.Files)
    //        {
    //            Files.Add(remind.ID);
    //        }
    //    }

    //    [DataMember]
    //    public long ID { get; set; }
    //    [DataMember]
    //    public string UserName { get; set; }
    //    [DataMember]
    //    public string ContactName { get; set; }
    //    [DataMember]
    //    public string Description { get; set; }
    //    [DataMember]
    //    public string Photo { get; set; }
    //    [DataMember]
    //    public int SortOrder { get; set; }
    //    [DataMember]
    //    public bool Deleted { get; set; }
    //    [DataMember]
    //    public Nullable<long> TmProfileID { get; set; }
    //    [DataMember]
    //    public Nullable<byte> BirthDate { get; set; }
    //    [DataMember]
    //    public Nullable<byte> BirthMonth { get; set; }
    //    [DataMember]
    //    public Nullable<short> BirthYear { get; set; }
    //    [DataMember]
    //    public ContactCorporativeStatus CorpStatus { get; set; }
    //    [DataMember]
    //    public Nullable<System.DateTime> Created { get; set; }
    //    [DataMember]
    //    public System.DateTime LastMod { get; set; }

    //    [DataMember]
    //    public virtual ICollection<ContactDataDTO> Data { get; set; }

    //    [DataMember]
    //    public ICollection<long> Tasks { get; set; }

    //    [DataMember]
    //    public ICollection<long> Labels { get; set; }

    //    [DataMember]
    //    public ICollection<long> Files { get; set; }
    //}

    //[DataContract]
    //public class UserDTO
    //{
    //    public UserDTO(ApplicationUser remind)
    //    {
    //        Id = remind.Id;
    //        Login = remind.UserName;
    //        EMail = remind.Email;

    //    }
    //    [DataMember]
    //    public String Id { get; set; }
    //    [DataMember]
    //    public string Login { get; set; }
    //    [DataMember]
    //    public string EMail { get; set; }
    //    [DataMember]
    //    public string Password { get; set; }
    //    [DataMember]
    //    public Nullable<System.DateTime> CreationDate { get; set; }
    //    [DataMember]
    //    public Nullable<System.DateTime> ApprovalDate { get; set; }
    //    [DataMember]
    //    public Nullable<System.DateTime> LastLoginDate { get; set; }
    //    [DataMember]
    //    public Nullable<bool> IsLocked { get; set; }
    //    [DataMember]
    //    public string PasswordQuestion { get; set; }
    //    [DataMember]
    //    public string PasswordAnswer { get; set; }
    //    [DataMember]
    //    public string ActivationToken { get; set; }
    //    [DataMember]
    //    public bool EmailConfirmed { get; set; }
    //    [DataMember]
    //    public string SecurityStamp { get; set; }
    //    [DataMember]
    //    public string PhoneNumber { get; set; }
    //    [DataMember]
    //    public bool PhoneNumberConfirmed { get; set; }
    //    [DataMember]
    //    public bool TwoFactorEnabled { get; set; }
    //    [DataMember]
    //    public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
    //    [DataMember]
    //    public bool LockoutEnabled { get; set; }
    //    [DataMember]
    //    public int AccessFailedCount { get; set; }

    //    [DataMember]
    //    public ICollection<long> Contacts { get; set; }
    //}
    [DataContract]
    public partial class LabelDTO
    {
        public LabelDTO() { }
        public LabelDTO(Label entity)
        {
            ID = entity.ID;
            Created = entity.Created.UtcDateTime;
            Text = entity.Text;
            //UserName = remind.UserName;
            Description = entity.Description;
            ParentID = entity.ParentID;
            SortOrder = entity.SortOrder;
            LastMod = entity.LastMod.UtcDateTime;
            Deleted = entity.Deleted;
            CompanyID = entity.CompanyID;
            OriginalID = 0;// remind.OriginalID;
            IsCompany = entity.IsCompany;
            IsSection = entity.IsSection;

            //Tasks = new List<long>();
            //foreach (Simple remind in remind.Tasks)
            //{
            //    Tasks.Add(remind.ID);
            //}

            //Contacts = new List<long>();
            //foreach (Contact remind in remind.Contacts)
            //{
            //    Contacts.Add(remind.ID);
            //}

            //Files = new List<long>();
            //foreach (File remind in remind.Files)
            //{
            //    Files.Add(remind.ID);
            //}
        }
        [DataMember]
        public long ID { get; set; }
        [DataMember]
        public System.DateTime Created { get; set; }
        [DataMember]
        public string Text { get; set; }
        //[DataMember]
        //public string UserName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public long? ParentID { get; set; }
        [DataMember]
        public int SortOrder { get; set; }
        [DataMember]
        public System.DateTime LastMod { get; set; }
        [DataMember]
        public bool Deleted { get; set; }
        [DataMember]
        public Nullable<long> CompanyID { get; set; }
        [DataMember]
        public Nullable<long> OriginalID { get; set; }
        [DataMember]
        public bool IsCompany { get; set; }
        [DataMember]
        public bool IsSection { get; set; }

        [DataMember]
        public ICollection<long> Tasks { get; set; }
        [DataMember]
        public ICollection<long> Contacts { get; set; }
        [DataMember]
        public ICollection<long> Files { get; set; }
    }

    //[DataContract]
    //public partial class WorkgroupMemberDTO
    //{
    //    public WorkgroupMemberDTO(WorkgroupMember remind)
    //    {
    //        MemberID = remind.ID;
    //        UserID = remind.UserID;
    //        UserInCompanyID = remind.UserInCompanyID;
    //        MemberState = remind.MemberState;
    //        Role = remind.Role;

    //        Workgroup = new WorkgroupDTO(remind.Workgroup);
    //    }
    //    [DataMember]
    //    public long MemberID { get; set; }
    //    [DataMember]
    //    public Nullable<long> UserID { get; set; }
    //    [DataMember]
    //    public Nullable<long> UserInCompanyID { get; set; }
    //    [DataMember]
    //    public WorkgroupMemberState MemberState { get; set; }
    //    [DataMember]
    //    public WorkgoupRole Role { get; set; }
    //    [DataMember]
    //    public WorkgroupDTO Workgroup { get; set; }
    //}

    //[DataContract]
    //public partial class ProfileRemindContactDTO
    //{

    //    public ProfileRemindContactDTO(ProfileRemindContact remind)
    //    {
    //        NotificationContactID = remind.ID;
    //        ConfirmationCode = remind.ConfirmationCode;
    //        ContactType = remind.ContactType;
    //        ProfileID = remind.ProfileID;
    //        Value = remind.Value;
    //        IsConfirmed = remind.IsConfirmed;
    //        ConfirmationErrorsCount = remind.ConfirmationErrorsCount;
    //        SendingErrorsCount = remind.SendingErrorsCount;
    //        ConfirmationSendsCount = remind.ConfirmationSendsCount;
    //        LastConfirmationSend = remind.LastConfirmationSend.HasValue ? new Nullable<DateTime>(remind.LastConfirmationSend.Value.UtcDateTime) : null;
    //        Username = remind.Username;

    //    }
    //    [DataMember]
    //    public long NotificationContactID { get; set; }
    //    [DataMember]
    //    public string ConfirmationCode { get; set; }
    //    [DataMember]
    //    public RemindContactType ContactType { get; set; }
    //    [DataMember]
    //    public long ProfileID { get; set; }
    //    [DataMember]
    //    public string Value { get; set; }
    //    [DataMember]
    //    public bool IsConfirmed { get; set; }
    //    [DataMember]
    //    public byte ConfirmationErrorsCount { get; set; }
    //    [DataMember]
    //    public short SendingErrorsCount { get; set; }
    //    [DataMember]
    //    public byte ConfirmationSendsCount { get; set; }
    //    [DataMember]
    //    public Nullable<System.DateTime> LastConfirmationSend { get; set; }
    //    [DataMember]
    //    public string Username { get; set; }
    //}

    //[DataContract]
    //public partial class ContactDataDTO
    //{
    //    public ContactDataDTO(ContactData remind)
    //    {
    //        DataID = remind.ID;
    //        ContactID = remind.Contact.ID;
    //        Type = remind.Type;
    //        Value = remind.Value;
    //        Name = remind.Name;
    //        SortOrder = remind.SortOrder;
    //    }

    //    [DataMember]
    //    public long DataID { get; set; }
    //    [DataMember]
    //    public long ContactID { get; set; }
    //    [DataMember]
    //    public int Type { get; set; }
    //    [DataMember]
    //    public string Value { get; set; }
    //    [DataMember]
    //    public string Name { get; set; }
    //    [DataMember]
    //    public int SortOrder { get; set; }
    //}

    //[DataContract]
    //public partial class UserInCompanyDTO
    //{
    //    public UserInCompanyDTO(UserInCompany remind)
    //    {
    //        UserInCompanyID = remind.ID;
    //        UserID = remind.UserID;
    //        CompanyID = remind.CompanyID;
    //        Status = remind.Status;
    //        SectionID = remind.SectionID;
    //        IsHeadOfSection = remind.IsHeadOfSection;
    //        EnableViewBills = remind.EnableViewBills;
    //        EnableAddUsers = remind.EnableAddUsers;
    //        Post = remind.Post;
    //        NameInCompany = remind.NameInCompany;
    //        Email = remind.Email;
    //        EnableManage = remind.EnableManage;
    //    }

    //    [DataMember]
    //    public long UserInCompanyID { get; set; }
    //    [DataMember]
    //    public Nullable<long> UserID { get; set; }
    //    [DataMember]
    //    public long CompanyID { get; set; }
    //    [DataMember]
    //    public UserInCompanyStatus Status { get; set; }
    //    [DataMember]
    //    public Nullable<long> SectionID { get; set; }
    //    [DataMember]
    //    public bool IsHeadOfSection { get; set; }
    //    [DataMember]
    //    public bool EnableViewBills { get; set; }
    //    [DataMember]
    //    public bool EnableAddUsers { get; set; }
    //    [DataMember]
    //    public string Post { get; set; }
    //    [DataMember]
    //    public string NameInCompany { get; set; }
    //    [DataMember]
    //    public string Email { get; set; }
    //    [DataMember]
    //    public bool EnableManage { get; set; }
    //}

    //[DataContract]
    //public partial class CompanyDTO
    //{

    //    public CompanyDTO(Company remind)
    //    {
    //        CompanyID = remind.ID;
    //        OwnerId = remind.OwnerId;
    //        Name = remind.Name;

    //        Members.Clear();
    //        //foreach (UserInCompany remind in remind.UserInCompany)
    //        //{
    //        //    Members.Add(new UserInCompanyDTO(remind));
    //        //}
    //    }

    //    [DataMember]
    //    public long CompanyID { get; set; }
    //    [DataMember]
    //    public long OwnerId { get; set; }
    //    [DataMember]
    //    public string Name { get; set; }
    //    [DataMember]
    //    public ICollection<UserInCompanyDTO> Members { get; set; }
    //}

    //[DataContract]
    //public partial class MessageDTO
    //{
    //    [DataMember]
    //    public long MessageID { get; set; }
    //    [DataMember]
    //    public Nullable<long> WorkgroupID { get; set; }
    //    [DataMember]
    //    public long From { get; set; }
    //    [DataMember]
    //    public Nullable<long> To { get; set; }
    //    [DataMember]
    //    public byte Type { get; set; }
    //    [DataMember]
    //    public System.DateTime Date { get; set; }
    //    [DataMember]
    //    public byte Status { get; set; }
    //    [DataMember]
    //    public string Text { get; set; }
    //    [DataMember]
    //    public Nullable<long> DiscussionID { get; set; }
    //    [DataMember]
    //    public string AuthorName { get; set; }
    //}

    //[DataContract]
    //public partial class RecordDTO
    //{
    //    public RecordDTO(Record remind)
    //    {
    //        ID = remind.ID;
    //        UserName = remind.UserName;
    //        Deleted = remind.Deleted;
    //        Date = remind.Date.UtcDateTime;
    //        StartTime = remind.StartTime;
    //        EndTime = remind.EndTime;
    //        TaskID = remind.TaskID;
    //        Waste = remind.Waste;
    //        WasCompleted = remind.WasCompleted;
    //        RecordText = remind.RecordText;
    //        FullText = remind.FullText;
    //        Created = remind.Created.UtcDateTime;
    //        LastMod = remind.LastMod.UtcDateTime;
    //    }
    //    [DataMember]
    //    public long ID { get; set; }
    //    [DataMember]
    //    public string UserName { get; set; }
    //    [DataMember]
    //    public bool Deleted { get; set; }
    //    [DataMember]
    //    public System.DateTime Date { get; set; }
    //    [DataMember]
    //    public Nullable<long> StartTime { get; set; }
    //    [DataMember]
    //    public Nullable<long> EndTime { get; set; }
    //    [DataMember]
    //    public Nullable<long> TaskID { get; set; }
    //    [DataMember]
    //    public Nullable<bool> Waste { get; set; }
    //    [DataMember]
    //    public Nullable<bool> WasCompleted { get; set; }
    //    [DataMember]
    //    public string RecordText { get; set; }
    //    [DataMember]
    //    public string FullText { get; set; }
    //    [DataMember]
    //    public System.DateTime Created { get; set; }
    //    [DataMember]
    //    public System.DateTime LastMod { get; set; }
    //}



    //[DataContract]
    //public partial class WorkgroupDTO
    //{

    //    public WorkgroupDTO(Workgroup remind)
    //    {
    //        ID = remind.ID;
    //        TaskID = remind.TaskID;
    //        OwnerID = remind.OwnerID;
    //        Name = remind.Name;
    //        State = remind.State;

    //        Members.Clear();
    //        //foreach (WorkgroupMember remind in remind.WorkgroupMembers)
    //        //{
    //        //    Members.Add(new WorkgroupMemberDTO(remind));
    //        //}
    //    }

    //    [DataMember]
    //    public long ID { get; set; }
    //    [DataMember]
    //    public long TaskID { get; set; }
    //    [DataMember]
    //    public long OwnerID { get; set; }
    //    [DataMember]
    //    public string Name { get; set; }
    //    [DataMember]
    //    public WorkgroupState State { get; set; }

    //    public ICollection<WorkgroupMemberDTO> Members { get; set; }
    //}

    //[DataContract]
    //public class SetWorkgroupMessageReadResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public SetWorkgroupMessageReadResult Result;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }

    //    public SetWorkgroupMessageReadResponse(SetWorkgroupMessageReadResult Result, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    [DataContract]
    public class SyncEntityInfo
    {
        [DataMember]
        public Nullable<System.DateTime> Created { get; set; }

        [DataMember]
        public long? ParentID;

        [DataMember]
        public Boolean Deleted;

        [DataMember]
        public System.DateTime LastMod { get; set; }

        [DataMember]
        public long EntityId;

        [DataMember]
        public EntityType EntityType;

        public SyncEntityInfo()
        { }

        public SyncEntityInfo(Task entity)
        {
            this.Created = entity.Created.UtcDateTime;
            this.ParentID = entity.ParentID;
            this.Deleted = entity.Deleted;
            this.LastMod = entity.LastMod.UtcDateTime;
            this.EntityId = entity.ID;
            this.EntityType = EntityType.TASK;
        }

        public SyncEntityInfo(Label entity)
        {
            this.Created = entity.Created.UtcDateTime;
            this.ParentID = entity.ParentID;
            this.Deleted = entity.Deleted;
            this.LastMod = entity.LastMod.UtcDateTime;
            this.EntityId = entity.ID;
            this.EntityType = EntityType.LABEL;
        }

        //public SyncEntityInfo(Contact remind)
        //{
        //    this.Created = remind.Created.UtcDateTime;
        //    this.ParentID = 0;
        //    this.Deleted = remind.Deleted;
        //    this.LastMod = remind.LastMod.UtcDateTime;
        //    this.EntityId = remind.ID;
        //    this.EntityType = EntityType.Contact;
        //}

        //public SyncEntityInfo(File remind)
        //{
        //    this.Created = remind.Created.UtcDateTime;
        //    this.ParentID = 0;
        //    this.Deleted = remind.Deleted;
        //    this.LastMod = remind.LastMod.UtcDateTime;
        //    this.EntityId = remind.ID;
        //    this.EntityType = EntityType.File;
        //}

        //public SyncEntityInfo(Record remind)
        //{
        //    this.Created = remind.Created.UtcDateTime;
        //    this.ParentID = 0;
        //    this.Deleted = remind.Deleted;
        //    this.LastMod = remind.LastMod.UtcDateTime;
        //    this.EntityId = remind.ID;
        //    this.EntityType = EntityType.DiaryRecord;
        //}
    }

    //[DataContract]
    //public class ClientInfo
    //{
    //    [DataMember]
    //    public String ClientName;

    //    [DataMember]
    //    public DateTime ClientTime;

    //    [DataMember]
    //    public int ClientTimezoneOffsetInMinutes;

    //    [DataMember]
    //    public String Language;
    //}

    //[DataContract]
    //public class ClientCredentials
    //{
    //    [DataMember]
    //    public String AuthToken;

    //    [DataMember]
    //    public String Username;
    //}

    //[DataContract]
    //public class BaseServiceRequest
    //{
    //    [DataMember]
    //    public ClientInfo ClientInfo;
    //}

    //public class AuthRequest : BaseServiceRequest
    //{
    //    public String Password;

    //    public String Username;

    //}

    //[DataContract]
    //public class AuthResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public AuthenticateResult Result;
    //    [DataMember]
    //    public String AuthToken;
    //    [DataMember]
    //    public UserDTO Profile;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }

    //    public AuthResponse()
    //    {
    //    }

    //    public AuthResponse(AuthenticateResult Result, String AuthToken, UserDTO Profile, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.AuthToken = AuthToken;
    //        this.Profile = Profile;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class AuthServiceRequest : BaseServiceRequest
    //{
    //    [DataMember]
    //    public ClientCredentials Credentials;
    //}

    //[DataContract]
    //public class GetContactResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public GetEntityResult Result;
    //    [DataMember]
    //    public ContactDTO Contact;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }

    //    public GetContactResponse(GetEntityResult Result, ContactDTO Contact, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.Contact = Contact;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class SetContactResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public SetEntityResult Result;
    //    [DataMember]
    //    public long EntityId;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }

    //    public SetContactResponse(SetEntityResult Result, long EntityId, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.EntityId = EntityId;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class ContactsListResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public GetEntityResult Result;
    //    [DataMember]
    //    public ContactDTO[] Entities;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }

    //    public ContactsListResponse(GetEntityResult Result, ContactDTO[] Entities, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.Entities = Entities;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class SetLabelResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public SetEntityResult Result;
    //    [DataMember]
    //    public long Entity;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }
    //    public SetLabelResponse(SetEntityResult Result, long Entity, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.Entity = Entity;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class GetLabelResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public GetEntityResult Result;
    //    [DataMember]
    //    public LabelDTO Label;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }
    //    public GetLabelResponse(GetEntityResult Result, LabelDTO Label, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.Label = Label;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class LabelsListResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public GetEntityResult Result;
    //    [DataMember]
    //    public LabelDTO[] Entities;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }
    //    public LabelsListResponse(GetEntityResult Result, LabelDTO[] Entities, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.Entities = Entities;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //public class CreateUserRequest : BaseServiceRequest
    //{
    //    public String Email;

    //    public String Password;
    //}

    [DataContract]
    public class CreateUserResponse : BaseServiceResponse
    {
        [DataMember]
        public CreateUserResult Result;
        //[DataMember]
        //public String AuthToken;
        //[DataMember]
        //public UserDTO Profile;

        [DataMember]
        public DateTime ServerTime { get; set; }
        public CreateUserResponse()
        {
        }

        public CreateUserResponse(CreateUserResult Result, DateTime ServerTime)
        {
            this.Result = Result;
            this.ServerTime = ServerTime;
        }
    }

    //[DataContract]
    //public class DiaryRecordResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public GetEntityResult Result;
    //    [DataMember]
    //    public RecordDTO Entity;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }
    //    public DiaryRecordResponse(GetEntityResult Result, RecordDTO Entity, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.Entity = Entity;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class DiaryRecordsListResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public GetEntityResult Result;
    //    [DataMember]
    //    public RecordDTO[] Entities;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }
    //    public DiaryRecordsListResponse(GetEntityResult Result, RecordDTO[] Entities, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.Entities = Entities;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class DiscussionPack
    //{
    //    [DataMember]
    //    public Nullable<long> GroupID { get; set; }

    //    [DataMember]
    //    public String GroupName;

    //    [DataMember]
    //    public DateTime LastDate;

    //    [DataMember]
    //    public long LastID;

    //    [DataMember]
    //    public String LastText;

    //    [DataMember]
    //    public int NewMessagesCount;

    //    [DataMember]
    //    public long OwnerID;

    //    [DataMember]
    //    public Nullable<long> TaskID { get; set; }

    //    [DataMember]
    //    public String TaskText;

    //    [DataMember]
    //    public Byte Type;

    //    public DiscussionPack()
    //    {
    //    }
    //}

    //[DataContract]
    //public class EntityListRequest : AuthServiceRequest
    //{
    //    [DataMember]
    //    public Boolean IncludeDependentEntities;

    //    [DataMember]
    //    public long[] EntityIdArray;

    //    public EntityListRequest()
    //    {
    //    }
    //}

    //[DataContract]
    //public class EntityTypesListRequest : AuthServiceRequest
    //{
    //    [DataMember]
    //    public Boolean GetContacts;

    //    [DataMember]
    //    public Boolean GetLabels;

    //    [DataMember]
    //    public Boolean GetDiary;

    //    [DataMember]
    //    public Boolean GetFiles;

    //    [DataMember]
    //    public Boolean GetTasks;

    //    [DataMember]
    //    public Nullable<System.DateTime> SyncDateTime { get; set; }

    //}

    //[DataContract]
    //public class EntityRequest : AuthServiceRequest
    //{
    //    [DataMember]
    //    public Boolean IncludeDependentEntities;

    //    [DataMember]
    //    public long EntityID;
    //}



    [DataContract]
    public class EntityListResponse : BaseServiceResponse
    {
        [DataMember]
        public GetEntityListResult Result;

        [DataMember]
        public SyncEntityInfo[] SyncEntities;

        [DataMember]
        public DateTime ServerTime { get; set; }
        public EntityListResponse(GetEntityListResult Result, SyncEntityInfo[] SyncEntities, DateTime ServerTime)
        {
            this.Result = Result;
            this.ServerTime = ServerTime;
            this.SyncEntities = SyncEntities;
            this.ServerTime = ServerTime;
        }
    }

    //[DataContract]
    //public class SendTaskMessageRequest : AuthServiceRequest
    //{
    //    [DataMember]
    //    public Nullable<long> LastLoadedMessageID { get; set; }

    //    [DataMember]
    //    public MessageDTO Message;
    //}

    //[DataContract]
    //public class SendTaskMessageResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public GetEntityResult Result;
    //    [DataMember]
    //    public MessageDTO[] Entities;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }
    //    public SendTaskMessageResponse(GetEntityResult Result, MessageDTO[] Entities, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.Entities = Entities;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class SetContactRequest : AuthServiceRequest
    //{
    //    [DataMember]
    //    public ContactDTO Contact;

    //    public SetContactRequest()
    //    {
    //    }
    //}

    //[DataContract]
    //public class SetLabelRequest : AuthServiceRequest
    //{
    //    [DataMember]
    //    public LabelDTO Label;
    //}

    //[DataContract]
    //public class SetDiaryRecordRequest : AuthServiceRequest
    //{

    //    [DataMember]
    //    public RecordDTO Record;

    //    public SetDiaryRecordRequest()
    //    {
    //    }
    //}

    //[DataContract]
    //public class SetTaskMessageReadRequest : AuthServiceRequest
    //{

    //    [DataMember]
    //    public long LastReadMessageID;

    //    [DataMember]
    //    public long WorkgroupID;

    //    public SetTaskMessageReadRequest()
    //    {
    //    }
    //}

    //[DataContract]
    //public class SetTaskRequest : AuthServiceRequest
    //{
    //    [DataMember]
    //    public TaskDTO Simple;
    //}

    //[DataContract]
    //public class TaskMessage
    //{

    //    [DataMember]
    //    public Nullable<long> AccUserID { get; set; }

    //    [DataMember]
    //    public long AuthorID;

    //    [DataMember]
    //    public String AuthorName;

    //    [DataMember]
    //    public DateTime Date;

    //    [DataMember]
    //    public long EventID;

    //    [DataMember]
    //    public long OwnerID;

    //    [DataMember]
    //    public Nullable<long> TaskID { get; set; }

    //    [DataMember]
    //    public String Text;

    //    [DataMember]
    //    public Byte Type;

    //    [DataMember]
    //    public long WorkgroupID;

    //    public TaskMessage()
    //    {
    //    }

    //}

    [DataContract]
    public class TasksListResponse : BaseServiceResponse
    {
        [DataMember]
        public GetEntityListResult Result;
        [DataMember]
        public TaskDto[] Entities;

        [DataMember]
        public DateTime ServerTime { get; set; }
        public TasksListResponse(GetEntityListResult Result, TaskDto[] Entities, DateTime ServerTime)
        {
            this.Result = Result;
            this.Entities = Entities;
            this.ServerTime = ServerTime;
        }
    }

    //[DataContract]
    //public class UserProfileResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public GetEntityResult Result;
    //    [DataMember]
    //    public UserDTO Entity;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }
    //    public UserProfileResponse(GetEntityResult Result, UserDTO Entity, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.Entity = Entity;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class WarningMessage
    //{

    //    [DataMember]
    //    public long CompanyID;

    //    [DataMember]
    //    public String CompanyName;

    //    [DataMember]
    //    public DateTime Date;

    //    [DataMember]
    //    public long EventID;

    //    [DataMember]
    //    public long RelID;

    //    [DataMember]
    //    public Byte Type;

    //    [DataMember]
    //    public String Username;

    //    public WarningMessage()
    //    {
    //    }

    //}

    //[DataContract]
    //public class WorkgroupMessagesRequest : AuthServiceRequest
    //{
    //    //0 - all
    //    //1 - before
    //    //2 - after
    //    [DataMember]
    //    public byte QueryType;

    //    [DataMember]
    //    public Nullable<long> RelatedMessageID { get; set; }

    //    [DataMember]
    //    public long WorkgroupID;

    //    public WorkgroupMessagesRequest()
    //    {
    //    }

    //}

    //[DataContract]
    //public class WorkgroupMessagesResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public GetEntityResult Result;

    //    [DataMember]
    //    public long LastReadMessage;

    //    [DataMember]
    //    public MessageDTO[] Messages;

    //    [DataMember]
    //    public int MessagesCountAfter;

    //    [DataMember]
    //    public int MessagesCountBefore;

    //    [DataMember]
    //    public byte QueryType;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }
    //    public WorkgroupMessagesResponse(GetEntityResult Result, long LastReadMessage,
    //       MessageDTO[] Messages, int MessagesCountAfter, int MessagesCountBefore, byte QueryType, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.LastReadMessage = LastReadMessage;
    //        this.Messages = Messages;
    //        this.MessagesCountAfter = MessagesCountAfter;
    //        this.MessagesCountBefore = MessagesCountBefore;
    //        this.QueryType = QueryType;
    //        this.ServerTime = ServerTime;
    //    }
    //}

    //[DataContract]
    //public class SetTaskListRequest : AuthServiceRequest
    //{
    //    [DataMember]
    //    public TaskDTO[] Tasks;
    //}

    //[DataContract]
    //public class SetEntityListResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public SetEntityListResult Result;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }

    //    [DataMember]
    //    public SetEntityResponse[] SetEntityResponseArray;
    //    public SetEntityListResponse(SetEntityListResult Result, SetEntityResponse[] SetEntityResponseArray, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.SetEntityResponseArray = SetEntityResponseArray;
    //        this.ServerTime = ServerTime;
    //    }

    //    public SetEntityListResponse()
    //    {

    //    }
    //}

    //[DataContract]
    //public class SetLabelListRequest : AuthServiceRequest
    //{
    //    [DataMember]
    //    public LabelDTO[] Labels;
    //}

    //[DataContract]
    //public class SetLabelListResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public SetEntityListResult Result;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }

    //    [DataMember]
    //    public SetEntityResponse[] SetLabelResponseArray;
    //    public SetLabelListResponse(SetEntityListResult Result, SetEntityResponse[] SetLabelResponseArray, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.SetLabelResponseArray = SetLabelResponseArray;
    //        this.ServerTime = ServerTime;
    //    }

    //    public SetLabelListResponse()
    //    {

    //    }
    //}

    //[DataContract]
    //public class SetContactListRequest : AuthServiceRequest
    //{
    //    [DataMember]
    //    public ContactDTO[] Contacts;
    //}

    //[DataContract]
    //public class SetContactListResponse : BaseServiceResponse
    //{
    //    [DataMember]
    //    public SetEntityListResult Result;

    //    [DataMember]
    //    public DateTime ServerTime { get; set; }

    //    [DataMember]
    //    public SetEntityResponse[] SetContactResponseArray;
    //    public SetContactListResponse(SetEntityListResult Result, SetEntityResponse[] SetContactResponseArray, DateTime ServerTime)
    //    {
    //        this.Result = Result;
    //        this.SetContactResponseArray = SetContactResponseArray;
    //        this.ServerTime = ServerTime;
    //    }

    //    public SetContactListResponse()
    //    {

    //    }
    //}
    public interface BaseServiceResponse
    {
        DateTime ServerTime { get; set; }
    }

    [DataContract]
    public class EntityData
    {
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public EntityType entityType { get; set; }
        public EntityData(long id, EntityType entityType)
        {
            this.id = id;
            this.entityType = entityType;
        }
    }


}

