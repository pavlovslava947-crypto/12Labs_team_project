using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    // Ссылка на компонент Animator
    private Animator anim;
    public AudioClip[] stepSounds;      // Массив для звуков
    private AudioSource audioSource;    // Ссылка на компонент звука

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // Инициализируем аниматор
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector2 keyboardInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) keyboardInput.y = 1;
            if (Keyboard.current.sKey.isPressed) keyboardInput.y = -1;
            if (Keyboard.current.aKey.isPressed) keyboardInput.x = -1;
            if (Keyboard.current.dKey.isPressed) keyboardInput.x = 1;
        }

        moveInput = keyboardInput.normalized;

        // --- ЛОГИКА АНИМАЦИИ ---
        if (anim != null)
        {
            // Передаем значение в параметр Speed. 
            // Используем величину вектора (magnitude), чтобы анимация включалась при движении в любую сторону.
            anim.SetFloat("Speed", moveInput.magnitude);
        }

        // --- ЛОГИКА РАЗВОРОТА (FLIP) ---
        // Разворачиваем персонажа влево или вправо в зависимости от нажатой клавиши
        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void PlayStepSound()
    {
        if (stepSounds.Length > 0 && audioSource != null)
        {
            // Выбираем случайный звук из массива
            int index = Random.Range(0, stepSounds.Length);
            audioSource.PlayOneShot(stepSounds[index]);
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}