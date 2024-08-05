using System.Globalization;
using Beatboard;
using UnityEngine;
using Random = System.Random;

namespace Beat
{
    public class BeatManager : MonoBehaviour
    {
        public GameObject beatPrefab;
        private readonly Random _random = new();

        public void CreateBeat(int index, int side, float speed)
        {
            GameObject beatObject = Instantiate(beatPrefab, Vector2.zero, Quaternion.identity, transform);
            var pos = BeatboardManager.GetBeatboardPosition(index);
            var sides = (int)BeatboardManager.GetBeatboardPoints(index);
            int angle;
            if (sides % 2 == 0)
            {
                angle = (360 / sides) * (side) + 180 / sides - 90;
            }
            else
            {
                angle = (360 / sides) * (side) - 90;
            }
           

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
            Vector2 edgePosition = pos + (Vector2)updatedDirection * (BeatboardManager.GetBeatboardSize(index) - (BeatboardManager.GetBeatboardSize(index) / 2.8f));

            // Position the beatObject at the edge
            beatObject.transform.position = edgePosition;

            // Move the beatObject in the Update method
            beatObject.GetComponent<BeatMovement>().SetMovement(updatedDirection, speed, pos,  (BeatboardManager.GetBeatboardSize(index) - (BeatboardManager.GetBeatboardSize(index) / 3.2f)));
        }
        

        private void Start()
        {
            // Start invoking the CreateBeatWrapper method repeatedly after a delay of beatInterval
            InvokeRepeating(nameof(CreateBeatWrapper), 1f, 1f);
            
        }

        private void CreateBeatWrapper()
        {
            for (int i = 0; i <= BeatboardManager.Beatboards.Count; i++)
            {
                var points = (int)BeatboardManager.GetBeatboardPoints(i);
                var side = _random.Next(points);
                CreateBeat(i, side, 20f);
            }
        }
        void Update()
        {
        
        }
    }
}