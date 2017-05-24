using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
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
    public List<UpgradeButtonController> UpgradeButtons;

    private Animator animator;
    private PlayerUpgrades upgrades;

    private bool isGameOver;
    private bool isPopoverOpen;
    public bool CanProgressToNextWave
    {
        get { return !isGameOver && !isPopoverOpen; }
    }

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

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("No animator found!");

        score = 0;
        isGameOver = false;
        isPopoverOpen = false;
        animator.SetBool("IsPopoverVisible", false);

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

        upgrades = GameObject.FindObjectOfType<PlayerUpgrades>();
        if (upgrades == null)
            Debug.LogError("No player upgrade manager found.");

        waveTime = 0;
        doShowWave = true;
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
        isGameOver = true;

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
        var enemyManager = UnityEngine.Object
            .FindObjectOfType<EnemySpawnManager>();
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

    public void SignalUpgradeUnlocked()
    {
        var choices = upgrades.GetUpgradeChoices(UpgradeButtons.Count);

        if (choices.Count == 0)
            return;

        // show upgrade buttons
        for (int i = 0; i < UpgradeButtons.Count; i++)
        {
            var button = UpgradeButtons[i];
            if (choices.Count > i)
            {
                button.gameObject.SetActive(true);
                button.SetUpgrade(choices[i]);
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }

        // show popover
        animator.SetBool("IsPopoverVisible", true);
        isPopoverOpen = true;
    }

    public void SelectUpgrade(UpgradeBase selectedUpgrade)
    {
        if (!isPopoverOpen)
            return;

        // unlock upgrade
        upgrades.Unlock(selectedUpgrade);

        // close popover
        animator.SetBool("IsPopoverVisible", false);
        isPopoverOpen = false;
    }

    private void ReturnToStartMenu()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
