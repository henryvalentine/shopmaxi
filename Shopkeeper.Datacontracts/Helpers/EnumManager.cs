namespace Shopkeeper.Datacontracts.Helpers
{
    public enum MessageEventEnum
    {
        New_Account = 1,
        New_User,
        Account_Confirmation,
        Password_Reset,
        Password_Reset_Successful,
        Activation_Link_Request,
        Payment_Alert
    }

    public enum MessageStatus
    {
        Sent = 1,
        Pending,
        Failed
    }

    public enum PurchaseOrderDeliveryStatus
    {
        Pending = 1,
        Partly_Delivered,
        Completely_Delivered
    }

    public enum PurchaseOrderInvoiceStatus
    {
        Pending = 1,
        Revoked,
        Accepted
    }
    
    public enum TransactionTypeEnum
    {
        Credit = 1,
        Debit
    }

    public enum EstimateStatus
    {
        Pending = 1,
        Converted_To_Invoice,
        ReUse
    }

    public enum SaleStatus
    {
        Completed = 1,
        Partly_Paid,
        Refund_Note_Issued,
        Partly_Refunded
    }


    public enum TransfereNoteStatus
    {
        Pending,
        Voided,
        Partly_Trasnfered,
        Completely_Transfered
    }

}