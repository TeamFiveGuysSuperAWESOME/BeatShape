using UnityEngine;

namespace Beat
{
    public class BeatData : MonoBehaviour
    {
        public double angle;
        public float speed;
    
        public float bpm = 120f;
        private float _timer;

        void Awake()
        {
        
        }

        /*void Refresh()
    {
        GameObject targetBoard = transform.parent.gameObject;
        Vector2 targetPos = targetBoard.transform.position;
        BeatboardData targetData = targetBoard.GetComponent<BeatboardData>();
        float targetSize = targetData.size;
        Debug.Log(timer);
        Debug.Log(timer >= bpm / 60); // BPM = 120
        
        
    }

    void Update()
    {
        timer += Time.deltaTime;
        Refresh();
        if(timer >= bpm / 60)
        {
            //Destroy(gameObject);
        }
    }*/
    }
}
