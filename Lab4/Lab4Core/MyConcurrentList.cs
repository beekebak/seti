namespace Lab4Core;

using System;
using System.Collections;
using System.Collections.Generic;

public class MyConcurrentList<T> : IList<T>
{
    private readonly List<T> _internalList = new List<T>();
    private readonly object _lock = new object();

    public MyConcurrentList() {}
    
    public MyConcurrentList(IEnumerable<T> collection)
    {
        _internalList.AddRange(collection);
    }
    
    public T this[int index]
    {
        get
        {
            lock (_lock)
            {
                return _internalList[index];
            }
        }
        set
        {
            lock (_lock)
            {
                _internalList[index] = value;
            }
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _internalList.Count;
            }
        }
    }

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        lock (_lock)
        {
            _internalList.Add(item);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _internalList.Clear();
        }
    }

    public bool Contains(T item)
    {
        lock (_lock)
        {
            return _internalList.Contains(item);
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (_lock)
        {
            _internalList.CopyTo(array, arrayIndex);
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        lock (_lock)
        {
            // Создаем копию для безопасной итерации
            return new List<T>(_internalList).GetEnumerator();
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(T item)
    {
        lock (_lock)
        {
            return _internalList.IndexOf(item);
        }
    }

    public void Insert(int index, T item)
    {
        lock (_lock)
        {
            _internalList.Insert(index, item);
        }
    }

    public bool Remove(T item)
    {
        lock (_lock)
        {
            return _internalList.Remove(item);
        }
    }

    public void RemoveAt(int index)
    {
        lock (_lock)
        {
            _internalList.RemoveAt(index);
        }
    }
    
    public int RemoveAll(Predicate<T> match)
    {
        lock (_lock)
        {
            return _internalList.RemoveAll(match);
        }
    }

    public T? Find(Predicate<T> match)
    {
        lock (_lock)
        {
            return _internalList.Find(match);
        }
    }

    public MyConcurrentList<TResult> ConvertAll<TResult>(Func<T, TResult> converter)
    {
        if (converter == null)
            throw new ArgumentNullException(nameof(converter));
        var convertedItems = _internalList.Select(converter);
        return new MyConcurrentList<TResult>(convertedItems);
    }
}
