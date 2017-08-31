using System;
using NUnit.Framework;
using NUnit.Snapshot.Json;

namespace NUnit.Snapshot.Tests
{
    [TestFixture]
    public class MyComplexClassTests
    {

        [Test]
        public void Test_Snapshot()
        {
            var test = new Exception();

            Assert.That(test, Matches.Snapshot);
            Assert.That(test, Matches.Snapshot.AsJson());
        }
    }
}