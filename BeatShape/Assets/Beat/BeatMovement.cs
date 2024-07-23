using Beatboard;
using UnityEngine;

namespace Beat
{
    public class BeatMovement : MonoBehaviour
    {
        private Vector2 _direction;
        private float _speed;
        private float _size;
        private readonly float _amplitude = 1f; // Adjust this value to control the amplitude of the sine wave
        private readonly float _frequency = 1f; // Adjust this value to control the frequency of the sine wave
        private readonly float _offset = 0f; // Adjust this value to shift the sine wave horizontally
        private float _personalTimeOffset; // Time offset for each object
        private readonly float _rotationSpeed = -BeatboardManager.RotationSpeed;

        public void SetMovement(Vector2 dir, float spd, float sze)
        {
            _direction = dir;
            _speed = spd;
            _size = sze;
            _personalTimeOffset = Time.time;
        }


        private void Update()
        {
            // Calculate the sine wave value using the personal time offset
            float sineValue = Mathf.Sin(((Time.time - _personalTimeOffset) * _frequency) + _offset) * _amplitude;

            // Apply the sine wave value to the speed
            float adjustedSpeed = _speed * sineValue * 3;

            // Move the object in the specified direction with the adjusted speed
            transform.Translate(_direction * (adjustedSpeed * Time.deltaTime));
        
            transform.RotateAround(Vector3.zero, Vector3.forward, _rotationSpeed * Time.deltaTime);

            // Check if the object has reached the BeatBoardObject
            if (Vector2.Distance(transform.position, Vector2.zero) <= _size + 0.01f && sineValue <= 0)
            {
                // Deactivate or destroy the beat object
                Destroy(gameObject); // Deactivate the game object
            }
        }
    }
}