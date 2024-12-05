using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;
using Beatboard;

namespace GameManager
{
    public class CameraManager : MonoBehaviour
    {
        private Bloom bloom;
        private DepthOfField depthOfField;
        private LensDistortion lensDistortion;
        private MotionBlur motionBlur;
        private ChromaticAberration chromaticAberration;
        private Vignette vignette;
        private ColorGrading colorGrading;
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

        public void ShakeCamera(float intensity, float dur)
        {
            StartCoroutine(ShakeCameraForDuration(intensity, dur));
        }

        private IEnumerator ShakeCameraForDuration(float intensity, float dur)
        {
            Vector3 originalPos = Camera.main.transform.position;
            float time = 0;
            while (time < dur) 
            {
                Camera.main.transform.position = originalPos + Random.insideUnitSphere * intensity;
                time += Time.deltaTime;
                yield return null;
            }
            Camera.main.transform.position = originalPos;
        }

        public void Bloom(float intensity, float threshold, Color color, string easing = "linear", float duration = 1f)
        {
            StartCoroutine(Bloomer(intensity, threshold, color, easing, duration));
        }

        private IEnumerator Bloomer(float intensity, float threshold, Color color, string easing, float duration)
        {
            float originalIntensity = bloom.intensity.value;
            float originalThreshold = bloom.threshold.value;
            Color originalColor = bloom.color.value;
            if (intensity == -1234f) intensity = originalIntensity;
            if (threshold == -1234f) threshold = originalThreshold;
            if (color == new Color(-1234f, -1234f, -1234f)) color = originalColor;
            float time = 0f;
            while (time < duration) 
            {
                float t = Easing.Ease(time / duration, easing);
                bloom.intensity.value = Mathf.Lerp(originalIntensity, intensity, t);
                bloom.threshold.value = Mathf.Lerp(originalThreshold, threshold, t);
                bloom.color.value = Color.Lerp(originalColor, color, t);
                time += Time.deltaTime;
                yield return null;
            }
            bloom.intensity.value = intensity;
            bloom.threshold.value = threshold;
            bloom.color.value = color;
        }

        public void Dof(float distance, float aperture, float length, string easing = "linear", float duration = 1f)
        {
            StartCoroutine(Doffer(distance, aperture, length, easing, duration));
        }

        private IEnumerator Doffer(float distance, float aperture, float length, string easing, float duration)
        {
            float originalDistance = depthOfField.focusDistance.value;
            float originalAperture = depthOfField.aperture.value;
            float originalLength = depthOfField.focalLength.value;
            if (distance == -1234f) distance = originalDistance;
            if (aperture == -1234f) aperture = originalAperture;
            if (length == -1234f) length = originalLength;
            float time = 0f;
            while (time < duration) 
            {
                float t = Easing.Ease(time / duration, easing);
                depthOfField.focusDistance.value = Mathf.Lerp(originalDistance, distance, t);
                depthOfField.aperture.value = Mathf.Lerp(originalAperture, aperture, t);
                depthOfField.focalLength.value = Mathf.Lerp(originalLength, length, t);
                time += Time.deltaTime;
                yield return null;
            }
            depthOfField.focusDistance.value = distance;
            depthOfField.aperture.value = aperture;
            depthOfField.focalLength.value = length;
        }

        public void Ld(float intensity, string easing = "linear", float duration = 1f)
        {
            StartCoroutine(Lder(intensity, easing, duration));
        }

        private IEnumerator Lder(float intensity, string easing, float duration)
        {
            float originalIntensity = lensDistortion.intensity.value;
            if (intensity == -1234f) intensity = originalIntensity;
            float time = 0f;
            while (time < duration) 
            {
                float t = Easing.Ease(time / duration, easing);
                lensDistortion.intensity.value = Mathf.Lerp(originalIntensity, intensity, t);
                time += Time.deltaTime;
                yield return null;
            }
            lensDistortion.intensity.value = intensity;
        }

        public void Mb(float intensity, string easing = "linear", float duration = 1f)
        {
            StartCoroutine(Mber(intensity, easing, duration));
        }

        private IEnumerator Mber(float intensity, string easing, float duration)
        {
            float originalIntensity = motionBlur.shutterAngle.value;
            if (intensity == -1234f) intensity = originalIntensity;
            float time = 0f;
            while (time < duration) 
            {
                float t = Easing.Ease(time / duration, easing);
                motionBlur.shutterAngle.value = Mathf.Lerp(originalIntensity, intensity, t);
                time += Time.deltaTime;
                yield return null;
            }
            motionBlur.shutterAngle.value = intensity;
        }

        public void Ca(float intensity, string easing = "linear", float duration = 1f)
        {
            StartCoroutine(Caer(intensity, easing, duration));
        }

        private IEnumerator Caer(float intensity, string easing, float duration)
        {
            float originalIntensity = chromaticAberration.intensity.value;
            if (intensity == -1234f) intensity = originalIntensity;
            float time = 0f;
            while (time < duration) 
            {
                float t = Easing.Ease(time / duration, easing);
                chromaticAberration.intensity.value = Mathf.Lerp(originalIntensity, intensity, t);
                time += Time.deltaTime;
                yield return null;
            }
            chromaticAberration.intensity.value = intensity;
        }

        public void Vignette(float intensity, float smoothness, float roundness, Color color, string easing = "linear", float duration = 1f)
        {
            StartCoroutine(Vignetter(intensity, smoothness, roundness, color, easing, duration));
        }

        private IEnumerator Vignetter(float intensity, float smoothness, float roundness, Color color, string easing, float duration)
        {
            float originalIntensity = vignette.intensity.value;
            float originalSmoothness = vignette.smoothness.value;
            float originalRoundness = vignette.roundness.value;
            Color originalColor = vignette.color.value;
            if (intensity == -1234f) intensity = originalIntensity;
            if (smoothness == -1234f) smoothness = originalSmoothness;
            if (roundness == -1234f) roundness = originalRoundness;
            if (color == new Color(-1234f, -1234f, -1234f)) color = originalColor;
            float time = 0f;
            while (time < duration) 
            {
                float t = Easing.Ease(time / duration, easing);
                vignette.intensity.value = Mathf.Lerp(originalIntensity, intensity, t);
                vignette.smoothness.value = Mathf.Lerp(originalSmoothness, smoothness, t);
                vignette.roundness.value = Mathf.Lerp(originalRoundness, roundness, t);
                vignette.color.value = Color.Lerp(originalColor, color, t);
                time += Time.deltaTime;
                yield return null;
            }
            vignette.intensity.value = intensity;
            vignette.smoothness.value = smoothness;
            vignette.roundness.value = roundness;
            vignette.color.value = color;
        }

        public void Cg(float brightness, float contrast, float saturation, string easing = "linear", float duration = 1f)
        {
            StartCoroutine(Cger(brightness, contrast, saturation, easing, duration));
        }

        private IEnumerator Cger(float brightness, float contrast, float saturation, string easing, float duration)
        {
            float originalBrightness = colorGrading.postExposure.value;
            float originalContrast = colorGrading.contrast.value;
            float originalSaturation = colorGrading.saturation.value;
            if (brightness == -1234f) brightness = originalBrightness;
            if (contrast == -1234f) contrast = originalContrast;
            if (saturation == -1234f) saturation = originalSaturation;
            float time = 0f;
            while (time < duration) 
            {
                float t = Easing.Ease(time / duration, easing);
                colorGrading.postExposure.value = Mathf.Lerp(originalBrightness, brightness, t);
                colorGrading.contrast.value = Mathf.Lerp(originalContrast, contrast, t);
                colorGrading.saturation.value = Mathf.Lerp(originalSaturation, saturation, t);
                time += Time.deltaTime;
                yield return null;
            }
            colorGrading.postExposure.value = brightness;
            colorGrading.contrast.value = contrast;
            colorGrading.saturation.value = saturation;
        }

        public void ChangeBBColor(Color color, string easing = "linear", float duration = 1f)
        {
            StartCoroutine(ChangeBBColorer(color, easing, duration));
        }

        private IEnumerator ChangeBBColorer(Color color, string easing, float duration)
        {
            Color originalColor = BeatboardManager.BeatboardColor;
            float time = 0f;
            while (time < duration) 
            {
                float t = Easing.Ease(time / duration, easing);
                BeatboardManager.BeatboardColor = Color.Lerp(originalColor, color, t);
                time += Time.deltaTime;
                yield return null;
            }
            BeatboardManager.BeatboardColor = color;
        }

        public void ChangeBGColor(Color color, string easing = "linear", float duration = 1f)
        {
            StartCoroutine(ChangeBGColorer(color, easing, duration));
        }

        private IEnumerator ChangeBGColorer(Color color, string easing, float duration)
        {
            Color originalColor = Camera.main.backgroundColor;
            float time = 0f;
            while (time < duration) 
            {
                float t = Easing.Ease(time / duration, easing);
                Camera.main.backgroundColor = Color.Lerp(originalColor, color, t);
                time += Time.deltaTime;
                yield return null;
            }
            Camera.main.backgroundColor = color;
        }

        void Start()
        {
            var volume = FindObjectsByType<PostProcessVolume>(FindObjectsSortMode.None)[0].GetComponent<PostProcessVolume>();
            volume.profile.TryGetSettings(out bloom);
            volume.profile.TryGetSettings(out depthOfField);
            volume.profile.TryGetSettings(out lensDistortion);
            volume.profile.TryGetSettings(out motionBlur);
            volume.profile.TryGetSettings(out chromaticAberration);
            volume.profile.TryGetSettings(out vignette);
            volume.profile.TryGetSettings(out colorGrading);
        }
    }
}