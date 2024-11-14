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
        private readonly float _amplitude = 1f;
        private readonly float _offset = 0f;
        private float _personalTimeOffset;
        private readonly float _rotationSpeed = -BeatboardManager.RotationSpeed;
        private float elapsedTime;
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
        }
        
        public void TryRemoveBeatScored()
        {
            if (Vector2.Distance(transform.position, _pos) > Math.Pow(_size, 1.3f) || sineValue > 0)
            {
                return;
            }
            Destroy(gameObject);
            Debug.Log("Beat removed");
            MainGameManager.Score += (int)Math.Pow(_size, 1.3f) / (int)Vector2.Distance(transform.position, _pos);
        }

        private void Update()
        {
            float secondsPerBeat = 60f / _bpm;
            elapsedTime = Time.time - _personalTimeOffset;
            //sineValue = Mathf.Sin(((elapsedTime / secondsPerBeat) * 0.5f * Mathf.PI * _spd) + _offset) * _amplitude;
            //sineValue = 
            //Easing.OutCubic((elapsedTime/secondsPerBeat))
            //adjustedSpeed = _size * 3.5f * sineValue;

            //transform.Translate(_direction * (adjustedSpeed * Time.deltaTime));
            //transform.RotateAround(_pos, Vector3.forward, _rotationSpeed * Time.deltaTime);
            //GetComponent<BeatData>().distance = Vector2.Distance(transform.position, _pos);

            //if (Vector2.Distance(transform.position, _pos) <= Math.Pow(_size, 1.1f) && sineValue <= 0)
            //{
            //    Destroy(gameObject);
            //}
            
            sineValue = elapsedTime/(secondsPerBeat*_sides/2) < 1f ? Easing.Ease(elapsedTime/(secondsPerBeat*_sides/2), _easing) : Easing.Ease(2-(elapsedTime/(secondsPerBeat*_sides/2)), _easing);
            transform.localPosition = new Vector3(Mathf.Cos((Mathf.PI/180)*(_angle))*(_boardsize+sineValue*_size), Mathf.Sin((Mathf.PI/180)*(_angle))*(_boardsize+sineValue*_size), transform.localPosition.z);

            if(elapsedTime/(secondsPerBeat*_sides) > 1f) Destroy(gameObject);

            Debug.Log(sineValue);
        }
    }
}