using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float xSpawnBound = 7f;

    private const float zSpawnBound = 8.7f;

    private const float MarbleRadius = 0.5f;

    private const string BarrierTag = "Barrier";

    private const float BarrierRemovalInterval = 1f;

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

    private bool isSimActive;

    [SerializeField]
    private GameObject marblePrefab;

    private List<GameObject> barriers;

    private readonly List<GameObject> marbles = new();

    private readonly List<ColorTexturePair> colorTexturePairs = new();

    private void Awake()
    {
        isSimActive = false;
        instance = this;

        barriers = GameObject.FindGameObjectsWithTag(BarrierTag).ToList();

        CreateColorTexturePairs();
        SpawnMarbles();
    }

    // Start is called before the first frame update
    void Start()
    {
        isSimActive = true;
        InvokeRepeating(nameof(RemoveBarrier), 1f, BarrierRemovalInterval);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSimActive)
        {
            return;
        }

        if (activeMarbleCount == 1 && marbles.Count == 1)
        {
            isSimActive = false;
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
        // Generate Random Spawn Position
        var x = Random.Range(-xSpawnBound, xSpawnBound);
        var z = Random.Range(-zSpawnBound, zSpawnBound);
        var spawnPos = new Vector3(x, 0, z);

        // Check for collision
        var collision = Physics.OverlapSphere(spawnPos, MarbleRadius);
        if (collision.Length != 0)
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

    private void RemoveBarrier()
    {
        if (barriers.Count <= 0)
        {
            return;
        }

        var index = Random.Range(0, barriers.Count);

        barriers[index].SetActive(false);
        barriers.RemoveAt(index);
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
        marbles[0].GetComponent<Marble>().DisplayWinner();
    }

    private struct ColorTexturePair
    {
        public Color Color;

        public Texture Texture;
    }
}
