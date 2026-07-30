using DG.Tweening;
using UnityEngine;

// 配置済みの箱を移動する。UIやGameObjectは生成しない。
public static class BoxTween
{
    // offset側から現在位置へ入る。
    public static Tween Enter(RectTransform box, Vector2 offset, float duration, Ease ease)
    {
        Vector2 position = box.anchoredPosition;
        box.anchoredPosition = position + offset;

        return box.DOAnchorPos(position, duration)
            .SetEase(ease)
            .OnKill(() => ResetPosition(box, position));
    }

    // 現在位置からoffset側へ出る。
    public static Tween Exit(RectTransform box, Vector2 offset, float duration, Ease ease)
    {
        Vector2 position = box.anchoredPosition;

        return box.DOAnchorPos(position + offset, duration)
            .SetEase(ease)
            .OnKill(() => ResetPosition(box, position));
    }

    // 再生停止後も次の操作を同じ位置から始められるように戻す。
    static void ResetPosition(RectTransform box, Vector2 position)
    {
        if (box != null) box.anchoredPosition = position;
    }
}
