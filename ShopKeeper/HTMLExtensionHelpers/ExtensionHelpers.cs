using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopKeeper.HTMLExtensionHelpers
{
    public class ExtensionHelpers
    {
        public Dictionary<string, object> RadioButtonPropHandler(bool condition)
        {
            try
            {
                var htmlAttributes = new Dictionary<string, object>();
                if (condition)
                {
                    htmlAttributes.Add("disabled", "disabled");
                    //htmlAttributes.Add("checked", "checked");
                }

                else
                {
                    htmlAttributes.Remove("disabled");
                    htmlAttributes.Remove("checked");
                }

                return htmlAttributes;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
               return new Dictionary<string, object>();
            }
        }

        public static Dictionary<string, object> CheckboxPropHandler(int condition, int id)
        {
            try
            {
                var htmlAttributes = new Dictionary<string, object>();
                if (condition > 0)
                {
                    //htmlAttributes.Add("disabled", "disabled");
                    htmlAttributes.Add("checked", "checked");
                }

                if (condition < 1)
                {
                    //htmlAttributes.Remove("disabled");
                    htmlAttributes.Remove("checked");
                }
                htmlAttributes.Add("style", "display: none");

                htmlAttributes.Add("class", "rtx" + id);
                return htmlAttributes;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
              return  new Dictionary<string, object>();
            }
        }

        public List<string> ListOption<T>(List<T> g, string val, string nm, string optionalLabel)
        {
            try
            {
                var list = new List<string>();

                if (g.Count < 1)
                {
                    list.Add("<option value=" + '"' + 0 + '"' + ">" + "-- List is empty --" + "</option>");
                    return list;
                }

                list.Add("<option value=" + '"' + 0 + '"' + ">" + optionalLabel + "</option>");

                list.AddRange(from lx in g let cx = lx.GetType().GetProperty(val) let tx = lx.GetType().GetProperty(nm) let ty = cx.GetValue(lx, null) let yx = tx.GetValue(lx, null) select "<option value=" + '"' + ty + '"' + ">" + yx + "</option>");

                return list;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return new List<string>();
            }
        }

        /// <summary>
        /// Ensures that all breakpoints on Enter key in a text stored in a database are returned as new line elements when the text is rendered on a view.
        /// </summary>
        /// <param name="htmlhelper"></param>
        /// <param name="data">The text to be rendered in the right format on a view</param>
        /// <returns></returns>
        public static IHtmlString DisplayFormattedData(HtmlHelper htmlhelper, string data)
        {
            var result = string.Join("<br />", data.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Select(htmlhelper.Encode));
            return new MvcHtmlString(result);
        }
    }
}