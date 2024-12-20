﻿using System;
using System.Collections.Generic;
using System.Linq;
using Beat;
using GameManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

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
            if (MainGameManager.GameReallyEnded) return;
            beatDataList = new List<BeatData>(FindObjectsByType<BeatData>(FindObjectsSortMode.None));

            if (MainGameManager.GameEnded)
            {
                var tempn = beatDataList.Count;
                foreach (BeatData beatData in beatDataList)
                {
                    if (beatData.input_offset == -9999f) tempn--;
                }
                if (tempn == 0) MainGameManager.GameReallyEnded = true;
            }


            closestBeat = beatDataList.OrderByDescending(beatData => beatData.input_offset).FirstOrDefault();
            if (closestBeat == null) return;

            if (Input.anyKeyDown && Input.touchCount == 0)
            {
                if (EventSystem.current.currentSelectedGameObject) return;
                BeatMovement beatMovement = closestBeat.GetComponent<BeatMovement>();
                BeatData beatData = closestBeat.GetComponent<BeatData>();
                if (beatData.scored) return;
                float inputOffset = beatData.input_offset;
                beatMovement.TryRemoveBeatScored(inputOffset);
            }
            foreach (Touch touch in Input.touches)
            {
                if (EventSystem.current.currentSelectedGameObject) return;
                if (touch.phase == TouchPhase.Began)
                {
                    BeatMovement beatMovement = closestBeat.GetComponent<BeatMovement>();
                    BeatData beatData = closestBeat.GetComponent<BeatData>();
                    if (beatData.scored) return;
                    float inputOffset = beatData.input_offset;
                    beatMovement.TryRemoveBeatScored(inputOffset);
                }
            }
        }
    }
}