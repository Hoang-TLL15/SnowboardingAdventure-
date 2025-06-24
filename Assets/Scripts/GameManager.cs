using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float Timer { get; private set; }
    // Ensure the Score property has a public setter
    public int Score { get; set; }
    public bool IsGameFinished { get; private set; }
    public bool IsFailed { get; private set; }
    public int HighScore { get; private set; }

    private const float MaxTime = 60f;
    private const int MaxScore = 1000;

    [SerializeField] private Text scoreText;         // Shows current score during play
    [SerializeField] private Text highScoreText;     // Shows high score
    [SerializeField] private Text finalScoreText;    // Shows final score after finish
    [SerializeField] private Text timerText;         // Shows countdown timer
    [SerializeField] private GameObject endGamePanel; // Assign in Inspector, shown on finish/fail

    [Header("Audio")]
    [SerializeField] private AudioSource backgroundMusic; // Assign in Inspector or via code

    private string HighScoreFilePath => Path.Combine(Application.persistentDataPath, $"highscore_{GetStageName()}.json");

    [System.Serializable]
    private class HighScoreData
    {
        public int highScore;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Ensure background music persists and only one instance plays
            if (backgroundMusic != null)
            {
                backgroundMusic.loop = true;
            }
        }
        else
        {
            // Destroy duplicate GameManager and its background music
            if (Instance.backgroundMusic != null && backgroundMusic != null && backgroundMusic != Instance.backgroundMusic)
            {
                Destroy(backgroundMusic.gameObject);
            }
            // Copy UI references from the old instance before destroying
            CopyUIReferences(Instance);
            Destroy(Instance.gameObject);
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // Check PlayerPrefs for sound state (set by Menu)
        if (backgroundMusic != null)
        {
            bool soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
            if (soundOn)
            {
                if (!backgroundMusic.isPlaying)
                    backgroundMusic.Play();
            }
            else
            {
                backgroundMusic.Stop();
            }
        }

        Time.timeScale = 1f; // Ensure normal time scale at game start

        LoadHighScore();
        ResetGameState();
        UpdateScoreUI();
        UpdateTimerUI();
        if (finalScoreText != null)
            finalScoreText.gameObject.SetActive(false);
        if (endGamePanel != null)
            endGamePanel.SetActive(false);
    }

    private void CopyUIReferences(GameManager oldInstance)
    {
        // Copy UI references from old instance if current ones are missing
        if (scoreText == null && oldInstance.scoreText != null)
            scoreText = oldInstance.scoreText;
        if (highScoreText == null && oldInstance.highScoreText != null)
            highScoreText = oldInstance.highScoreText;
        if (finalScoreText == null && oldInstance.finalScoreText != null)
            finalScoreText = oldInstance.finalScoreText;
        if (timerText == null && oldInstance.timerText != null)
            timerText = oldInstance.timerText;
        if (endGamePanel == null && oldInstance.endGamePanel != null)
            endGamePanel = oldInstance.endGamePanel;
    }

    private void ResetGameState()
    {
        Timer = 0f;
        Score = 0;
        IsGameFinished = false;
        IsFailed = false;
    }

    void Update()
    {
        if (!IsGameFinished && !IsFailed)
        {
            Timer += Time.deltaTime;
            UpdateTimerUI();

            if (Timer > MaxTime)
            {
                IsFailed = true;
                Score = 0;
                Debug.Log("Failed: Time's up!");
                UpdateScoreUI();
                ShowFinalScore();
                ShowEndGamePanel();
            }
        }
    }

    public void Finish()
    {
        if (!IsGameFinished && !IsFailed)
        {
            IsGameFinished = true;
            float timeUsed = Mathf.Min(Timer, MaxTime);
            Score = Mathf.RoundToInt(MaxScore * (1f - (timeUsed / MaxTime)));
            if (Score < 0) Score = 0;
            Debug.Log($"Finish! Score: {Score}");
            if (Score > HighScore)
            {
                HighScore = Score;
                SaveHighScore();
            }
            UpdateScoreUI();
            ShowFinalScore();
        }
    }

    public void AddLapPoints()
    {
        if (IsGameFinished || IsFailed)
            return;

        Score += 100;
        Debug.Log($"Lap completed! Score: {Score}");
        UpdateScoreUI();
    }

    // New function to return to menu (scene 0)
    public void ReturnToMenu()
    {
        Debug.Log("Returning to menu...");
        // Destroy the GameManager instance since we're going back to menu
        if (Instance == this)
        {
            Instance = null;
        }
        backgroundMusic.Stop();
        SceneManager.LoadScene(0);
    }

    // New function to replay the current level
    public void ReplayLevel()
    {
        Debug.Log("Restarting level...");
        // Reset time scale before reloading
        Time.timeScale = 1f;

        // Stop the music before reloading
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        // Get current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reset game state but keep the GameManager instance
        ResetGameState();

        // Reload the current scene
        SceneManager.LoadScene(currentSceneIndex);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Score;
        }
        if (highScoreText != null)
        {
            highScoreText.text = "" + HighScore;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            float timeLeft = Mathf.Max(0f, MaxTime - Timer);
            timerText.text = "Time: " + timeLeft.ToString("F1");
        }
    }

    private void ShowFinalScore()
    {
        if (finalScoreText != null)
        {
            finalScoreText.text = "" + Score;
            finalScoreText.gameObject.SetActive(true);
        }
    }

    private void ShowEndGamePanel()
    {
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
        }
    }

    public void ShowEndGamePanelPublic()
    {
        ShowFinalScore();
        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);
        }
    }

    private void SaveHighScore()
    {
        var data = new HighScoreData { highScore = HighScore };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(HighScoreFilePath, json);
    }

    private void LoadHighScore()
    {
        if (File.Exists(HighScoreFilePath))
        {
            string json = File.ReadAllText(HighScoreFilePath);
            var data = JsonUtility.FromJson<HighScoreData>(json);
            HighScore = data.highScore;
        }
        else
        {
            HighScore = 0;
        }
    }

    // Helper to get a unique stage name, can be replaced with your own logic
    private string GetStageName()
    {
        // Example: use the current scene name as the stage identifier
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }
}