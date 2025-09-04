using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class SceneFadeIn : MonoBehaviour
{
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private bool forceStretchFullScreen = true;

    CanvasGroup cg;
    RectTransform rt;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();

        if (forceStretchFullScreen && rt != null)
        {
            // Force full-screen stretch
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }

        // start fully black
        cg.alpha = 1f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    void Start()
    {
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, t / fadeInTime);
            yield return null;
        }
        cg.alpha = 0f;
        // Optional: disable after fade
        gameObject.SetActive(false);
    }

    // (Optional) Call this later to fade back to black before a scene change
    public IEnumerator FadeOut(float duration)
    {
        gameObject.SetActive(true);
        cg.alpha = 0f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }
        cg.alpha = 1f;
    }
}
    