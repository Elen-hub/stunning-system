using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPool<T> where T : class
{
    public delegate void del_Register(T item);
    Queue<T> _pool;
    public MemoryPool(int capacity)
    {
        _pool = new Queue<T>(capacity);
    }
    public void Register(T item)
    {
        _pool.Enqueue(item);
    }
    public T GetItem()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();

        return null;
    }
    public void Clear()
    {
        _pool.Clear();
    }
}
public class ObjectMemoryPool<T> where T : Object
{
    public delegate void del_Register(T item);
    T _resourceCache;
    Queue<T> _pool;
    public ObjectMemoryPool(int capacity)
    {
        _pool = new Queue<T>(capacity);
    }
    public ObjectMemoryPool(int capacity, T cacheResource)
    {
        _pool = new Queue<T>(capacity);
        _resourceCache = cacheResource;
    }
    public void Register(T item)
    {
        _pool.Enqueue(item);
    }
    public T GetItem()
    {
        if (_pool.Count > 0)
            return _pool.Dequeue();

        return null;
    }
    public T InstantiateItem() => Object.Instantiate(_resourceCache);
    public void Clear()
    {
        _resourceCache = null;
        while (_pool.Count > 0)
            Object.DestroyImmediate(_pool.Dequeue());
    }
}