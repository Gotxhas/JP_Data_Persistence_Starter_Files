using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text bestScoreText;
    public Text userWelcome;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;
    private string currentPlayer;
    private string bestPlayer;

    public bool m_GameOver = false;
    public int bestScore;


    // Start is called before the first frame update
    void Start()
    {
        //LoadBestScore();

        if (DataManager.Instance != null)
        {
            currentPlayer = DataManager.Instance.currentPlayer;
            userWelcome.text = $"Welcome {currentPlayer}";
        }

        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }


    public void BestScore()
    {
        if (m_Points > bestScore)
        {
            bestScore = m_Points;
            bestPlayer = DataManager.Instance.currentPlayer;
            bestScoreText.text = $"Best Score: {bestScore}: {bestPlayer}";
        }
    }


    public void SaveData()
    {

        SaveData data = new SaveData();

        data.currentPlayer = currentPlayer;
        data.bestScore = bestScore;
        data.bestPlayer = bestPlayer;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }


    public void LoadBestScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";


        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            currentPlayer = data.bestPlayer;
        }

        bestScoreText.text = $"Best Score: {bestScore}: {currentPlayer}";
    }
}
