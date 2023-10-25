using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Portfolio.Entities
{
    public class EntityRegistry
    {
        private const int MaxComponentTypes = 255;

        private Entity[] _entities;
        private readonly IList[] _components = new IList[MaxComponentTypes];

        private int[] _entityIndexBuffer;
        private int _entityCounter = -1;

        private readonly Queue<int> _entityIndexGraveyard = new(32);
        private readonly Dictionary<Type, int> _componentTypeIndexes = new(MaxComponentTypes);
        private readonly int _threadId;

        public EntityRegistry(int capacity)
        {
            _threadId = Environment.CurrentManagedThreadId;
            _entities = new Entity[capacity];
            _entityIndexBuffer = new int[capacity];
        }

        public ReadOnlySpan<int> Query(Query query)
        {
            CheckCurrentThreadId();

            var matchQueryRequired = !query.RequiredMask.IsEmpty();
            var matchQueryOptional = !query.OptionalMask.IsEmpty();
            var matchQueryExcluded = !query.ExcludedMask.IsEmpty();
            var matchedEntitiesCount = 0;

            for (var entityIndex = 0; entityIndex <= _entityCounter; entityIndex++)
            {
                ref readonly var entity = ref _entities[entityIndex];

                if (entity.Mask.IsEmpty())
                {
                    continue;
                }

                if (matchQueryRequired && !entity.Mask.ContainsAll(query.RequiredMask) ||
                    matchQueryOptional && !entity.Mask.ContainsAny(query.OptionalMask) ||
                    matchQueryExcluded && query.ExcludedMask.ContainsAny(entity.Mask))
                {
                    continue;
                }

                _entityIndexBuffer[matchedEntitiesCount++] = entity.Index;
            }

            return new ReadOnlySpan<int>(_entityIndexBuffer, 0, matchedEntitiesCount);
        }

        public Entity Create()
        {
            CheckCurrentThreadId();

            var entityIndex = _entityIndexGraveyard.Count > 0
                ? _entityIndexGraveyard.Dequeue()
                : Interlocked.Increment(ref _entityCounter);

            if (!_entities.IndexInBounds(entityIndex))
            {
                Array.Resize(ref _entities, _entities.Length * 2);
                Array.Resize(ref _entityIndexBuffer, _entityIndexBuffer.Length * 2);
            }

            var entity = new Entity(entityIndex);
            _entities[entityIndex] = entity;

            return entity;
        }

        public void Destroy(Entity entity)
        {
            CheckCurrentThreadId();

            TriggerComponentsOnDestroy(entity);

            _entityIndexGraveyard.Enqueue(entity.Index);
            _entities[entity.Index] = new Entity();
        }

        public bool HasComponent<T>(Entity entity)
        {
            CheckCurrentThreadId();

            return _entities[entity.Index].Mask.IsSet(GetComponentTypeIndex<T>());
        }

        public ref T GetComponent<T>(Entity entity)
        {
            CheckCurrentThreadId();

            return ref GetComponents<T>()[entity.Index];
        }

        public T[] GetComponents<T>()
        {
            CheckCurrentThreadId();

            var componentTypeIndex = GetComponentTypeIndex<T>();

            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            _components[componentTypeIndex] ??= new T[_entities.Length];

            return (T[]) _components[componentTypeIndex];
        }

        public void AddComponent<T>(ref Entity entity, T component) where T : struct
        {
            CheckCurrentThreadId();

            var componentTypeIndex = GetComponentTypeIndex<T>();

            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            _components[componentTypeIndex] ??= new T[_entities.Length];

            var components = (T[]) _components[componentTypeIndex];

            if (!components.IndexInBounds(entity.Index))
            {
                Array.Resize(ref components, Math.Max(entity.Index, 1) * 2);
            }

            components[entity.Index] = component;
            _components[componentTypeIndex] = components;

            entity = entity.WithMask(entity.Mask.Set(componentTypeIndex));
            _entities[entity.Index] = entity;
        }

        public void RemoveComponent<T>(ref Entity entity)
        {
            CheckCurrentThreadId();

            var componentTypeIndex = GetComponentTypeIndex<T>();

            entity = entity.WithMask(entity.Mask.Unset(componentTypeIndex));
            _entities[entity.Index] = entity;
        }

        public int GetComponentTypeIndex<T>()
        {
            CheckCurrentThreadId();

            return GetComponentTypeIndex(typeof(T));
        }

        public int GetComponentTypeIndex(Type type)
        {
            CheckCurrentThreadId();

            if (_componentTypeIndexes.Count > MaxComponentTypes)
            {
                throw new Exception("Reached maximum number of components");
            }

            _componentTypeIndexes[type] = _componentTypeIndexes.Count;
            return _componentTypeIndexes[type];
        }

        private void TriggerComponentsOnDestroy(Entity entity)
        {
            foreach (var componentTypeIndex in _componentTypeIndexes.Values)
            {
                if (!_components[componentTypeIndex].IndexInBounds(entity.Index))
                {
                    continue;
                }

                if (_components[componentTypeIndex][entity.Index] is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private void CheckCurrentThreadId()
        {
            Debug.Assert(_threadId == Environment.CurrentManagedThreadId);
        }
    }
}
