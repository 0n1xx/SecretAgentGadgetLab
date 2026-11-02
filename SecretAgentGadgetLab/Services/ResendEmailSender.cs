using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SecretAgentGadgetLab.Services
{
    /// <summary>
    /// IEmailSender via Resend HTTP API.
    /// Set RESEND_API_KEY and optionally Email:FromAddress in Railway / env.
    /// </summary>
    public class ResendEmailSender : IEmailSender
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<ResendEmailSender> _logger;

        public ResendEmailSender(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<ResendEmailSender> logger)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _config["RESEND_API_KEY"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogError("RESEND_API_KEY is not set — confirmation email was not sent to {Email}", email);
                return;
            }

            var fromAddress = _config["Email:FromAddress"] ?? "noreply@secretagentlab.store";
            var fromName = _config["Email:FromName"] ?? "Secret Agent Gadget Lab";

            var payload = new
            {
                from = $"{fromName} <{fromAddress}>",
                to = new[] { email },
                subject,
                html = htmlMessage
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var client = _httpClientFactory.CreateClient("resend");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var response = await client.PostAsync("https://api.resend.com/emails", content);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Resend API error {Status}: {Body}", (int)response.StatusCode, body);
            }
            else
            {
                _logger.LogInformation("Email sent via Resend to {Email} ({Subject})", email, subject);
            }
        }

        public static string BuildConfirmEmailHtml(string confirmLink)
        {
            return $@"
<!DOCTYPE html>
<html lang=""en"">
<head><meta charset=""UTF-8""><meta name=""viewport"" content=""width=device-width,initial-scale=1""></head>
<body style=""margin:0;padding:0;background:#0a0f0a;font-family:Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background:#0a0f0a;padding:48px 0;"">
    <tr><td align=""center"">
      <table width=""480"" cellpadding=""0"" cellspacing=""0""
             style=""background:#111811;border-radius:12px;border:1px solid #1f3d1f;overflow:hidden;"">
        <tr><td style=""padding:32px 40px 24px;"">
          <div style=""font-size:12px;letter-spacing:2px;color:#7CFF7C;text-transform:uppercase;"">Secret Agent Gadget Lab</div>
          <h1 style=""font-size:22px;color:#e8ffe8;margin:16px 0 12px;"">Confirm your email</h1>
          <p style=""font-size:14px;color:#9ab09a;line-height:1.6;margin:0 0 28px;"">
            Sorry — we require email confirmation before access. Click below to activate your account.
          </p>
          <a href=""{confirmLink}""
             style=""display:inline-block;padding:14px 28px;background:#7CFF7C;color:#0a0f0a;font-weight:700;text-decoration:none;border-radius:8px;"">
            Confirm email
          </a>
          <p style=""font-size:12px;color:#556655;margin:28px 0 0;line-height:1.6;"">
            If you didn't create an account, ignore this message.
          </p>
        </td></tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";
        }
    }
}
