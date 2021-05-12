using Order.Application.Models;
using System.Threading.Tasks;

namespace Order.Application.Contracts.Infastructure
{
    public interface IEmailService
    {
        Task<bool> SendEmail(Email email);
    }
}
