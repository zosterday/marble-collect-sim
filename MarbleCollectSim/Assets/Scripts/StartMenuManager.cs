using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    private const string SimulationScene = "SimScene";

    private const int DefaultMarbleSpawnCount = 50;

    [SerializeField]
    private Button startButton;

    [SerializeField]
    private TMP_InputField marbleSpawnCountInput;

    public void StartGame()
    {
        if (!int.TryParse(marbleSpawnCountInput.text, out var marbleSpawnCount))
        {
            marbleSpawnCount = DefaultMarbleSpawnCount;
        }

        marbleSpawnCount = Mathf.Min(marbleSpawnCount, DefaultMarbleSpawnCount);
        marbleSpawnCount = Mathf.Max(marbleSpawnCount, 1);

        StateMachine.MarbleSpawnCount = marbleSpawnCount;

        SceneManager.LoadScene(SimulationScene);
    }
}
