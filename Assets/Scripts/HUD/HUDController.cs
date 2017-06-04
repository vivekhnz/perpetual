﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
public class HUDController : MonoBehaviour
{
    public Text GameOverText;
    public Text ScoreText;
    public Text MessageText;
    public float ShowWaveTime;
    public Text WaveText;
    public Text RoundText;
    public Text HighScoreText;
    public Image ControlHintImage;
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
    private int wave;
    private int round;
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
            ScoreText.text = "SCORE: " + score;

        if (MessageText != null)
            MessageText.text = "GET READY!";

        if (WaveText != null)
            WaveText.text = "WAVE 1";

        if (RoundText != null)
            RoundText.text = "ROUND 1";

        if (HighScoreText != null)
            HighScoreText.text = "HIGH SCORE: " + highscore;

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
            GameOverText.text = "GAME OVER";

        SendGameOverTelemetry(score, round, wave);

        // destroy all enemies and spawners
        var enemyManager = UnityEngine.Object
            .FindObjectOfType<EnemySpawnManager>();
        Destroy(enemyManager);

        // go to start screen
        Invoke("ReturnToStartMenu", 2);
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

    public void AddScore(int score)
    {
        this.score += score;

        // change high score if beaten
        if (this.score > highscore)
        {
            HighScoreText.text = "HIGH SCORE: " + this.score;
        }
        ScoreText.text = "SCORE: " + this.score;
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

        // show control hints
        if (selectedUpgrade.Tutorial != null)
            ShowControlHintImage(selectedUpgrade.Tutorial, 5.0f);
    }

    private void ReturnToStartMenu()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
