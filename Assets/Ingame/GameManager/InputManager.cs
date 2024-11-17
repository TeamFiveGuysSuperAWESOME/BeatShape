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
            beatDataList = new List<BeatData>(FindObjectsByType<BeatData>(FindObjectsSortMode.None));
            closestBeat = beatDataList.OrderByDescending(beatData => beatData.input_offset).FirstOrDefault();
            
            if (Input.anyKeyDown && closestBeat != null)
            {
                BeatMovement beatMovement = closestBeat.GetComponent<BeatMovement>();
                beatMovement.TryRemoveBeatScored();
            }
        }
    }
}