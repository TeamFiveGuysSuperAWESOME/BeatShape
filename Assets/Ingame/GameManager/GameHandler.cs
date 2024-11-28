using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using Beat;
using Beatboard;

namespace GameManager {
    public class GameHandler : MonoBehaviour
    {
        private BeatboardManager _beatboardManager;
        private BeatManager _beatManager;
        private CameraManager _cameraManager;
        private List<JSONNode> Boards;
        private JSONNode _boardsData;
        private List<int> _currentBoardPoints;
        private List<float> _beatIntervals;
        private List<float> _nextBeatTimes;
        private List<float> _currentBoardSizes;
        private float _startTime;
        private float _bpm;
        private float _elapsedTime = 0f;

        void Awake()
        {
            _elapsedTime = 0f;
        }

        public void Initialize(
            BeatboardManager beatboardManager,
            BeatManager beatManager,
            CameraManager cameraManager,
            List<JSONNode> boards,
            JSONNode boardsData,
            List<int> currentBoardPoints,
            List<float> beatIntervals,
            List<float> nextBeatTimes,
            List<float> currentBoardSizes,
            float startTime,
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
            _nextBeatTimes = nextBeatTimes;
            _currentBoardSizes = currentBoardSizes;
            _startTime = startTime;
            _bpm = bpm;
        }

        public void HandleGame()
        {
            if (MainGameManager.Paused) return;
            _elapsedTime += Time.deltaTime;
            var timeSinceStart = _elapsedTime - _startTime;
            if (Boards == null || _boardsData == null || _beatIntervals == null) return;
            var gameEnded = 0;

            for (var i = 0; i < _boardsData.Count; i++)
            {
                if (timeSinceStart < _nextBeatTimes[i]) continue;

                var currentCycle = _boardsData["Board" + (i + 1)]
                    ["Cycle" + (Mathf.FloorToInt((timeSinceStart / _beatIntervals[i] - 1) / _currentBoardPoints[i]) + 1)];
                if (currentCycle == null) {gameEnded++; continue;}
                var prevCycle = _boardsData["Board" + (i + 1)]
                    ["Cycle" + (Mathf.FloorToInt((timeSinceStart / _beatIntervals[i] - 1) / _currentBoardPoints[i]))];
                var currentSide = (Mathf.FloorToInt((timeSinceStart / _beatIntervals[i] - 1) % _currentBoardPoints[i]) + 1).ToString();
                //if (currentCycle == null || currentCycle[currentSide] == null) continue;

                var currentPoint = currentCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                var currentSize = currentCycle["Size"]?.AsFloat ?? _currentBoardSizes[i];
                if (currentSize == 0) currentSize = _currentBoardSizes[i];
                var prevPoint = prevCycle["Points"]?.AsInt ?? _currentBoardPoints[i];
                //Debug.Log(int.Parse(currentSide) + " / " + _nextBeatTimes[i] + " / " + _currentBoardPoints[i]);
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
                    if (currentPoint != _currentBoardPoints[i] && currentPoint != 0)
                    {
                        _currentBoardPoints[i] = currentPoint;
                        currentCycle = _boardsData[i][Mathf.FloorToInt((timeSinceStart / _beatIntervals[i] - 1) / currentPoint)];
                        currentSide = (Mathf.FloorToInt((timeSinceStart / _beatIntervals[i] - 1) % currentPoint) + 1).ToString();
                        _beatIntervals[i] = 60f / _bpm / currentPoint;
                    }
                }

                _nextBeatTimes[i] += _beatIntervals[i];

                JSONNode currentBeat = currentCycle[currentSide];

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
                string easing = currentBeat["Easing"] != null ? currentBeat["Easing"] : "outquint";
                _beatManager.CreateBeat(i, 1, _currentBoardSizes[i], _currentBoardPoints[i], int.Parse(currentSide), speed, _bpm*4, size, color, easing);
            }
            if (gameEnded == _boardsData.Count) MainGameManager.GameEnded = true;
        }

        public void HandleCamera(JSONNode currentBeat) 
        {
            // Camera Movement
                if (currentBeat["Camera"] != null)
                {
                    JSONNode camera = currentBeat["Camera"];
                    if (camera["Position"] != null) _cameraManager.MoveCamera(camera["Position"][0], camera["Position"][1], camera["Easing"], camera["Duration"]);
                    if (camera["Rotation"] != null) _cameraManager.RotateCamera(camera["Rotation"], camera["Easing"], camera["Duration"]);
                    if (camera["Zoom"] != null) _cameraManager.ZoomCamera(camera["Zoom"], camera["Easing"], camera["Duration"]);
                }

                //Post Processing
                /*if (currentBeat["PP"] != null)
                {
                    JSONNode pp = currentBeat["PP"];
                    if (pp["General"] != null) _cameraManager.Cg(pp["General"]["Exposure"], pp["General"]["Contrast"], pp["General"]["Saturation"]);
                    if (pp["Bloom"] != null) _cameraManager.Bloom(pp["Bloom"]["Intensity"], pp["Bloom"]["Threshold"], 
                    new Color(pp["Bloom"]["Color"][0], pp["Bloom"]["Color"][1], pp["Bloom"]["Color"][2]));
                    if (pp["DoF"] != null) _cameraManager.Dof(pp["DoF"]["FocusDistance"], pp["DoF"]["Aperture"], 
                    pp["DoF"]["FocalLength"]);
                    if (pp["Lens Distortion"] != null) _cameraManager.Ld(pp["Lens Distortion"]["Intensity"]);
                    //if (pp["Motion Blur"] != null) _cameraManager.Mb(pp["Motion Blur"]["Intensity"]);
                    if (pp["Chromatic Aberration"] != null) _cameraManager.Ca(pp["Chromatic Aberration"]["Intensity"]);
                    if (pp["Vignette"] != null) _cameraManager.Vignette(pp["Vignette"]["Intensity"], 
                    pp["Vignette"]["Smoothness"], pp["Vignette"]["Roundness"], 
                    new Color(pp["Vignette"]["Color"][0], pp["Vignette"]["Color"][1], pp["Vignette"]["Color"][2]));
                }*/

                if (currentBeat["PP"] != null)
                {
                    JSONNode pp = currentBeat["PP"];
                    float duration = pp["Duration"] != null ? pp["Duration"].AsFloat : 1f;
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
