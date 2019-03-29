using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopKeeper.GenericHelpers.BillingUtilities
{
    public class GenericBill
    {
        //public class PaymentNotificationRequest
        //{
              
        //}

        public class PaymentNotificationResponse
        {

        }

        public class CustomerInformationRequest
        {
            public string MerchantReference { get; set; }
            public string CustReference { get; set; }
            public string ServiceUsername { get; set; }
            public string ServicePassword { get; set; }
            public string FtpUsername { get; set; }
            public string FtpPassword { get; set; }
        }

        public class CustomerInformationResponse
        {
            public int Status { get; set; }
            public List<Customer> Customers { get; set; }
            
        }

        public class Customer
        {
            public string CustReference { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string OtherName { get; set; }
            public string Email { get; set; }
            public string ThirdPartyCode { get; set; }
            public string Phone { get; set; }
        }


        public class PaymentNotificationRequest
        {
            public int Status { get; set; }
            public List<Customer> Customers { get; set; }
            
        }
    }
}


//<PaymentNotificationRequest>
//<RouteId>HTTPGENERICv31</RouteId>
//<ServiceUrl>http://ies.mutuallifeng.com/life/xml/notifresp.php</ServiceUrl>
//<ServiceUsername/>
//<ServicePassword/>
//<FtpUrl>http://ies.mutuallifeng.com/life/xml/notifresp.php</FtpUrl>
//<FtpUsername/>
//<FtpPassword/>
//<Payments>
//<Payment>
//<ProductGroupCode>HTTPGENERICv31</ProductGroupCode>
//<PaymentLogId>8963962</PaymentLogId>
//<CustReference>ISP/13/136849/LAG</CustReference>
//<AlternateCustReference>--N/A--</AlternateCustReference>
//<Amount>10000.00</Amount>
//<PaymentStatus>0</PaymentStatus>
//<PaymentMethod>Cash</PaymentMethod>
//<PaymentReference>SBP|BRH|MTBS|7-07-2014|380481</PaymentReference>
//<TerminalId/>
//<ChannelName>Bank Branc</ChannelName>
//<Location>ILUPEJU BRANCH</Location>
//<IsReversal>False</IsReversal>
//<PaymentDate>07/07/2014 16:08:34</PaymentDate>
//<SettlementDate>07/08/2014 00:00:01</SettlementDate>
//<InstitutionId>MTBS</InstitutionId>
//<InstitutionName>Mutual Benefits Life Assurance</InstitutionName>
//<BranchName>ILUPEJU BRANCH</BranchName>
//<BankName>Sterling Bank Plc</BankName>
//<FeeName/>
//<CustomerName>BRIGHT OSEGHELE EHIZOJIE</CustomerName>
//<OtherCustomerInfo>|</OtherCustomerInfo>
//<ReceiptNo>1418814666</ReceiptNo>
//<CollectionsAccount>900090559901000600</CollectionsAccount>
//<ThirdPartyCode/>
//<PaymentItems>
//<PaymentItem>
//<ItemName>Premium Payment</ItemName>
//<ItemCode>1100</ItemCode>
//<ItemAmount>10000.00</ItemAmount>
//<LeadBankCode>SBP</LeadBankCode>
//<LeadBankCbnCode>232</LeadBankCbnCode>
//<LeadBankName>Sterling Bank Plc</LeadBankName>
//<CategoryCode/>
//<CategoryName/>
//<ItemQuantity>1</ItemQuantity>
//</PaymentItem>
//</PaymentItems>
//<BankCode>SBP</BankCode>
//<CustomerAddress/>
//<CustomerPhoneNumber/>
//<DepositorName/>
//<DepositSlipNumber>6236182</DepositSlipNumber>
//<PaymentCurrency>566</PaymentCurrency>
//<OriginalPaymentLogId/>
//<OriginalPaymentReference/>
//<Teller>AKINOLA BASHIRU</Teller>
//</Payment>
//</Payments>
//</PaymentNotificationRequest>