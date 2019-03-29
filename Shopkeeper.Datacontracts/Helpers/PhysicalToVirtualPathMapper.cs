using System;
using System.Web;

namespace Shopkeeper.Datacontracts.Helpers
{
    public static class PhysicalToVirtualPathMapper
    {
        public static string MapPath(string path)
        {
            return @"~/" + path.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], String.Empty).Replace('\\', '/');
        }
    }

    public static class InvoiceNumberGenerator
    {
        public static string GenerateNumber(long id)
        {
            try
            {
                if (id < 1)
                {
                    return "";
                }

                string sid;

                if (id >= 999999)
                {
                    sid = id.ToString();
                }
                else
                {
                    var preceedingZeros = "";
                    var numStr = id.ToString();
                    const string countStr = "99999";

                    for (var z = numStr.Length; z < countStr.Length; z++)
                    {
                        preceedingZeros += "0";
                    }

                    sid = preceedingZeros + numStr;
                }

                return sid;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}