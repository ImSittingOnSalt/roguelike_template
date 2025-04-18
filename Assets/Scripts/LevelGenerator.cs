using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("���������")]
    public int gridSize = 4;         // ������ ����� 4x4
    public GameObject[] roomPrefabs; // ������� ������ (0: ���1, 1: ���2 � �.�.)
    public GameObject startRoomPrefab;
    public GameObject endRoomPrefab;

    private int[,] _levelGrid;       // ������� ������
    private Vector2Int _startPos;
    private Vector2Int _endPos;
    private int _currentLevel = 1;

    void Start()
    {
        GenerateLevel(_currentLevel);
    }

    public void GenerateLevel(int level)
    {
        // ������������ ���������� ������
        int targetRooms = 8 + 2 * (level - 1);
        _levelGrid = new int[gridSize, gridSize];

        // 1. ��������� ��������� �������
        _startPos = new Vector2Int(0, Random.Range(0, gridSize));
        _levelGrid[_startPos.x, _startPos.y] = 1;

        // 2. �������� ������ � ������� ��� ��������� �������� ������
        GenerateConnectedRooms(_startPos, targetRooms);

        // 3. ��������� ��������� �������
        PlaceEndRoom();

        // 4. ������������� �������
        InstantiateRooms();
    }

    void GenerateConnectedRooms(Vector2Int start, int targetRooms)
    {
        List<Vector2Int> activeCells = new List<Vector2Int> { start };
        int createdRooms = 1;

        while (createdRooms < targetRooms)
        {
            if (activeCells.Count == 0)
            {
                // ���� �������� ������ ���, �������� ��������� ������������ �������
                List<Vector2Int> existingRooms = new List<Vector2Int>();
                for (int x = 0; x < gridSize; x++)
                {
                    for (int y = 0; y < gridSize; y++)
                    {
                        if (_levelGrid[x, y] == 1)
                        {
                            existingRooms.Add(new Vector2Int(x, y));
                        }
                    }
                }
                activeCells.Add(existingRooms[Random.Range(0, existingRooms.Count)]);
            }

            int index = Random.Range(0, activeCells.Count);
            Vector2Int current = activeCells[index];
            List<Vector2Int> neighbors = GetEmptyNeighbors(current);

            if (neighbors.Count > 0)
            {
                Vector2Int newRoom = neighbors[Random.Range(0, neighbors.Count)];
                _levelGrid[newRoom.x, newRoom.y] = 1;
                activeCells.Add(newRoom);
                createdRooms++;
            }
            else
            {
                activeCells.RemoveAt(index);
            }
        }
    }

    void PlaceEndRoom()
    {
        // ���� ����� ������� ����� �� ������
        Vector2Int farthest = _startPos;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (_levelGrid[x, y] == 1 &&
                    Vector2Int.Distance(new Vector2Int(x, y), _startPos) >
                    Vector2Int.Distance(farthest, _startPos))
                {
                    farthest = new Vector2Int(x, y);
                }
            }
        }
        _endPos = farthest;
    }

    void InstantiateRooms()
    {
        DestroyCurrentLevel();

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (_levelGrid[x, y] == 1)
                {
                    Vector3 pos = new Vector3(x * 20, y * 20, 0);
                    GameObject room;

                    if (new Vector2Int(x, y) == _startPos)
                        room = Instantiate(startRoomPrefab, pos, Quaternion.identity);
                    else if (new Vector2Int(x, y) == _endPos)
                        room = Instantiate(endRoomPrefab, pos, Quaternion.identity);
                    else
                        room = Instantiate(roomPrefabs[GetRoomType(x, y)], pos, Quaternion.identity);

                    // ��������� ������ ��� ���� ������
                    ConfigureRoomDoors(room.GetComponent<RoomController>(), x, y);
                }
            }
        }
    }

    int GetRoomType(int x, int y)
    {
        // ������ ����������� ���� ������� �� ���������� �������
        int neighbors = 0;
        if (x > 0 && _levelGrid[x - 1, y] == 1) neighbors++;
        if (x < gridSize - 1 && _levelGrid[x + 1, y] == 1) neighbors++;
        if (y > 0 && _levelGrid[x, y - 1] == 1) neighbors++;
        if (y < gridSize - 1 && _levelGrid[x, y + 1] == 1) neighbors++;
        return Mathf.Clamp(neighbors, 0, 3);
    }

    List<Vector2Int> GetEmptyNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        // ��������� ��� 4 �����������
        if (cell.x > 0 && _levelGrid[cell.x - 1, cell.y] == 0)
            neighbors.Add(new Vector2Int(cell.x - 1, cell.y));
        if (cell.x < gridSize - 1 && _levelGrid[cell.x + 1, cell.y] == 0)
            neighbors.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.y > 0 && _levelGrid[cell.x, cell.y - 1] == 0)
            neighbors.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.y < gridSize - 1 && _levelGrid[cell.x, cell.y + 1] == 0)
            neighbors.Add(new Vector2Int(cell.x, cell.y + 1));
        return neighbors;
    }

    void ConfigureRoomDoors(RoomController room, int x, int y)
    {
        // ���������� ����� ������ ���, ��� ���� ������
        room.doors[0].SetActive(y < gridSize - 1 && _levelGrid[x, y + 1] == 1); // ����
        room.doors[1].SetActive(x < gridSize - 1 && _levelGrid[x + 1, y] == 1); // �����
        room.doors[2].SetActive(y > 0 && _levelGrid[x, y - 1] == 1);            // ���
        room.doors[3].SetActive(x > 0 && _levelGrid[x - 1, y] == 1);            // ����
    }

    void DestroyCurrentLevel()
    {
        // �������� ���������� ����� �� FindObjectsByType � ��������� ������ ����������
        foreach (RoomController room in FindObjectsByType<RoomController>(FindObjectsSortMode.None))
        {
            Destroy(room.gameObject);
        }
    }

    public void LoadNextLevel()
    {
        _currentLevel++;
        DestroyCurrentLevel();
        GenerateLevel(_currentLevel);
        PlayerController.Instance.transform.position = Vector3.zero;
    }
}