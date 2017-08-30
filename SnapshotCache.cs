using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NUnitSnapshotExample
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
        private readonly String _snapshotPath;

        public SnapshotCache()
        {
            _snapshotPath = Path.Combine(Environment.CurrentDirectory, SnapshotRoot);
        }

        public String GetOrSaveSnapshot(String snapshot)
        {
            var key = GetSnapshotKey();
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("Snapshots can only be created from classes decorated with the [TestFixture] attribute");
            }
            var previousSnapshot = GetSnapshot(key);
            if (String.IsNullOrWhiteSpace(previousSnapshot))
            {
                SaveSnapshot(key, snapshot);
                return snapshot;
            }
            return previousSnapshot;
        }

        private static readonly IDictionary<String, Int32> NextIterationLookup = new Dictionary<String, Int32>();

        private static String GetSnapshotKey()
        {
            var stackTrace = new StackTrace();
            foreach (var stackFrame in stackTrace.GetFrames() ?? new StackFrame[0])
            {
                var method = stackFrame.GetMethod();
                if (method?.DeclaringType?.GetCustomAttributes(typeof(TestFixtureAttribute), true).Any() ?? false)
                {
                    var partialKey = $"{method.DeclaringType.FullName}_{method.Name}";
                    var key = String.Empty;

                    if (NextIterationLookup.ContainsKey(partialKey))
                    {
                        return $"{partialKey}_{++NextIterationLookup[partialKey]}";
                    }

                    NextIterationLookup.Add(key, 1);

                    return partialKey;
                }
            }

            return null;
        }

        private String GetSnapshot(String key)
        {
            var snapshotPath = GetSnapshotPath(key);
            return File.Exists(snapshotPath)
                ? File.ReadAllText(snapshotPath)
                : null;
        }

        public void SaveSnapshot(String key, String snapshot)
        {
            if (!Directory.Exists(_snapshotPath))
            {
                Directory.CreateDirectory(_snapshotPath);
            }

            File.AppendAllText(GetSnapshotPath(key), snapshot, Encoding.UTF8);
        }

        private String GetSnapshotPath(String key)
        {
            return Path.Combine(_snapshotPath, $"{key}.{SnapshotExtension}");
        }
    }
}