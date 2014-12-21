using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CodeMercury.Components;
using CodeMercury.Services;
using CodeMercury.WebApi.Components;
using CodeMercury.WebApi.Controllers;
using Example;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.WebApi
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                // Register controllers

                Component.For<CompletionController>()
                    .DependsOn(
                        Dependency.OnComponent<IInvocationObserver, HttpInvoker>())
                    .LifestyleTransient(),

                Component.For<InvocationController>()
                    .DependsOn(Dependency.OnComponent<IInvoker, LocalInvoker>())
                    .LifestyleTransient(),

                // Register components

                Component.For<LocalInvoker>()
                    .LifestyleSingleton(),
                Component.For<HttpInvoker>()
                    .DependsOn(
                        Dependency.OnValue("requesterUri", new Uri("http://localhost:9090/")),
                        Dependency.OnValue("serverUri", new Uri("http://localhost:9090/")))
                    .LifestyleSingleton(),
                Component.For<IProxyActivator>()
                    .ImplementedBy<ProxyActivator>()
                    .DependsOn(
                        Dependency.OnValue("activators", new Dictionary<Type, Func<IInvoker, IProxy>>
                        {
                            { typeof(IGizmoCache), invoker => new ProxyGizmoCache(invoker) }
                        }))
                    .LifestyleSingleton(),
                Component.For<IProxyContainer, IProxyResolver>()
                    .ImplementedBy<ProxyContainer>()
                    .DependsOn(
                        Dependency.OnComponent<IInvoker, HttpInvoker>())
                    .LifestyleSingleton(),
                Component.For<IServiceContainer, IServiceResolver>()
                    .ImplementedBy<ServiceContainer>()
                    .LifestyleSingleton()

                );
        }
    }
}
