using System.Threading.Tasks;
using Business.MessageBrokers.Models;
using Core.Utilities.Results;

namespace Business.MessageBrokers.Manager.GetClientCreationMessage
{
    public interface IGetClientCreationMessageService
    {
        Task<IResult> GetClientCreationMessageQuery(CreateClientMessageComamnd message);
    }
}