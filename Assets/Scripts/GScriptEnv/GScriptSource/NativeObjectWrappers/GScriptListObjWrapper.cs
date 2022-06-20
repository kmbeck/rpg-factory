using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class GScriptListObjWrapper<T> : IEnumerable
{   
    public List<T> inst;

    public GScriptListObjWrapper() { 
        inst = new List<T>();
    }

    [GScript]
    public void add(T item) {
        inst.Add(item);
    }

    public void Add(T item) {
        add(item);
    }

    [GScript]
    public T remove(int idx) {
        T temp = inst[idx];
        inst.RemoveAt(idx);
        return temp;
    }

    [GScript]
    public bool contains(T item) {
        return inst.Contains(item);
    }

    [GScript]
    public int len() {
        return inst.Count;
    }

    [GScript]
    public void clear() {
        inst.Clear();
    }

    // Define the indexer to allow client code to use [] notation.
    public T this[int i]
    {
        get { return inst[i]; }
        set { inst[i] = value; }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
       return (IEnumerator) GetEnumerator();
    }

    public GScriptListEnumerator<T> GetEnumerator()
    {
        return new GScriptListEnumerator<T>(inst);
    }
}

// When you implement IEnumerable, you must also implement IEnumerator.
public class GScriptListEnumerator<T> : IEnumerator<T>, IDisposable
{
    public List<T> items;

    // Enumerators are positioned before the first element
    // until the first MoveNext() call.
    int position = -1;

    public GScriptListEnumerator(List<T> _items)
    {
        items = _items;
    }

    public bool MoveNext()
    {
        position++;
        return (position < items.Count);
    }

    public void Reset()
    {
        position = -1;
    }

    public void Dispose()
    {
        Dispose();
        GC.SuppressFinalize(this);
    }

    object IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }

    public T Current
    {
        get
        {
            try
            {
                return items[position];
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException();
            }
        }
    }
}