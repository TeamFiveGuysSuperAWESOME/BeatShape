using System;
using System.Collections.Generic;
using Beat;
using GameManager;
using UnityEngine;

namespace Ingame.GameManager
{
    public class InputManager : MonoBehaviour
    {
        public List<float> beatList;
        public BeatData closestBeat;
        private void Update()
        {
            beatList.Clear();
            BeatData[] beatDataArray = FindObjectsOfType<BeatData>();
            float minDistance = float.MaxValue;
            foreach (var beatData in beatDataArray)
            {
                beatList.Add(beatData.distance);
                if (beatData.distance < minDistance)
                {
                    minDistance = beatData.distance;
                    closestBeat = beatData;
                }
            }
            Debug.Log(string.Join(", ", beatList));
            
            if (Input.anyKeyDown)
            {
                BeatMovement beatMovement = closestBeat.GetComponent<BeatMovement>();
                if (beatMovement != null)
                {
                    beatMovement.RemoveBeatScored();
                }
            }
        }
    }
}