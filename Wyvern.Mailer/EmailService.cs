using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Wyvern.Mailer;

public class EmailService
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly bool _useSsl;
    private readonly bool _useStartTls;

    public EmailService(string host, int port, string username, string password, bool useSsl = false, bool useStartTls = true)
    {
        _host = host;
        _port = port;
        _username = username;
        _password = password;
        _useSsl = useSsl;
        _useStartTls = useStartTls;
    }

    public async Task SendEmailAsync(string fromName, string fromEmail, string toName, string toEmail, string subject, string bodyHtml)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = bodyHtml };

        using var client = new SmtpClient();

        if (_useSsl)
            await client.ConnectAsync(_host, _port, SecureSocketOptions.SslOnConnect);
        else if (_useStartTls)
            await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls);
        else
            await client.ConnectAsync(_host, _port, SecureSocketOptions.None);

        await client.AuthenticateAsync(_username, _password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendEmailAsync(EmailType type, string locale, string toName, string toEmail, object data)
    {
        var templateName = TemplateManager.GetTemplateName(type);
        var subjectKey = TemplateManager.GetSubjectKey(type);

        var bodyHtml = TemplateManager.RenderTemplate(templateName, data, locale);
        var subject = TemplateManager.GetLocaleString(subjectKey, data, locale);

        await SendEmailAsync("Wyvern", _username, toName, toEmail, subject, bodyHtml);
    }
}
