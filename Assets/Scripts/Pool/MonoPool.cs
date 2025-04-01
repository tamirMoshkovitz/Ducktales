using System;
using System.Collections.Generic;
using Pool;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pool
{
    public class MonoPool<T> : MonoSingleton<MonoPool<T>> where T : MonoBehaviour, IPoolable
    {
        [SerializeField] private int poolSize = 5;
        [SerializeField] private T prefab;
        [SerializeField] private Transform parent;
    
        private Stack<T> _available;
        private int _increasePoolSizeBy;

        private void Awake()
        {
            _available = new Stack<T>();
            AddItemsToPool(poolSize);
            _increasePoolSizeBy = poolSize;
        }
    
        public virtual T Get()
        {
            if (_available.Count == 0)
            {
                AddItemsToPool(_increasePoolSizeBy);
            }
            var obj = _available.Pop();
            obj.gameObject.SetActive(true);
            obj.Reset();
            return obj;
        }

        public void Return(T obj)
        {
            obj.transform.position = parent.position;
            obj.gameObject.SetActive(false);
            _available.Push(obj);
        }
    
        private void AddItemsToPool(int itemsToAdd)
        {
            for (int i = 0; i < itemsToAdd; i++)
            {
                var obj = Instantiate(prefab, parent);
                obj.gameObject.SetActive(false);
                _available.Push(obj);
            }
        }
    }
}