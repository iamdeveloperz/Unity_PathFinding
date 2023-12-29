
using System;

public interface IHeapItem<in T> : IComparable<T>
{
    public int HeapIndex { get; set; }
}