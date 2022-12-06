using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Lean.Gui;

public class GameController : MonoBehaviour
{

    // UI elements
    public Text scoreLabel;
    public Text timeLabel;
    public Text promptLabel;
    public GameObject promptPanel;
    public Text resultLabel;
    public LeanButton menuButton;
    public LeanButton replayButton;
    public GameObject panel;

    // Scene objects
    public MeshRenderer parkAreaMeshRenderer;
    public Material whiteParkMaterial;
    public Material greenParkMaterial;
    public ParticleSystem particleSystems1;
    public ParticleSystem particleSystems2;

    private CarController carController;

    // Game logic
    public int parkTimes = 0;
    public int totalParkTimes;
    float shakeTime = 0f;
    public float timeRemaining;
    public bool isCarInParkArea = false;
    public GameState state = GameState.BeforeMove;
    public enum GameState
    {
        BeforeStart,
        BeforeMove,
        Started,
        CarStopped,
        Won, 
        Lost
    }

    public void StartGame()
    {
        state = GameState.Started;
        promptLabel.text = "Press R to Respawn";
        promptLabel.gameObject.SetActive(false);
        promptPanel.gameObject.SetActive(false);
    }

    public void CarStartMove()
    {
        state = GameState.Started;
        promptLabel.gameObject.SetActive(false);
        promptPanel.gameObject.SetActive(false);
    }
    
    public void CarStopped()
    {
        state = GameState.CarStopped;
        promptLabel.text = "Press R to Respawn";
        promptLabel.gameObject.SetActive(true);
        promptPanel.gameObject.SetActive(true);
    }

    public void CarParked()
    {
        if (!isCarInParkArea)
        {
            isCarInParkArea = true;
            parkTimes++;
            scoreLabel.text = "Parked: " + parkTimes + "/" + totalParkTimes;
            if (parkTimes == totalParkTimes)
            {
                GameWon();
            }
        }
    }

    public void GameWon()
    {
        state = GameState.Won;
        promptLabel.gameObject.SetActive(false);
        promptPanel.gameObject.SetActive(false);
        resultLabel.text = "You Won!";
        resultLabel.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        replayButton.gameObject.SetActive(true);
        panel.SetActive(true);

        particleSystems1.Play();
        particleSystems2.Play();

        parkAreaMeshRenderer.material = greenParkMaterial;
    }

    public void GameLost()
    {
        state = GameState.Lost;
        promptLabel.gameObject.SetActive(false);
        promptPanel.gameObject.SetActive(false);
        resultLabel.text = "Time's up, You Lost!";
        resultLabel.color = Color.red;
        resultLabel.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);
        replayButton.gameObject.SetActive(true);
        panel.SetActive(true);
    }

    void DisplayTime(float timeToDisplay)
    {
        // timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timeLabel.text = "Time Left: " + string.Format("{0:00}:{1:00}", minutes, seconds);
        if (timeToDisplay < 10)
        {
            if (seconds % 2 == 0)
            {
                timeLabel.color = Color.red;
            }
            else
            {
                timeLabel.color = Color.white;
            }
        }
    }
    
    void Start()
    {
        carController = GameObject.Find("TankFree_Red").GetComponent<CarController>();

        Debug.Log("GameController Start");

        menuButton.OnClick.AddListener(() => {
            Debug.Log("MenuButton Clicked");
            SceneManager.LoadScene("MenuScene");
        });
        replayButton.OnClick.AddListener(() => {
            Debug.Log("ReplayButton Clicked");
            SceneManager.LoadScene("Level1");
        });

        
        resultLabel.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        replayButton.gameObject.SetActive(false);
        panel.SetActive(false);

        scoreLabel.text = "Parked: " + parkTimes + "/" + totalParkTimes;
        DisplayTime(timeRemaining);
        
        promptLabel.text = "Press W to Start";
        promptLabel.gameObject.SetActive(true);
        promptPanel.gameObject.SetActive(true);

        particleSystems1.Stop();
        particleSystems2.Stop();
    }

    void Update()
    {
        if (timeRemaining > 0 && state != GameState.BeforeStart && state != GameState.Won && state != GameState.Lost)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else if (timeRemaining <= 0)
        {
            timeLabel.text = "Time Left: 00:00";
            GameLost();
        }

        if (state == GameState.Started || state == GameState.CarStopped)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                carController.ResetPosition();
                promptLabel.gameObject.SetActive(false);
                promptPanel.gameObject.SetActive(false);
                state = GameState.BeforeMove;
            }
        }

        shakeTime += Time.deltaTime;
        if (!(state == GameState.Won || state == GameState.Lost))
        {
            if (shakeTime % 1 > 0.8f)
            {
                parkAreaMeshRenderer.material = whiteParkMaterial;
            }
            else
            {
                parkAreaMeshRenderer.material = greenParkMaterial;
            }
        }
    }
}
