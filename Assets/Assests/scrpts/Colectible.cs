using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int value = 1;
    public AudioClip pickupSfx;
    public GameObject pickupVfxPrefab;

    private SpriteRenderer sr;
    private Collider2D col;
    private AudioSource audioSrc;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        audioSrc = GetComponent<AudioSource>(); // optional
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Add score
        if (ScoreManager.Instance) ScoreManager.Instance.Add(value);

        // Effects
        if (pickupVfxPrefab) Instantiate(pickupVfxPrefab, transform.position, Quaternion.identity);
        if (pickupSfx && audioSrc) audioSrc.PlayOneShot(pickupSfx);

        // Hide immediately, destroy after sfx
        sr.enabled = false;
        col.enabled = false;
        Destroy(gameObject, (pickupSfx && audioSrc) ? pickupSfx.length : 0f);
    }
}
