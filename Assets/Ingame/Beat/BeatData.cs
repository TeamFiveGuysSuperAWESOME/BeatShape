using System;
using UnityEngine;

namespace Beat
{
    public class BeatData : MonoBehaviour
    {
        public double angle;
        public float speed;
        public float input_offset;
        public bool missedLogged = false;
        public bool scored = false;
        public bool displayed = false;
    }
}
