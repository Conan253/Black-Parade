using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadEndOnTrigger : MonoBehaviour
{
    [SerializeField] private string endSceneName = "EndScene";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(endSceneName);
        }
    }
}
