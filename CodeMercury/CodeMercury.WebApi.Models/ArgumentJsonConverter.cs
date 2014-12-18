using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi.Models
{
    public class ArgumentJsonConverter : JsonCreationConverter<Argument>
    {
        protected override Argument Create(Type objectType, JObject obj)
        {
            var kind = obj["kind"].ToObject<ArgumentKind>();
            if (kind == ArgumentKind.Proxy)
            {
                return new ProxyArgument();
            }
            if (kind == ArgumentKind.Service)
            {
                return new ServiceArgument();
            }
            if (kind == ArgumentKind.Static)
            {
                return new StaticArgument();
            }
            if (kind == ArgumentKind.Value)
            {
                return new ValueArgument();
            }
            if (kind == ArgumentKind.Void)
            {
                return new VoidArgument();
            }
            throw new Exception();
        }
    }
}
