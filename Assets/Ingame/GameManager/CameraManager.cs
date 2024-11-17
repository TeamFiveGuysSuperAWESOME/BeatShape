using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameManager
{
    public class CameraManager : MonoBehaviour
    {
        public void MoveCamera(float x, float y, string easing, float dur)
        {
            StartCoroutine(MoveCameraToPosition(new Vector3(x, y, -10), easing, dur));
        }

        private IEnumerator MoveCameraToPosition(Vector3 endPos, string easing, float dur)
        {
            Vector3 startPos = Camera.main.transform.position;
            float time = 0;
            while (time < dur) 
            {
                Camera.main.transform.position = Vector3.Lerp(startPos, endPos, Easing.Ease(time / dur, easing));
                time += Time.deltaTime;
                yield return null;
            }
            Camera.main.transform.position = endPos;
        }

        public void ZoomCamera(float size, string easing, float dur)
        {
            StartCoroutine(ZoomCameraToSize(size, easing, dur));
        }
        
        private IEnumerator ZoomCameraToSize(float endSize, string easing, float dur)
        {
            float startSize = Camera.main.orthographicSize;
            float time = 0;
            while (time < dur) 
            {
                Camera.main.orthographicSize = Easing.Ease(time / dur, easing) * (endSize - startSize) + startSize;
                time += Time.deltaTime;
                yield return null;
            }
            Camera.main.orthographicSize = endSize;
        }

        public void RotateCamera(float angle, string easing, float dur)
        {
            StartCoroutine(RotateCameraToAngle(angle, easing, dur));
        }

        private IEnumerator RotateCameraToAngle(float endAngle, string easing, float dur)
        {
            float startAngle = Camera.main.transform.rotation.eulerAngles.z;
            float time = 0;
            while (time < dur) 
            {
                Camera.main.transform.rotation = Quaternion.Euler(0, 0, Easing.Ease(time / dur, easing) * (endAngle - startAngle) + startAngle);
                time += Time.deltaTime;
                yield return null;
            }
            Camera.main.transform.rotation = Quaternion.Euler(0, 0, endAngle);
        }
    }
}