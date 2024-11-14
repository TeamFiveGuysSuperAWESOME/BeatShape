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
            closestBeat = null;
            beatDataList = new List<BeatData>(FindObjectsOfType<BeatData>());
            float minDistance = float.MaxValue;
            foreach (var beatData in beatDataList)
            {
                if (beatData.distance < minDistance && beatData.distance != 0)
                {
                    minDistance = beatData.distance;
                    closestBeat = beatData;
                }
            }
            Debug.Log(string.Join(", ", beatDataList.Select(b => b.distance)) + " / " + minDistance);

            if (Input.anyKeyDown && closestBeat != null)
            {
                BeatMovement beatMovement = closestBeat.GetComponent<BeatMovement>();
                beatMovement.TryRemoveBeatScored();
            }
        }
    }
}