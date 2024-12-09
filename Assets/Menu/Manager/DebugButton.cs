using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using GameManager;

public class DebugButton : MonoBehaviour, IPointerDownHandler
{
    TextMeshProUGUI tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (MenuManager.DebugMode) MainGameManager.levelJsonData = null;
        MenuManager.DebugMode = !MenuManager.DebugMode;
        Debug.Log("Debug mode toggled!");
    }

    void Update()
    {
        if(MenuManager.DebugMode) {tmp.text = "Debug: ON";}
        else {tmp.text = "Debug: OFF";}
    }
}
