using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

	public Text GameOverText;
    public Slider HealthText;
    public Text ScoreText;
    public Text WaveText;
    public float showWaveTime;
    public Text HUDWaveText;

    private float PlayerScore;
    private bool doShowWave;
    private float waveTime;

	void Start () {
        PlayerScore = 0;
		
        if (GameOverText != null)
			GameOverText.text = string.Empty;
        
        if (ScoreText != null)
            ScoreText.text = "Score: " + PlayerScore;
        
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
	
	public void GameOver() {
		if (GameOverText == null)
			return;
		GameOverText.text = "Game Over";
	}

    public void UpdateHealth(float health)
    {
        HealthText.value = health;
    }

    public void AddScore(float score)
    {
        PlayerScore += score;
        ScoreText.text = "Score: " + PlayerScore;
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
