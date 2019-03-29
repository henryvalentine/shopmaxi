using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Xml;

namespace ShopKeeper.Controllers
{
    public class HanshakerController : Controller
    {
        // GET api/hanshaker
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/hanshaker/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/hanshaker
        public void ConfirmCustomerDetails()
        {

            var res = "";
            if (Request.InputStream != null)
            {
                var stream = new StreamReader(Request.InputStream);
                string x = stream.ReadToEnd();
                res = HttpUtility.UrlDecode(x);
            }
            
            
            
            //var httpWReq = (HttpWebRequest)WebRequest.Create("http://api.eve-online.com/" + "url" + "?userID=" + "userId" + "&apiKey=" + "apiKey");
            //var httpWResp = (HttpWebResponse)httpWReq.GetResponse();
           
            //var ts = httpWReq.ToString();
            //var sr = new StreamReader(httpWResp.GetResponseStream(), Encoding.ASCII);
            //var sGlobal = sr.ReadToEnd();
            //sr.Close();

            //var xml = new XmlDocument();
            //xml.LoadXml(sGlobal);
            //var nodelist = xml.SelectNodes("/eveapi/result/rowset/row");
            //for (int i = 0; i < nodelist.Count; i++)
            //{
            //   // Set for each element in the nodelist to XmlNode
            //   var node = nodelist[i];

            //   // Get the node's attributes
            //   XmlAttributeCollection attCol = node.Attributes;

               // Print them for testing
              //Console.WriteLine(attCol[0].Value + "<br/>"); (remove comments if you wish to test this in the console)
             // if (i == 0)
             // radioButton1.Text = attCol[0].Value;
             // else
             //if (i == 1)
             // radioButton2.Text = attCol[0].Value;
             // else
             // if (i == 2)
             // radioButton3.Text = attCol[0].Value;
             //}
        }

        // PUT api/hanshaker/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/hanshaker/5
        public void Delete(int id)
        {
        }
    }
}
