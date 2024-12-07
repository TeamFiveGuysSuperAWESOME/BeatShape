using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameManager
{
    public class CountDownManager : MonoBehaviour
    {
        public float startTime = 3.0f;
        public float leftTime = 3.0f;
        public float totalTime = 3.0f;
        public int count = 4;
        public TextMeshProUGUI startText;

        public void RefreshTimer(float onetick_time, float offset, int countnum)
        {
            startTime = onetick_time;
            leftTime = onetick_time + offset + onetick_time;
            totalTime = leftTime;
            count = countnum + 1;
        }

        void Update()
        {
            if (!MainGameManager.GameStarted || !startText) return;

            leftTime -= Time.deltaTime;
            float currentTime = Mathf.Floor(count - (count - 1) / (totalTime - 1) * (totalTime - leftTime));
            Debug.Log("currentTime: " + leftTime);
            if (leftTime < 1)
            {
                startText.text = "";
                enabled = false;
                return;
            }
            
            if(currentTime > count) startText.text = "Ready";
            else startText.text = Mathf.Round(currentTime).ToString();
        }
    }
}