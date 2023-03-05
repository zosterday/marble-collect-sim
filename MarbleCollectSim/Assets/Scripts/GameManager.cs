using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: Add object pooling for gems and powerups to optimize when all done

//Need to figure out how to connect the leaderboard entries to actual marble
//want a copy of the gameobject so might just have a dictionary mapping the marble to the marble that is the entry in the leaderboard

public class GameManager : MonoBehaviour
{
    private const string GroundTag = "Ground";

    private const float XSpawnBound = 13f;

    private const float ZSpawnBound = 13f;

    private const float MarbleRadius = 0.5f;

    private const int PowerUpSpawnCount = 2;

    private const int GemMinSpawnCount = 3;

    private const int GemMaxSpawnCount = 5;

    [SerializeField]
    private List<Color> colors;
    
    [SerializeField]
    private List<Texture> textures;

    [SerializeField]
    private GameObject endGamePanel;

    private int activeMarbleCount;

    public static GameManager Instance
    {
        get
        {
            if (instance is null)
            {
                throw new System.InvalidOperationException("Instance of GameManager is null");
            }

            return instance;
        }
    }

    private static GameManager instance;

    public bool IsSimActive { get; private set; }

    [SerializeField]
    private GameObject marblePrefab;

    [SerializeField]
    private GameObject gemPrefab;

    [SerializeField]
    private List<GameObject> powerUpPrefabs;

    private readonly List<GameObject> marbles = new();

    private readonly List<ColorTexturePair> colorTexturePairs = new();

    private void Awake()
    {
        IsSimActive = false;
        instance = this;

        CreateColorTexturePairs();
        SpawnMarbles();
    }

    // Start is called before the first frame update
    void Start()
    {
        IsSimActive = true;
        InvokeRepeating(nameof(SpawnGems), 0f, 5f);
        InvokeRepeating(nameof(SpawnPowerUps), 1f, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsSimActive)
        {
            return;
        }

        //TODO: Make this time based instead, there is no win condition 
        if (activeMarbleCount == 1 && marbles.Count == 1)
        {
            IsSimActive = false;
            EndGame();
        }
    }

    private void CreateColorTexturePairs()
    {
        foreach (var color in colors)
        {
            foreach (var texture in textures)
            {
                var colorTexturePair = new ColorTexturePair()
                {
                    Color = color,
                    Texture = texture,
                };
                colorTexturePairs.Add(colorTexturePair);
            }
        }
    }

    private void SpawnMarbles()
    {
        for (var i = 0; i < StateMachine.MarbleSpawnCount; i++)
        {
            var marbleSpawned = false;

            var tryCount = 0;

            while (!marbleSpawned && tryCount <= 20)
            {
                marbleSpawned = TrySpawnMarble();
                tryCount++;
            }
        }
    }

    private bool TrySpawnMarble()
    {
        if (!TrySpawn(out var spawnPos))
        {
            return false;
        }

        // Instantiate marble prefab
        var marble = Instantiate(marblePrefab, spawnPos, Quaternion.identity);
        var renderer = marble.GetComponent<Renderer>();

        var randIndex = Random.Range(0, colorTexturePairs.Count);
        var colorTexturePair = colorTexturePairs[randIndex];
        colorTexturePairs.RemoveAt(randIndex);
        renderer.material.color = colorTexturePair.Color;
        renderer.material.mainTexture = colorTexturePair.Texture;


        // Add marble to marbles list
        marbles.Add(marble);
        activeMarbleCount++;

        return true;
    }

    private void SpawnGems()
    {
        var gemSpawnCount = Random.Range(GemMinSpawnCount, GemMinSpawnCount);

        for (var i = 0; i < gemSpawnCount; i++)
        {
            var gemSpawned = false;

            var tryCount = 0;

            while (!gemSpawned && tryCount <= 20)
            {
                gemSpawned = TrySpawnGem();
                tryCount++;
            }
        }
    }

    private bool TrySpawnGem()
    {
        if (!TrySpawn(out var spawnPos))
        {
            return false;
        }

        Instantiate(gemPrefab, spawnPos, Quaternion.identity);
        
        return true;
    }

    private void SpawnPowerUps()
    {
        for (var i = 0; i < PowerUpSpawnCount; i++)
        {
            var powerUpSpawned = false;

            var tryCount = 0;

            while (!powerUpSpawned && tryCount <= 20)
            {
                powerUpSpawned = TrySpawnPowerUp();
                tryCount++;
            }
        }
    }

    private bool TrySpawnPowerUp()
    {
        if (!TrySpawn(out var spawnPos))
        {
            return false;
        }

        var randomIndex = Random.Range(0, powerUpPrefabs.Count);
        var pickupPrefab = powerUpPrefabs[randomIndex];
        Instantiate(pickupPrefab, spawnPos, Quaternion.identity);

        return true;
    }

    private bool TrySpawn(out Vector3 spawnPos)
    {
        // Generate Random Spawn Position
        var x = Random.Range(-XSpawnBound, XSpawnBound);
        var z = Random.Range(-ZSpawnBound, ZSpawnBound);
        spawnPos = new Vector3(x, 0.5f, z);

        // Check for collision
        var collisions = Physics.OverlapSphere(spawnPos, MarbleRadius);
        foreach (var collision in collisions)
        {
            if (!collision.CompareTag(GroundTag))
            {
                return false;
            }
        }

        return true;
    }

    public void RemoveMarble(GameObject gameObject)
    {
        gameObject.SetActive(false);
        marbles.Remove(gameObject);
        activeMarbleCount--;
    }

    private void EndGame()
    {
        endGamePanel.SetActive(true);
        CancelInvoke();

        //TODO: Figure out display winner thing based on leaderboard entry
        marbles[0].GetComponent<Marble>().DisplayWinner();
    }

    private struct ColorTexturePair
    {
        public Color Color;

        public Texture Texture;
    }
}
