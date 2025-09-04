using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] string gameSceneName = "Game";  // set your scene name
    [SerializeField] float preLoadDelay = 0.25f;     // small pause after click
    [SerializeField] float fadeDuration = 1.0f;      // UI/music fade time

    [Header("UI")]
    [SerializeField] CanvasGroup fadeOverlay;        // CanvasGroup on FadeOverlay
    [SerializeField] Button startButton;             // optional: to disable during transition
    [SerializeField] Button exitButton;              // optional

    [Header("Audio")]
    [SerializeField] AudioSource menuMusic;          // the AudioSource that plays menu music
    [SerializeField] AudioSource uiSfxSource;        // optional: a SFX source
    [SerializeField] AudioClip clickSfx;             // optional: click sound

    bool isTransitioning;

    void Start()
    {
        // Ensure time/audio unpaused when entering menu
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (fadeOverlay) fadeOverlay.alpha = 0f;
        if (menuMusic && !menuMusic.isPlaying) menuMusic.Play();
    }

    public void OnStartGame()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (uiSfxSource && clickSfx) uiSfxSource.PlayOneShot(clickSfx, 0.8f);

        if (startButton) startButton.interactable = false;
        if (exitButton)  exitButton.interactable  = false;

        StartCoroutine(DoStartTransition());
    }

    public void OnExit()
    {
        if (isTransitioning) return;
        if (uiSfxSource && clickSfx) uiSfxSource.PlayOneShot(clickSfx, 0.8f);
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    IEnumerator DoStartTransition()
    {
        // tiny delay so the click SFX can be heard
        if (preLoadDelay > 0f) yield return new WaitForSecondsRealtime(preLoadDelay);

        // parallel fade: UI overlay to black + music down to 0
        float t = 0f;
        float startVol = (menuMusic ? menuMusic.volume : 0f);
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / fadeDuration);

            if (fadeOverlay) fadeOverlay.alpha = k;                 // fade screen to black
            if (menuMusic)  menuMusic.volume = Mathf.Lerp(startVol, 0f, k); // fade music

            yield return null;
        }

        if (menuMusic) { menuMusic.Stop(); menuMusic.volume = startVol; }

        // load the game scene (async with short black screen looks best)
        AsyncOperation op = SceneManager.LoadSceneAsync(gameSceneName);
        // (Optional) force one extra frame of black
        while (!op.isDone) yield return null;
    }
}
