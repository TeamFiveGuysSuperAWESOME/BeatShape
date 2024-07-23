using UnityEngine;

public class BeatMovement : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private float maxDistance = 50f; // Adjust this value to set the desired altitude
    private float amplitude = 1f; // Adjust this value to control the amplitude of the sine wave
    private float frequency = 1f; // Adjust this value to control the frequency of the sine wave
    private float offset = 0f; // Adjust this value to shift the sine wave horizontally
    private float personalTimeOffset; // Time offset for each object

    public void SetMovement(Vector2 dir, float spd)
    {
        direction = dir;
        speed = spd;
        personalTimeOffset = Time.time; // Set the personal time offset when the object is created
    }

    private void Update()
    {
        // Calculate the sine wave value using the personal time offset
        float sineValue = Mathf.Sin(((Time.time - personalTimeOffset) * frequency) + offset) * amplitude;

        // Apply the sine wave value to the speed
        float adjustedSpeed = speed * sineValue * 2;

        // Move the object in the specified direction with the adjusted speed
        transform.Translate(direction * adjustedSpeed * Time.deltaTime);

        // Check if the object has reached the BeatBoardObject
        if (Vector2.Distance(transform.position, Vector2.zero) <= 1f)
        {
            // Deactivate or destroy the beat object
            Destroy(gameObject); // Deactivate the game object
        }
    }
}