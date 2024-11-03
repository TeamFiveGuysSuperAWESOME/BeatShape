using System.Globalization;
using Beatboard;
using UnityEngine;
using Random = System.Random;

namespace Beat
{
    public class BeatManager : MonoBehaviour
    {
        public GameObject beatPrefab;
        public int beatIndex;
        
        public void CreateBeat(int index, int sides, int side, float speed, float bpm, float size, Color color)
        {
            var pos = BeatboardManager.GetBeatboardPosition(index);
            GameObject beatObject = Instantiate(beatPrefab, pos, Quaternion.identity, transform);
            beatObject.name = "Beat of board" + index;
            beatObject.GetComponent<SpriteRenderer>().material.color = color;
            beatObject.transform.localScale = new Vector3(size * 10f, size * 10f, 1f);
            int angle = (sides % 2 == 0) ? (360 / sides) * side + 180 / sides - 90 : (360 / sides) * side - 90;

            BeatData beatData = beatObject.GetComponent<BeatData>();
            beatData.angle = angle;
            beatData.speed = speed;

            var angleRadians = angle * Mathf.Deg2Rad;
            Vector3 direction = new Vector2(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
            Quaternion rotation = BeatboardManager.Rotation;
            Vector3 updatedDirection = rotation * new Vector3(direction.x, direction.y, 0f);
            Vector2 edgePosition = pos + (Vector2)updatedDirection * (BeatboardManager.GetBeatboardSize(index) - BeatboardManager.GetBeatboardSize(index) / 2.4f);
            beatObject.transform.position = edgePosition;

            beatObject.GetComponent<BeatMovement>().SetMovement(
                updatedDirection, speed, bpm, pos,
                BeatboardManager.GetBeatboardSize(index) - BeatboardManager.GetBeatboardSize(index) / 2.5f);
        }

        private void Start()
        {
        }

        void Update()
        {
        }
    }
}