using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDController : MonoBehaviour
{
    public Text GameOverText;
    public Slider HealthText;
    public Text ScoreText;
    public Text WaveText;
    public float showWaveTime;
    public Text HUDWaveText;
    public Text HighScoreText;

    public bool IsGameOver { get; private set; }

    private int score;
    private int highscore;
    private bool doShowWave;
    private float waveTime;

    void Start()
    {
        // retrieve the highscore
        if (PlayerPrefs.HasKey("HighScore"))
            highscore = PlayerPrefs.GetInt("HighScore");

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

        if (HighScoreText != null)
            HighScoreText.text = "High Score: " + highscore;

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

        // did the player beat the highscore?
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("HighScore", highscore);
        }

        // show game over text
        if (GameOverText != null)
            GameOverText.text = "Game Over";

        // destroy all enemies and spawners
        var enemyManager = Object.FindObjectOfType<EnemySpawnManager>();
        Destroy(enemyManager);

        // go to start screen
        Invoke("ReturnToStartMenu", 2);
    }

    public void UpdateHealth(float health)
    {
        HealthText.value = health;
    }

    public void AddScore(int score)
    {
        this.score += score;

        // change high score if beaten
        if (this.score > highscore)
        {
            HighScoreText.text = "High Score: " + this.score;
        }
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

    public void ShowWave(string bossWave)
    {
        // Overloaded function to display the boss wave as string and not int.
        if (WaveText != null)
            WaveText.text = "BOSS";

        if (HUDWaveText != null)
            HUDWaveText.text = "BOSS";

        doShowWave = true;
        waveTime = Time.time;
    }

    private void ReturnToStartMenu()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
