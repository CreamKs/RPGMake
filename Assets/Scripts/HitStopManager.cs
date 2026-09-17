using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance;

    private Coroutine hitStopCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void Stop(float duration)
    {
        if (hitStopCoroutine != null)
        {
            StopCoroutine(hitStopCoroutine);
        }

        hitStopCoroutine = StartCoroutine(HitStopCoroutine(duration));
    }

    private IEnumerator HitStopCoroutine(float duration)
    {
        Time.timeScale = 0.1f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1f;

        hitStopCoroutine = null;
    }
}