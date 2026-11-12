using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnPoint[] spawnPositions;
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner;
    [SerializeField] private SpawnAreaPoints playerSpawnPoint = SpawnAreaPoints.VillageEntrance;
    [SerializeField] private PuzzleStatus[] puzzleStatuses;

    private GameObject player;

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if(instance == null)
            {
                Debug.LogError("Game Manager is null");
            }
            return instance;
        }
    }

    [Serializable]
    public class SpawnPoint
    {
        public SpawnAreaPoints name;
        public Transform transform;
        public BoxCollider2D collider;
    }

    [Serializable]
    public class PuzzleStatus
    {
        public PuzzleType PuzzleType;
        public bool SolvedStatus = false;
    }

    private void Awake()
    {
        if(instance == null) instance = this;
    }
    private void Start()
    {
        player = GameObject.Find("Player");

        TeleportPlayerToArea(playerSpawnPoint);
    }

    public void TeleportPlayerToArea(SpawnAreaPoints spawnArea)
    {
        SpawnPoint spawnPoint = GetSpawnPoint(spawnArea);

        player.transform.position = spawnPoint.transform.position;
        cinemachineConfiner.BoundingShape2D = spawnPoint.collider;
    }

    private SpawnPoint GetSpawnPoint(SpawnAreaPoints spawnArea)
    {
        foreach(var item in spawnPositions)
        {
            if(item.name == spawnArea)
            {
                return item;
            }
        }
        return null;
    }

    public void SolvedPuzzle(PuzzleType puzzleType)
    {
        foreach(var item in puzzleStatuses)
        {
            if(item.PuzzleType == puzzleType)
            {
                item.SolvedStatus = true;
            }
        }
    }
}

public enum PuzzleType { GearBalance, MayorRecord, ClockTower}

public enum SpawnAreaPoints
{
    VillageEntrance,
    HotelEntrance,
    HotelGroundFloor,
    HotelFirstFloor,
    ClockTowerEntrance,
    ClockMakerRoomEntrance,
    ClockMakerRoom,
    MayorRoomEntrance,
    MayorRoom,
    PlayerRoomEntrance,
    PlayerRoom,
    ClockTowerInside
}