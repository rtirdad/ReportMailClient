using MailService.Models;
using System.Threading.Tasks;

namespace MailService.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailrequest);

    }
}
