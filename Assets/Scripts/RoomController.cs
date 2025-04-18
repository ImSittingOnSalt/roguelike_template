using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("��������� �������")]
    public int maxEnemiesInRoom = 5; // ����� ����� ������
    public int minWaveSize = 1;      // ����������� ���������� ������ � �����
    public int maxWaveSize = 4;      // ������������ ���������� ������ � �����
    public float waveDelay = 2f;     // �������� ����� �������

    [Header("������� � �����")]
    public GameObject[] enemyPrefabs; // ������� ������
    public Transform[] spawnPoints;   // ����� ������
    public GameObject doorsParent; // ������������ ������ �� ����� �������
    public GameObject[] doors;

    private List<GameObject> _currentEnemies = new List<GameObject>();
    private List<GameObject> _exitDoors = new List<GameObject>();
    private int _totalSpawnedEnemies;
    private bool _isActive;

    void Start()
    {
        // �������� ����� ������ �� �������� �������� �������
        spawnPoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            spawnPoints[i] = transform.GetChild(i);
        }

        // �������� ��� ����� �� ������������� �������
        if (doorsParent != null)
        {
            foreach (Transform child in doorsParent.transform)
            {
                _exitDoors.Add(child.gameObject);
                child.gameObject.SetActive(false); // ���������� ����� ���������
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isActive)
        {
            _isActive = true;
            SetDoorsState(true); // ������� �����
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

        SetDoorsState(false); // ������� ����� ����� ���� ����
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

        // ���� ������ �� �������� � ����� ����������� - ������� �����
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
