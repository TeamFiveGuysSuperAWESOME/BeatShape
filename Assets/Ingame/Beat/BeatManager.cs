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
        
        public void CreateBeat(int index, int boardnum, float boardsize, int sides, int side, float speed, float bpm, float size, Color color, string easing)
        {
            Debug.Log("CreateBeat" + side);
            var pos = BeatboardManager.GetBeatboardPosition(index);
            GameObject beatObject = Instantiate(beatPrefab, pos, Quaternion.identity, GameObject.FindWithTag("boardmanager").GetComponent<BeatboardManager>().beatboards[boardnum-1].transform);
            beatObject.name = "Beat of board" + index;
            beatObject.GetComponent<SpriteRenderer>().material.color = color;
            beatObject.transform.localScale = new Vector3(size * 10f, size * 10f, 1f);
            int angle = -((360 / sides) * side - 180 / sides - 90);

            BeatData beatData = beatObject.GetComponent<BeatData>();
            beatData.angle = angle;
            beatData.speed = speed;
            beatData.displayed = false;
            beatData.scored = false;

            beatObject.transform.localPosition = new Vector3(Mathf.Cos((Mathf.PI/180)*(angle))*boardsize, Mathf.Sin((Mathf.PI/180)*(angle))*boardsize, beatObject.transform.localPosition.z);

            beatObject.GetComponent<BeatMovement>().SetMovement(
                angle, sides, boardsize, speed, bpm, pos, easing,
                20f);
        }

        private void Start()
        {
        }

        void Update()
        {
        }
    }
}