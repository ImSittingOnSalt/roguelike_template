using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
    public bool isPlayerBullet; // Определяет, чья это пуля

    private Rigidbody2D rb;
    //private Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //player = GameObject.Find("hero").transform;

        //Vector2 direction = (player.position - transform.position).normalized;
        //rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerBullet && other.CompareTag("Enemy"))
        {
            // Нанести урон врагу (добавьте скрипт Enemy с методом TakeDamage)
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(1);
            Destroy(gameObject);
        }
        else if (!isPlayerBullet && other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null) player.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}
