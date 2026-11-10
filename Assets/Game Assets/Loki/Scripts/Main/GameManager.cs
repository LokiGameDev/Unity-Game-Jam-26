using System;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private SpawnPoint[] spawnPositions;
    [SerializeField] private CinemachineConfiner2D cinemachineConfiner;

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

    private void Awake()
    {
        if(instance == null) instance = this;
    }
    private void Start()
    {
        player = GameObject.Find("Player");

        TeleportPlayerToArea(0);
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
}

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