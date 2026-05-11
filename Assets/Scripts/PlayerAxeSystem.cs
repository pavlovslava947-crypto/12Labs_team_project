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

    [Header("Настройки удара")]
    [SerializeField] private float attackDuration = 0.2f; // Скорость удара
    [SerializeField] private float swingAngle = -90f;    // Угол вращения

    private bool isAttacking = false;

    void Update()
    {
        if (Keyboard.current != null)
        {
            float moveX = 0;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1;

            if (moveX != 0) lastDirection = new Vector2(moveX, 0).normalized;

            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (isHolding) Drop();
                else TryPickUp();
            }

            // ИСПРАВЛЕНО: Оставили один блок для атаки
            if (isHolding && !isAttacking && Mouse.current.leftButton.wasPressedThisFrame)
            {
                // Выход из стелса
                if (TryGetComponent(out PlayerStealth stealth))
                {
                    stealth.EndStealth(false);
                }

                StartCoroutine(PerformSwing());
                CheckHit();
            }
        }
    }

    private void CheckHit()
    {
        // ИСПРАВЛЕНО: Смещаем начало луча (0.7f), чтобы он начинался ЗА пределами игрока
        Vector2 origin = (Vector2)transform.position + (lastDirection * 0.7f);

        // Пускаем луч (не используя itemLayer, чтобы он видел Бабушку на любом слое)
        RaycastHit2D hit = Physics2D.Raycast(origin, lastDirection, rayDistance);

        // Рисуем луч в сцене (Маджента), чтобы ты видел его точку старта
        Debug.DrawRay(origin, lastDirection * rayDistance, Color.magenta, 0.5f);

        if (hit.collider != null)
        {
            Debug.Log("Удар попал в: " + hit.collider.name); // Проверь это в консоли!

            if (hit.collider.CompareTag("OldLady"))
            {
                if (hit.collider.TryGetComponent(out OldLadyHealth health))
                {
                    health.TakeDamage();
                }
            }
        }
    }

    private System.Collections.IEnumerator PerformSwing()
    {
        isAttacking = true;
        Quaternion startRot = currentAxe.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, swingAngle);

        float elapsed = 0;
        while (elapsed < attackDuration)
        {
            elapsed += Time.deltaTime;
            currentAxe.transform.localRotation = Quaternion.Lerp(startRot, endRot, elapsed / attackDuration);
            yield return null;
        }

        // Возвращаем назад
        currentAxe.transform.localRotation = startRot;
        isAttacking = false;
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

        // 1. Сохраняем текущий слой, чтобы вернуть его потом
        // (По желанию, но лучше просто запомнить, что он был "Items")

        // 2. МЕНЯЕМ СЛОЙ НА Ignore Raycast (слой номер 2)
        currentAxe.layer = LayerMask.NameToLayer("Ignore Raycast");

        // Если у топора есть "дети" (лезвие, палка), меняем слой и им
        foreach (Transform child in currentAxe.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        currentAxe.transform.SetParent(holdPoint);
        currentAxe.transform.localPosition = Vector3.zero;
        currentAxe.transform.localRotation = Quaternion.identity;

        if (currentAxe.TryGetComponent(out Rigidbody2D rb)) rb.simulated = false;
        if (currentAxe.TryGetComponent(out Collider2D col)) col.enabled = false;
    }

    private void Drop()
    {
        // 3. ВОЗВРАЩАЕМ СЛОЙ ОБРАТНО (на Items)
        currentAxe.layer = LayerMask.NameToLayer("Items");

        foreach (Transform child in currentAxe.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Items");
        }

        isHolding = false;
        currentAxe.transform.SetParent(null);
        currentAxe.transform.position = (Vector2)transform.position + lastDirection * 0.5f;

        if (currentAxe.TryGetComponent(out Rigidbody2D rb)) rb.simulated = true;
        if (currentAxe.TryGetComponent(out Collider2D col)) col.enabled = true;

        currentAxe = null;
    }
}