using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public static bool IsOpen => Instance != null && Instance.panel.activeSelf;

    [Header("UI")]
    [SerializeField] private GameObject panel;     // assign DialoguePanel
    [SerializeField] private TMP_Text text;        // assign DialogueText

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (panel) panel.SetActive(false);
    }

    public void Show(string line)
    {
        if (!panel || !text) return;
        text.text = line;
        panel.SetActive(true);
    }

    public void Hide()
    {
        if (!panel) return;
        panel.SetActive(false);
    }
}
