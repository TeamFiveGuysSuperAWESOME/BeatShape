﻿using System;
using Beatboard;
using GameManager;
using UnityEngine;

namespace Beat
{
    public class BeatMovement : MonoBehaviour
    {
        private Vector2 _direction;
        private float _spd;
        private float _bpm;
        private float _size;
        private Vector2 _pos;
        private readonly float _amplitude = 1f;
        private readonly float _offset = 0f;
        private float _personalTimeOffset;
        private readonly float _rotationSpeed = -BeatboardManager.RotationSpeed;

        public void SetMovement(Vector2 dir, float spd, float bpm, Vector2 pos, float sze)
        {
            _direction = dir;
            _spd = spd;
            _bpm = bpm;
            _size = sze;
            _pos = pos;
            _personalTimeOffset = Time.time;
        }

        private void Update()
        {
            float secondsPerBeat = 60f / _bpm;
            float elapsedTime = Time.time - _personalTimeOffset;
            float sineValue = Mathf.Sin(((elapsedTime / secondsPerBeat) * 0.5f * Mathf.PI * _spd) + _offset) * _amplitude;
            float adjustedSpeed = _size * 3.5f * sineValue;

            transform.Translate(_direction * (adjustedSpeed * Time.deltaTime));
            transform.RotateAround(_pos, Vector3.forward, _rotationSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, _pos) <= Math.Pow(_size, 1.1f) && sineValue <= 0)
            {
                Destroy(gameObject);
            }

            if (Vector2.Distance(transform.position, _pos) <= Math.Pow(_size, 1.3f) && sineValue <= 0 && Input.anyKeyDown)
            {
                Destroy(gameObject);
                Debug.Log(Vector2.Distance(transform.position, _pos));
                MainGameManager.Score += (int)Math.Pow(_size, 1.3f) / (int)Vector2.Distance(transform.position, _pos);
            }
        }
    }
}