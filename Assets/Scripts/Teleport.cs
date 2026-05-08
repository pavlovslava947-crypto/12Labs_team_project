using UnityEngine;

public class TeleportPlayer : MonoBehaviour
{
    // Точка, куда будет телепортирован игрок
    public Transform teleportTarget;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, что вошел именно игрок
        if (other.CompareTag("Player"))
        {
            // Перемещаем игрока
            other.transform.position = teleportTarget.position;
        }
    }
}