using System;

namespace NUnit.Snapshot
{
    public interface ISnapshotSerializer
    {
        String Serialize<T>(T source);
    }

    public class SnapshotSerializer : ISnapshotSerializer
    {
        public static ISnapshotSerializer Default = new SnapshotSerializer();
        public String Serialize<T>(T source)
        {
            return source.ToString();
        }
    }
}