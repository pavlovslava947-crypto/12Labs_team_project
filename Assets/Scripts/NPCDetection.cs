using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCDetection : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float moveSpeed = 3f;

    private PlayerStealth stealthScript;
    private Rigidbody2D rb;
    private Animator anim;
    private bool chasing = false;

    void Start()
    {
        stealthScript = player.GetComponent<PlayerStealth>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        // Логика обнаружения
        if (!stealthScript.IsStealth() && distance <= detectionRange)
        {
            if (!chasing)
            {
                chasing = true;
                stealthScript.EndStealth(true);
            }
        }

        UpdateAnimationAndFlip();
    }

    void UpdateAnimationAndFlip()
    {
        if (anim == null) return;

        if (chasing)
        {
            anim.SetFloat("Speed", 1f);

            if (player.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (player.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            anim.SetFloat("Speed", 0f);
        }
    }

    void FixedUpdate()
    {
        if (chasing)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Ищем компонент здоровья на игроке
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Наносим 1 единицу урона (можно вынести в public переменную damage)
                playerHealth.TakeDamage(1);

                // Чтобы NPC не наносил урон каждый кадр при залипании в игрока,
                // сбрасываем погоню. Ему придется снова заметить игрока.
                chasing = false;
            }
            else
            {
                // Если вдруг забыли повесить скрипт PlayerHealth на игрока:
                Debug.LogWarning("На Игроке не найден скрипт PlayerHealth! Перезагружаю сцену по умолчанию.");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }


    // ВОТ ЭТОТ МЕТОД ДОЛЖЕН БЫТЬ СТРОГО ЗДЕСЬ
    public void OnTeleport()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (stealthScript.IsStealth() || distance > detectionRange)
        {
            chasing = false;
        }
    }
}