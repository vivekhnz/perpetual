using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

	public Text GameOverText;

	// Use this for initialization
	void Start () {
		if (GameOverText == null)
			return;
		GameOverText.text = string.Empty;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void GameOver() {
		if (GameOverText == null)
			return;
		GameOverText.text = "Game Over";
	}
}
