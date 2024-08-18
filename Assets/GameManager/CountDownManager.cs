using UnityEngine;
using UnityEngine.UI;

namespace GameManager
{
    public class CountDownManager : MonoBehaviour
    {

        public float timeLeft = 3.0f;
        public Text startText; // used for showing countdown from 3, 2, 1 


        void Update()
        {
            if (!MainGameManager._gameStarted) return;
            timeLeft -= Time.deltaTime;
            startText.text = (timeLeft).ToString("0");
            if (timeLeft < 0)
            {
                startText.text = "";
            }
        }
    }
}