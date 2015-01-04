using CodeMercury.Components;
using CodeMercury.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeMercury.Tests
{
    [TestClass]
    public class ArgumentMaterializerTests
    {
        Lazy<ArgumentMaterializer> argumentMaterializer;

        ArgumentMaterializer ArgumentMaterializer
        {
            get { return argumentMaterializer.Value; }
        }

        [TestInitialize]
        public void Initialize()
        {
            argumentMaterializer = new Lazy<ArgumentMaterializer>(() => new ArgumentMaterializer());
        }

        [TestMethod]
        public void MaterializesVoid()
        {
            ArgumentMaterializer.Materialize(typeof(void), Argument.Void);
        }

        [TestMethod]
        public void MaterializesValue()
        {
            var value = (int)ArgumentMaterializer.Materialize(typeof(int), Argument.Value(7));
            Assert.AreEqual(7, value);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException))]
        public void MaterializesCancellation()
        {
            ArgumentMaterializer.Materialize(typeof(int), Argument.Canceled);
        }

        [TestMethod]
        [ExpectedException(typeof(CapturedException<IndexOutOfRangeException>))]
        public void MaterializesCapturedException()
        {
            ArgumentMaterializer.Materialize(typeof(int), Argument.Exception(new IndexOutOfRangeException()));
        }

        [TestMethod]
        public async Task MaterializesCompletedTask()
        {
            var task = (Task)ArgumentMaterializer.Materialize(typeof(Task), Argument.Task(Argument.Void));
            await task;
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException), AllowDerivedTypes = true)]
        public async Task MaterializesCanceledTask()
        {
            var task = (Task)ArgumentMaterializer.Materialize(typeof(Task), Argument.Task(Argument.Canceled));
            await task;
        }

        [TestMethod]
        [ExpectedException(typeof(CapturedException<IndexOutOfRangeException>))]
        public async Task MaterializesFaultedTask()
        {
            var task = (Task)ArgumentMaterializer.Materialize(typeof(Task), Argument.Task(Argument.Exception(new IndexOutOfRangeException())));
            await task;
        }

        [TestMethod]
        public async Task MaterializesCompletedGenericTask()
        {
            var task = (Task<int>)ArgumentMaterializer.Materialize(typeof(Task<int>), Argument.Task(Argument.Value(7)));
            var value = await task;
            Assert.AreEqual(7, value);
        }

        [TestMethod]
        [ExpectedException(typeof(OperationCanceledException), AllowDerivedTypes = true)]
        public async Task MaterializesCanceledGenericTask()
        {
            var task = (Task<int>)ArgumentMaterializer.Materialize(typeof(Task<int>), Argument.Task(Argument.Canceled));
            await task;
        }

        [TestMethod]
        [ExpectedException(typeof(CapturedException<IndexOutOfRangeException>))]
        public async Task MaterializesFaultedGenericTask()
        {
            var task = (Task<int>)ArgumentMaterializer.Materialize(typeof(Task<int>), Argument.Task(Argument.Exception(new IndexOutOfRangeException())));
            await task;
        }
    }
}
