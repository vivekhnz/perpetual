using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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
    private bool isFlashing = false; // Used for checking whether to blink WaveText.
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
        // Clear wave text after showWaveTime(s) if there is a WaveText on screen.
        // Does not deal with the blinking WaveText effect.
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

    // Overloaded function to display the boss wave as string and not int.
    // Superseeded by the FlashingWaveText function.
    public void ShowWave(string waveString)
    {
        if (WaveText != null)
            WaveText.text = waveString;

        if (HUDWaveText != null)
            HUDWaveText.text = waveString;

        doShowWave = true;
        waveTime = Time.time;
    }

    // Flashes the WaveText.
    public void StartFlashingWaveText(string text)
    {
        isFlashing = true;
        StartCoroutine(FlashWaveText(text));
        StartCoroutine(StopBlinking());
    }

    // Actually performs the flashing effect.
    public IEnumerator FlashWaveText(string text)
    {
        while (isFlashing)
        {
            WaveText.text = text;
            yield return new WaitForSeconds(.5f);
            WaveText.text = string.Empty;
            yield return new WaitForSeconds(.5f);
        }
    }

    // Stops the effect after showWaveTime(s)
    private IEnumerator StopBlinking()
    {
        yield return new WaitForSeconds(showWaveTime);
        isFlashing = false;
        WaveText.text = string.Empty;
    }

    private void ReturnToStartMenu()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
