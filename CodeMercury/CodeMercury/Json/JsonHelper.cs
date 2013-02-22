using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace CodeMercury.Json
{
    public static class JsonHelper
    {
        public static JToken ToJson(object _object)
        {
            if (_object == null)
                return new JValue((object)null);
            return JToken.FromObject(_object);
        }

        public static object ToObject(JToken token, Type type)
        {
            if (token == null)
                return null;
            return token.ToObject(type);
        }

        public static T ToObject<T>(JToken token)
        {
            if (token == null)
                return default(T);
            return token.ToObject<T>();
        }
    }
}
