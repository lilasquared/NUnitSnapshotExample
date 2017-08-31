using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NUnit.Snapshot
{
    public interface ISnapshotCache
    {
        String GetOrSaveSnapshot(String snapshot);
    }

    public class SnapshotCache : ISnapshotCache
    {
        public static ISnapshotCache Default = new SnapshotCache();

        private const String SnapshotRoot = ".snapshots";
        private const String SnapshotExtension = "snap";

        public String GetOrSaveSnapshot(String snapshot)
        {
            var info = GetSnapshotInfo();

            var previousSnapshot = GetSnapshot(info);
            if (String.IsNullOrWhiteSpace(previousSnapshot))
            {
                SaveSnapshot(info, snapshot);
                return snapshot;
            }
            return previousSnapshot;
        }

        private static readonly IDictionary<String, Int32> SnapshotNumberLookup = new Dictionary<String, Int32>();

        private static SnapshotInfo GetSnapshotInfo()
        {
            var stackTrace = new StackTrace(true);
            foreach (var stackFrame in stackTrace.GetFrames() ?? new StackFrame[0])
            {
                var method = stackFrame.GetMethod();
                if (method?.DeclaringType?.GetCustomAttributes(typeof(TestFixtureAttribute), true).Any() ?? false)
                {
                    var info = new SnapshotInfo
                    {
                        MethodName = method.Name,
                        ClassName = method.DeclaringType.FullName,
                        FolderLocation = Path.GetDirectoryName(stackFrame.GetFileName())
                    };

                    if (!SnapshotNumberLookup.ContainsKey(info.SnapshotKey))
                    {
                        SnapshotNumberLookup.Add(info.SnapshotKey, 0);
                    }

                    return info.SetNumber(SnapshotNumberLookup[info.SnapshotKey]++);
                }
            }

            throw new InvalidOperationException("Snapshots can only be created from classes decorated with the [TestFixture] attribute");
        }

        private static String GetSnapshot(SnapshotInfo info)
        {
            return SnapshotExists(info)
                ? File.ReadAllText(info.SnapshotFullPath)
                : null;
        }

        private static Boolean SnapshotExists(SnapshotInfo info)
        {
            return File.Exists(info.SnapshotFullPath);
        }

        private static void SaveSnapshot(SnapshotInfo info, String snapshot)
        {
            if (!Directory.Exists(info.SnapshotDirectory))
            {
                Directory.CreateDirectory(info.SnapshotDirectory);
            }

            File.AppendAllText(info.SnapshotFullPath, snapshot, Encoding.UTF8);
        }

        private class SnapshotInfo
        {
            private Int32 _snapshotNumber = 0;

            public String ClassName { get; set; }
            public String MethodName { get; set; }
            public String FolderLocation { get; set; }

            public String SnapshotKey => $"{ClassName}_{MethodName}";
            public String SnapshotFileName => $"{SnapshotKey}_{_snapshotNumber}.{SnapshotExtension}";
            public String SnapshotDirectory => Path.Combine(FolderLocation, SnapshotRoot);
            public String SnapshotFullPath => Path.Combine(SnapshotDirectory, SnapshotFileName);

            public SnapshotInfo SetNumber(Int32 number)
            {
                _snapshotNumber = number;
                return this;
            }
        }
    }
}