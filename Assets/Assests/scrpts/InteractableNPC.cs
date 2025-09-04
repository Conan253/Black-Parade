using UnityEngine;

public class InteractableNPC : MonoBehaviour
{
    [TextArea] public string dialogueLine = "Hey there. Be careful out there.";
    public KeyCode interactKey = KeyCode.E;

    [Header("Refs")]
    [SerializeField] private GameObject prompt; // assign Prompt_E (world-space)
    private bool playerInRange;

    void Start()
    {
        if (prompt) prompt.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        // Start or close dialogue with the same key
        if (Input.GetKeyDown(interactKey))
        {
            if (!DialogueManager.IsOpen)
            {
                DialogueManager.Instance?.Show(dialogueLine);
            }
            else
            {
                DialogueManager.Instance?.Hide();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (prompt) prompt.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (prompt) prompt.SetActive(false);

        // Auto-close dialogue when you leave the zone (optional)
        if (DialogueManager.IsOpen) DialogueManager.Instance.Hide();
    }
}
