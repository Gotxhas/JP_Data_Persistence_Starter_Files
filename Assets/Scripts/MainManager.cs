using System.IO;
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
    public Image imagePanel;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
        InstantiateBricks();
    }

    private void InstantiateBricks()
    {
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

    private void UpdateUI()
    {
        if (DataManager.Instance != null)
        {
            currentPlayer = DataManager.Instance.currentPlayer;
            bestScore = DataManager.Instance.bestScore;
            bestPlayer = DataManager.Instance.bestPlayer;

            if (bestScore > 0 && currentPlayer != bestPlayer)
            {
                userWelcome.text = $"Welcome {currentPlayer}, you'll have to beat {bestPlayer} if you want to become the king of this game";
                bestScoreText.text = $"Best Score: {bestPlayer}: {bestScore}";
                ImageRandomColor(imagePanel, Color.red);
            }

            else if (bestScore > 0 && currentPlayer == bestPlayer)
            {
                userWelcome.text = $"Nicely done {currentPlayer}, you are our best player";
                bestScoreText.text = $"Best Score: {bestPlayer}: {bestScore}";
                ImageRandomColor(imagePanel, Color.blue);
            }

            else
            {
                userWelcome.text = $"Welcome {currentPlayer}, let's see what you can do";
                bestScoreText.text = $"Best Score: {bestPlayer}: {bestScore}";
                ImageRandomColor(imagePanel, Color.cyan);
            }

        }
    }

    public void ImageRandomColor(Image imagePanel, Color mycolor)
    {
        imagePanel.color = mycolor;
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
        //Checks if the current score is higher than bestscore. If higher, save data.
        if (m_Points > bestScore)
        {
            DataManager.Instance.bestScore = m_Points;
            DataManager.Instance.bestPlayer = currentPlayer;
            bestScoreText.text = $"Best Score: {currentPlayer}: {m_Points}";
            SaveData();
        }

        return;
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void SaveData()
    {
        SaveData data = new SaveData();

        data.bestScore = DataManager.Instance.bestScore;
        data.bestPlayer = DataManager.Instance.bestPlayer;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }
}
