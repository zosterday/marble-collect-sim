using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

//TODO: Add object pooling for gems and powerups to optimize when all done

//TODOs:
//when I instantiate the leaderboardIcons, they need to be setactive(false)
//Add Leaderboard
//Do end game logic
//Do the end game winner panel
//Add object pooling for pickups
//Clean up code, make general helper functions when possible. Eliminate duplicate code
//Need to figure out how to connect the leaderboard entries to actual marble
//want a copy of the gameobject so might just have a dictionary mapping the marble to the marble that is the entry in the leaderboard

public class GameManager : MonoBehaviour
{
    private const string GroundTag = "Ground";

    private const float XSpawnBound = 13f;

    private const float ZSpawnBound = 13f;

    private const float MarbleRadius = 0.5f;

    private const int PowerUpSpawnCount = 3;

    private const int GemMinSpawnCount = 4;

    private const int GemMaxSpawnCount = 8;

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

    public bool IsSimActive { get; private set; }

    private static GameManager instance;

    private float countdownTimer = 60f;

    [SerializeField]
    private GameObject marblePrefab;

    [SerializeField]
    private GameObject leaderboardIconPrefab;

    [SerializeField]
    private GameObject gemPrefab;

    [SerializeField]
    private List<GameObject> powerUpPrefabs;

    [SerializeField]
    private List<Color> colors;

    [SerializeField]
    private List<Texture> textures;

    [SerializeField]
    private GameObject endGamePanel;

    [SerializeField]
    private TextMeshProUGUI countdownTimerText;

    [SerializeField]
    private List<TextMeshProUGUI> leaderboardTexts;

    [SerializeField]
    private Canvas canvas;

    private readonly List<GameObject> marbles = new();

    private readonly List<GameObject> leaderboardIcons = new();

    private readonly List<ColorTexturePair> colorTexturePairs = new();

    private readonly Dictionary<GameObject, GameObject> leaderboardIconsByMarbles = new();

    private void Awake()
    {
        IsSimActive = false;
        instance = this;

        CreateColorTexturePairs();
        SpawnMarbles();

        IsSimActive = true;
    }

    void Start()
    {
        InvokeRepeating(nameof(SpawnGems), 0f, 3f);
        InvokeRepeating(nameof(SpawnPowerUps), 1f, 4f);

        UpdateLeaderboard();
    }

    void Update()
    {
        if (!IsSimActive)
        {
            return;
        }

        if (countdownTimer <= 0f)
        {
            IsSimActive = false;
            EndGame();
        }

        countdownTimer -= Time.deltaTime;
        countdownTimerText.text = countdownTimer.ToString();
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

        // Instantiate and set up corresponding leaderboard icon
        var leaderboardIcon = Instantiate(leaderboardIconPrefab, Vector3.zero, Quaternion.identity);

        // Set parent to be leaderboardPanel
        leaderboardIcon.transform.SetParent(canvas.transform);

        var iconRenderer = leaderboardIcon.GetComponent<Renderer>();
        iconRenderer.material.color = colorTexturePair.Color;
        iconRenderer.material.mainTexture = colorTexturePair.Texture;

        leaderboardIcons.Add(leaderboardIcon);
        leaderboardIconsByMarbles.Add(marble, leaderboardIcon);

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

    public void UpdateGemCount(GameObject gameObject, int updatedGemCount)
    {
        if (!leaderboardIconsByMarbles.TryGetValue(gameObject, out var leaderboardEntryObj))
        {
            Debug.Log("Could not get leaderboard entry corresponding given marble gameobject.");
        }

        var leaderboardIcon = leaderboardEntryObj.GetComponent<LeaderboardIcon>();
        leaderboardIcon.GemCount = updatedGemCount;

        UpdateLeaderboard();
    }

    private void UpdateLeaderboard()
    {
        leaderboardIcons.Sort((g1, g2) => g2.GetComponent<LeaderboardIcon>().GemCount - g1.GetComponent<LeaderboardIcon>().GemCount);

        var first = leaderboardIcons[0].GetComponent<LeaderboardIcon>();
        var second = leaderboardIcons[1].GetComponent<LeaderboardIcon>();
        var third = leaderboardIcons[2].GetComponent<LeaderboardIcon>();
        var fourth = leaderboardIcons[3].GetComponent<LeaderboardIcon>();
        var fifth = leaderboardIcons[4].GetComponent<LeaderboardIcon>();

        leaderboardTexts[0].text = $"1st: {first.GemCount}";
        leaderboardTexts[1].text = $"2nd: {second.GemCount}";
        leaderboardTexts[2].text = $"3rd: {third.GemCount}";
        leaderboardTexts[3].text = $"4th: {fourth.GemCount}";
        leaderboardTexts[4].text = $"5th: {fifth.GemCount}";

        foreach (var iconObj in leaderboardIcons)
        {
            iconObj.SetActive(false);
        }

        first.SetLeaderboardPosition(1);
        second.SetLeaderboardPosition(2);
        third.SetLeaderboardPosition(3);
        fourth.SetLeaderboardPosition(4);
        fifth.SetLeaderboardPosition(5);
    }

    private void EndGame()
    {
        endGamePanel.SetActive(true);
        CancelInvoke();

        leaderboardIcons[0].GetComponent<LeaderboardIcon>().DisplayWinner();
    }

    private struct ColorTexturePair
    {
        public Color Color;

        public Texture Texture;
    }
}
