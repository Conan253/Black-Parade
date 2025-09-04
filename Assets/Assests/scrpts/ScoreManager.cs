using UnityEngine;
using TMPro;  // If you use TextMeshPro for UI

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TMP_Text scoreText;   // Drag a TMP Text here (Canvas)

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Add(int value)
    {
        score += value;
        if (scoreText) scoreText.text = $"Score: {score}";
    }
}
