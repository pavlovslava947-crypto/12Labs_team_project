using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCDetection : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float moveSpeed = 3f;

    private PlayerStealth stealthScript;
    private Rigidbody2D rb;
    private Animator anim; // Ссылка на аниматор
    private bool chasing = false;

    void Start()
    {
        stealthScript = player.GetComponent<PlayerStealth>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); // Инициализируем аниматор

        // Убедимся, что Rigidbody настроен правильно для 2D
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

        // --- ЛОГИКА АНИМАЦИИ И РАЗВОРОТА ---
        UpdateAnimationAndFlip();
    }

    void UpdateAnimationAndFlip()
    {
        if (anim == null) return;

        if (chasing)
        {
            // Если гонимся — передаем скорость 1 (включает Walk)
            anim.SetFloat("Speed", 1f);

            // Разворот в сторону игрока
            if (player.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1); // Смотрит вправо
            }
            else if (player.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1); // Смотрит влево
            }
        }
        else
        {
            // Если стоим на месте — скорость 0 (включает Idle)
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}