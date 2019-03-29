using System.ComponentModel;

// ReSharper disable InconsistentNaming
namespace Shopkeeper.Repositories.Utilities
{
    public enum MonthList
    {
        [Description("January")]
        Janurary = 1,
        [Description("February")]
        February = 2,
        [Description("March")]
        March = 3,
        [Description("April")]
        April = 4,
        [Description("May")]
        May = 5,
        [Description("June")]
        June = 6,
        [Description("July")]
        July = 7,
        [Description("August")]
        August = 8,
        [Description("September")]
        September = 9,
        [Description("October")]
        October = 10,
        [Description("November")]
        November = 11,
        [Description("December")]
        December = 12
    }
    
    public enum CompletionStatus
    {
        Completed = 1,
        Uncompleted
    }

    public enum AddressTypeEnum
    {
        Registered = 1,
        Operational,
        Branch_Office,
    }

    public enum DefaultImageView
    {
        Default_View = 1,
    }

    public enum ConnStringName
    {
        ShopKeeperStoreEntities
    }

    public enum EmailTemplateEnum
    {
        Token_Request = 1,
        Activate_Your_Account,
        Password_Updated,
        Registeration_Completed,
        UserProfilenel_Account,
        Update_Email,
        Update_PhoneNumber,
    }

    public enum ServiceResponseEnum
    {
        Invalid_Provider_Key = 300,
        Invalid_Provider_Secrete = 303,
        Unknown_Provider = 304,
        User_not_found = 404,
        User_Authentication_Failed = 402,
        Success = 200,
        Server_Error = 500,
        User_Account_Locked_Out = 501,
        Company_Account_Locked_Out = 402,
        User_Account_Requires_Verification = 503,
        
    }
  
    public enum GenderInfo
    {
        Male = 1,
        Female
    }
    
    public enum EmployeeStatus
    {
        Active = 1,
        Suspended,
        On_Leave,
        InActive
    }

    public enum SubscriptionStatus
    {
        Active = 1,
        InActive,
        Trial,
        Extended,
        Suspended,
        Expired
    }

    public enum SubscriptionPackageInfo
    {
        Trial = 1
    }
}
