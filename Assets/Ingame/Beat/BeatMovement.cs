using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Beatboard;
using GameManager;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Beat
{
    public class BeatMovement : MonoBehaviour
    {
        private float _angle;
        private float _boardsize;
        private int _cycleOff;
        private int _sideOff;
        private float _bpm;
        private float _size;
        private Vector2 _pos;
        private string _easing;
        public int _sides;
        //private readonly float _amplitude = 1f;
        //private readonly float _offset = 0f;
        //private readonly float _rotationSpeed = -BeatboardManager.RotationSpeed;
        private float _elapsedTime = 0f;
        //private float _elapsedTime;
        private float _secondsPerBeat;
        private float _sineValue;
        private BeatManager beatmanager;
        private bool _hasPlayedSound = false;
        private bool _missedLogged = false;
        private bool _displayed = false;
        private int _displaying = 0;
        private readonly object _scoreLock = new object();
        
        void Awake() 
        {
            beatmanager = GameObject.FindWithTag("beatmanager").GetComponent<BeatManager>();
        }

        public void SetMovement(float angle, int sides, float boardsize, int cycleOffset, int sideOff, float bpm, Vector2 pos, string eas, float sze)
        {
            _angle = angle;
            _boardsize = boardsize;
            _cycleOff = cycleOffset;
            _sideOff = sideOff;
            _bpm = bpm;
            _size = sze;
            _pos = pos;
            _easing = eas;
            _sides = sides;

            _secondsPerBeat = 60f / _bpm;
        }
        
        public void TryRemoveBeatScored(float inputOffset)
        {
            BeatData beatData = GetComponent<BeatData>();
            
            lock(_scoreLock)
            {
                if (beatData.scored) return;
                
                if (MainGameManager.isCalibrating) {
                    if (inputOffset < -_secondsPerBeat) return;
                    MainGameManager.CBeatTimes.Add(inputOffset);
                    beatData.scored = true;
                    beatData.input_offset = -9999f;
                    _displayed = true;
                    StartCoroutine(RemoveBeatRoutine());
                    return;
                }

                if (inputOffset < -0.2f)
                {
                    MainGameManager.Judgement[0] += 1;
                    StartCoroutine(DisplayIndicator("Too EARLY", Color.red));
                    StartCoroutine(ChangeColorRoutine(new Color(0.5f, 0f, 0f)));
                    MainGameManager.Overload += 1;
                    return;
                }
                if (inputOffset > 0.2f)
                {
                    MainGameManager.Judgement[1] += 1;
                    StartCoroutine(DisplayIndicator("Too LATE", Color.red));
                    StartCoroutine(ChangeColorRoutine(new Color(0.5f, 0f, 0f)));
                    MainGameManager.Overload += 1;
                    return;
                }

                beatData.scored = true;
                beatData.input_offset = -9999f;

                switch (inputOffset) {
                    case float n when n < -0.16f:
                        MainGameManager.Judgement[2] += 1;
                        StartCoroutine(DisplayIndicator("EARLY", Color.red));
                        //StartCoroutine(ChangeColorRoutine(Color.red));
                        MainGameManager.Score += 1;
                        break;
                    case float n when n > 0.16f:
                        MainGameManager.Judgement[3] += 1;
                        StartCoroutine(DisplayIndicator("LATE", Color.red));
                        //StartCoroutine(ChangeColorRoutine(Color.red));
                        MainGameManager.Score += 1;
                        break;
                    case float n when n < -0.1f:
                        MainGameManager.Judgement[4] += 1;
                        StartCoroutine(DisplayIndicator("Early", Color.yellow));
                        //StartCoroutine(ChangeColorRoutine(Color.yellow));
                        MainGameManager.Score += 2;
                        break;
                    case float n when n > 0.1f:
                        MainGameManager.Judgement[5] += 1;
                        StartCoroutine(DisplayIndicator("Late", Color.yellow));
                        //StartCoroutine(ChangeColorRoutine(Color.yellow));
                        MainGameManager.Score += 2;
                        break;
                    default:
                        MainGameManager.Judgement[6] += 1;
                        StartCoroutine(DisplayIndicator("PERFECT", Color.green));
                        //StartCoroutine(ChangeColorRoutine(Color.green));
                        MainGameManager.Score += 4;
                        break;
                }
                StartCoroutine(RemoveBeatRoutine());
            }
        }

        private IEnumerator DisplayIndicator(string text, Color color = default)
        {
            _displaying += 1;
            GameObject scoreText = new("Indicator");
            scoreText.transform.localScale = new Vector3(0f, 0f, 1f);
            scoreText.transform.SetParent(GameObject.FindWithTag("Canvas").transform);
            scoreText.transform.position = new Vector3(0, 0, -1f);
            scoreText.layer = 5;
            var tmp = scoreText.AddComponent<TextMeshPro>();
            RectTransform rectTransform = scoreText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(300f, 150f);
            tmp.text = text;
            tmp.fontSize = 170;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = color == default ? Color.white : color;
            tmp.autoSizeTextContainer = true; 

            float animDuration = 0.1f;
            float holdDuration = 0.1f;
            float elapsedTime = 0f;
            while (elapsedTime < animDuration)
            {
                elapsedTime += Time.deltaTime;
                float scale = Easing.Ease(elapsedTime / animDuration, "outcubic");
                float yaxis = Easing.Ease(elapsedTime / animDuration, "outcubic") * 50f;
                scoreText.transform.localPosition = new Vector3(0, yaxis, -1f);
                scoreText.transform.localScale = new Vector3(scale, scale, 1);
                yield return null;
            }
            yield return new WaitForSeconds(holdDuration);
            elapsedTime = 0f;
            while (elapsedTime < animDuration)
            {
                elapsedTime += Time.deltaTime;
                float scale = Easing.Ease(1 - elapsedTime / animDuration, "incubic");
                scoreText.transform.localScale = new Vector3(scale, scale, 1);
                yield return null;
            }

            Destroy(scoreText);
            _displaying -= 1;
        }

        private IEnumerator ChangeColorRoutine(Color color) {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            Color startColor = spriteRenderer.color;
            float fadeTime = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.deltaTime;
                spriteRenderer.color = Color.Lerp(startColor, color, elapsedTime / fadeTime);
                yield return null;
            }
        }

        private IEnumerator RemoveBeatRoutine()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            //Color startColor = spriteRenderer.color;
            float startSize = transform.localScale.x;
            float outTime = 0.1f;
            float elapsedTime = 0f;

            while (elapsedTime < outTime)
            {
                elapsedTime += Time.deltaTime;
                //spriteRenderer.color = Color.Lerp(startColor, Color.clear, elapsedTime / fadeTime);
                transform.localScale = new Vector3(Mathf.Lerp(startSize, 0f, elapsedTime / outTime), Mathf.Lerp(startSize, 0f, elapsedTime / outTime), 1f);
                yield return null;
            }

            yield return new WaitUntil(() => _displaying == 0);
            yield return new WaitForSeconds(0.5f);
            Destroy(gameObject);
        }

        void Update()
        {
            if (MainGameManager.Paused) return;

            _elapsedTime += Time.deltaTime;
            float sideOff = _sideOff == 0 ? 0 : (float)_sideOff / _sides;
            float standardSpb = _secondsPerBeat * 4 * (_cycleOff + sideOff + 1);
            float standardTime = _elapsedTime - standardSpb;
            BeatData beatData = GetComponent<BeatData>();
            if (beatData.input_offset != -9999f) beatData.input_offset = standardTime;
            
            if (standardTime < 0f)
            {
                _sineValue = _elapsedTime/(standardSpb/2) < 1f ? Easing.Ease(_elapsedTime/(standardSpb/2), _easing) : Easing.Ease(2-(_elapsedTime/(standardSpb/2)), _easing);
                transform.localPosition = new Vector3(Mathf.Cos((Mathf.PI/180)*(_angle))*(_boardsize+_sineValue*_size*(_cycleOff + 1 + sideOff)), Mathf.Sin((Mathf.PI/180)*(_angle))*(_boardsize+_sineValue*_size*(_cycleOff + 1 + sideOff)), 100f);
                
                if (!_hasPlayedSound && Mathf.Abs(standardTime) < 0f)
                { 
                    beatmanager.Audio_Kick();
                    _hasPlayedSound = true;
                }
            }
            else 
            {
                transform.localScale = new Vector3(0,0,0);
                lock(_scoreLock)
                {
                    bool isMissed = !beatData.scored && 
                        ((!MainGameManager.isCalibrating && standardTime > 0.2f) ||
                        (MainGameManager.isCalibrating && standardTime > 0.5f));
                    
                    if (isMissed && !beatData.scored)
                    {
                        if (!_missedLogged)
                        {
                            MainGameManager.Judgement[7] += 1;
                            _missedLogged = true;
                        }
                        beatData.scored = true;
                        beatData.input_offset = -9999f;
                        StartCoroutine(RemoveBeatRoutine());

                        if (MainGameManager._debugTime > 0f) return;
                        MainGameManager.GameReallyEnded = true;
                        MainGameManager.IsGameOver = true;
                        MainGameManager.WhyGameOver = "¡MISSED!";
                    }
                }
            }
        }
    }
}