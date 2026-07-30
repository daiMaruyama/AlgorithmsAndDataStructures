using System;


/// <summary>
/// 自作キュー（FIFO）。循環配列を使い、満杯時は容量を2倍に拡張する。
/// </summary>
public partial class Que<T>
{
    T[] items;
    int head;  // 次に取り出す要素（Front）の物理index
    int tail;  // 次に追加する空き場所の物理index
    int count; // 現在入っている要素数

    /// <param name="capacity">実際に格納できる要素数。</param>
    public Que(int capacity = 4)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "capacity は 1 以上にしてください。");
        }

        items = new T[capacity];
        head = 0;
        tail = 0;
        count = 0;
    }

    public int Capacity { get { return items.Length; } }

    public void Enqueue(T x)
    {
        if (count == Capacity)
        {
            Grow();
        }

        items[tail] = x;
        tail = (tail + 1) % Capacity;
        count++;
    }

    public T Dequeue()
    {
        T value;
        if (!TryDequeue(out value))
        {
            throw new InvalidOperationException("Que is empty.");
        }
        return value;
    }

    public bool TryDequeue(out T value)
    {
        if (IsEmpty())
        {
            value = default(T);
            return false;
        }

        value = items[head];
        items[head] = default(T);
        head = (head + 1) % Capacity;
        count--;
        return true;
    }

    public T Front()
    {
        if (IsEmpty()) throw new InvalidOperationException("Que is empty.");
        return items[head];
    }

    public bool TryFront(out T value)
    {
        if (IsEmpty())
        {
            value = default(T);
            return false;
        }

        value = Front();
        return true;
    }

    public T Back()
    {
        if (IsEmpty()) throw new InvalidOperationException("Que is empty.");
        int backIndex = (tail - 1 + Capacity) % Capacity;
        return items[backIndex];
    }

    public bool TryBack(out T value)
    {
        if (IsEmpty())
        {
            value = default(T);
            return false;
        }

        value = Back();
        return true;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            int physicalIndex = (head + index) % Capacity;
            return items[physicalIndex];
        }
    }

    public int Size() { return count; }

    public bool IsEmpty() { return count == 0; }

    public void Clear()
    {
        Array.Clear(items, 0, items.Length);
        head = 0;
        tail = 0;
        count = 0;
    }

    void Grow()
    {
        int oldCapacity = Capacity;
        int newCapacity = checked(oldCapacity * 2);
        T[] expandedItems = new T[newCapacity];

        // 物理位置ではなく、Frontからの論理順で詰め直す。
        for (int i = 0; i < count; i++)
        {
            int oldPhysicalIndex = (head + i) % oldCapacity;
            expandedItems[i] = items[oldPhysicalIndex];
        }

        items = expandedItems;
        head = 0;
        tail = count;
    }
}
