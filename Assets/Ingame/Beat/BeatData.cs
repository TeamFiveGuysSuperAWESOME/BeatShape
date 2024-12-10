using System;
using System.Threading;
using UnityEngine;

namespace Beat
{
    public class BeatData : MonoBehaviour
    {
        private int _scoredFlag = 0;
        public double angle;
        public float speed;
        public float input_offset = -9998f;
        public bool missedLogged = false;
        public bool scored 
        { 
            get { return _scoredFlag == 1; }
            set 
            {
                // Interlocked.Exchange를 사용해 원자적으로 상태 변경
                Interlocked.Exchange(ref _scoredFlag, value ? 1 : 0);
            }
        }
        public bool displayed = false;
    }
}
