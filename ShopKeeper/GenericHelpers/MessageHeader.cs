using System;
using System.Net.Mail;
using System.Net.Mime;
using ImportPermitPortal.DataObjects.Helpers;
using Shopkeeper.Datacontracts.Helpers;

namespace ShopKeeper.GenericHelpers
{
    /// <summary>
    /// The Header for all messages
    /// </summary>
    public static class MessageHeader
    {
        private static string setLabelMsg()
        {
            return "</label></div></div><br /><br /><div style=\"margin-right: -15px; margin-left: -15px; display: table; clear: both; width: 100%; margin-left: 2%; margin-bottom: 2%\">";
        }
        /// <summary>
        /// Completes the Message body
        /// </summary>
        /// <returns></returns>
        private static string SetBodyMarkUp()
        {

            return "</div></div><br /><br />";
        }

        /// <summary>
        /// Compiles the email message markup with the message content
        /// </summary>
        /// <param name="msgId">
        /// the Id of last email message
        /// </param>
        /// <param name="path">
        /// Logo path
        /// </param>
        /// <param name="mail">
        /// Mail to be formatted and eventually sent
        /// </param>
        /// <returns></returns>
        public static MailMessage GetMsgMarkUp(long msgId, string path, MailMessage mail)
        {
            try
            {
                _filePath = path;
                var formatedMail = Kffl(mail, msgId);
                if (formatedMail == null)
                {
                    return null;
                }
                return formatedMail;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace, ex.Source, ex.Message);
                return null;
            }
        }

        private static string _filePath = "";

        private static MailMessage Kffl(MailMessage mail, long msgId)
        {
            try
            {
                var logo = new LinkedResource(_filePath, MediaTypeNames.Image.Jpeg)
                {
                    ContentId = "msgLog"
                };
                
                const string header = "<div style=\"border: 1px solid #c0c0c0; margin-right: -15px;margin-left: -15px;display: table; clear: both; width: 100%\">"
                                   + "<div style=\"margin-right: -15px;margin-left: -15px;display: table; clear: both;\">"
                                    + "<img src= \"cid:msgLog\" /></div>"
                                     + "<div style= \"margin-top: 2%; margin-right: -15px; margin-left: -15px; display: table; clear: both; width: 100%\">"
                                      //+ "<div style= \"width: 10%;position: relative;min-height: 1px;padding-right: 15px;padding-left: 15px;float: left;\"></div>"
                                      + "<div style=\"width: 30%;position: relative;min-height: 1px;padding-right: 15px;padding-left: 25px;float: left;\">"
                                       + "<label style=\"color: #000; font-weight: bold\">DPR-CONNECT :: <span style=\"font-weight: normal\">Single Sign on, many possibilities!</span></label></div>"
                                       + "<div style=\"width: 40%;position: relative;min-height: 1px;padding-right: 15px;padding-left: 15px;float: left;\">"
                                       + "<label style=\"font-weight: bold; text-decoration: underline\">Ref: DPR/Connect/";

                var msgLabel = setLabelMsg();
                var msgEndPart = SetBodyMarkUp();

                var mailBody = header + DateTime.Now.Year + "/" + (msgId + 1) + msgLabel + mail.Body + msgEndPart;

                var avHtml = AlternateView.CreateAlternateViewFromString(mailBody, null, MediaTypeNames.Text.Html);
                avHtml.LinkedResources.Add(logo);
                mail.AlternateViews.Add(avHtml);
                mail.IsBodyHtml = true;
                //mail.Attachments.Add(att);
                return mail;
            }
            catch (Exception ex)
            {
                ErrorLogger.LogError(ex.StackTrace,ex.Source,ex.Message);
                return new MailMessage();
            }
        }
    }
}