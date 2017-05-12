using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HUDController : MonoBehaviour
{
    public Text GameOverText;
    public Slider HealthText;
    public Text ScoreText;
    public Text MessageText;
    public float ShowWaveTime;
    public Text WaveText;
    public Text RoundText;
    public Text HighScoreText;
    public Slider BossHealth;

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

        if (MessageText != null)
            MessageText.text = "Wave 1";

        if (WaveText != null)
            WaveText.text = "Wave 1";

        if (RoundText != null)
            RoundText.text = "Round 1";

        if (HighScoreText != null)
            HighScoreText.text = "High Score: " + highscore;

        waveTime = 0;
        doShowWave = true;

        BossHealth.gameObject.SetActive(false);
    }

    void Update()
    {
        // Clear wave text after showWaveTime(s) if there is a WaveText on screen.
        // Does not deal with the blinking WaveText effect.
        if (doShowWave && (Time.time - waveTime) > ShowWaveTime)
        {
            doShowWave = false;
            MessageText.text = string.Empty;
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

    public void ShowRoundAndWave(int round, int wave)
    {
        if (MessageText != null)
            MessageText.text = "Wave " + wave;

        if (WaveText != null)
            WaveText.text = "Wave " + wave;

        if (RoundText != null)
            RoundText.text = "Round " + round;

        doShowWave = true;
        waveTime = Time.time;
    }

    public void SignalBossFight()
    {
        if (WaveText != null)
            WaveText.text = "Boss Fight";

        StartCoroutine(FlashWaveText("BOSS FIGHT!"));
        StartCoroutine(StopBlinking(ShowWaveTime));
    }

    public IEnumerator FlashWaveText(string text)
    {
        isFlashing = true;
        while (isFlashing)
        {
            MessageText.text = text;
            yield return new WaitForSeconds(.5f);
            MessageText.text = string.Empty;
            yield return new WaitForSeconds(.5f);
        }
    }

    private IEnumerator StopBlinking(float duration)
    {
        yield return new WaitForSeconds(duration);
        isFlashing = false;
        MessageText.text = string.Empty;
    }

    private void ReturnToStartMenu()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void UpdateBossHealth(float health)
    {
        if (health > 0)
        {
            BossHealth.gameObject.SetActive(true);
            BossHealth.value = health;
        }
        else
        {
            BossHealth.gameObject.SetActive(false);
        }
    }
}
