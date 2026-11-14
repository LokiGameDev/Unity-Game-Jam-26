using System.Collections;
using UnityEngine;

public class DisableAfterLifetime : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(StartFade());
    }

    private IEnumerator StartFade()
    {
        yield return new WaitForSeconds(2.5f);
        gameObject.SetActive(false);
    }
}
