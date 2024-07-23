using Beatboard;
using UnityEngine;
using Random = System.Random;

namespace Beat
{
    public class BeatManager : MonoBehaviour
    {
        public GameObject beatPrefab;
        private readonly Random _random = new();

        public void CreateBeat(int sides, int side, float speed, Vector2 pos)
        {
            GameObject beatObject = Instantiate(beatPrefab, Vector2.zero, Quaternion.identity, transform);
            var angle = (360 / sides * side) + 180 / sides;

            // Set beatData
            BeatData beatData = beatObject.GetComponent<BeatData>();
            beatData.angle = angle;
            beatData.speed = speed;

            var angleRadians = angle * Mathf.Deg2Rad;

            // Calculate direction vector
            Vector3 direction = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
        
            Quaternion rotation = BeatboardManager.Rotation;

            Vector3 updatedDirection = rotation * new Vector3(direction.x, direction.y, 0f);

            // Calculate the position at the edge of the beatBoardObject
            Vector2 edgePosition = pos + (Vector2)updatedDirection * (20f / 2f);

            // Position the beatObject at the edge
            beatObject.transform.position = edgePosition;

            // Move the beatObject in the Update method
            beatObject.GetComponent<BeatMovement>().SetMovement(updatedDirection, speed, 20f);
        }

        private void Start()
        {
            // Start invoking the CreateBeatWrapper method repeatedly after a delay of beatInterval
            InvokeRepeating(nameof(CreateBeatWrapper), 1f, 1f);
            
        }

        private void CreateBeatWrapper()
        {
            var side = _random.Next(4);
            CreateBeat(4, side, 10f, new Vector2(0, 0));
            Debug.Log("create");
        }
        void Update()
        {
        
        }
    }
}