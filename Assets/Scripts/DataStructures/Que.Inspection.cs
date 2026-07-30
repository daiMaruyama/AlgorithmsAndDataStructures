using System;

// 確認シーンで「リング配列の実際の並び」を描くためだけの覗き窓。
// キューとしての動作には一切関係しないので Que.cs から切り離してある。
// このファイルを削除しても Que<T> はそのまま動く（確認シーンの物理ビューだけが消える）。
public partial class Que<T>
{
    internal int HeadIndex { get { return head; } }
    internal int TailIndex { get { return tail; } }

    /// <summary>物理index（配列の生の位置）で中身を読む。論理順ではない。</summary>
    internal T GetPhysicalSlot(int physicalIndex)
    {
        if (physicalIndex < 0 || physicalIndex >= Capacity)
        {
            throw new ArgumentOutOfRangeException(nameof(physicalIndex));
        }
        return items[physicalIndex];
    }

    /// <summary>その物理スロットが今使用中か。</summary>
    internal bool IsPhysicalSlotOccupied(int physicalIndex)
    {
        if (physicalIndex < 0 || physicalIndex >= Capacity)
        {
            throw new ArgumentOutOfRangeException(nameof(physicalIndex));
        }

        // 「先頭から数えて何番目のスロットか」を逆算し、count 未満なら使用中。
        int logicalOffset = (physicalIndex - head + Capacity) % Capacity;
        return logicalOffset < count;
    }
}
