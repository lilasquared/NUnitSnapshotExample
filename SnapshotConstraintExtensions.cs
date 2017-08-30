namespace NUnitSnapshotExample
{
    public static class SnapshotConstraintExtensions
    {
        public static SnapshotConstraint Json(this SnapshotConstraint constraint)
        {
            return constraint.WithSerializer(JsonSerializer.Default);
        }
    }
}