using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameManager
{
    public class CountDownManager : MonoBehaviour
    {
        public float startTime = 3.0f;
        public float leftTime = 3.0f;
        public TextMeshProUGUI startText; // used for showing countdown from 3, 2, 1 

        public void RefreshTimer(float onetick_time)
        {
            startTime = onetick_time;
            leftTime = onetick_time*1.5f;
        }

        void Update()
        {
            if (!MainGameManager.GameStarted || !startText) return;

            leftTime -= Time.deltaTime;
            float currentTime = (leftTime/startTime)*4;
            if (leftTime < 0)
            {
                startText.text = "";
                enabled = false;
                return;
            }
            
            if(currentTime > 4) startText.text = "Ready";
            else startText.text = Mathf.Floor(currentTime).ToString();
        }
    }
}