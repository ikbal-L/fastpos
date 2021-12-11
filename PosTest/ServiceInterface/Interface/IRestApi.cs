namespace ServiceInterface.Interface
{
    public interface IRestApi
    {
        string BaseUrl { get; set; }
        string Prefix { get; set; }

        string Action(string endPoint, object arg = null, string subPath = "");
        string Resource(string resource, string endPoint, object arg = null, string subPath = "");
        string Resource<T>(string endPoint, object arg = null, string subPath = "", string resource = null);
    }
}