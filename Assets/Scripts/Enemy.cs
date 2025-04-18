using UnityEngine;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float speed = 2f;
    public float shootCooldown = 2f;
    public int health = 3;
    public float wanderRadius = 3f;   // Радиус для случайного перемещения
    public float idleTime = 1f;     // Пауза после достижения точки
    public event Action OnDeath; // Событие смерти

    private Transform _player;
    private Vector3 _targetPosition;
    private float _shootTimer;
    private bool _isMoving = true;

    void Start()
    {
        _player = GameObject.Find("hero").transform;
        SetNewRandomTarget(); // Начальная цель
        StartCoroutine(MovementRoutine());
    }

    void Update()
    {
        bool isPlayerNear = Vector2.Distance(transform.position, _player.position) < 10f;

        if (isPlayerNear)
        {
            ChaseAndShoot();
        }
        else if (_isMoving)
        {
            MoveToTarget();
        }
    }

    IEnumerator MovementRoutine()
    {
        while (true)
        {
            if (!_isMoving)
            {
                yield return new WaitForSeconds(idleTime);
                SetNewRandomTarget();
                _isMoving = true;
            }
            yield return null;
        }
    }

    void SetNewRandomTarget()
    {
        // Генерация случайной точки в радиусе
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * wanderRadius;
        _targetPosition = transform.position + new Vector3(randomPoint.x, randomPoint.y, 0);
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPosition,
            speed * Time.deltaTime
        );

        // Проверка достижения цели
        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
        {
            _isMoving = false;
        }
    }

    void ChaseAndShoot()
    {
        _isMoving = false;
        Vector3 moveDirection = (_player.position - transform.position).normalized;
        transform.position += moveDirection * (speed * Time.deltaTime);

        _shootTimer -= Time.deltaTime;
        if (_shootTimer <= 0)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().isPlayerBullet = false;
            Vector2 direction = (_player.position - transform.position).normalized;
            bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bullet.GetComponent<Bullet>().speed;
            _shootTimer = shootCooldown;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            OnDeath?.Invoke(); // Вызываем событие при смерти
            Destroy(gameObject);
        }
    }
}
