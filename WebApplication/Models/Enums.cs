using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace WebApplication.Models
{
    public enum AuthenticateResult
    {
        SUCCESS = 0,
        USERNAME_AND_PASSWORD_DO_NOT_MATCH = 1,
        USERNAME_DOES_NOT_EXIST = 2,
        GOOGLE_SIGN_IN_ERROR_UNKNOWN = 3,   
    }

    public enum CreateUserResult
    {
        SUCCESS = 0,
        THE_USER_WITH_REQUESTED_USERNAME_ALREADY_EXISTS = 1,
        ERROR_UNKNOWN = 2
    }

    public enum GetEntityResult
    {
        SUCCESS = 0,
        ACCESS_DENIED = 1,
        ENTITY_IS_NOT_FOUND = 2,
        ENTITY_IS_DELETED = 3,
    }

    public enum SetEntityResult
    {
        SAVED = 0,
        ENTITY_ON_THE_SERVER_IS_NEWER = 1,
        ENTITY_HAS_INCORRECT_DATA = 2,
        ENTITY_IS_NOT_FOUND = 3,
        ENTITY_IS_DELETED = 4,
        PARENT_ENTITY_IS_NOT_FOUND = 5,
        PARENT_ENTITY_IS_DELETED = 6,
        ACCESS_DENIED = 7,  
    }

    public enum GetEntityListResult
    {
        SUCCESS = 0,
        ACCESS_DENIED = 1,
        NOT_ALL_ENTITIES_ARE_FOUND = 2
    }

    public enum SetEntityListResult
    {
        SUCCESS = 0,
        ACCESS_DENIED = 1,
    }

    public enum RecurrenceInterval : int
    {
        ONE_TIME = 1, MINUTES = 2, HOURS = 3, DAYS = 4, WEEKS = 5, MONTHS_ON_DATE = 6, MONTHS_ON_NTH_WEEK_DAY = 7, YEARS = 8
    }
    public enum TaskPriority : int
    {
        NORMAL = 0, LOW = 1, HIGH = 2
    }


    public enum ReminderTimeMode
    {
        TIME_BEFORE_EVENT = 1, TIME_AFTER_EVENT = 2, AFTER_NOW = 3, ABSOLUTE_TIME = 4
    }
  

    public enum EntityType
    {
        TASK = 0, LABEL = 1, CONTACT = 2, FILE = 3, DIARY_RECORD=4, MESSAGE=5, REMINDER = 6
    }
}
