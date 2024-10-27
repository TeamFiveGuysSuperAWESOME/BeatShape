using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameManager
{
    public class CountDownManager : MonoBehaviour
    {

        public float timeLeft = 3.0f;
        public TextMeshProUGUI startText; // used for showing countdown from 3, 2, 1 


        void Update()
        {
            if (!MainGameManager.GameStarted || !startText) return;
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0)
            {
                startText.text = "";
                enabled = false;
                return;
            }
            startText.text = timeLeft.ToString("0");
        }
    }
}