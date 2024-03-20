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

    private const int DefaultMarbleSpawnCount = 12;

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

        if (marbleSpawnCount < 1)
        {
            marbleSpawnCount = DefaultMarbleSpawnCount;
        }

        StateMachine.MarbleSpawnCount = marbleSpawnCount;

        SceneManager.LoadScene(SimulationScene);
    }
}
