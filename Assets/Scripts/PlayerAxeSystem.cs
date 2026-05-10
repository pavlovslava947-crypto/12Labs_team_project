using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAxeSystem : MonoBehaviour
{
    [Header("Настройки луча")]
    [SerializeField] private float rayDistance = 1.5f;
    [SerializeField] private LayerMask itemLayer; // Выберите слой Items

    [Header("Слот для топора")]
    [SerializeField] private Transform holdPoint; // Пустой объект-дочерний игроку

    private GameObject currentAxe;
    private bool isHolding = false;
    private Vector2 lastDirection = Vector2.right;

    void Update()
    {
        // 1. Читаем движение через новый Input System (Keyboard.current)
        if (Keyboard.current != null)
        {
            // Получаем ввод по горизонтали (A/D или Стрелки)
            float moveX = 0;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1;

            if (moveX != 0)
            {
                lastDirection = new Vector2(moveX, 0).normalized;
            }

            // 2. Проверка нажатия клавиши E через новую систему
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (isHolding) Drop();
                else TryPickUp();
            }
        }
    }
    private void TryPickUp()
    {
        // Пускаем луч в сторону последнего движения
        RaycastHit2D hit = Physics2D.Raycast(transform.position, lastDirection, rayDistance, itemLayer);

        if (hit.collider != null)
            Debug.Log("Луч попал в: " + hit.collider.name);
        else
            Debug.Log("Луч ни во что не попал");

        // Визуализация луча в редакторе (зеленый - попал, красный - нет)
        Debug.DrawRay(transform.position, lastDirection * rayDistance, hit.collider ? Color.green : Color.red, 0.5f);

        if (hit.collider != null && hit.collider.CompareTag("Axe"))
        {
            PickUp(hit.collider.gameObject);
        }
    }

    private void PickUp(GameObject axe)
    {
        currentAxe = axe;
        isHolding = true;

        // Привязываем к игроку
        currentAxe.transform.SetParent(holdPoint);
        currentAxe.transform.localPosition = Vector3.zero;
        currentAxe.transform.localRotation = Quaternion.identity;

        // Отключаем физику и коллизии, пока топор в руках
        if (currentAxe.TryGetComponent(out Rigidbody2D rb)) rb.simulated = false;
        if (currentAxe.TryGetComponent(out Collider2D col)) col.enabled = false;
    }

    private void Drop()
    {
        isHolding = false;

        // Отвязываем
        currentAxe.transform.SetParent(null);

        // Немного откидываем топор вперед, чтобы не подобрать его сразу же
        currentAxe.transform.position = (Vector2)transform.position + lastDirection * 0.5f;

        // Включаем физику обратно
        if (currentAxe.TryGetComponent(out Rigidbody2D rb)) rb.simulated = true;
        if (currentAxe.TryGetComponent(out Collider2D col)) col.enabled = true;

        currentAxe = null;
    }
}