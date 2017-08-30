using NUnit.Framework.Constraints;

namespace NUnitSnapshotExample
{
    public class SnapshotConstraint : Constraint
    {
        private ISnapshotSerializer _snapshotSerializer;
        private ISnapshotCache _snapshotCache;

        public SnapshotConstraint() : this (SnapshotSerializer.Default, SnapshotCache.Default)
        {
            
        }

        public SnapshotConstraint(ISnapshotSerializer serializer, ISnapshotCache cache)
        {
            _snapshotSerializer = serializer;
            _snapshotCache = cache;
        }

        public SnapshotConstraint WithSerializer(ISnapshotSerializer serializer)
        {
            _snapshotSerializer = serializer;
            return this;
        }

        public SnapshotConstraint WithCache(ISnapshotCache cache)
        {
            _snapshotCache = cache;
            return this;
        }

        private readonly NUnitEqualityComparer _comparer = NUnitEqualityComparer.Default;
        private Tolerance _tolerance = Tolerance.Default;

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var newSnapshot = _snapshotSerializer.Serialize(actual);
            

            var snapshot = _snapshotCache.GetOrSaveSnapshot(newSnapshot);

            return new EqualConstraintResult(new EqualConstraint(snapshot), newSnapshot, _comparer.AreEqual(snapshot, newSnapshot, ref _tolerance));
        }
    }
}