using Store.Core.Entities;

namespace Store.Core.Services
{
    public interface IEmailSettings
    {
       public void SendEmail(Mail email);
    }
}
