using UnityEngine;
using TMPro;
using UnityEngine.UI; // Добавь это, чтобы работать с Image

public class PlayerStealth : MonoBehaviour
{
    [Header("Stealth Settings")]
    public float stealthDuration = 5f;

    [Header("UI")]
    public TextMeshProUGUI stealthText;
    public Image stealthOverlay; // Ссылка на наше черное изображение

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    private bool isStealth = false;
    private float stealthTimer;

    private Color normalColor;
    private Color stealthColor;

    void Start()
    {
        stealthText.gameObject.SetActive(false);
        if (stealthOverlay != null) stealthOverlay.gameObject.SetActive(false); // Прячем в начале

        normalColor = spriteRenderer.color;
        stealthColor = normalColor;
        stealthColor.a = 0.5f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isStealth)
        {
            ActivateStealth();
        }

        if (isStealth)
        {
            stealthTimer -= Time.deltaTime;
            stealthText.text = "Стелс: " + Mathf.Ceil(stealthTimer).ToString();

            if (stealthTimer <= 0)
            {
                EndStealth(false);
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

        // Включаем затемнение
        if (stealthOverlay != null) stealthOverlay.gameObject.SetActive(true);
    }

    public void EndStealth(bool detected)
    {
        isStealth = false;
        spriteRenderer.color = normalColor;

        // Выключаем затемнение
        if (stealthOverlay != null) stealthOverlay.gameObject.SetActive(false);

        if (detected)
        {
            stealthText.gameObject.SetActive(true);
            stealthText.text = "ОБНАРУЖЕН";
            stealthText.color = Color.red;
        }
        else
        {
            stealthText.gameObject.SetActive(false);
        }
    }

    public bool IsStealth() => isStealth;
}