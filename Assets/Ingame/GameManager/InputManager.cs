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
            beatDataList.AddRange(FindObjectsOfType<BeatData>());
        }

        private void Update()
        {
            beatDataList = new List<BeatData>(FindObjectsOfType<BeatData>());
            float minOffset = -float.MaxValue;
            foreach (var beatData in beatDataList)
            {
                if (beatData.input_offset > minOffset)
                {
                    minOffset = beatData.input_offset;
                    closestBeat = beatData;
                }
            }
            //Debug.Log(string.Join(", ", beatDataList.Select(b => b.distance)) + " / " + minDistance);
            
            if (Input.anyKeyDown && closestBeat != null)
            {
                BeatMovement beatMovement = closestBeat.GetComponent<BeatMovement>();
                beatMovement.TryRemoveBeatScored();
            }
        }
    }
}