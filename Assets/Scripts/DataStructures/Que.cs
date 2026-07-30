using System;


/// <summary>
/// 自作キュー（FIFO）。固定長配列をリングバッファとして使う。
/// </summary>
public class Que<T>
{
    readonly T[] items;
    int head; // queueの先頭要素のindex
    int tail; // queueの末尾要素のindex + 1
              // items[head, tail) にQueueを格納する（tail < head のときは配列末尾で折り返す）

    readonly int N;

    /// <param name="n">内部配列のサイズ。空と満杯を区別するため1スロット常に空けるので、実容量は n - 1。</param>
    public Que(int n = 10000)
    {
        if (n < 2) throw new ArgumentOutOfRangeException(nameof(n), "n は 2 以上にすること。");
        N = n;
        items = new T[N];
        head = 0;
        tail = 0;
    }

    /// <summary>実際に入れられる要素数。内部配列サイズ - 1。</summary>
    public int Capacity { get { return N - 1; } }

    /// <summary>満杯のときは何もせず捨てる（Cpp版と同じ挙動）。落としたくないなら TryEnqueue を使う。</summary>
    public void Enqueue(T x) { TryEnqueue(x); }

    public bool TryEnqueue(T x)
    {
        if (IsFull()) return false;
        items[tail] = x;
        tail++;
        tail %= N;
        return true;
    }

    public void Dequeue()
    {
        if (IsEmpty()) return;
        items[head] = default(T); // Cpp版のNUL埋めと同じ。参照型を残さずGCに回す意味もある
        head++;
        head %= N;
    }

    public T Front()
    {
        if (IsEmpty()) throw new InvalidOperationException("Que is empty.");
        return items[head];
    }

    public int Size() { return (tail - head + N) % N; }

    public bool IsEmpty() { return tail == head; }

    public bool IsFull() { return ((head - 1) + N) % N == tail; }

    public void Clear()
    {
        Array.Clear(items, 0, N);
        head = 0;
        tail = 0;
    }

    /// <summary>先頭から末尾の順に並べ直して返す。折り返しを気にせず中身を見たいとき用。</summary>
    public T[] ToArray()
    {
        var result = new T[Size()];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = items[(head + i) % N];
        }
        return result;
    }
}

