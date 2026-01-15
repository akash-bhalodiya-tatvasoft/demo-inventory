using System.Net;
using System.Net.Mail;
using Inventory.Models.Entities;
using Inventory.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Inventory.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendOrderCreatedEmailAsync(
        string toEmail,
        string userName,
        Order order)
    {
        var subject = $"Order Confirmation - #{order.Id}";
        var body = $"""
                    <html>
                    <body>
                        <p>Hello {userName},</p>

                        <p>Your order has been placed successfully.</p>

                        <p>
                            Order ID: {order.Id}<br/>
                            Order Date: {order.CreatedAt:dd MMM yyyy}
                        </p>

                        <p><strong>Order Items:</strong></p>

                        {string.Join("", order.OrderItems.Select(item => $"""
                            <p>
                                Product: {item.Product.Name}<br/>
                                Quantity: {item.Quantity}<br/>
                                Unit Price: {item.UnitPrice:C}<br/>
                                Total: {item.TotalPrice:C}
                            </p>
                            <hr/>
                        """))}

                        <p>
                            <strong>Total Amount:</strong> {order.TotalAmount:C}
                        </p>

                        <p>
                            Thank you for shopping with us.
                        </p>

                        <p>
                            Regards,<br/>
                            Inventory App
                        </p>
                    </body>
                    </html>
                    """;

        await SendMailAsync(toEmail, subject, body);
    }

    public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        var subject = "OTP";
        var body = $"""
                    <html>
                    <body>
                        <p>OTP is:</p>
                        <h2>{otp}</h2>
                        <p>OTP expires after 5 minutes.</p>
                    </body>
                    </html>
                    """;

        await SendMailAsync(toEmail, subject, body);
    }

    private async Task SendMailAsync(
        string toEmail,
        string subject,
        string htmlBody)
    {
        var smtpHost = _configuration["SmtpSettings:Server"];
        var smtpPort = int.Parse(_configuration["SmtpSettings:Port"]!);
        var senderName = _configuration["SmtpSettings:SenderName"];
        var senderEmail = _configuration["SmtpSettings:SenderEmail"];
        var senderPassword = _configuration["SmtpSettings:Password"];

        var message = new MailMessage
        {
            From = new MailAddress(senderEmail!, senderName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        using var smtp = new SmtpClient(smtpHost!, smtpPort)
        {
            Credentials = new NetworkCredential(
                senderEmail,
                senderPassword),
            EnableSsl = true
        };

        await smtp.SendMailAsync(message);
    }
}
