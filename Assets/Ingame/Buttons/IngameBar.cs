using UnityEngine;
using TMPro;
using GameManager;
using System.Collections;

public class IngameBar : MonoBehaviour
{
    bool onClick = false;
    float timer;
    
    public GameObject bar_front;
    LineRenderer lr;
    AudioSource audioSource;
    public GameObject text;
    TextMeshProUGUI text_tmp;

    void Awake()
    {
        lr = bar_front.GetComponent<LineRenderer>();
        text_tmp = text.GetComponent<TextMeshProUGUI>();
    }
    void Start()
    {
        StartCoroutine(SetStartPos());
    }

    public IEnumerator SetStartPos() 
    {
        yield return new WaitUntil(() => MainGameManager._musicLength != -1);
        lr.SetPosition(1, new Vector3(MainGameManager._debugTime / MainGameManager._musicLength * 10, 0, 0));
        text_tmp.text = MainGameManager.OnSliderMove(lr.GetPosition(1).x / 10);
    }

    void OnMouseDown()
    {
        onClick = true;
    }
    void OnMouseUp()
    {
        onClick = false;
    }

    void Update()
    {
        if (onClick)
        {
            Vector3 newPos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, 0, 0);
            newPos = new Vector3(transform.InverseTransformPoint(newPos).x + 0.5f, 0, 0);
            if (newPos.x < 0) { lr.SetPosition(1, new Vector3(0, 0, 0)); }
            else if (newPos.x > 1) { lr.SetPosition(1, new Vector3(10, 0, 0)); }
            else { lr.SetPosition(1, newPos * 10); }
            text_tmp.text = MainGameManager.OnSliderMove(lr.GetPosition(1).x / 10);

        }
    }
}
