using UnityEngine;
using TMPro;

public class PlayerStealth : MonoBehaviour
{
    [Header("Stealth Settings")]
    public float stealthDuration = 5f;

    [Header("UI")]
    public TextMeshProUGUI stealthText;

    [Header("Visual")]
    public SpriteRenderer spriteRenderer;

    private bool isStealth = false;
    private float stealthTimer;

    private Color normalColor;
    private Color stealthColor;

    void Start()
    {
        stealthText.gameObject.SetActive(false);

        normalColor = spriteRenderer.color;

        stealthColor = normalColor;
        stealthColor.a = 0.5f;
    }

    void Update()
    {
        // Включение стелса
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isStealth)
        {
            ActivateStealth();
        }

        // Таймер стелса
        if (isStealth)
        {
            stealthTimer -= Time.deltaTime;

            stealthText.text = "Стелс: " + Mathf.Ceil(stealthTimer).ToString();

            if (stealthTimer <= 0)
            {
                EndStealth(true);
            }
        }
    }

    void ActivateStealth()
    {
        isStealth = true;
        stealthTimer = stealthDuration;

        stealthText.gameObject.SetActive(true);

        spriteRenderer.color = stealthColor;
    }

    public void EndStealth(bool detected)
    {
        isStealth = false;

        spriteRenderer.color = normalColor;

        if (detected)
        {
            stealthText.text = "ОБНАРУЖЕН";
            stealthText.color = Color.red;
        }
        else
        {
            stealthText.gameObject.SetActive(false);
        }
    }

    public bool IsStealth()
    {
        return isStealth;
    }
}