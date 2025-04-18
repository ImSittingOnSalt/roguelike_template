using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Настройки комнаты")]
    public int maxEnemiesInRoom = 5; // Общий лимит врагов
    public int minWaveSize = 1;      // Минимальное количество врагов в волне
    public int maxWaveSize = 4;      // Максимальное количество врагов в волне
    public float waveDelay = 2f;     // Задержка между волнами

    [Header("Префабы и точки")]
    public GameObject[] enemyPrefabs; // Префабы врагов
    public Transform[] spawnPoints;   // Точки спавна
    public GameObject doorsParent; // Родительский объект со всеми дверями
    public GameObject[] doors;

    private List<GameObject> _currentEnemies = new List<GameObject>();
    private List<GameObject> _exitDoors = new List<GameObject>();
    private int _totalSpawnedEnemies;
    private bool _isActive;

    void Start()
    {
        // Получаем точки спавна из дочерних объектов комнаты
        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }

        // Получаем все двери из родительского объекта
        if (doorsParent != null)
        {
            foreach (Transform child in doorsParent.transform)
            {
                _exitDoors.Add(child.gameObject);
                child.gameObject.SetActive(false); // Изначально двери выключены
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isActive)
        {
            _isActive = true;
            SetDoorsState(true); // Закрыть двери
            StartCoroutine(SpawnWaves());
        }
    }

    IEnumerator SpawnWaves()
    {
        while (_totalSpawnedEnemies < maxEnemiesInRoom)
        {
            int waveSize = Mathf.Min(
                Random.Range(minWaveSize, maxWaveSize + 1),
                maxEnemiesInRoom - _totalSpawnedEnemies
            );

            SpawnWave(waveSize);
            _totalSpawnedEnemies += waveSize;

            yield return new WaitUntil(() => _currentEnemies.Count == 0);
            yield return new WaitForSeconds(waveDelay);
        }

        SetDoorsState(false); // Открыть двери после всех волн
    }

    void SpawnWave(int waveSize)
    {
        for (int i = 0; i < waveSize; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            _currentEnemies.Add(enemy);
            enemy.GetComponent<Enemy>().OnDeath += () => RemoveEnemy(enemy);
        }
    }

    void RemoveEnemy(GameObject enemy)
    {
        _currentEnemies.Remove(enemy);
        Destroy(enemy);

        // Если врагов не осталось и волны закончились - открыть двери
        if (_currentEnemies.Count == 0 && _totalSpawnedEnemies >= maxEnemiesInRoom)
        {
            SetDoorsState(false);
        }
    }

    void SetDoorsState(bool isClosed)
    {
        foreach (GameObject door in _exitDoors)
        {
            door.SetActive(isClosed);
        }
    }
}
