using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace ShopKeeper.GenericHelpers.ProviderKeyHelper
{
    public static class AccessProviderKeyGenerator
    {
      public static long GenerateAccessKey()
        {
            var byt = new byte[1024];
            var rngCrypto = new RNGCryptoServiceProvider();
            rngCrypto.GetBytes(byt);
            var accessKey = BitConverter.ToUInt32(byt, 0);
            return accessKey;
        }

        public static int GetRandomInteger(int maxValue)
        {
            return new Random((int)DateTime.Now.Ticks).Next(1, maxValue);
        }
    }
}