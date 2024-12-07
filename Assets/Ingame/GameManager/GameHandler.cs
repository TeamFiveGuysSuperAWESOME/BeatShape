using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using Beat;
using Beatboard;
using System;
using UnityEngine.XR;

namespace GameManager
{
    public class GameHandler : MonoBehaviour
    {
        private BeatboardManager _beatboardManager;
        private BeatManager _beatManager;
        private CameraManager _cameraManager;
        private List<JSONNode> Boards;
        private JSONNode _boardsData;
        private List<int> _currentBoardPoints;
        private List<double> _beatIntervals;
        private List<double> _beatIntervalsTmp;
        private List<double> _nextBeatTimes;
        private List<double> _nextPointTimes;
        private List<int> _nextPointCycles;
        private List<float> _currentBoardSizes;
        private double _startTime;
        public static double pauseTime = 0;
        private float _bpm;

        void Awake()
        {
            pauseTime = 0;
        }

        public void Initialize(
            BeatboardManager beatboardManager,
            BeatManager beatManager,
            CameraManager cameraManager,
            List<JSONNode> boards,
            JSONNode boardsData,
            List<int> currentBoardPoints,
            List<double> beatIntervals,
            List<double> nextBeatTimes,
            List<float> currentBoardSizes,
            double startTime,
            float bpm
        )
        {
            _beatboardManager = beatboardManager;
            _beatManager = beatManager;
            _cameraManager = cameraManager;
            Boards = boards;
            _boardsData = boardsData;
            _currentBoardPoints = currentBoardPoints;
            _beatIntervals = beatIntervals;
            _beatIntervalsTmp = beatIntervals;
            _nextBeatTimes = nextBeatTimes;
            _currentBoardSizes = currentBoardSizes;
            _startTime = startTime;
            _bpm = bpm;
            _nextPointTimes = new List<double>();
            _nextPointCycles = new List<int>();
            for (var i = 0; i < _boardsData.Count; i++)
            {
                _nextPointTimes.Add(0);
                _nextPointCycles.Add(0);
            }
            if (MainGameManager._debugTime != 0)
            {
                for (float i = 0; i < MainGameManager._debugTime * 1000; i++)
                {
                    HandleGameFF(i / 1000);
                }
            }
        }

        public void HandleGame()
        {
            if (MainGameManager.Paused) return;
            if (MainGameManager.GameReallyEnded) return;
            //_elapsedTime = ;
            double timeSinceStart = AudioSettings.dspTime - _startTime - pauseTime;
            if (Boards == null || _boardsData == null || _beatIntervals == null) return;
            var gameEnded = 0;

            for (var i = 0; i < _boardsData.Count; i++)
            {
                if (timeSinceStart < _nextBeatTimes[i]) continue;

                var currentBoard = _boardsData["Board" + (i + 1)];
                var beats = (timeSinceStart - _nextPointTimes[i]) / _beatIntervals[i] - 1;
                var currentCycle = currentBoard
                    ["Cycle" + ((int)Math.Floor(beats / _currentBoardPoints[i]) + 1 + _nextPointCycles[i])];
                //Debug.Log("Cycle" + ((int)Math.Floor(beats / _currentBoardPoints[i]) + 1 + _nextPointCycles[i]));
                var prevCycle = currentBoard
                    ["Cycle" + ((int)Math.Floor(beats / _currentBoardPoints[i]) + _nextPointCycles[i])];
                var nextCycle = currentBoard
                    ["Cycle" + ((int)Math.Floor(beats / _currentBoardPoints[i]) + 2 + _nextPointCycles[i])];
                var currentSide = ((int)Math.Floor(beats % _currentBoardPoints[i]) + 1).ToString();

                var currentPoint = currentCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                if (currentPoint == 0) { gameEnded++; continue; }
                var currentSize = currentCycle["Size"]?.AsFloat ?? _currentBoardSizes[i];
                if (currentSize == 0) currentSize = _currentBoardSizes[i];
                var prevPoint = prevCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                var nextPoint = nextCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                if (int.Parse(currentSide) == _currentBoardPoints[i] && prevPoint != _currentBoardPoints[i] && prevPoint != 0)
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
                    if (currentPoint != prevPoint && currentPoint != 0)
                    {
                        _currentBoardPoints[i] = currentPoint;
                        currentCycle = _boardsData[i][(int)Math.Floor(beats / currentPoint) + _nextPointCycles[i]];
                        currentSide = ((int)Math.Floor(beats % currentPoint) + 1).ToString();
                        _beatIntervals[i] = 60f / _bpm / currentPoint;
                        _beatIntervalsTmp[i] = 60f / _bpm / currentPoint;
                    }
                }

                if (int.Parse(currentSide) == currentPoint && currentPoint != nextPoint && nextPoint != 0)
                {
                    _nextPointCycles[i] = (int)Math.Floor(beats / _currentBoardPoints[i]) + 1 + _nextPointCycles[i];
                    _nextPointTimes[i] = nextPoint % 2 == 0 ? _nextBeatTimes[i] : _nextBeatTimes[i] - 30 / _bpm / nextPoint;
                    _beatIntervals[i] = 60f / _bpm / nextPoint;
                    _beatIntervalsTmp[i] = 30f / _bpm / currentPoint + 30f / _bpm / nextPoint;
                }

                _nextBeatTimes[i] += _beatIntervalsTmp[i];

                //Debug.Log(currentPoint + " " + currentSide + " " + _beatIntervalsTmp[i] + " " +  _nextBeatTimes[i] + " " + timeSinceStart);

                JSONNode currentBeat = currentCycle?[currentSide]?.AsObject ?? null;
                if (currentBeat == null) continue;

                //Camera Handling
                HandleCamera(currentBeat);

                if (!currentBeat["Beat"]) continue;

                // Beat Creation
                float size = currentBeat["Size"] != null ? currentBeat["Size"].AsFloat : 1;
                Color color = new(
                    currentBeat["Color"]?[0]?.AsFloat ?? 1,
                    currentBeat["Color"]?[1]?.AsFloat ?? 1,
                    currentBeat["Color"]?[2]?.AsFloat ?? 1
                );
                if (color == Color.black) color = Color.white;
                float speed = currentBeat["Speed"] != null ? currentBeat["Speed"].AsFloat : 1;
                string easing = currentBeat["Easing"] != null ? currentBeat["Easing"] : "outcubic";
                _beatManager.CreateBeat(i, 1, _currentBoardSizes[i], _currentBoardPoints[i], int.Parse(currentSide), speed, _bpm * 4, size, color, easing);
            }
            if (gameEnded == _boardsData.Count) MainGameManager.GameEnded = true;
        }

        public void HandleGameFF(double time)
        {
            double timeSinceStart = time;
            if (Boards == null || _boardsData == null || _beatIntervals == null) return;
            var gameEnded = 0;

            for (var i = 0; i < _boardsData.Count; i++)
            {
                if (timeSinceStart < _nextBeatTimes[i]) continue;

                var currentBoard = _boardsData["Board" + (i + 1)];
                var beats = (timeSinceStart - _nextPointTimes[i]) / _beatIntervals[i] - 1;
                var currentCycle = currentBoard
                    ["Cycle" + ((int)Math.Floor(beats / _currentBoardPoints[i]) + 1 + _nextPointCycles[i])];
                //Debug.Log("Cycle" + ((int)Math.Floor(beats / _currentBoardPoints[i]) + 1 + _nextPointCycles[i]));
                if (currentCycle == null) { gameEnded++; continue; }
                var prevCycle = currentBoard
                    ["Cycle" + ((int)Math.Floor(beats / _currentBoardPoints[i]) + _nextPointCycles[i])];
                var nextCycle = currentBoard
                    ["Cycle" + ((int)Math.Floor(beats / _currentBoardPoints[i]) + 2 + _nextPointCycles[i])];
                var currentSide = ((int)Math.Floor(beats % _currentBoardPoints[i]) + 1).ToString();

                var currentPoint = currentCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                var currentSize = currentCycle["Size"]?.AsFloat ?? _currentBoardSizes[i];
                if (currentSize == 0) currentSize = _currentBoardSizes[i];
                var prevPoint = prevCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                var nextPoint = nextCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                if (int.Parse(currentSide) == _currentBoardPoints[i] && prevPoint != _currentBoardPoints[i] && prevPoint != 0)
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
                    if (currentPoint != prevPoint && currentPoint != 0)
                    {
                        _currentBoardPoints[i] = currentPoint;
                        currentCycle = _boardsData[i][(int)Math.Floor(beats / currentPoint) + _nextPointCycles[i]];
                        currentSide = ((int)Math.Floor(beats % currentPoint) + 1).ToString();
                        _beatIntervals[i] = 60f / _bpm / currentPoint;
                        _beatIntervalsTmp[i] = 60f / _bpm / currentPoint;
                    }
                }

                if (int.Parse(currentSide) == currentPoint && currentPoint != nextPoint && nextPoint != 0)
                {
                    _nextPointCycles[i] = (int)Math.Floor(beats / _currentBoardPoints[i]) + 1 + _nextPointCycles[i];
                    _nextPointTimes[i] = nextPoint % 2 == 0 ? _nextBeatTimes[i] : _nextBeatTimes[i] - 30 / _bpm / nextPoint;
                    _beatIntervals[i] = 60f / _bpm / nextPoint;
                    _beatIntervalsTmp[i] = 30f / _bpm / currentPoint + 30f / _bpm / nextPoint;
                }

                _nextBeatTimes[i] += _beatIntervalsTmp[i];

                //Debug.Log(currentPoint + " " + currentSide + " " + _beatIntervalsTmp[i] + " " +  _nextBeatTimes[i] + " " + timeSinceStart);

                JSONNode currentBeat = currentCycle?[currentSide]?.AsObject ?? null;
                if (currentBeat == null) continue;

                //Camera Handling
                HandleCamera(currentBeat, true);
            }
            if (gameEnded == _boardsData.Count) MainGameManager.GameEnded = true;
        }

        public void HandleCamera(JSONNode currentBeat, bool isFF = false)
        {
            // Camera Movement
            if (currentBeat["Camera"] != null)
            {
                JSONNode camera = currentBeat["Camera"];
                float duration = camera["Duration"] != null && !isFF ? camera["Duration"].AsFloat : 0f;
                string easing = camera["Easing"] != null ? camera["Easing"] : "linear";
                if (camera["BBColor"] != null) _cameraManager.ChangeBBColor(new Color(camera["BBColor"][0], camera["BBColor"][1], camera["BBColor"][2]), easing, duration);
                if (camera["BGColor"] != null) _cameraManager.ChangeBGColor(new Color(camera["BGColor"][0], camera["BGColor"][1], camera["BGColor"][2]), easing, duration);
                if (camera["BGImage"] != null) GameObject.FindGameObjectsWithTag("BackgroundImg")[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Levels/1/" + camera["BGImage"]);
                if (camera["Position"] != null) _cameraManager.MoveCamera(camera["Position"][0], camera["Position"][1], easing, duration);
                if (camera["Rotation"] != null) _cameraManager.RotateCamera(camera["Rotation"], easing, duration);
                if (camera["Zoom"] != null) _cameraManager.ZoomCamera(camera["Zoom"], easing, duration);
                if (camera["Shake"] != null && !isFF) _cameraManager.ShakeCamera(camera["Shake"]["Intensity"], camera["Shake"]["Duration"]);
            }

            //Post Processing
            if (currentBeat["PP"] != null)
            {
                JSONNode pp = currentBeat["PP"];
                float duration = pp["Duration"] != null && !isFF ? pp["Duration"].AsFloat : 0f;
                string easing = pp["Easing"] != null ? pp["Easing"] : "linear";
                if (pp["General"] != null)
                {
                    float exposure = pp["General"]["Exposure"] != null ? pp["General"]["Exposure"].AsFloat : -1234f;
                    float contrast = pp["General"]["Contrast"] != null ? pp["General"]["Contrast"].AsFloat : -1234f;
                    float saturation = pp["General"]["Saturation"] != null ? pp["General"]["Saturation"].AsFloat : -1234f;
                    _cameraManager.Cg(exposure, contrast, saturation, easing, duration);
                }
                if (pp["Bloom"] != null)
                {
                    float intensity = pp["Bloom"]["Intensity"] != null ? pp["Bloom"]["Intensity"].AsFloat : -1234f;
                    float threshold = pp["Bloom"]["Threshold"] != null ? pp["Bloom"]["Threshold"].AsFloat : -1234f;
                    Color _color = pp["Bloom"]["Color"] != null ?
                        new Color(
                            pp["Bloom"]["Color"][0] != null ? pp["Bloom"]["Color"][0].AsFloat : -1234f,
                            pp["Bloom"]["Color"][1] != null ? pp["Bloom"]["Color"][1].AsFloat : -1234f,
                            pp["Bloom"]["Color"][2] != null ? pp["Bloom"]["Color"][2].AsFloat : -1234f
                        ) : new Color(-1234f, -1234f, -1234f);
                    _cameraManager.Bloom(intensity, threshold, _color, easing, duration);
                }
                if (pp["DoF"] != null)
                {
                    float focusDistance = pp["DoF"]["FocusDistance"] != null ? pp["DoF"]["FocusDistance"].AsFloat : -1234f;
                    float aperture = pp["DoF"]["Aperture"] != null ? pp["DoF"]["Aperture"].AsFloat : -1234f;
                    float focalLength = pp["DoF"]["FocalLength"] != null ? pp["DoF"]["FocalLength"].AsFloat : -1234f;
                    _cameraManager.Dof(focusDistance, aperture, focalLength, easing, duration);
                }
                if (pp["Lens Distortion"] != null)
                {
                    float intensity = pp["Lens Distortion"]["Intensity"] != null ? pp["Lens Distortion"]["Intensity"].AsFloat : -1234f;
                    _cameraManager.Ld(intensity, easing, duration);
                }
                if (pp["Chromatic Aberration"] != null)
                {
                    float intensity = pp["Chromatic Aberration"]["Intensity"] != null ? pp["Chromatic Aberration"]["Intensity"].AsFloat : -1234f;
                    _cameraManager.Ca(intensity, easing, duration);
                }
                if (pp["Vignette"] != null)
                {
                    float intensity = pp["Vignette"]["Intensity"] != null ? pp["Vignette"]["Intensity"].AsFloat : -1234f;
                    float smoothness = pp["Vignette"]["Smoothness"] != null ? pp["Vignette"]["Smoothness"].AsFloat : -1234f;
                    float roundness = pp["Vignette"]["Roundness"] != null ? pp["Vignette"]["Roundness"].AsFloat : -1234f;
                    Color _color = pp["Vignette"]["Color"] != null ?
                        new Color(
                            pp["Vignette"]["Color"][0] != null ? pp["Vignette"]["Color"][0].AsFloat : -1234f,
                            pp["Vignette"]["Color"][1] != null ? pp["Vignette"]["Color"][1].AsFloat : -1234f,
                            pp["Vignette"]["Color"][2] != null ? pp["Vignette"]["Color"][2].AsFloat : -1234f
                        ) : new Color(-1234f, -1234f, -1234f);
                    _cameraManager.Vignette(intensity, smoothness, roundness, _color, easing, duration);
                }
            }
        }
    }
}
