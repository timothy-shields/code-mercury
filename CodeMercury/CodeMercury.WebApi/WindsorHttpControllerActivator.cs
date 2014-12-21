using Castle.Windsor;
using System;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace CodeMercury.WebApi
{
    /// <summary>
    /// The Castle Windsor composition root.
    /// See: http://blog.ploeh.dk/2012/10/03/DependencyInjectioninASP.NETWebAPIwithCastleWindsor/
    /// </summary>
    public class WindsorHttpControllerActivator : IHttpControllerActivator
    {
        private readonly IWindsorContainer container;

        public WindsorHttpControllerActivator(IWindsorContainer container)
        {
            this.container = container;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = (IHttpController)container.Resolve(controllerType);
            request.RegisterForDispose(Disposable.Create(() => container.Release(controller)));
            return controller;
        }
    }
}
