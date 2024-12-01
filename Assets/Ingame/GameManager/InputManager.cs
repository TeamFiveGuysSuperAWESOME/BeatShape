using System;
using System.Collections.Generic;
using System.Linq;
using Beat;
using GameManager;
using UnityEngine;

namespace Ingame.GameManager
{
    public class InputManager : MonoBehaviour
    {
        public BeatData closestBeat;
        private List<BeatData> beatDataList = new List<BeatData>();

        private void Start()
        {
            beatDataList.AddRange(FindObjectsByType<BeatData>(FindObjectsSortMode.None));
        }

        private void Update()
        {
            if (MainGameManager.Paused) return;
            beatDataList = new List<BeatData>(FindObjectsByType<BeatData>(FindObjectsSortMode.None));
            if (beatDataList.Count == 0 && MainGameManager.GameEnded) MainGameManager.GameReallyEnded = true;
            closestBeat = beatDataList.OrderByDescending(beatData => beatData.input_offset).FirstOrDefault();
            
            if (Input.anyKeyDown && closestBeat != null)
            {
                BeatMovement beatMovement = closestBeat.GetComponent<BeatMovement>();
                beatMovement.TryRemoveBeatScored();
            }
            /*foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began && closestBeat != null)
                {
                    BeatMovement beatMovement = closestBeat.GetComponent<BeatMovement>();
                    beatMovement.TryRemoveBeatScored();
                }
            }*/
        }
    }
}