using Newtonsoft.Json;

namespace Pro.Common
{
    public static class JsonManager
    {
        public static T StringJson2Object<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
