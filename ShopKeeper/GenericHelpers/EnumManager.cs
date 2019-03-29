using System.ComponentModel;

// ReSharper disable InconsistentNaming
namespace ShopKeeper.GenericHelpers
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
    
    public enum WellReportFields
    {
        [Description("Company")]
        Company = 1,
        [Description(" Well Name")]
        Well_Name,
        [Description("Well Type")]
        Well_Type,
         [Description("Well Class")]
        Well_Class,
        [Description("Terrain")]
         Terrain,
        [Description("Zone")]
        Zone,
        [Description("Total Depth(FT)")]
        Total_Depth,
        [Description("SPU Date")]
        SPU_Date,
        //[Description("Rig Name")]
        //Rig_Name
    }

    #region Billing Enums
    public enum PaymentCurrency
    {
        Nigerian_Naira = 566,
        US_Dollar = 840
    }

    public enum CustomerValidationResponseStatus
    {
        Valid_Customer = 0,
        Invalid_Customer = 1,
        Expired_Customer = 2
    }

    public enum PaymentNotificationResponseStatus
    {
        Payment_Successful_Received = 0,
        Payment_Rejected = 1
    }

    #endregion



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
    public enum UserProfilenelSysLog
    {
        logId = 6
    }

    public enum SuperAdmin
    {
        Super_Admin = 1
    }

    public enum CompanyAdmin
    {
        Company_Admin = 2
    }

    public enum DirectorEnum
    {
        Director = 1
    }
    public enum GenderInfo
    {
        Male = 1,
        Female
    }

    public enum UserTypeEnum
    {
        Company = 1,
        UserProfile
    }

    public enum EmployeeStatus
    {
        Active = 1,
        Suspended,
        On_Leave,
        InActive
    }

}
