using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class HUDController : MonoBehaviour
{
    public float ShowWaveTime;
    public Text ScoreText;
    public Text MessageText;
    public Text WaveText;
    public Text RoundText;
    public Text HighScoreText;
    public Image ControlHintImage;

    public GameObject GameOverPanel;
    public Text GameOverScoreText;
    public Text GameOverHighScoreText;
    public Text GameOverRoundText;
    public Text GameOverWaveText;
    public Text GameOverStreakText;
    public Text GameOverMultikillText;

    public List<UpgradeButtonController> UpgradeButtons;
    public float TimeToScoreMultiply = 1.0f;
    public Text ScoreMultiplierText;
    public Text MultikillLabel;
    public int UntouchableAmount = 5;
    public int UntouchableBonus = 100;

    private Animator animator;
    private PlayerUpgrades upgrades;
    private PopupManager popups;
    private ReticleController reticle;

    private bool isGameOver;
    private bool isPopoverOpen;
    public bool CanProgressToNextWave
    {
        get { return !isGameOver && !isPopoverOpen; }
    }

    private int score;
    private int highscore;
    private int wave;
    private int round;
    private bool doShowWave;
    private bool isFlashing = false; // Used for checking whether to blink WaveText.
    private float waveTime;
    private float timeSinceScore;
    private int scoreMultiplier;
    private int highestMultikill;
    private int streak;
    private int longestStreak;

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

        GameOverPanel.SetActive(false);

        if (ScoreText != null)
            ScoreText.text = "SCORE: " + score;

        if (MessageText != null)
            MessageText.text = "GET READY!";

        if (WaveText != null)
            WaveText.text = "WAVE 1";

        if (RoundText != null)
            RoundText.text = "ROUND 1";

        if (HighScoreText != null)
            HighScoreText.text = "HIGH SCORE: " + highscore;

        if (ScoreMultiplierText != null)
            ScoreMultiplierText.text = string.Empty;

        if (MultikillLabel != null)
            MultikillLabel.text = string.Empty;

        upgrades = GameObject.FindObjectOfType<PlayerUpgrades>();
        if (upgrades == null)
            Debug.LogError("No player upgrade manager found.");

        popups = GameObject.FindObjectOfType<PopupManager>();
        if (popups == null)
            Debug.LogError("No popup manager found.");

        reticle = GameObject.FindObjectOfType<ReticleController>();
        if (reticle == null)
            Debug.LogError("No reticle found.");

        waveTime = 0;
        doShowWave = true;
        timeSinceScore = Time.time;
        scoreMultiplier = 1;
        streak = 0;
        longestStreak = 0;
        highestMultikill = 0;
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

        // update score multiplier if reset
        if (Time.time - timeSinceScore > TimeToScoreMultiply)
        {
            ScoreMultiplierText.text = string.Empty;
            MultikillLabel.text = string.Empty;
        }
    }

    public void GameOver()
    {
        isGameOver = true;

        // show player score and highscore before highscore is overwritten
        DisplayEndGameStatistics();

        // did the player beat the highscore?
        if (score > highscore)
        {
            highscore = score;
            PlayerPrefs.SetInt("HighScore", highscore);
        }

        // go back to regular cursor
        reticle.ShowCursor();

        GameOverPanel.SetActive(true);
        SendGameOverTelemetry(score, round, wave);

        // destroy all enemies and spawners
        var enemyManager = UnityEngine.Object
            .FindObjectOfType<EnemySpawnManager>();
        Destroy(enemyManager);
    }

    private void DisplayEndGameStatistics()
    {
        // show important stats
        GameOverScoreText.text = score.ToString();
        GameOverHighScoreText.text = highscore.ToString();
        GameOverRoundText.text = round.ToString();
        GameOverWaveText.text = wave.ToString();
        GameOverStreakText.text = longestStreak.ToString();
        GameOverMultikillText.text = highestMultikill.ToString();
    }

    private void SendGameOverTelemetry(int score, int round, int wave)
    {
        // send telemetry regarding the end results of a game
        Analytics.CustomEvent("GameOverScores", new Dictionary<string, object>()
        {
            { "Score", score },
            { "Round", round },
            { "Wave", wave }
        });
    }

    public void ResetStreak()
    {
        scoreMultiplier = 1;
        ScoreMultiplierText.text = string.Empty;
        MultikillLabel.text = string.Empty;
        streak = 0;
    }

    public void AddScore(int score)
    {
        // update score multiplier
        if (Time.time - timeSinceScore < TimeToScoreMultiply)
        {
            scoreMultiplier++;
        }
        else
        {
            scoreMultiplier = 1;
        }
        highestMultikill = Math.Max(highestMultikill, scoreMultiplier);

        this.score += score * scoreMultiplier;
        timeSinceScore = Time.time;

        streak++;
        longestStreak = Math.Max(longestStreak, streak);
        if (streak % UntouchableAmount == 0)
        {
            var bonusMultiplier = 0.9f + (0.1f * (streak / UntouchableAmount));
            this.score += (int)(UntouchableBonus * bonusMultiplier);
            popups.CreatePlayerPopup($"{streak} KILL STREAK", 0.5f, true);
        }

        ScoreText.text = "SCORE: " + this.score;

        if (scoreMultiplier > 1)
        {
            ScoreMultiplierText.text = $"x{scoreMultiplier}";
            MultikillLabel.text = "MULTIKILL";
        }
        else
        {
            ScoreMultiplierText.text = string.Empty;
            MultikillLabel.text = string.Empty;
        }
    }

    public void ShowRoundAndWave(int round, int wave)
    {
        // save round and wave numbers for telemetry
        this.round = round;
        this.wave = wave;

        // append the numbers to hud text
        if (MessageText != null)
            MessageText.text = "WAVE " + wave;

        if (WaveText != null)
            WaveText.text = "WAVE " + wave;

        if (RoundText != null)
            RoundText.text = "ROUND " + round;

        doShowWave = true;
        waveTime = Time.time;
    }

    public void SignalBossFight()
    {
        if (WaveText != null)
            WaveText.text = "BOSS FIGHT";

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

    // displays an image for a set amount of time
    public void ShowControlHintImage(Sprite sprite, float displayTime)
    {
        ControlHintImage.enabled = true;
        if (sprite != null)
            ControlHintImage.sprite = sprite;
        Invoke("HideControlHintImage", displayTime);
    }

    private void HideControlHintImage()
    {
        ControlHintImage.enabled = false;
    }

    public void SignalUpgradeUnlocked()
    {
        var choices = upgrades.GetUpgradeChoices(UpgradeButtons.Count);

        if (choices.Count == 0)
            return;

        // go back to regular cursor
        reticle.ShowCursor();

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

        // go back to reticle
        reticle.ShowReticle();

        // show control hints
        if (selectedUpgrade.Tutorial != null)
            ShowControlHintImage(selectedUpgrade.Tutorial, 5.0f);
    }
}
