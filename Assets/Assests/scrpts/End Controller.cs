using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndSceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup fadeOverlay;   // FadeOverlay CanvasGroup
    [SerializeField] private TMP_Text endText;          // "END" TMP_Text
    [SerializeField] private GameObject buttonGroup;    // parent of the buttons
    [SerializeField] private Button firstSelected;      // e.g., Main Menu button

    [Header("Timing")]
    [SerializeField] private float fadeInTime = 1.0f;   // screen black -> clear
    [SerializeField] private float endTextFade = 0.6f;  // fade in END text
    [SerializeField] private float delayBeforeButtons = 0.3f;

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string gameScene = "Game"; // for Restart

    void Start()
    {
        // Start fully black with hidden text/buttons
        if (fadeOverlay) fadeOverlay.alpha = 1f;
        if (endText) endText.alpha = 0f;
        if (buttonGroup) buttonGroup.SetActive(false);

        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // Fade screen from black
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.unscaledDeltaTime;
            if (fadeOverlay) fadeOverlay.alpha = Mathf.Lerp(1f, 0f, t / fadeInTime);
            yield return null;
        }
        if (fadeOverlay) fadeOverlay.alpha = 0f;

        // Fade in "END"
        t = 0f;
        while (t < endTextFade)
        {
            t += Time.unscaledDeltaTime;
            if (endText) endText.alpha = Mathf.Lerp(0f, 1f, t / endTextFade);
            yield return null;
        }
        if (endText) endText.alpha = 1f;

        yield return new WaitForSecondsRealtime(delayBeforeButtons);

        // Show buttons
        if (buttonGroup) buttonGroup.SetActive(true);
        if (firstSelected) UnityEngine.EventSystems.EventSystem.current.
            SetSelectedGameObject(firstSelected.gameObject);
    }

    // --- Button hooks ---
    public void OnMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void OnExit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
