using System.Globalization;
using Beatboard;
using UnityEngine;
using Random = System.Random;

namespace Beat
{
    public class BeatManager : MonoBehaviour
    {
        AudioSource audioSource;

        public GameObject beatPrefab;
        public int beatIndex;
        
        public void CreateBeat(int index, int boardnum, float boardsize, int sides, int side, int cycleOffset, int sideTimeOffset, int customSide,  float bpm, float size, Color color, string easing)
        {
            var pos = BeatboardManager.GetBeatboardPosition(index);
            GameObject beatObject = Instantiate(beatPrefab, pos, Quaternion.identity, GameObject.FindWithTag("boardmanager").GetComponent<BeatboardManager>().beatboards[boardnum-1].transform);
            beatObject.name = "Beat of board" + index;
            beatObject.GetComponent<SpriteRenderer>().material.color = color;
            beatObject.transform.localScale = new Vector3(size * 10f, size * 10f, 1f);
            int adjustedSide = customSide == 0 ? side : customSide;
            int angle = -((360 / sides) * adjustedSide - 180 / sides - 90);

            BeatData beatData = beatObject.GetComponent<BeatData>();
            beatData.angle = angle;
            beatData.displayed = false;
            beatData.scored = false;
            beatData.input_offset = -9998f;

            beatObject.transform.localPosition = new Vector3(Mathf.Cos((Mathf.PI/180)*(angle))*boardsize, Mathf.Sin((Mathf.PI/180)*(angle))*boardsize, beatObject.transform.localPosition.z);

            beatObject.GetComponent<BeatMovement>().SetMovement(
                angle, sides, boardsize, cycleOffset, sideTimeOffset == 0 ? 0 : sideTimeOffset - side, bpm, pos, easing,
                20f);
        }

        public void Audio_Kick()
        {
            audioSource.Play();
        }

        public static void ChangeSfxVolume()
        {
            GameObject.FindWithTag("beatmanager").GetComponent<AudioSource>().volume = MenuSoundManager.sfxVolume;
        }

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = MenuSoundManager.sfxVolume;
        }
        private void Start()
        {
            audioSource.volume = MenuSoundManager.sfxVolume;
        }

        void Update()
        {
        }
    }
}