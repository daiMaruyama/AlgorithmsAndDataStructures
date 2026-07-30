using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class QueueCheckController : MonoBehaviour
{
    const int InitialCapacity = 5;

    [Header("UI")]
    [SerializeField] InputField valueInput;
    [SerializeField] Text resultText;
    [SerializeField] Text sizeText;
    [SerializeField] RectTransform dequeueLabel;
    [SerializeField] RectTransform enqueueLabel;

    [Header("Logical Order")]
    [SerializeField] GameObject[] logicalRoots;
    [SerializeField] Text[] logicalValues;
    [SerializeField] Text[] logicalMarkers;
    [SerializeField] Outline[] logicalOutlines;

    [Header("Physical Array")]
    [SerializeField] GameObject[] physicalRoots;
    [SerializeField] Text[] physicalIndexes;
    [SerializeField] Text[] physicalValues;
    [SerializeField] Text[] physicalMarkers;
    [SerializeField] Outline[] physicalOutlines;

    [Header("Animation")]
    [SerializeField, Min(0f)] float duration = 0.22f;
    [SerializeField, Min(0f)] float distance = 96f;
    [SerializeField] Ease enqueueEase = Ease.OutCubic;
    [SerializeField] Ease dequeueEase = Ease.InCubic;

    Que<int> queue;
    Tween animation;

    void Start()
    {
        queue = new Que<int>(InitialCapacity);
        Refresh();
        ShowResult(string.Empty);
    }

    public void Enqueue()
    {
        if (IsAnimating() || !TryReadInput(out int value)) return;

        queue.Enqueue(value);
        ShowResult($"Enqueue {value}");

        // 追加した箱を表示してから、右の入口から中へ動かす。
        Refresh();
        PlayEnter(queue.Size() - 1);
    }

    public void TryDequeue()
    {
        if (IsAnimating()) return;

        bool success = queue.TryDequeue(out int value);
        ShowResult(success ? $"Dequeue {value}" : "Empty");

        if (success) PlayExit();
        else Refresh();
    }

    public void Clear()
    {
        if (IsAnimating()) return;

        queue.Clear();
        ShowResult("Clear");
        Refresh();
    }

    void PlayEnter(int index)
    {
        RectTransform box = GetLogicalBox(index);
        if (box == null || duration <= 0f) return;

        animation = BoxTween.Enter(box, Vector2.right * distance, duration, enqueueEase);
    }

    void PlayExit()
    {
        RectTransform frontBox = GetLogicalBox(0);
        if (frontBox == null || duration <= 0f)
        {
            Refresh();
            return;
        }

        Sequence sequence = DOTween.Sequence();
        sequence.Join(BoxTween.Exit(frontBox, Vector2.left * distance, duration, dequeueEase));

        // Frontが出る間に、残りの箱を出口側へ一つ分ずつ詰める。
        float spacing = GetLogicalBox(1).anchoredPosition.x - frontBox.anchoredPosition.x;
        int previousCount = Mathf.Min(queue.Size() + 1, logicalRoots.Length);
        for (int i = 1; i < previousCount; i++)
            sequence.Join(BoxTween.Exit(GetLogicalBox(i), Vector2.left * spacing, duration, dequeueEase));

        animation = sequence.OnComplete(Refresh);
    }

    RectTransform GetLogicalBox(int index)
    {
        if (index < 0 || index >= logicalRoots.Length) return null;
        return logicalRoots[index].transform as RectTransform;
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
        RefreshLogicalOrder();
        RefreshPhysicalArray();

        string front = queue.TryFront(out int frontValue) ? frontValue.ToString() : "-";
        string back = queue.TryBack(out int backValue) ? backValue.ToString() : "-";
        sizeText.text = $"Size {queue.Size()} / {queue.Capacity}    Front {front}    Back {back}";
    }

    // 利用者から見える並び。0番が出口のFront。
    void RefreshLogicalOrder()
    {
        for (int i = 0; i < logicalRoots.Length; i++)
        {
            bool visible = i < queue.Size();
            logicalRoots[i].SetActive(visible);
            if (!visible) continue;

            logicalValues[i].text = queue[i].ToString();

            bool isFront = i == 0;
            bool isBack = i == queue.Size() - 1;

            // ラベルを箱幅に収め、隣の箱のラベルと重ならないようにする。
            if (isFront && isBack) SetMarker(logicalMarkers[i], "↑ FRONT\n↑ BACK", true);
            else if (isFront) SetMarker(logicalMarkers[i], "↑ FRONT");
            else if (isBack) SetMarker(logicalMarkers[i], "↑ BACK");
            else SetMarker(logicalMarkers[i], string.Empty);

            logicalOutlines[i].effectColor = isFront
                ? new Color(0.76f, 0.20f, 0.16f)
                : isBack
                    ? new Color(0.12f, 0.38f, 0.68f)
                    : new Color(0.18f, 0.18f, 0.18f);
            logicalOutlines[i].effectDistance = isFront || isBack
                ? new Vector2(3f, -3f)
                : new Vector2(1f, -1f);
        }

        // 操作ラベルをFrontとBackの位置へ合わせる。
        RectTransform frontBox = logicalRoots[0].transform as RectTransform;
        int backIndex = Mathf.Clamp(queue.Size() - 1, 0, logicalRoots.Length - 1);
        RectTransform backBox = logicalRoots[backIndex].transform as RectTransform;
        dequeueLabel.anchoredPosition = new Vector2(frontBox.anchoredPosition.x, dequeueLabel.anchoredPosition.y);
        enqueueLabel.anchoredPosition = new Vector2(backBox.anchoredPosition.x, enqueueLabel.anchoredPosition.y);
    }

    // 内部リング配列。headとtailがどのindexにいるかを示す。
    void RefreshPhysicalArray()
    {
        for (int i = 0; i < physicalRoots.Length; i++)
        {
            bool exists = i < queue.Capacity;
            physicalRoots[i].SetActive(exists);
            if (!exists) continue;

            physicalIndexes[i].text = $"[{i}]";
            bool occupied = queue.IsPhysicalSlotOccupied(i);
            physicalValues[i].text = occupied ? queue.GetPhysicalSlot(i).ToString() : "-";

            bool isHead = i == queue.HeadIndex;
            bool isTail = i == queue.TailIndex;
            if (isHead && isTail)
                SetMarker(
                    physicalMarkers[i],
                    queue.Size() == queue.Capacity ? "↑ HEAD / TAIL\nFULL" : "↑ HEAD / TAIL",
                    true);
            else if (isHead) SetMarker(physicalMarkers[i], "↑ HEAD");
            else if (isTail) SetMarker(physicalMarkers[i], "↑ TAIL");
            else SetMarker(physicalMarkers[i], string.Empty);

            physicalOutlines[i].effectColor = isHead
                ? new Color(0.76f, 0.20f, 0.16f)
                : isTail
                    ? new Color(0.12f, 0.38f, 0.68f)
                    : new Color(0.45f, 0.45f, 0.45f);
            physicalOutlines[i].effectDistance = isHead || isTail
                ? new Vector2(3f, -3f)
                : new Vector2(1f, -1f);
        }
    }

    static void SetMarker(Text marker, string label, bool wide = false)
    {
        marker.text = label;
        marker.rectTransform.sizeDelta = wide
            ? new Vector2(90f, 34f)
            : new Vector2(52f, 25f);
    }

    bool IsAnimating() => animation != null && animation.IsActive();
    void ShowResult(string message) => resultText.text = message;
    void OnDisable() => animation?.Kill();
}
