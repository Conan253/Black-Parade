using UnityEngine;
using TMPro;
using System.Collections;

public class BeginScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text beginText;
    [SerializeField] private float showTime = 1.5f;

    void Start()
    {
        StartCoroutine(ShowBeginText());
    }

    IEnumerator ShowBeginText()
    {
        beginText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(showTime);
        beginText.gameObject.SetActive(false);
    }
}
