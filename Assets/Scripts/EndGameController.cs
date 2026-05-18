using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGameController : MonoBehaviour
{
    public Image fadeImage;
    public TextMeshProUGUI endText;
    public GameObject menuButton;

    public float fadeDuration = 2f;

    public void TriggerEnd()
    {
        StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        // Затемнение
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            float alpha = t / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, alpha);

            yield return null;
        }

        // Показ текста
        yield return new WaitForSeconds(1f);

        endText.gameObject.SetActive(true);

        // Плавное появление текста
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime;

            endText.alpha = t;

            yield return null;
        }

        // Кнопка
        yield return new WaitForSeconds(1f);

        menuButton.SetActive(true);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}