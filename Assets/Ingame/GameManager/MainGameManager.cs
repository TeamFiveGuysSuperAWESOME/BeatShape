using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using System.IO;
using System.Net.Mime;
using Beat;
using Beatboard;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Runtime.InteropServices;
using System.Data;
using Unity.VisualScripting;
//using NativeFilePickerNamespace; // unable in WebGL
using UnityEngine.Android;
using UnityEngine.Networking;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.Rendering.PostProcessing;

namespace GameManager
{
    public class MainGameManager : MonoBehaviour
    {
        private static BeatboardManager _beatboardManager;
        private static BeatManager _beatManager;
        private static CameraManager _cameraManager;
        public static int LevelNumber;
        private AudioClip _levelAudioContent = null;
        private float volume;
        public static float sfxvolume;
        public static AudioClip kickSound;
        private static string _levelName, _levelDescription, _levelAuthor;
        public static float _bpm;
        private static float _offset;
        private static List<JSONNode> Boards = new();
        private static JSONNode _boardsData;
        private static List<int> _currentBoardPoints = new();
        private static List<double> _beatIntervals = new(), _nextBeatTimes = new();
        private static List<float> _currentBoardSizes = new();
        private double _startTime;
        public static float _debugTime = 0f;
        private float _musicLength;
        public static string JsonFilePath;
        private bool _isJsonFileLoaded = false;
        public static bool GameStarted = false;
        public static bool Paused = false;
        public static bool GameEnded = false;
        public static bool GameReallyEnded = false;
        public static bool IsGameOver = false;
        public static string WhyGameOver = string.Empty;
        private static bool ResultShown = false;
        private bool _nowYouCanLeave = false;
        public static int Score = 0;
        //public static int Combo = 0;
        public static float Overload = 0f;
        public static int[] Judgement = {0, 0, 0, 0, 0, 0, 0, 0};
        //public TextMeshProUGUI scoreText;
        private FadeInScreen screen;
        public TextAsset textFile;
        private GameHandler _gameHandler;
        public static bool DebugMode = MenuManager.DebugMode;
        private bool _isLeaving = false;
        private bool _isRestarting = false;
        private float animtimer = 0f;
        [SerializeField] private GameObject practiceIndicator;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Slider ffSlider;
        [SerializeField] private GameObject ffSliderObj;
        [SerializeField] private TextMeshProUGUI ffSliderPercent;
        [SerializeField] private TextMeshProUGUI ffSliderPercentFull;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject judgementPanel;
        [SerializeField] private TextMeshPro finalScoreText;
        [SerializeField] private TextMeshPro perfectText;
        [SerializeField] private TextMeshPro earlyText;
        [SerializeField] private TextMeshPro lateText;
        [SerializeField] private TextMeshPro earlyBadText;
        [SerializeField] private TextMeshPro lateBadText;
        [SerializeField] private TextMeshPro tooEarlyText;
        [SerializeField] private TextMeshPro tooLateText;
        [SerializeField] private GameObject missedText;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void UploadFile();
#endif

        private string levelJsonData;

        void Awake()
        {
            GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>().active = false;
            LevelNumber = MenuManager.levelNumber;
            screen = GameObject.FindWithTag("screen").GetComponent<FadeInScreen>();
            _beatboardManager = FindFirstObjectByType<BeatboardManager>();
            _beatManager = FindFirstObjectByType<BeatManager>();
            _cameraManager = FindFirstObjectByType<CameraManager>();
            _levelName = string.Empty;
            _levelDescription = string.Empty;
            _levelAuthor = string.Empty;
            _bpm = 0f;
            _offset = 0f;
            Boards.Clear();
            _boardsData = null;
            _currentBoardPoints.Clear();
            _beatIntervals.Clear();
            _nextBeatTimes.Clear();
            _currentBoardSizes.Clear();
            JsonFilePath = string.Empty;
            GameStarted = false;
            Score = 0;
            //Combo = 0;
            Judgement = new[] {0, 0, 0, 0, 0, 0, 0, 0};
            Paused = false;
            GameEnded = false;
            GameReallyEnded = false;
            IsGameOver = false;
            WhyGameOver = string.Empty;
            ResultShown = false;
            DebugMode = MenuManager.DebugMode;
            volume = MenuSoundManager.musicVolume;
            sfxvolume = MenuSoundManager.sfxVolume;
            kickSound = Resources.Load<AudioClip>("Sounds/kickdrum");
            gameOverPanel.SetActive(false);
            practiceIndicator.SetActive(!_debugTime.Equals(0f));
            ffSliderObj.SetActive(!_debugTime.Equals(0f));
        }

        public static MainGameManager Instance { get; private set; }

        void Start()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                NativeFilePicker.Permission permission = NativeFilePicker.CheckPermission();
                if (permission != NativeFilePicker.Permission.Granted)
                {
                    NativeFilePicker.RequestPermission();
                }
            } // android permission

            Debug.Log("Debug Mode: " + DebugMode);
            if (DebugMode)
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                UploadFile();
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
                StartCoroutine(AndroidLoadCustomLevel());
#endif
#if UNITY_EDITOR
            StartGame();
#endif
            }
            else
            {
                StartGame();
            }


            StartCoroutine(DecreaseOverloadRoutine());
        }

        public IEnumerator AndroidLoadCustomLevel() {
            var jsonType = NativeFilePicker.ConvertExtensionToFileType("json");
            var oggType = NativeFilePicker.ConvertExtensionToFileType("ogg");
            string levelFileContent = null;
            _levelAudioContent = null;
            string uri = null;
            NativeFilePicker.PickFile((path) =>
            {
                if (path == null)
                {
                    Debug.Log("failed to load file");
                    LoadMainMenu();
                    return;
               }
                //Debug.Log($"file path: {path}");
                levelFileContent = File.ReadAllText(path);
            }, new string[] {jsonType});
            yield return new WaitUntil(() => levelFileContent != null);
            Debug.Log("json loaded");
            UnityWebRequest audioLoader = null;
            NativeFilePicker.PickFile((path) =>
            {
                if (path == null)
                {
                    Debug.Log("failed to load file");
                    LoadMainMenu();
                    return;
               }
                //Debug.Log($"file path: {path}");
                var builder = new UriBuilder(path) {Scheme = Uri.UriSchemeFile}; 
                uri = builder.ToString();
            }, new string[] {oggType});
            yield return new WaitUntil(() => uri != null);
            audioLoader = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.OGGVORBIS);
            audioLoader.SendWebRequest();
            yield return new WaitUntil(() => audioLoader.isDone);
            Debug.Log("audio loaded");
            _levelAudioContent = DownloadHandlerAudioClip.GetContent(audioLoader);
            yield return new WaitUntil(() => _levelAudioContent != null);
            Debug.Log($"file content: {levelFileContent}");
            OnFileUpload(levelFileContent);
            yield return null;
        }

        public void OnFileUpload(string jsonData)
        {
            levelJsonData = jsonData;
            StartGame();
            _isJsonFileLoaded = true;
        }

        public void StartGame()
        {
            if (GameStarted) return;

            textFile = Resources.Load<TextAsset>("Levels/" + LevelNumber + "/level");
            //textFile = 
            var levelString = textFile.text;
            if (DebugMode)
            {
#if (UNITY_WEBGL||UNITY_ANDROID) && !UNITY_EDITOR
                levelString = levelJsonData;
#endif
#if UNITY_EDITOR
            levelString = textFile.text;
#endif
            }
            else
            {
                levelString = textFile.text;
            }
            var levelDataJsonNode = JSON.Parse(levelString)["Data"];
            _boardsData = JSON.Parse(levelString)["Boards"];

            _levelName = levelDataJsonNode["LevelName"];
            _levelDescription = levelDataJsonNode["LevelDescription"];
            _levelAuthor = levelDataJsonNode["LevelAuthor"];
            var levelAudio = levelDataJsonNode["AudioFile"];
            GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Levels/1/" + levelAudio);
            if (_levelAudioContent != null)
            {
                GetComponent<AudioSource>().clip = _levelAudioContent;
            }
            _musicLength = GetComponent<AudioSource>().clip.length;
            ffSlider.value = _debugTime / _musicLength;
            ffSliderPercentFull.text = (Mathf.Round(_musicLength)).ToString();
            _bpm = levelDataJsonNode["Bpm"];
            _offset = levelDataJsonNode["Offset"] / 1000f;

            foreach (var board in levelDataJsonNode["Boards"]) Boards.Add(board);
            CreateBeatboardAtStart(Boards);

            _startTime = 30 / _bpm + _offset - _debugTime;
#if UNITY_ANDROID && !UNITY_EDITOR
            _startTime -= 0.15f;
#endif

            for (var i = 0; i < Boards.Count; i++)
            {
                _beatIntervals.Insert(i, 60f / _bpm / Boards[i]["points"]);
                _nextBeatTimes.Insert(i, _beatIntervals[i]);
            }

            foreach (var t in Boards) _currentBoardPoints.Add(t["points"]);
            foreach (var t in Boards) _currentBoardSizes.Add(t["size"]);

            var background = levelDataJsonNode["Background"];
            BeatboardManager.BeatboardColor = new Color(background["BBColor"]?[0]?.AsFloat ?? 0, background["BBColor"]?[1]?.AsFloat ?? 0, background["BBColor"]?[2]?.AsFloat ?? 0);
            Camera.main.backgroundColor = new Color(background["BGColor"]?[0]?.AsFloat ?? 1, background["BGColor"]?[1]?.AsFloat ?? 1, background["BGColor"]?[2]?.AsFloat ?? 1);
            if (background["BGImage"] != null)
            {
                var backgroundSprite = Resources.Load<Sprite>("Levels/1/" + background["BGImage"]);
                if (backgroundSprite != null)
                {
                    GameObject backgroundObj = new("Background");
                    //backgroundObj.transform.SetParent(Camera.main.transform, false);
                    backgroundObj.tag = "BackgroundImg";
                    SpriteRenderer spriteRenderer = backgroundObj.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = backgroundSprite;
                    float screenHeight = Camera.main.orthographicSize * 2;
                    float screenWidth = screenHeight * Screen.width / Screen.height;
                    Vector2 spriteSize = backgroundSprite.bounds.size;
                    float scale = Mathf.Max(screenWidth / spriteSize.x, screenHeight / spriteSize.y);
                    backgroundObj.transform.localScale = new Vector3(scale, scale, 1);
                    backgroundObj.transform.position = new Vector3(0, 0, 200);
                    spriteRenderer.sortingLayerName = "Background";
                    spriteRenderer.sortingOrder = -1;
                }
            }
        }
        
        void FixedUpdate()
        {
            if (!GameStarted) return;
            _gameHandler.HandleGame();
        }

        void Update()
        {
            if (_isLeaving) 
            {
                screen.screenState = "FadeIn";
                animtimer += Time.deltaTime;
                if (animtimer > 0.75f) { _isLeaving = false; SceneManager.LoadScene("MainMenu"); }
            }
            if (_isRestarting) 
            {
                screen.screenState = "FadeIn";
                animtimer += Time.deltaTime;
                if (animtimer > 0.75f) { _isRestarting = false; SceneManager.LoadScene("Ingame"); }
            }

            if (!GameStarted) {
                GameObject.FindWithTag("countdown").GetComponent<TextMeshProUGUI>().text = "Space to Start";
                if (Paused) return;
                if (_isJsonFileLoaded) GameObject.FindWithTag("countdown").GetComponent<CountDownManager>().RefreshTimer(60f / _bpm, 30 / _bpm + _offset, Boards[0]["points"]);
                if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)) {
                    if (EventSystem.current.currentSelectedGameObject) return;
                    _startTime += AudioSettings.dspTime;
                    _gameHandler = gameObject.AddComponent<GameHandler>();
                    _gameHandler.Initialize(
                        _beatboardManager,
                        _beatManager,
                        _cameraManager,
                        Boards,
                        _boardsData,
                        _currentBoardPoints,
                        _beatIntervals,
                        _nextBeatTimes,
                        _currentBoardSizes,
                        _startTime,
                        _bpm
                    );
                    GetComponent<AudioSource>().time = _debugTime;
                    GetComponent<AudioSource>().volume = volume;
                    GetComponent<AudioSource>().Play();
                    GameStarted = true;
                }
                return;
            }

            //scoreText.text = Overload.ToString();
            if (Overload >= 3 && _debugTime == 0f)
            {
                GameReallyEnded = true;
                IsGameOver = true;
                WhyGameOver = "¡OVERLOADED!";
            }

            if (GameReallyEnded)
            {
                if (!ResultShown) {
                    StartCoroutine(GameEndFlash());
                    Debug.Log("Game Ended");
                    gameOverPanel.SetActive(true);

                    SetTextRenderQueue(finalScoreText, 3002);
                    SetTextRenderQueue(perfectText, 3002);
                    SetTextRenderQueue(earlyText, 3002);
                    SetTextRenderQueue(lateText, 3002);
                    SetTextRenderQueue(earlyBadText, 3002);
                    SetTextRenderQueue(lateBadText, 3002);
                    SetTextRenderQueue(tooEarlyText, 3002);
                    SetTextRenderQueue(tooLateText, 3002);
                    
                    finalScoreText.text = _debugTime == 0 ? "Score: " + Score : "Practice Mode";
                    missedText.SetActive(_debugTime > 0);

                    ResultAudio resultAudio = GameObject.FindWithTag("resultAudio").GetComponent<ResultAudio>();
                    if (IsGameOver) 
                    {
                        resultAudio.Audio_Snare();
                        finalScoreText.text = WhyGameOver;
                        judgementPanel.SetActive(false);
                    }
                    else {resultAudio.Audio_Cymbal();}

                    perfectText.text = Judgement[6].ToString();
                    earlyText.text = Judgement[2].ToString();
                    lateText.text = Judgement[3].ToString();
                    earlyBadText.text = Judgement[4].ToString();
                    lateBadText.text = Judgement[5].ToString();
                    tooEarlyText.text = Judgement[0].ToString();
                    tooLateText.text = Judgement[1].ToString();
                    missedText.GetComponent<TextMeshPro>().text = Judgement[7].ToString();
                    StartCoroutine(Wait(1.5f));
                    ResultShown = true;
                }

                if(_nowYouCanLeave && (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))) {
                    if (EventSystem.current.currentSelectedGameObject) return;
                    if (IsGameOver) 
                    {
                        if (_debugTime == 0f) {Restart(); return;}
                        if (_debugTime > 0f) {RestartAsPracMode(); return;}
                    }
                    if (_debugTime > 0f) {Restart(); return;}
                    LoadMainMenu();
                }
            }
        }
        private IEnumerator GameEndFlash()
        {
            var pp = GetComponent<PostProcessVolume>().profile.GetSetting<ColorGrading>();
            pp.active = true;
            float originalBrightness = pp.postExposure.value;
            for (float i = 0; i < 0.01f; i += Time.deltaTime)
            {
                pp.postExposure.value = i * 5000 + originalBrightness;
                yield return null;
            }
            yield return new WaitForSeconds(0.01f);
            for (float i = 0.5f; i > 0; i -= Time.deltaTime)
            {
                pp.postExposure.value = i * 50 + originalBrightness;
                yield return null;
            }
        }

        private IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _nowYouCanLeave = true;
        }

        private void SetTextRenderQueue(TextMeshPro text, int queueValue)
        {
            Material newMaterial = new Material(text.material);
            newMaterial.renderQueue = queueValue;
            text.material = newMaterial;
        }

        private static void CreateBeatboardAtStart(List<JSONNode> boards)
        {
            foreach (var board in boards)
            {
                int points = board["points"];
                Vector2 position = new Vector2(board["position"][0], board["position"][1]);
                float size = board["size"];
                _beatboardManager.ManageBeatboard(null, -1, points, 0, size, position);
            }
        }

        private IEnumerator DecreaseOverloadRoutine()
        {
            while (!GameReallyEnded) 
            {
                if (Overload >= 0.5f)
                {
                    Overload -= 0.5f;
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        public void LoadMainMenu()
        {
            if (_isLeaving || _isRestarting) return;
            animtimer = 0f;
            _debugTime = 0f;
            _isLeaving = true;
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }
        public void Restart()
        {
            if (_isLeaving || _isRestarting) return;
            animtimer = 0f;
            _isRestarting = true;
            _debugTime = 0f;
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }

        public void RestartAsPracMode()
        {
            if (_isLeaving || _isRestarting) return;
            if (_debugTime == 0f) _debugTime = 0.01f;
            animtimer = 0f;
            _isRestarting = true;
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }

        public void OnSliderMove() 
        {
            _debugTime = _musicLength * ffSlider.value;
            ffSliderPercent.text = "Start From " + Mathf.Round(_debugTime) + "s";
        }
    }
}