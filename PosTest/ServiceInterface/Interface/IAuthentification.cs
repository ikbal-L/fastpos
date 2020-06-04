using ServiceInterface.Model;

namespace ServiceInterface.Interface
{
    public interface IAuthentification
    {
        int Authenticate(string user, string password, Annex annex, Terminal terminal);
    }
}
