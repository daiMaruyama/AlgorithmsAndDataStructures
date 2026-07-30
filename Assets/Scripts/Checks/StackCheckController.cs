using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StackCheckController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] InputField valueInput;
    [SerializeField] Text resultText;
    [SerializeField] Text sizeText;
    [SerializeField] GameObject[] boxRoots;
    [SerializeField] Text[] boxValues;
    [SerializeField] Text[] boxMarkers;
    [SerializeField] Outline[] boxOutlines;

    [Header("Animation")]
    [SerializeField, Min(0f)] float duration = 0.3f;
    [SerializeField, Min(0f)] float distance = 72f;
    [SerializeField] Ease pushEase = Ease.OutCubic;
    [SerializeField] Ease popEase = Ease.InCubic;

    readonly Stk<int> stack = new Stk<int>();
    Tween animation;

    void Start()
    {
        Refresh();
        ShowResult(string.Empty);
    }

    public void Push()
    {
        if (IsAnimating() || !TryReadInput(out int value)) return;

        stack.Push(value);
        ShowResult($"Push {value}");

        // 追加した箱を表示してから、上から所定位置へ落とす。
        Refresh();
        PlayEnter(stack.Size() - 1);
    }

    public void TryPop()
    {
        if (IsAnimating()) return;

        bool success = stack.TryPop(out int value);
        ShowResult(success ? $"Pop {value}" : "Empty");

        if (success) PlayExit(stack.Size());
        else Refresh();
    }

    public void Clear()
    {
        if (IsAnimating()) return;

        stack.Clear();
        ShowResult("Clear");
        Refresh();
    }

    void PlayEnter(int index)
    {
        RectTransform box = GetBox(index);
        if (box == null || duration <= 0f) return;

        animation = BoxTween.Enter(box, Vector2.up * distance, duration, pushEase);
    }

    void PlayExit(int index)
    {
        RectTransform box = GetBox(index);
        if (box == null || duration <= 0f)
        {
            Refresh();
            return;
        }

        animation = BoxTween.Exit(box, Vector2.up * distance, duration, popEase)
            .OnComplete(Refresh);
    }

    RectTransform GetBox(int index)
    {
        if (index < 0 || index >= boxRoots.Length) return null;
        return boxRoots[index].transform as RectTransform;
    }

    bool TryReadInput(out int value)
    {
        if (int.TryParse(valueInput.text, out value))
        {
            valueInput.text = string.Empty;
            valueInput.ActivateInputField();
            return true;
        }

        ShowResult("整数を入力");
        valueInput.ActivateInputField();
        return false;
    }

    void Refresh()
    {
        for (int i = 0; i < boxRoots.Length; i++)
        {
            bool visible = i < stack.Size();
            boxRoots[i].SetActive(visible);
            if (!visible) continue;

            boxValues[i].text = stack[i].ToString();

            bool isBottom = i == 0;
            bool isTop = i == stack.Size() - 1;
            // ラベルは箱の右側に置き、矢印で対象を示す。
            if (isBottom && isTop) boxMarkers[i].text = "← TOP / BOTTOM";
            else if (isBottom) boxMarkers[i].text = "← BOTTOM";
            else if (isTop) boxMarkers[i].text = "← TOP";
            else boxMarkers[i].text = string.Empty;

            boxOutlines[i].effectColor = isTop
                ? new Color(0.76f, 0.20f, 0.16f)
                : isBottom
                    ? new Color(0.12f, 0.38f, 0.68f)
                    : new Color(0.18f, 0.18f, 0.18f);
            boxOutlines[i].effectDistance = isTop || isBottom
                ? new Vector2(3f, -3f)
                : new Vector2(2f, -2f);
        }

        string top = stack.TryTop(out int topValue) ? topValue.ToString() : "-";
        string bottom = stack.TryBottom(out int bottomValue) ? bottomValue.ToString() : "-";
        sizeText.text = $"Size {stack.Size()}    Bottom {bottom}    Top {top}";
    }

    bool IsAnimating() => animation != null && animation.IsActive();
    void ShowResult(string message) => resultText.text = message;
    void OnDisable() => animation?.Kill();
}
