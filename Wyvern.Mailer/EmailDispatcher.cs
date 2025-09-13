using System.Threading.Tasks;

namespace Wyvern.Mailer
{
    public static class EmailDispatcher
    {
        private static readonly EmailService _emailService = new EmailService(
            "mail.wyvern.gg",
            587,
            "noreply@wyvern.gg",
            "YOUR_PASSWORD_HERE"
        );

        public static async Task SendEmailAsync(
            EmailType type,
            string locale,
            object data,
            string recipientEmail,
            string recipientName = "")
        {
            string templateName = GetTemplateName(type);
            string subjectKey = GetSubjectKey(type);

            string bodyHtml = TemplateManager.RenderTemplate(templateName, data, locale);
            string subject = TemplateManager.GetLocaleString(subjectKey, data, locale);

            await _emailService.SendEmailAsync(
                "Wyvern Social",
                "noreply@wyvern.gg",
                string.IsNullOrWhiteSpace(recipientName) ? recipientEmail : recipientName,
                recipientEmail,
                subject,
                bodyHtml
            );
        }

        private static string GetTemplateName(EmailType type) => type switch
        {
            EmailType.Welcome => "welcome.hbs",
            _ => throw new System.ArgumentOutOfRangeException(nameof(type), "Unknown email type")
        };

        private static string GetSubjectKey(EmailType type) => type switch
        {
            EmailType.Welcome => "Welcome.Subject",
            _ => throw new System.ArgumentOutOfRangeException(nameof(type), "Unknown email type")
        };
    }
}
