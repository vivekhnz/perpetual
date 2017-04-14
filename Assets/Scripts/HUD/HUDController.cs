using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

	public Text GameOverText;
    public Slider HealthText;
    public Text ScoreText;

    private float PlayerScore;

	void Start () {
        PlayerScore = 0;
		if (GameOverText == null)
			return;
		GameOverText.text = string.Empty;
        ScoreText.text = "Score: " + PlayerScore;
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
}
