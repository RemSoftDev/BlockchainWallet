using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Wallet.Helpers
{
    public static class EmailHelper
    {
        public static async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Blowaud", "Blowaud@blowaud.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587);
                await client.AuthenticateAsync("Blowaud@gmail.com", "BlowaudPassword");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }

        public static string GetEmailConfirmationMessage(string url)
        {
            return $"To confirm email click : <a href='{url}'>link</a>";
        }

        public static string GetResetPasswordMessage(string url)
        {
            return $"To reset password click : <a href='{url}'>link</a>";
        }
    }
}