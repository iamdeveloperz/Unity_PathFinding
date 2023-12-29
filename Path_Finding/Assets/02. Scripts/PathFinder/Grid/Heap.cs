
using System.Collections.Generic;

public class Heap<T> where T : IHeapItem<T>
{
    #region Member Variables

    private readonly List<T> _Items = new List<T>();

    #endregion



    #region Properties

    public int Count => _Items.Count;

    #endregion

    

    // Constructor
    public Heap(int maxHeapSize)
    {
        // 초기 크기를 설정하여 List의 재할당을 최소화
        _Items.Capacity = maxHeapSize;
    }
    
    

    #region Heap => Add, Remove

    public void Add(T item)
    {
        item.HeapIndex = Count;
        _Items.Add(item);
        SortUp(item);
    }

    public T RemoveFirst()
    {
        if (Count == 0)
        {
            throw new System.InvalidOperationException("Heap is empty.");
        }
        
        var firstItem = _Items[0];
        var lastItem = _Items[Count - 1];

        // 마지막 요소를 첫 번째 위치로 이동
        _Items[0] = lastItem;
        lastItem.HeapIndex = 0;

        // 리스트에서 마지막 요소 제거
        _Items.RemoveAt(Count - 1);

        // 리스트가 비어있지 않은 경우에만 SortDown 호출
        if (Count > 0)
        {
            SortDown(_Items[0]);
        }
        
        return firstItem;
    }

    #endregion



    #region Update Item

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    #endregion
    

    
    #region Sorting Heap
    
    private void SortUp(T item)
    {
        var parentIndex = ParentIndex(item.HeapIndex);
        while (item.HeapIndex > 0)
        {
            var parentItem = _Items[parentIndex];
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = ParentIndex(item.HeapIndex);
        }
    }

    private void SortDown(T item)
    {
        while (true)
        {
            var childIndexLeft = ChildIndexLeft(item.HeapIndex);
            var childIndexRight = ChildIndexRight(item.HeapIndex);
            var swapIndex = childIndexLeft;

            if (childIndexLeft < Count)
            {
                if (childIndexRight < Count && _Items[childIndexLeft].CompareTo(_Items[childIndexRight]) < 0)
                {
                    swapIndex = childIndexRight;
                }

                if (item.CompareTo(_Items[swapIndex]) < 0)
                {
                    Swap(item, _Items[swapIndex]);
                }
                else break;
            }
            else break;
        }
    }

    #endregion



    #region Sub Methods

    private void Swap(T itemA, T itemB)
    {
        _Items[itemA.HeapIndex] = itemB;
        _Items[itemB.HeapIndex] = itemA;
        
        (itemA.HeapIndex, itemB.HeapIndex) = (itemB.HeapIndex, itemA.HeapIndex);
    }
    
    public bool Contains(T item)
    {
        if (item.HeapIndex < 0 || item.HeapIndex >= _Items.Count) return false;

        return Equals(_Items[item.HeapIndex], item);
    }
    
    private static int ParentIndex(int childIndex) => (childIndex - 1) / 2;

    private static int ChildIndexLeft(int parentIndex) => 2 * parentIndex + 1;

    private static int ChildIndexRight(int parentIndex) => 2 * parentIndex + 2;

    #endregion
    
}
