using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static GameManager Instance;

    [Header("UI Elements")]
    public Text scoreText;
    public Text bestScoreText;
    public Text coinText;
    public Slider energySlider;
    public GameObject gameOverPanel;
    public Text overheatText;
    public Image overheatOverlay;

    [Header("Audio Clips")]
    public AudioSource backgroundMusic;
    public AudioClip hitSound;
    public AudioClip gameOverSound;
    public AudioClip coinSound;

    [Header("Difficulty Settings")]
    public float gameSpeedMultiplier = 1f;
    public float speedIncreaseRate = 0.015f;
    public float maxSpeedMultiplier = 2.5f;

    private int score = 0;
    private int coins = 0;
    private int bestScore = 0;
    private bool isGameOver = false;
    private bool isOverheatedState = false;

    void Awake()
    {
        instance = this;
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (overheatText != null) overheatText.gameObject.SetActive(false);
        if (overheatOverlay != null) overheatOverlay.color = new Color(1f, 0f, 0f, 0f);
        UpdateUI();
    }

    void Update()
    {
        if (!isGameOver)
        {
            gameSpeedMultiplier += speedIncreaseRate * Time.deltaTime;
            if (gameSpeedMultiplier > maxSpeedMultiplier)
            {
                gameSpeedMultiplier = maxSpeedMultiplier;
            }
        }

        if (isOverheatedState && !isGameOver)
        {
            float t = Time.unscaledTime * 5f;
            float alpha = Mathf.PingPong(t, 1f);

            if (overheatOverlay != null)
            {
                overheatOverlay.color = new Color(1f, 0f, 0f, alpha * 0.3f);
            }

            if (overheatText != null)
            {
                overheatText.color = new Color(1f, 0f, 0f, alpha);
            }
        }
    }

    public void AddScore()
    {
        if (isGameOver) return;
        score++;
        UpdateUI();
    }

    public void AddCoin()
    {
        if (isGameOver) return;
        coins++;
        if (coinSound != null)
        {
            AudioSource.PlayClipAtPoint(coinSound, Camera.main.transform.position);
        }
        UpdateUI();
    }

    public void UpdateEnergyUI(float current, float max, bool isOverheated)
    {
        if (isGameOver) return;

        if (energySlider != null)
        {
            energySlider.maxValue = max;
            energySlider.value = current;
        }

        isOverheatedState = isOverheated;

        if (overheatText != null)
        {
            overheatText.gameObject.SetActive(isOverheated);
        }

        if (!isOverheated)
        {
            if (overheatOverlay != null) overheatOverlay.color = new Color(1f, 0f, 0f, 0f);
        }
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "SCORE: " + score;
        if (bestScoreText != null) bestScoreText.text = "BEST: " + bestScore;
        if (coinText != null) coinText.text = "COINS: " + coins;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        isOverheatedState = false;

        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            AudioSource[] playerAudios = player.GetComponents<AudioSource>();
            foreach (AudioSource audio in playerAudios)
            {
                audio.Stop();
            }
        }

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("BestScore", bestScore);
            PlayerPrefs.Save();
        }

        if (hitSound != null) AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position);
        if (gameOverSound != null) AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position);

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (overheatText != null) overheatText.gameObject.SetActive(false);
        if (overheatOverlay != null) overheatOverlay.color = new Color(1f, 0f, 0f, 0f);
        UpdateUI();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}