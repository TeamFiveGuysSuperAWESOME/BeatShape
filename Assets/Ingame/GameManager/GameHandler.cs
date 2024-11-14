
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using Beat;
using Beatboard;

namespace GameManager {
    public class BeatHandler : MonoBehaviour
    {
        private BeatboardManager _beatboardManager;
        private BeatManager _beatManager;
        private List<JSONNode> Boards;
        private JSONNode _boardsData;
        private List<int> _currentBoardPoints;
        private List<float> _beatIntervals;
        private List<float> _nextBeatTimes;
        private List<float> _currentBoardSizes;
        private float _startTime;
        private int _bpm;

        public void Initialize(
            BeatboardManager beatboardManager,
            BeatManager beatManager,
            List<JSONNode> boards,
            JSONNode boardsData,
            List<int> currentBoardPoints,
            List<float> beatIntervals,
            List<float> nextBeatTimes,
            List<float> currentBoardSizes,
            float startTime,
            int bpm
        )
        {
            _beatboardManager = beatboardManager;
            _beatManager = beatManager;
            Boards = boards;
            _boardsData = boardsData;
            _currentBoardPoints = currentBoardPoints;
            _beatIntervals = beatIntervals;
            _nextBeatTimes = nextBeatTimes;
            _currentBoardSizes = currentBoardSizes;
            _startTime = startTime;
            _bpm = bpm;
        }

        public void HandleGame()
        {
            var time = Time.time - _startTime;
            if (Boards == null || _boardsData == null || _beatIntervals == null) return;

            for (var i = 0; i < _boardsData.Count; i++)
            {
                if (time < _nextBeatTimes[i]) continue;

                var currentCycle = _boardsData["Board" + (i + 1)]
                    ["Cycle" + (Mathf.FloorToInt((time / _beatIntervals[i] - 1) / _currentBoardPoints[i]) + 1)];
                var prevCycle = _boardsData["Board" + (i + 1)]
                    ["Cycle" + (Mathf.FloorToInt((time / _beatIntervals[i] - 1) / _currentBoardPoints[i]))];
                var currentSide = (Mathf.FloorToInt((time / _beatIntervals[i] - 1) % _currentBoardPoints[i]) + 1).ToString();
                if (currentCycle == null || currentCycle[currentSide] == null) continue;

                var currentPoint = currentCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                var currentSize = currentCycle["Size"]?.AsFloat ?? _currentBoardSizes[i];
                if (currentSize == 0) currentSize = _currentBoardSizes[i];
                var prevPoint = prevCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                if (int.Parse(currentSide) == currentCycle.Count - 2 && prevPoint != _currentBoardPoints[i] && prevPoint != 0)
                {
                    _beatboardManager.ManageBeatboard(BeatboardManager.Beatboards[i], prevPoint, _currentBoardPoints[i],
                        _currentBoardSizes[i], currentSize, new Vector2(Boards[i]["position"][0],
                            Boards[i]["position"][1]));
                }
                if (int.Parse(currentSide) == 1)
                {
                    if (!Mathf.Approximately(currentSize, _currentBoardSizes[i]) && currentSize != 0)
                    {
                        _beatboardManager.ManageBeatboard(BeatboardManager.Beatboards[i], _currentBoardPoints[i], _currentBoardPoints[i],
                            _currentBoardSizes[i], currentSize, new Vector2(Boards[i]["position"][0],
                                Boards[i]["position"][1]));
                        _currentBoardSizes[i] = currentSize;
                    }
                    if (currentPoint != _currentBoardPoints[i] && currentPoint != 0)
                    {
                        _currentBoardPoints[i] = currentPoint;
                        currentCycle = _boardsData[i][Mathf.FloorToInt((time / _beatIntervals[i] - 1) / currentPoint)];
                        currentSide = (Mathf.FloorToInt((time / _beatIntervals[i] - 1) % currentPoint) + 1).ToString();
                        _beatIntervals[i] = 60f / _bpm / currentPoint;
                    }
                }

                _nextBeatTimes[i] += _beatIntervals[i];

                if (!currentCycle[currentSide]["Beat"]) continue;

                float size = (currentCycle[currentSide]["Size"] != null ? currentCycle[currentSide]["Size"].AsFloat : 1) * BeatboardManager.GetBeatboardSize(i) / 20f;
                Color color = new Color(
                    currentCycle[currentSide]["Color"]?[0]?.AsFloat ?? 1,
                    currentCycle[currentSide]["Color"]?[1]?.AsFloat ?? 1,
                    currentCycle[currentSide]["Color"]?[2]?.AsFloat ?? 1
                );
                if (color == Color.black) color = Color.white;
                float speed = currentCycle[currentSide]["Speed"] != null ? currentCycle[currentSide]["Speed"].AsFloat : 1;
                _beatManager.CreateBeat(i, _currentBoardPoints[i], int.Parse(currentSide), speed, _bpm * 4, size, color);
            }
        }
    }
}
