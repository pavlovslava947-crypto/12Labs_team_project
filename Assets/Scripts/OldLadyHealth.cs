using UnityEngine;
using System.Collections;

public class OldLadyHealth : MonoBehaviour
{
    public int health = 3;
    public SpriteRenderer spriteRenderer;
    public float flashDuration = 0.1f;
    public int flashCount = 4;

    private bool isInvulnerable = false;

    public void TakeDamage()
    {
        Debug.Log("Бабушка получила урон! Текущее HP: " + health);
        if (isInvulnerable) return;

        health--;
        if (health <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlickerEffect());
        }
    }

    private IEnumerator FlickerEffect()
    {
        isInvulnerable = true;
        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(flashDuration);
        }
        isInvulnerable = false;
    }

    private void Die()
    {
        Debug.Log("Бабушка повержена.");
        Destroy(gameObject);
    }
}