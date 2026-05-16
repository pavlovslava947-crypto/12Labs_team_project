using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TeleportPlayer : MonoBehaviour
{
    public Transform teleportTarget;

    [Header("Fade (Только для Игрока)")]
    public Image fadeImage;
    public float fadeDuration = 0.5f;

    [Header("Sound")]
    public AudioSource audioSource;

    private bool isPlayerTeleporting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Если зашел Игрок
        if (other.CompareTag("Player") && !isPlayerTeleporting)
        {
            StartCoroutine(PlayerTeleportSequence(other.transform));
        }
        // Если зашел NPC (Дворник или Старуха)
        else if (other.CompareTag("NPC"))
        {
            NPCTeleport(other.gameObject);
        }
    }

    // Логика для ИГРОКА (с затемнением)
    IEnumerator PlayerTeleportSequence(Transform player)
    {
        isPlayerTeleporting = true;

        if (audioSource != null) audioSource.Play();

        // Затемнение экрана игрока
        if (fadeImage != null) yield return StartCoroutine(Fade(0, 1));

        ExecutePhysicsTeleport(player);

        yield return new WaitForSeconds(0.2f);

        // Осветление экрана игрока
        if (fadeImage != null) yield return StartCoroutine(Fade(1, 0));

        isPlayerTeleporting = false;
    }

    // Логика для NPC (Мгновенно, без анимации экрана)
    private void NPCTeleport(GameObject npc)
    {
        if (audioSource != null) audioSource.Play();

        Debug.Log($"NPC {npc.name} зашел в телепорт. Перемещаю...");
        ExecutePhysicsTeleport(npc.transform);

        // Даем команду обновить логику через его скрипт
        NPCDetection npcScript = npc.GetComponent<NPCDetection>();
        if (npcScript != null)
        {
            npcScript.OnTeleport();
        }
    }

    // Общий метод для безопасного перемещения сбросом физики
    private void ExecutePhysicsTeleport(Transform objTransform)
    {
        if (teleportTarget == null)
        {
            Debug.LogError($"ОШИБКА: На объекте {gameObject.name} не назначен teleportTarget!");
            return;
        }

        Rigidbody2D rb = objTransform.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            objTransform.position = teleportTarget.position;
            rb.position = teleportTarget.position;
        }
        else
        {
            objTransform.position = teleportTarget.position;
        }
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}