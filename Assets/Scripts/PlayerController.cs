using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    public DynamicJoystick joystick;
    public delegate void HealthChanged(int health);
    public static event HealthChanged OnHealthChanged;
    public int maxHealth = 5;
    public int currentHealth;
    public float speed = 5f;
    [Header("Auto Attack")]
    public float attackRadius = 5f; // Радиус обнаружения врагов
    public float attackCooldown = 1f; 
    public GameObject playerBulletPrefab; 

    private float currentCooldown;


    private Rigidbody2D rb;
    private Vector2 inputDirection;
    

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth); // Инициализация U
    }

    void Awake()
    {
        Instance = this; // Инициализация статической ссылки, нужно чтобы из юай могла обратиться сюда к статам
    }

    void FixedUpdate()
    {
        inputDirection = new Vector2(joystick.Horizontal, joystick.Vertical);
        rb.linearVelocity = inputDirection * speed;
    }

    void Update()
    {
        if (inputDirection.x != 0)
            transform.rotation = Quaternion.Euler(0, inputDirection.x > 0 ? 180 : 0, 0);
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0)
        {
            TryShoot();
            currentCooldown = attackCooldown;
        }
    }

    void TryShoot()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy")
            .Where(e => Vector2.Distance(transform.position, e.transform.position) <= attackRadius).ToList(); // Найти всех врагов в радиусе

        if (enemies.Count == 0) return;

        var closestEnemy = enemies
            .OrderBy(e => Vector2.Distance(transform.position, e.transform.position)).First(); // Выбрать ближайшего врага

        // Создать пулю и направить её к врагу
        GameObject bullet = Instantiate(playerBulletPrefab, transform.position, Quaternion.identity);
        Vector2 direction = (closestEnemy.transform.position - transform.position).normalized;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bullet.GetComponent<Bullet>().speed;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth); // Обновить UI

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Логика смерти: перезагрузка сцены или экран поражения
        Debug.Log("Игрок умер!");
    }
}