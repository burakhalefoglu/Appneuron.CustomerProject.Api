using System.IO;

namespace Core.Utilities.Mail.Helpers
{
    public static class MailContentHepler
    {
        public static string GetResetMailContent(string name, string token)
        {
            var MailText = ReadMailText("ResetMail.html");
            MailText = MailText.Replace("[Link]",
                    "https://localhost:44375/WebAPI/api/Auth/resetpassword?token=" + token)
                .Replace("[Name]", name);
            return MailText;
        }

        private static string ReadMailText(string templateName)
        {
            var path = Directory.GetCurrentDirectory();
            var FilePath = Path.GetFullPath(Path.Combine(path,
                @"..\" + "\\Core\\Utilities\\Mail\\MailTemplates\\" + templateName));

            var str = new StreamReader(FilePath);
            var MailText = str.ReadToEnd();
            str.Close();
            return MailText;
        }
    }
}