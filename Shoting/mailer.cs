using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace Shoting
{
    public static class mailer
    {
        public static void SendMail(string text, string subject, string DestinationMail = "excrucire@gmail.com", MemoryStream attachment = null, string fileName = null)
        {
            try
            {
                MailMessage mail = new MailMessage("bezveze@moj.tz", DestinationMail);

                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Host = "smtp.gmail.com";

                client.Timeout = 200000;

                mail.Subject = subject;
                mail.Body = text;

                mail.From = new MailAddress("odavde@momo.hr", "Shoting");

                if (attachment != null)
                {
                    System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("image/png");
                    Attachment attach = new Attachment(attachment, ct);
                    attach.ContentDisposition.FileName = fileName;

                    mail.Attachments.Add(attach);
                }

                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("subtle.parser", "ovojesifra");

                client.Send(mail);

                client.Dispose();
                mail.Dispose();
            }
            catch (Exception e)
            {
                //Console.WriteLine("Mail NOT sent!", "mailer ERROR");
                return;
            }

            //Console.WriteLine("Mail sent!");
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}