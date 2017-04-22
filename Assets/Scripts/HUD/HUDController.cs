using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public Text GameOverText;
    public Slider HealthText;
    public Text ScoreText;
    public Text WaveText;
    public float showWaveTime;
    public Text HUDWaveText;

    public bool IsGameOver { get; private set; }

    private int score;
    private bool doShowWave;
    private float waveTime;

    void Start()
    {
        score = 0;
        IsGameOver = false;

        if (GameOverText != null)
            GameOverText.text = string.Empty;

        if (ScoreText != null)
            ScoreText.text = "Score: " + score;

        if (WaveText != null)
            WaveText.text = "Wave 1";

        if (HUDWaveText != null)
            HUDWaveText.text = "Wave 1";

        waveTime = 0;
        doShowWave = true;
    }

    void Update()
    {
        if (doShowWave && (Time.time - waveTime) > showWaveTime)
        {
            doShowWave = false;
            WaveText.text = string.Empty;
        }
    }

    public void GameOver()
    {
        IsGameOver = true;

        // show game over text
        if (GameOverText != null)
            GameOverText.text = "Game Over";

        // destroy all enemies and spawners
        var enemyManager = Object.FindObjectOfType<EnemySpawnManager>();
        Destroy(enemyManager);
    }

    public void UpdateHealth(float health)
    {
        HealthText.value = health;
    }

    public void AddScore(int score)
    {
        this.score += score;
        ScoreText.text = "Score: " + this.score;
    }

    public void ShowWave(int wave)
    {
        if (WaveText != null)
            WaveText.text = "Wave " + wave;

        if (HUDWaveText != null)
            HUDWaveText.text = "Wave " + wave;

        doShowWave = true;
        waveTime = Time.time;
    }
}
