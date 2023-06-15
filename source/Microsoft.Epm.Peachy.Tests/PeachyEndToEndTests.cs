namespace Microsoft.Epm.Peachy.Tests
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public sealed class PeachyEndToEndTests //// TODO mark all test classes as sealed
    {
        [TestMethod]
        public async Task GetAuthorizationSystem()
        {
            var serverTask = Program.Main(new string[0]); //// TODO wait for the servertask to compelte
            var tests = new EpmEndToEndTests(new Uri("http://localhost:8080"));
            await tests.GetAuthorizationSystem();
        }

        [TestMethod]
        public async Task GetInvalidSegment()
        {
            var serverTask = Program.Main(new string[0]); //// TODO wait for the servertask to compelte
            var tests = new EpmEndToEndTests(new Uri("http://localhost:8080"));
            await tests.GetInvalidSegment();
        }
    }
}
