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
        private readonly Uri requesterUri;
        private readonly Uri serverUri;

        public WindsorInstaller(Uri requesterUri, Uri serverUri)
        {
            this.requesterUri = requesterUri;
            this.serverUri = serverUri;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                // Register controllers
                
                Component.For<InvocationController>()
                    .DependsOn(
                        Dependency.OnComponent<IInvoker, LocalInvoker>())
                    .LifestyleTransient(),

                // Register components

                Component.For<LocalInvoker>()
                    .LifestyleSingleton(),
                Component.For<HttpInvoker>()
                    .DependsOn(
                        Dependency.OnValue("requesterUri", requesterUri),
                        Dependency.OnValue("serverUri", serverUri))
                    .LifestyleSingleton(),
                Component.For<IProxyActivator>()
                    .ImplementedBy<ProxyActivator>()
                    .DependsOn(
                        Dependency.OnValue("activators", new Dictionary<Type, Func<IInvoker, object>>
                        {
                            { typeof(IGizmoCache), invoker => new ProxyGizmoCache(invoker) }
                        }))
                    .LifestyleSingleton(),
                Component.For<IProxyResolver>()
                    .ImplementedBy<ProxyResolver>()
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
