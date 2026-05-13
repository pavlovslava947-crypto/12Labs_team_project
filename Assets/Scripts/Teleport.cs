using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TeleportPlayer : MonoBehaviour
{
    public Transform teleportTarget;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    [Header("Sound")]
    public AudioSource audioSource;

    private bool isTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTeleporting)
        {
            StartCoroutine(TeleportSequence(other.transform));
        }
    }

    IEnumerator TeleportSequence(Transform player)
    {
        isTeleporting = true;

        // Воспроизведение звука
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Затемнение
        yield return StartCoroutine(Fade(0, 1));

        // Телепорт
        player.position = teleportTarget.position;

        // Небольшая пауза
        yield return new WaitForSeconds(0.2f);

        // Осветление
        yield return StartCoroutine(Fade(1, 0));

        isTeleporting = false;
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                elapsedTime / fadeDuration
            );

            fadeImage.color = new Color(
                color.r,
                color.g,
                color.b,
                alpha
            );

            yield return null;
        }

        fadeImage.color = new Color(
            color.r,
            color.g,
            color.b,
            endAlpha
        );
    }
}