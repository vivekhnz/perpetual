using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreLoader : MonoBehaviour {

    public Text HighScoreText;

    private int hiScore;

	// Use this for initialization
	void Start () {
        hiScore = 0;

		if (PlayerPrefs.HasKey("HighScore"))
        {
            hiScore = PlayerPrefs.GetInt("HighScore");
        }

        HighScoreText.text = "High Score: " + hiScore;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
