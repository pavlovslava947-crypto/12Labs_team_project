using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("UI Settings (Опционально)")]
    [Tooltip("Если оставить пустым, скрипт сам создаст текстовое поле на экране")]
    public TextMeshProUGUI healthText;

    void Start()
    {
        currentHealth = maxHealth;

        // Если текстовое поле не назначено в инспекторе, создаем его автоматически
        if (healthText == null)
        {
            SetupDefaultUI();
        }

        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Игрок получил урон! Текущее ХП: {currentHealth}");

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Обновление текста на экране
    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"Здоровье: {currentHealth} / {maxHealth}";

            // Визуальный эффект: если ХП остается 1, красим текст в красный
            if (currentHealth <= 1)
            {
                healthText.color = Color.red;
            }
            else
            {
                healthText.color = Color.white;
            }
        }
    }

    private void Die()
    {
        Debug.Log("Игрок погиб. Перезагрузка сцены...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Автоматическое создание Canvas и TextMeshPro, если в инспекторе пусто
    private void SetupDefaultUI()
    {
        // 1. Создаем Canvas
        GameObject canvasObj = new GameObject("HealthCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // 2. Создаем объект для текста внутри Canvas
        GameObject textObj = new GameObject("HealthText");
        textObj.transform.SetParent(canvasObj.transform);

        // 3. Настраиваем TextMeshPro
        healthText = textObj.AddComponent<TextMeshProUGUI>();
        healthText.fontSize = 36;
        healthText.alignment = TextAlignmentOptions.TopLeft;

        // 4. Позиционируем текст в левом верхнем углу экрана
        RectTransform rectTransform = healthText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(20, -20); // Отступ от краев экрана
        rectTransform.sizeDelta = new Vector2(300, 50);
    }

    public int GetCurrentHealth() => currentHealth;
}