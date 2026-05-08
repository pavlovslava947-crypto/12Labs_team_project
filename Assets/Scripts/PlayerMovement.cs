using UnityEngine;
using UnityEngine.InputSystem; // Добавляем эту строку!

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    // В новой системе мы можем считывать ввод прямо в Update вот так:
    void Update()
    {
        // Считываем WASD или стрелки через новый API
        Vector2 keyboardInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) keyboardInput.y = 1;
            if (Keyboard.current.sKey.isPressed) keyboardInput.y = -1;
            if (Keyboard.current.aKey.isPressed) keyboardInput.x = -1;
            if (Keyboard.current.dKey.isPressed) keyboardInput.x = 1;
        }

        moveInput = keyboardInput.normalized;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }
}