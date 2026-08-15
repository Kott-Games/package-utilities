using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace Kott.Utilities
{
    /// <summary>
    /// Pools prefab instances under a specified container transform, reusing them via Unity's pooling API
    /// to avoid repeated instantiate/destroy cycles.
    /// </summary>
    /// <remarks>
    /// Lifecycle: use Unity callbacks (Awake, OnEnable, OnDisable, OnDestroy) for setup/teardown.
    /// Treat <see cref="Get()"/> like a fresh instantiation: always reinitialize/reset state.
    /// An object should not be aware of whether it came from a pool or was instantiated manually.
    /// </remarks>
    public class PrefabObjectPool<T> : IObjectPool<T>, IEnumerable<T>
        where T : Object
    {
        private readonly ObjectPool<T> _pool;
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly bool _orderHierarchy;
        private readonly List<T> _activeInstances;

        public int CountInactive => _pool.CountInactive;
        public int CountActive => _pool.CountActive;
        public IReadOnlyList<T> ActiveInstances => _activeInstances;

        /// <param name="prefab">Prefab to instantiate.</param>
        /// <param name="container">Parent transform for all pooled instances.</param>
        /// <param name="collectionCheck">Whether to enable Unity's collection checks.</param>
        /// <param name="defaultCapacity">Initial pool capacity.</param>
        /// <param name="maxSize">Maximum number of pooled instances.</param>
        /// <param name="orderHierarchy">If true, moves instances to the end of <paramref name="container"/> on <see cref="Get()"/>.</param>
        public PrefabObjectPool(T prefab, Transform container,
            bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000,
            bool orderHierarchy = true)
        {
            _pool = new(PoolCreate, PoolGet, PoolRelease, PoolDestroy, collectionCheck, defaultCapacity, maxSize);
            _activeInstances = new(defaultCapacity);
            _prefab = prefab;
            _container = container;
            _orderHierarchy = orderHierarchy;
        }

        public T Get()
        {
            return _pool.Get();
        }

        public PooledObject<T> Get(out T v)
        {
            v = Get();
            return new PooledObject<T>(v, this);
        }

        public void Release(T instance)
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
            _pool.Release(instance);
        }

        public void ReleaseActive()
        {
            for (int i = _activeInstances.Count - 1; i >= 0; i--)
            {
                _pool.Release(_activeInstances[i]);
            }
        }

        public void Clear()
        {
            _pool.Clear();
        }

        private T PoolCreate()
        {
            return Object.Instantiate(_prefab, _container);
        }

        private void PoolGet(T instance)
        {
            GameObject gameObject = GetGameObject(instance);
            gameObject?.SetActive(true);
            _activeInstances.Add(instance);
            
            if (!_orderHierarchy) return;
            gameObject?.transform.SetAsLastSibling();
        }

        private void PoolRelease(T instance)
        {
            GameObject gameObject = GetGameObject(instance);
            gameObject?.SetActive(false);
            _activeInstances.Remove(instance);
        }

        private void PoolDestroy(T instance)
        {
            GameObject gameObject = GetGameObject(instance);
            if (!gameObject) return;
            
            Object.Destroy(gameObject);
        }
        
        private GameObject GetGameObject(T instance)
        {
            if (instance is GameObject gameObject) return gameObject;
            return (instance as Component)?.gameObject;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _activeInstances.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}