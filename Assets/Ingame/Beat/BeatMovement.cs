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
        private float _personalTimeOffset;
        //private readonly float _rotationSpeed = -BeatboardManager.RotationSpeed;
        private float elapsedTime;
        private float secondsPerBeat;
        private float sineValue;
        private float adjustedSpeed;

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
            _personalTimeOffset = Time.time;

            secondsPerBeat = 60f / _bpm;
        }
        
        public void TryRemoveBeatScored()
        {
            if (elapsedTime-(secondsPerBeat*_sides) < -0.15f) //offset = -100 ~ 100ms
            {
                Debug.Log("Too Early");
                return;
            }
            if (elapsedTime-(secondsPerBeat*_sides) > 0.15f)
            {
                Debug.Log("Too Late");
                return;
            }
            Destroy(gameObject);
            switch (elapsedTime-(secondsPerBeat*_sides)) {
                case float n when n < -0.075f:
                    Debug.Log("Early!");
                    break;
                case float n when n > 0.075f:
                    Debug.Log("Late!");
                    break;
                case float n when n < -0.025f:
                    Debug.Log("Early");
                    break;
                case float n when n > 0.025f:
                    Debug.Log("Late");
                    break;
                default:
                    Debug.Log("Perfect");
                    break;
            }
            MainGameManager.Score += 1;
        }

        private void Update()
        {
            elapsedTime = Time.time - _personalTimeOffset;
            GetComponent<BeatData>().input_offset = elapsedTime - (secondsPerBeat*_sides);

            if(elapsedTime - (secondsPerBeat*_sides) < 0f) {
                sineValue = elapsedTime/(secondsPerBeat*_sides/2) < 1f ? Easing.Ease(elapsedTime/(secondsPerBeat*_sides/2), _easing) : Easing.Ease(2-(elapsedTime/(secondsPerBeat*_sides/2)), _easing);
                transform.localPosition = new Vector3(Mathf.Cos((Mathf.PI/180)*(_angle))*(_boardsize+sineValue*_size), Mathf.Sin((Mathf.PI/180)*(_angle))*(_boardsize+sineValue*_size), 100f);
            }
            else {
                GetComponent<SpriteRenderer>().sprite = null;
                if(elapsedTime - (secondsPerBeat*_sides) > 0.15f) Destroy(gameObject);
            }
        }
    }
}