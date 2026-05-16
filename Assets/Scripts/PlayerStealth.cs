using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerStealth : MonoBehaviour
{
    [Header("Stealth Settings")]
    public float stealthDuration = 5f;
    public float cooldownDuration = 10f; // Время перезарядки стелса

    [Header("UI")]
    public TextMeshProUGUI stealthText;
    public Image stealthOverlay;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    private bool isStealth = false;
    private float stealthTimer;

    private bool isCooldown = false;    // Находится ли стелс на перезарядке
    private float cooldownTimer;        // Таймер перезарядки

    private Color normalColor;
    private Color stealthColor;

    void Start()
    {
        stealthText.gameObject.SetActive(false);
        if (stealthOverlay != null) stealthOverlay.gameObject.SetActive(false);

        normalColor = spriteRenderer.color;
        stealthColor = normalColor;
        stealthColor.a = 0.5f;
    }

    void Update()
    {
        // Стелс можно включить, только если мы не в стелсе И нет кулдауна
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isStealth && !isCooldown)
        {
            ActivateStealth();
        }

        // Логика активного стелса
        if (isStealth)
        {
            stealthTimer -= Time.deltaTime;
            stealthText.text = "Стелс: " + Mathf.Ceil(stealthTimer).ToString();

            if (stealthTimer <= 0)
            {
                EndStealth(false);
            }
        }
        // Логика перезарядки (работает, когда стелс выключен)
        else if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            // Проверяем, чтобы текст не затерло надписью "ОБНАРУЖЕН" слишком рано
            if (stealthText.text != "ОБНАРУЖЕН")
            {
                stealthText.gameObject.SetActive(true);
                stealthText.color = Color.yellow; // Желтый цвет для перезарядки
                stealthText.text = "Перезарядка: " + Mathf.Ceil(cooldownTimer).ToString();
            }

            if (cooldownTimer <= 0)
            {
                isCooldown = false;
                stealthText.gameObject.SetActive(false); // Прячем текст, стелс снова готов
            }
        }
    }

    void ActivateStealth()
    {
        isStealth = true;
        stealthTimer = stealthDuration;

        stealthText.gameObject.SetActive(true);
        stealthText.color = Color.white;

        spriteRenderer.color = stealthColor;

        if (stealthOverlay != null) stealthOverlay.gameObject.SetActive(true);
    }

    public void EndStealth(bool detected)
    {
        // Если игрок и так НЕ был в стелсе, ничего не делаем и кулдаун не запускаем!
        if (!isStealth) return;

        isStealth = false;
        spriteRenderer.color = normalColor;

        if (stealthOverlay != null) stealthOverlay.gameObject.SetActive(false);

        // Теперь перезарядка включится только если стелс реально работал
        isCooldown = true;
        cooldownTimer = cooldownDuration;

        if (detected)
        {
            stealthText.gameObject.SetActive(true);
            stealthText.text = "ОБНАРУЖЕН";
            stealthText.color = Color.red;

            StartCoroutine(ClearDetectedStatus());
        }
    }

    private System.Collections.IEnumerator ClearDetectedStatus()
    {
        yield return new WaitForSeconds(1.5f);
        // Если за это время кулдаун еще идет, сбрасываем текст "ОБНАРУЖЕН", 
        // чтобы Update начал показывать оставшееся время кулдауна
        if (isCooldown && stealthText.text == "ОБНАРУЖЕН")
        {
            stealthText.text = "";
        }
    }

    public bool IsStealth() => isStealth;

    // Дополнительный метод на случай, если врагам нужно проверить, можно ли сейчас активировать стелс
    public bool CanUseStealth() => !isStealth && !isCooldown;
}