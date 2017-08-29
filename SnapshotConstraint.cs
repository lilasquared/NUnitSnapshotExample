using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace NUnitSnapshotExample
{
    public class SnapshotConstraint : Constraint
    {
        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private String SnapshotDirectory => Path.Combine(AssemblyDirectory, "__snapshots__");
        private readonly NUnitEqualityComparer _comparer = NUnitEqualityComparer.Default;
        private Tolerance _tolerance = Tolerance.Default;

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var actualToCompare = actual.ToString();
            var stackTrace = new StackTrace();
            var className = String.Empty;
            var methodName = String.Empty;
            foreach (var stackFrame in stackTrace.GetFrames() ?? new StackFrame[0])
            {
                var method = stackFrame.GetMethod();
                if (method?.DeclaringType?.GetCustomAttributes(typeof(TestFixtureAttribute), true).Any() ?? false)
                {
                    className = method.DeclaringType.FullName;
                    methodName = method.Name;
                    break;
                }
            }

            var key = $"{className}_{methodName}";

            var snapshot = GetOrSaveSnapshot(key, actualToCompare);

            return new EqualConstraintResult(new EqualConstraint(snapshot), actualToCompare, _comparer.AreEqual(snapshot, actualToCompare, ref _tolerance));
        }

        private String GetOrSaveSnapshot(String key, String snapshot)
        {
            var previousSnapshot = GetSnapshot(key);
            if (String.IsNullOrWhiteSpace(previousSnapshot))
            {
                SaveSnapshot(key, snapshot);
                return snapshot;
            }
            return previousSnapshot;
        }

        public void SaveSnapshot(String key, String snapshot)
        {
            if (!Directory.Exists(SnapshotDirectory))
            {
                Directory.CreateDirectory(SnapshotDirectory);
            }

            File.AppendAllText(Path.Combine(SnapshotDirectory, key), snapshot, Encoding.UTF8);
        }

        public String GetSnapshot(String key)
        {
            var snapshotFile = Path.Combine(SnapshotDirectory, key);
            return File.Exists(snapshotFile) ? File.ReadAllText(snapshotFile) : String.Empty;
        }
    }
}