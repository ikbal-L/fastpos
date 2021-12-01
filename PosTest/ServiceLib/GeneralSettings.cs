using Newtonsoft.Json;

namespace ServiceLib
{
    
    public class GeneralSettings 
    {
        [JsonProperty] public int TableNumber { get; set; }

        [JsonProperty]
        public int NumberCategores { get; set; }

       

        [JsonProperty]
        public string NumberProducts { get; set; }


        [JsonProperty]
        public string ServerHost { get; set; }

        
    }
}