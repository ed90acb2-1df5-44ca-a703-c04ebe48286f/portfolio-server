using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Portfolio.Entities.Collections;

namespace Portfolio.Entities
{
    public class World
    {
        private const int MaxComponentTypes = 255;

        private Entity[] _entities;
        private Entity[] _entityBuffer;
        private Bitmask255[] _entityMasks;
        private int _entityCounter;

        private readonly Queue<Entity> _graveyard = new(32);

        private readonly IList[] _components = new IList[MaxComponentTypes];
        private readonly Dictionary<Type, int> _componentTypeIndexes = new(MaxComponentTypes);

        public World(int capacity = 256)
        {
            _entities = new Entity[capacity];
            _entityBuffer = new Entity[capacity];
            _entityMasks = new Bitmask255[capacity];
        }

        public ReadOnlySpan<Entity> Query(Query query)
        {
            var matchQueryRequired = !query.RequiredMask.IsEmpty();
            var matchQueryExcluded = !query.ExcludedMask.IsEmpty();
            var matchedEntitiesCount = 0;

            for (var entityIndex = 0; entityIndex <= _entityCounter; entityIndex++)
            {
                var mask = _entityMasks[entityIndex];

                if (mask.IsEmpty())
                {
                    continue;
                }

                if (matchQueryRequired && !mask.ContainsAll(query.RequiredMask) ||
                    matchQueryExcluded && mask.ContainsAny(query.ExcludedMask))
                {
                    continue;
                }

                _entityBuffer[matchedEntitiesCount++] = _entities[entityIndex];
            }

            return new ReadOnlySpan<Entity>(_entityBuffer, 0, matchedEntitiesCount);
        }

        public Entity Create()
        {
            var entity = _graveyard.Count > 0
                ? _graveyard.Dequeue().WithNewVersion()
                : new Entity(_entityCounter++);

            if (!_entities.IndexInBounds(entity.Index))
            {
                var newSize = _entities.Length * 2;
                Array.Resize(ref _entities, newSize);
                Array.Resize(ref _entityMasks, newSize);
                Array.Resize(ref _entityBuffer, newSize);
            }

            _entities[entity.Index] = entity;

            return entity;
        }

        public void Destroy(Entity entity)
        {
            ValidateEntity(entity);

            TriggerComponentsOnDestroy(entity);

            _graveyard.Enqueue(entity);
            _entities[entity.Index] = Entity.Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidEntity(Entity entity)
        {
            return _entities[entity.Index] == entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasComponent<T>(Entity entity)
        {
            ValidateEntity(entity);

            return _entityMasks[entity.Index].IsSet(GetComponentTypeIndex<T>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetComponent<T>(Entity entity)
        {
            ValidateEntity(entity);

            return ref Components<T>()[entity.Index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] Components<T>()
        {
            var componentTypeIndex = GetComponentTypeIndex<T>();

            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            _components[componentTypeIndex] ??= new T[_entities.Length];

            return (T[]) _components[componentTypeIndex];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetComponent<T>(Entity entity, T component)
        {
            ValidateEntity(entity);

            var components = Components<T>();

            if (!components.IndexInBounds(entity.Index))
            {
                Array.Resize(ref components, _entities.Length);
            }

            components[entity.Index] = component;

            var componentTypeIndex = GetComponentTypeIndex<T>();
            _entityMasks[entity.Index] = _entityMasks[entity.Index].Set(componentTypeIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveComponent<T>(Entity entity)
        {
            ValidateEntity(entity);

            var componentTypeIndex = GetComponentTypeIndex<T>();
            _entityMasks[entity.Index] = _entityMasks[entity.Index].Unset(componentTypeIndex);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetComponentTypeIndex<T>()
        {
            return GetComponentTypeIndex(typeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetComponentTypeIndex(Type type)
        {
            if (_componentTypeIndexes.Count > MaxComponentTypes)
            {
                throw new Exception("Reached maximum number of components");
            }

            if (_componentTypeIndexes.TryGetValue(type, out var componentTypeIndex))
            {
                return componentTypeIndex;
            }

            componentTypeIndex = _componentTypeIndexes.Count;
            _componentTypeIndexes[type] = componentTypeIndex;
            return componentTypeIndex;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ValidateEntity(Entity entity)
        {
            if (IsValidEntity(entity))
            {
                return;
            }

            throw new Exception($"Entity version mismatch. Entity index: {entity.Index}");
        }
    }
}
