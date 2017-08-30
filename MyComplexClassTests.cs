using NUnit.Framework;

namespace NUnitSnapshotExample
{
    [TestFixture]
    public class MyComplexClassTests
    {
        [Test]
        public void Test()
        {
            var myComplexClass = new MyComplexClass();

            Assert.That(myComplexClass.Field1, Is.EqualTo("something"));
            Assert.That(myComplexClass.Field2, Is.EqualTo("somethingElse"));
            Assert.That(myComplexClass.SubField1, Is.EqualTo("another thing"));
        }

        [Test]
        public void Test_Snapshot()
        {
            var myComplexClass = new MyComplexClass();

            Assert.That(myComplexClass, Matches.Snapshot.Json());
        }

        [Test]
        public void Test_Snapshot_Json()
        {
            var myComplexClass = new MyComplexClass();

            // Assert.That(myComplexClass, Matches.SnapshotJson());
        }
    }
}