using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCDetection : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float moveSpeed = 3f;

    private PlayerStealth stealthScript;
    private Rigidbody2D rb;
    private bool chasing = false;

    void Start()
    {
        stealthScript = player.GetComponent<PlayerStealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        // Если игрок НЕ в стелсе и находится в радиусе видимости
        if (!stealthScript.IsStealth() && distance <= detectionRange)
        {
            if (!chasing) // Если только что заметили
            {
                chasing = true;
                // Теперь надпись ОБНАРУЖЕН появится, так как нас увидел NPC
                stealthScript.EndStealth(true);
            }
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
            // Если NPC коснулся игрока — это проигрыш
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}