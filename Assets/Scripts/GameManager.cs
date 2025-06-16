using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float Timer { get; private set; }
    public int Score { get; private set; }
    public bool IsGameFinished { get; private set; }
    public bool IsFailed { get; private set; }

    private const float MaxTime = 60f;
    private const int MaxScore = 1000;

    [SerializeField] private Text scoreText; // Assign in Inspector

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreUI();
    }

    void Update()
    {
        if (!IsGameFinished && !IsFailed)
        {
            Timer += Time.deltaTime;
            if (Timer > MaxTime)
            {
                IsFailed = true;
                Score = 0;
                Debug.Log("Failed: Time's up!");
                UpdateScoreUI();
                // You can trigger fail UI here
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
            UpdateScoreUI();
            // You can trigger win UI here
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

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + Score;
        }
    }
}