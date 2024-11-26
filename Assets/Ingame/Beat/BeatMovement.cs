using System;
using Beatboard;
using GameManager;
using UnityEngine;

namespace Beat
{
    public class BeatMovement : MonoBehaviour
    {
        private float _angle;
        private float _boardsize;
        private float _spd;
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

        public void SetMovement(float angle, int sides, float boardsize, float spd, float bpm, Vector2 pos, string eas, float sze)
        {
            _angle = angle;
            _boardsize = boardsize;
            _spd = spd;
            _bpm = bpm;
            _size = sze;
            _pos = pos;
            _easing = eas;
            _sides = sides;

            _secondsPerBeat = 60f / _bpm;
        }
        
        public void TryRemoveBeatScored()
        {
            float inputOffset = _elapsedTime - _secondsPerBeat * 4;
            if (inputOffset < -0.15f) //offset = -150 ~ 150ms
            {
                Debug.Log("Too Early! / " + inputOffset.ToString());
                MainGameManager.Overload += 1;
                return;
            }
            if (inputOffset > 0.15f)
            {
                Debug.Log("Too Late! / " + inputOffset.ToString());
                MainGameManager.Overload += 1;
                return;
            }
            switch (inputOffset) {
                case float n when n < -0.1f:
                    Debug.Log("Early! / " + inputOffset.ToString());
                    MainGameManager.Score += 1;
                    break;
                case float n when n > 0.1f:
                    Debug.Log("Late! / " + inputOffset.ToString());
                    MainGameManager.Score += 1;
                    break;
                case float n when n < -0.07f:
                    Debug.Log("Early / " + inputOffset.ToString());
                    MainGameManager.Score += 3;
                    break;
                case float n when n > 0.07f:
                    Debug.Log("Late / " + inputOffset.ToString());
                    MainGameManager.Score += 3;
                    break;
                default:
                    Debug.Log("Perfect / " + inputOffset.ToString());
                    MainGameManager.Score += 5;
                    break;
            }
            Destroy(gameObject);
        }

        private void Update()
        {
            if (MainGameManager.Paused) return;
            _elapsedTime += Time.deltaTime;
            //_elapsedTime = _elapsedTime;
            GetComponent<BeatData>().input_offset = _elapsedTime - (_secondsPerBeat*4);

            if(_elapsedTime - (_secondsPerBeat*4) < 0f) {
                _sineValue = _elapsedTime/(_secondsPerBeat*4/2) < 1f ? Easing.Ease(_elapsedTime/(_secondsPerBeat*4/2), _easing) : Easing.Ease(2-(_elapsedTime/(_secondsPerBeat*4/2)), _easing);
                transform.localPosition = new Vector3(Mathf.Cos((Mathf.PI/180)*(_angle))*(_boardsize+_sineValue*_size), Mathf.Sin((Mathf.PI/180)*(_angle))*(_boardsize+_sineValue*_size), 100f);
            }
            else {
                GetComponent<SpriteRenderer>().sprite = null;
                if(_elapsedTime - (_secondsPerBeat*4) > 0.15f) Destroy(gameObject);
            }
        }
    }
}