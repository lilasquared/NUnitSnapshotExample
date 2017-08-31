using NUnit.Framework.Constraints;

namespace NUnit.Snapshot
{
    public class SnapshotConstraint : Constraint
    {
        private ISnapshotSerializer _snapshotSerializer;
        private ISnapshotCache _snapshotCache;

        public SnapshotConstraint() 
            : this (SnapshotSerializer.Default, SnapshotCache.Default) { }

        public SnapshotConstraint(ISnapshotSerializer serializer, ISnapshotCache cache)
        {
            WithSerializer(serializer);
            WithCache(cache);
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

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var newSnapshot = _snapshotSerializer.Serialize(actual);

            var snapshot = _snapshotCache.GetOrSaveSnapshot(newSnapshot);

            if (newSnapshot == snapshot)
            {
                return new ConstraintResult(this, actual, ConstraintStatus.Success);
            }

            return new ConstraintResult(this, actual, ConstraintStatus.Failure);
        }
    }
}