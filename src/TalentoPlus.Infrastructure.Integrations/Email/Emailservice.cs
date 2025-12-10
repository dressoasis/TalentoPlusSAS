using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TalentoPlus.Infrastructure.Integrations.Email;

public interface IEmailService
{
    Task<bool> SendWelcomeEmailAsync(string toEmail, string employeeName);
    Task<bool> SendEmailAsync(string toEmail, string subject, string body);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendWelcomeEmailAsync(string toEmail, string employeeName)
    {
        var subject = "Bienvenido a TalentoPlus S.A.S.";
        
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #2563eb;'>¡Bienvenido a TalentoPlus!</h2>
                    
                    <p>Hola <strong>{employeeName}</strong>,</p>
                    
                    <p>Tu registro en TalentoPlus S.A.S. ha sido completado exitosamente.</p>
                    
                    <p>Tu cuenta está siendo revisada por nuestro equipo de Recursos Humanos. 
                    Una vez aprobada, podrás acceder a la plataforma con tus credenciales.</p>
                    
                    <div style='background-color: #f3f4f6; padding: 15px; border-radius: 5px; margin: 20px 0;'>
                        <p style='margin: 0;'><strong>Próximos pasos:</strong></p>
                        <ul>
                            <li>Espera la aprobación de tu cuenta</li>
                            <li>Recibirás un correo de confirmación</li>
                            <li>Podrás acceder con tu email y contraseña</li>
                        </ul>
                    </div>
                    
                    <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
                    
                    <p style='margin-top: 30px;'>
                        Saludos cordiales,<br>
                        <strong>Equipo de TalentoPlus S.A.S.</strong>
                    </p>
                    
                    <hr style='margin-top: 30px; border: none; border-top: 1px solid #e5e7eb;'>
                    <p style='font-size: 12px; color: #6b7280; text-align: center;'>
                        Este es un correo automático, por favor no responder.
                    </p>
                </div>
            </body>
            </html>
        ";

        return await SendEmailAsync(toEmail, subject, body);
    }

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var smtpHost = _configuration["SMTP:Host"];
            var smtpPort = int.Parse(_configuration["SMTP:Port"] ?? "587");
            var smtpUser = _configuration["SMTP:User"];
            var smtpPass = _configuration["SMTP:Password"];
            var smtpFrom = _configuration["SMTP:From"] ?? smtpUser;

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
            {
                _logger.LogError("Configuración SMTP incompleta");
                return false;
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpFrom, "TalentoPlus S.A.S."),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
            
            _logger.LogInformation("Correo enviado exitosamente a {Email}", toEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar correo a {Email}", toEmail);
            return false;
        }
    }
}