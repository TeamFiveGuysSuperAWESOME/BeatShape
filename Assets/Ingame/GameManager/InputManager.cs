using System;
using System.Collections.Generic;
using System.Linq;
using Beat;
using GameManager;
using UnityEditor;
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

            if (MainGameManager.GameEnded) 
            {
                var tempn = beatDataList.Count;
                foreach (BeatData beatData in beatDataList)
                {
                    if (beatData.input_offset >= 0f || beatData.input_offset == -9999f) tempn--;
                }
                if (tempn == 0) MainGameManager.GameReallyEnded = true;
            }
            

            closestBeat = beatDataList.OrderByDescending(beatData => beatData.input_offset).FirstOrDefault();
            if (closestBeat == null) return;

            if (Input.anyKeyDown)
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