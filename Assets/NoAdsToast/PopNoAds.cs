using DG.Tweening;
using UnityEngine;

public class PopNoAds : MonoBehaviour
{
    RectTransform rect;

    [Header("Positions")]
    public float startY = -150f;
    public float targetY = 250f;

    [Header("Animation Timing")]
    public float moveDuration = 0.35f;

    [Header("Lifetime")]
    public float destroyAfter = 3f;

    void Start()
    {
        rect = GetComponent<RectTransform>();

        // Set starting position
        Vector2 pos = rect.anchoredPosition;
        pos.y = startY;
        rect.anchoredPosition = pos;

        PlayAnimation();
    }

    void PlayAnimation()
    {
        rect.DOKill();

        rect.DOAnchorPosY(targetY, moveDuration).SetEase(Ease.OutCubic);

        // Destroy after delay
        Destroy(gameObject, destroyAfter);
    }
}
