using System;
using System.Collections.Generic;

/// <summary>
/// 自作スタック（LIFO）。List&lt;T&gt;の末尾を天面(Top)として使う。
/// </summary>
public class Stk<T>
{
    readonly List<T> items = new List<T>();

    public Stk() { } // 作る必要わからんけど一応

    public void Push(T x) { items.Add(x); }

    public T Pop()
    {
        T value;
        if (!TryPop(out value))
        {
            throw new InvalidOperationException("Stk is empty.");
        }
        return value;
    }

    public bool TryPop(out T value)
    {
        if (IsEmpty())
        {
            value = default(T);
            return false;
        }

        value = items[items.Count - 1];
        items.RemoveAt(items.Count - 1); // itemsの末尾(上)を削除する(0-based)
        return true;
    }

    public T Top()
    {
        T value;
        if (!TryTop(out value))
        {
            throw new InvalidOperationException("Stk is empty.");
        }
        return value;
    }

    public bool TryTop(out T value)
    {
        if (IsEmpty())
        {
            value = default(T);
            return false;
        }

        value = items[items.Count - 1]; // 末尾(上)を確認
        return true;
    }

    public T Bottom()
    {
        T value;
        if (!TryBottom(out value))
        {
            throw new InvalidOperationException("Stk is empty.");
        }
        return value;
    }

    public bool TryBottom(out T value)
    {
        if (IsEmpty())
        {
            value = default(T);
            return false;
        }

        value = items[0]; // 先頭が底
        return true;
    }

    // index 0 が底、Size()-1 が天面
    public T this[int index] { get { return items[index]; } }

    public int Size() { return items.Count; }

    public bool IsEmpty() { return Size() == 0; }

    public void Clear() { items.Clear(); }
}
