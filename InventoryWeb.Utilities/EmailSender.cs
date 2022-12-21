using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace InventoryWeb.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(subject, htmlMessage, email);
        }
        public Task Execute(string subject, string mensaje, string email)
        {
            MailMessage mm = new MailMessage();
            mm.To.Add(email);
            mm.Subject = subject;
            mm.Body = mensaje;
            mm.From = new MailAddress("enrique.gomezcr@gmail.com");
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.sendgrid.net");
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("apikey",
                "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");
            return smtp.SendMailAsync(mm);
        }
        //public Task SendEmailAsync(string email, string subject, string htmlMessage)
        //{
        //    throw new NotImplementedException();
        //}
    }

}
