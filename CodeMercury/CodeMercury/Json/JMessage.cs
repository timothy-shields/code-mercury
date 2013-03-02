using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography;

namespace CodeMercury.Json
{
    /// <summary>
    /// A self-describing JSON-serialized object.
    /// </summary>
    public class JMessage
    {
        public Type Type { get; set; }
        public JToken Value { get; set; }

        public static JMessage FromValue<T>(T value)
        {
            return new JMessage
            {
                Type = typeof(T),
                Value = JsonHelper.ToJson(value)
            };
        }

        public object ToObject()
        {
            return JsonHelper.ToObject(Value, Type);
        }

        /// <summary>
        /// If this message's value is of type T, then apply the action.
        /// </summary>
        public void When<T>(Action<T> action)
        {
            if (typeof(T).IsAssignableFrom(Type))
                action(JsonHelper.ToObject<T>(Value));
        }

        public static string Hash(JMessage message)
        {
            using (var sha1 = SHA1.Create())
                return Convert.ToBase64String(sha1.ComputeHash(Serialize(message)));
        }

        public static byte[] Serialize(JMessage message)
        {
            return Encoding.ASCII.GetBytes(JToken.FromObject(message).ToString(Formatting.None));
        }

        public static JMessage Deserialize(byte[] message)
        {
            return JToken.Parse(Encoding.ASCII.GetString(message)).ToObject<JMessage>();
        }
    }
}
