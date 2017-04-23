using UnityEngine;
using UnityEngine.UI;

public class HighScoreLoader : MonoBehaviour
{
    public Text HighScoreText;

    private int highscore;

    void Start()
    {
        highscore = 0;

        if (PlayerPrefs.HasKey("HighScore"))
            highscore = PlayerPrefs.GetInt("HighScore");

        HighScoreText.text = highscore.ToString();
    }
}
