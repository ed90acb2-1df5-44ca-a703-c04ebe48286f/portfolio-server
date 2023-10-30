using Portfolio.Entities.Collections;

namespace Portfolio.Entities
{
    public struct QueryBuilder
    {
        private readonly World _registry;

        private Bitmask255 _requiredMask;
        private Bitmask255 _excludedMask;

        public QueryBuilder(World registry)
        {
            _registry = registry;
            _requiredMask = new Bitmask255();
            _excludedMask = new Bitmask255();
        }

        public QueryBuilder Require<T>()
        {
            _requiredMask = _requiredMask.Set(_registry.GetComponentTypeIndex<T>());
            return this;
        }

        public QueryBuilder Exclude<T>()
        {
            _excludedMask = _excludedMask.Set(_registry.GetComponentTypeIndex<T>());
            return this;
        }

        public Query Build()
        {
            return new Query(_requiredMask, _excludedMask);
        }
    }
}
