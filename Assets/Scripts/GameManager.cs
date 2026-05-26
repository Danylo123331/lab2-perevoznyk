using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public Text scoreText;
    public Slider energySlider;
    public GameObject gameOverPanel;
    public Text overheatText;
    public Image overheatOverlay;

    private int score = 0;
    private bool isGameOver = false;
    private bool isOverheatedState = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (overheatText != null) overheatText.gameObject.SetActive(false);

        // Зробимо тло повністю прозорим, але не вимкненим
        if (overheatOverlay != null) overheatOverlay.color = new Color(1f, 0f, 0f, 0f);

        UpdateScoreUI();
    }

    void Update()
    {
        if (isOverheatedState && !isGameOver)
        {
            float t = Time.unscaledTime * 5f;
            float alpha = Mathf.PingPong(t, 1f);

            if (overheatOverlay != null)
            {
                // Мигання тла (макс прозорість 0.3)
                overheatOverlay.color = new Color(1f, 0f, 0f, alpha * 0.3f);
            }

            if (overheatText != null)
            {
                // Мигання тексту (макс прозорість 1)
                overheatText.color = new Color(1f, 0f, 0f, alpha);
            }
        }
    }

    public void AddScore()
    {
        if (isGameOver) return;
        score++;
        UpdateScoreUI();
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
            // Після остигання повертаємо тло в нульову прозорість
            if (overheatOverlay != null) overheatOverlay.color = new Color(1f, 0f, 0f, 0f);
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "SCORE: " + score;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        isOverheatedState = false;

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (overheatText != null) overheatText.gameObject.SetActive(false);

        // Прибираємо тло в момент смерті
        if (overheatOverlay != null) overheatOverlay.color = new Color(1f, 0f, 0f, 0f);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}