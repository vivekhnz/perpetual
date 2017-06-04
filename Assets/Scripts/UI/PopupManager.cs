using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PopupManager : MonoBehaviour
{
    public ScorePopupController Popup;

    private Canvas worldCanvas;
    private PlayerMovement player;

    void Start()
    {
        worldCanvas = GetComponent<Canvas>();
        player = Object.FindObjectOfType<PlayerMovement>();
    }

    public void CreatePopup(string text, Vector3 position, Vector2 velocity,
        float animationSpeed = 1.0f, bool playSound = false)
    {
        var popup = Instantiate(Popup);
        popup.transform.SetParent(worldCanvas.transform);
        popup.Initialize(text, position, velocity, animationSpeed, playSound);
    }

    public void CreatePlayerPopup(string text, float animationSpeed = 1.0f,
    bool playSound = false)
    {
        var popup = Instantiate(Popup);
        popup.transform.SetParent(worldCanvas.transform);
        popup.Initialize(text, player.transform.position, Vector2.up, animationSpeed,
            playSound);
    }
}
