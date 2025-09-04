using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Behavior")]
    [SerializeField] List<string> playInScenes = new List<string> { "Game", "EndScene" };
    [SerializeField] float fadeTime = 0.75f;

    AudioSource src;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        src = GetComponent<AudioSource>();
        src.loop = true;
        if (!src.isPlaying) src.Play();

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        bool shouldPlay = playInScenes.Contains(newScene.name);

        StopAllCoroutines();
        if (shouldPlay)
        {
            if (!src.isPlaying) src.Play();
            StartCoroutine(FadeTo(1f, fadeTime));   // ensure audible
        }
        else
        {
            StartCoroutine(FadeOutAndMaybeStop());
        }
    }

    IEnumerator FadeOutAndMaybeStop()
    {
        yield return FadeTo(0f, fadeTime);
        src.Stop();
    }

    IEnumerator FadeTo(float target, float seconds)
    {
        float start = src.volume, t = 0f;
        while (t < seconds)
        {
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, target, t / seconds);
            yield return null;
        }
        src.volume = target;
    }
}
