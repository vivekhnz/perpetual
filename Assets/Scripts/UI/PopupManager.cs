using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class PopupManager : MonoBehaviour
{
    public ScorePopupController Popup;

    private Canvas worldCanvas;

    void Start()
    {
        worldCanvas = GetComponent<Canvas>();
    }

    public void CreatePopup(string text, Vector3 position, Vector2 velocity)
    {
        var popup = Instantiate(Popup);
        popup.transform.SetParent(worldCanvas.transform);
        popup.Initialize(text, position, velocity);
    }
}
