using Castle.Windsor;
using CodeMercury.Expressions;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;

namespace CodeMercury.WebApi
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var container = new WindsorContainer())
            {
                container.Install(new WindsorInstaller());

                var port = int.Parse(args[0]);
                var url = string.Format("http://localhost:{0}/", port);
                var activator = new WindsorCompositionRoot(container);
                var startup = new Startup(activator);
                using (WebApp.Start(url, startup.Configuration))
                {
                    Expression<Func<string>> expression = () => string.Join(" * ", 1, 2, 3, 4);

                    using (var client = new HttpClient())
                    {
                        var response = client.PostAsJsonAsync(url + "calls", CreateCall(expression)).Result;

                        var json = JToken.Parse(response.Content.ReadAsStringAsync().Result);

                        Console.WriteLine(response);
                        Console.WriteLine(expression.ToString() + " ---> " + json["return_value"].ToObject(json["return_type"].ToObject<Type>()));
                    }

                    Console.ReadLine();
                }
            }
        }

        private static Models.Call CreateCall(LambdaExpression expression)
        {
            return new Models.Call
            {
                Function = new Models.Function
                {
                    DeclaringType = expression.Body.As<MethodCallExpression>().Method.DeclaringType,
                    Name = expression.Body.As<MethodCallExpression>().Method.Name,
                    Parameters = expression.Body.As<MethodCallExpression>().Method.GetParameters().Select(parameter => new Models.Parameter
                    {
                        ParameterType = parameter.ParameterType,
                        Name = parameter.Name
                    }).ToList()
                },
                Arguments = expression
                    .Body
                    .As<MethodCallExpression>()
                    .Arguments
                    .Select(ExpressionHelper.Evaluate)
                    .Select(JToken.FromObject)
                    .ToList()
            };
        }
    }
}
