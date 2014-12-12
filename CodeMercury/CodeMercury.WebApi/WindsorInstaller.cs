using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CodeMercury.Components;
using CodeMercury.WebApi.Controllers;
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

                Component.For<InvocationController>()
                    .DependsOn(Dependency.OnComponent<IInvoker, LocalInvoker>())
                    .DependsOn(Dependency.OnComponent<IInvocationObserver, HttpInvoker>())
                    .LifestyleTransient(),

                // Register components

                Component.For<LocalInvoker>()
                    .LifestyleSingleton(),
                Component.For<HttpInvoker>()
                    .DependsOn(Dependency.OnValue<HttpClient>(new HttpClient { BaseAddress = new Uri("http://localhost:9090/") }))
                    .LifestyleSingleton()
                
                );
        }
    }
}
