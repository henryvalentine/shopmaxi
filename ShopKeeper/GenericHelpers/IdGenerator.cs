
using System;

namespace ShopKeeper.GenericHelpers
{
    public class IdGenerator
    {
         public static string GenerateId(long companyId)
         {
             return companyId + " -" + DateTime.Now.ToString("yyyyMMdd");
         }
    }
}
