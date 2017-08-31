namespace NUnit.Snapshot.Json
{
    public static class SnapshotConstraintExtensions
    {
        public static SnapshotConstraint AsJson(this SnapshotConstraint constraint)
        {
            return constraint.WithSerializer(JsonSerializer.Default);
        }
    }
}