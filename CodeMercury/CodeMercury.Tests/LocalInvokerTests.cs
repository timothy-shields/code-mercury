using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CodeMercury.Components;
using Moq;
using CodeMercury.Services;
using CodeMercury.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeMercury.Tests
{
    [TestClass]
    public class LocalInvokerTests
    {
        Mock<IServiceResolver> serviceResolver;
        Mock<IProxyResolver> proxyResolver;
        Lazy<LocalInvoker> localInvoker;

        LocalInvoker LocalInvoker
        {
            get { return localInvoker.Value; }
        }

        [TestInitialize]
        public void Initialize()
        {
            serviceResolver = new Mock<IServiceResolver>(MockBehavior.Strict);
            proxyResolver = new Mock<IProxyResolver>(MockBehavior.Strict);
            localInvoker = new Lazy<LocalInvoker>(() => new LocalInvoker(serviceResolver.Object, proxyResolver.Object));
        }

        [TestMethod]
        public async Task StaticMethodInvocationRunsToCompletion()
        {
            var invocation = new Invocation(
                Argument.Static,
                new Method(typeof(Math), "Max", new List<Type> { typeof(int), typeof(int) }),
                new List<Argument> { Argument.Value(5), Argument.Value(7) });

            var argument = await LocalInvoker.InvokeAsync(invocation);

            Assert.AreEqual(7, argument.CastTo<ValueArgument>().Value.CastTo<int>());
        }

        [TestMethod]
        public async Task InstanceMethodInvocationResolvesServiceAndRunsToCompletion()
        {
            var serviceId = Guid.NewGuid();
            serviceResolver.Setup(x => x.Resolve(serviceId)).Returns(new List<int> { 3, 5, 7, 6 }).Verifiable();

            var invocation = new Invocation(
                Argument.Service(serviceId),
                new Method(typeof(List<int>), "IndexOf", new List<Type> { typeof(int) }),
                new List<Argument> { Argument.Value(7) });

            var argument = await LocalInvoker.InvokeAsync(invocation);

            serviceResolver.Verify(x => x.Resolve(serviceId), Times.Once());

            Assert.AreEqual(2, argument.CastTo<ValueArgument>().Value.CastTo<int>());
        }
    }
}
