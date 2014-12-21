using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    internal class ArgumentJsonConverter : JsonCreationConverter<Argument>
    {
        protected override Argument Create(Type objectType, JObject jObject)
        {
            ArgumentKind kind;
            try
            {
                kind = jObject["kind"].ToObject<ArgumentKind>();
            }
            catch (Exception e)
            {
                throw new JsonException(@"An Argument must have a ""kind"" property with an ArgumentKind value.", e);
            }
            switch (kind)
            {
                case ArgumentKind.Proxy:
                    return new ProxyArgument();
                case ArgumentKind.Service:
                    return new ServiceArgument();
                case ArgumentKind.Static:
                    return new StaticArgument();
                case ArgumentKind.Task:
                    return new TaskArgument();
                case ArgumentKind.Value:
                    return new ValueArgument();
                case ArgumentKind.Void:
                    return new VoidArgument();
                default:
                    throw new JsonException("Unhandled ArgumentKind: " + kind);
            }
        }
    }
}
