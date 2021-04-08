using Newtonsoft.Json;

namespace ServiceInterface.ExtentionsMethod
{
   public static class ObjectExtentions
    {
        public static T Clone<T>(this T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
