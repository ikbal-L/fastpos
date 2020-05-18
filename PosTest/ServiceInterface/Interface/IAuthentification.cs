using ServiceInterface.Model;

namespace ServiceInterface.Interface
{
    public interface IAuthentification
    {
        bool Authenticate(string user, string password, Annex annex, Terminal terminal);
    }
}
