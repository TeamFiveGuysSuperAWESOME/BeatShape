using System;
using System.Collections;
using System.Collections.Generic;
using GameManager;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Beatboard
{
    public class BeatboardManager : MonoBehaviour
    {
        public GameObject beatboardPrefab;
        public static List<GameObject> Beatboards = new();
        public List<GameObject> updateBeatboards;
        public static List<int> UpdateBbIndex = new();
        public Color beatboardColor = Color.white;
        public List<int> currentPoints;
        public const float RotationSpeed = 25f;
        public static Quaternion Rotation;
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");


        private void CreateBeatboard(float points, float size, Vector2 position, bool update, int index)
        {
            RemoveBeatboard("update", null, index);

            if (points < 3) points = 360f;

            GameObject beatboardObject = Instantiate(beatboardPrefab, position, Quaternion.identity, transform);
            beatboardObject.transform.rotation = Rotation;

            BeatboardData bbdata = beatboardObject.GetComponent<BeatboardData>();
            bbdata.points = points;
            bbdata.size = size;
            bbdata.position = position;

            if (update)
            {
                updateBeatboards.Add(beatboardObject);
                if (!UpdateBbIndex.Contains(index)) UpdateBbIndex.Add(index);
                beatboardObject.name = "BeatboardUpdate" + index;
            }
            else
            {
                if (index != -1)
                {
                    Beatboards[index] = beatboardObject;
                    beatboardObject.name = "Beatboard " + index;
                }
                else
                {
                    Beatboards.Add(beatboardObject);
                    beatboardObject.name = "Beatboard " + Beatboards.Count;
                }
            }

            MeshFilter meshFilter = beatboardObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = beatboardObject.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;

            Material beatboardMaterial = new Material(Shader.Find("Unlit/Color")) { color = beatboardColor };
            meshRenderer.material = beatboardMaterial;

            int numVertices = Mathf.CeilToInt(points) + 1;
            Vector3[] vertices = new Vector3[numVertices + 1];
            List<int> triangles = new List<int>();

            float angleStep = 360f / points;
            vertices[0] = Vector3.zero;

            for (int i = 0; i < numVertices - 1; i++)
            {
                float angle = (90f - i * angleStep) * Mathf.Deg2Rad;
                vertices[i + 1] = new Vector3(size * Mathf.Cos(angle), size * Mathf.Sin(angle), 0f);
            }

            if (points % 1 > 0)
            {
                float fractionalAngle = (90f - (numVertices - 1) * angleStep) * Mathf.Deg2Rad;
                vertices[numVertices] = Vector3.Lerp(vertices[numVertices - 1], new Vector3(size * Mathf.Cos(fractionalAngle), size * Mathf.Sin(fractionalAngle), 0f), points % 1);
            }

            for (int i = 1; i < numVertices; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add((i % (numVertices - 1)) + 1);
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            GameObject transparentOverlay = new GameObject("TransparentOverlay", typeof(MeshFilter), typeof(MeshRenderer))
            {
                transform = { rotation = Rotation, position = position, parent = beatboardObject.transform }
            };

            Material secondMaterial = new Material(Shader.Find("Standard"))
            {
                color = new Color(beatboardColor.r, beatboardColor.g, beatboardColor.b, 0.35f),
                renderQueue = -1
            };
            secondMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            secondMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            secondMaterial.SetInt(ZWrite, 0);
            secondMaterial.DisableKeyword("_ALPHATEST_ON");
            secondMaterial.EnableKeyword("_ALPHABLEND_ON");

            MeshFilter secondMeshFilter = transparentOverlay.GetComponent<MeshFilter>();
            MeshRenderer secondMeshRenderer = transparentOverlay.GetComponent<MeshRenderer>();
            secondMeshRenderer.material = secondMaterial;

            Mesh secondMesh = new Mesh();
            secondMeshFilter.mesh = secondMesh;

            Vector3[] secondVertices = new Vector3[numVertices + 1];
            secondVertices[0] = Vector3.zero;

            for (int i = 0; i < numVertices - 1; i++)
            {
                float angle = (90f - i * angleStep) * Mathf.Deg2Rad;
                secondVertices[i + 1] = new Vector3(1.5f * size * Mathf.Cos(angle), 1.5f * size * Mathf.Sin(angle), 0f);
            }

            if (points % 1 > 0)
            {
                float fractionalAngle = (90f - (numVertices - 1) * angleStep) * Mathf.Deg2Rad;
                secondVertices[numVertices] = Vector3.Lerp(secondVertices[numVertices - 1], new Vector3(1.5f * size * Mathf.Cos(fractionalAngle), 1.5f * size * Mathf.Sin(fractionalAngle), 0f), points % 1);
            }

            secondMesh.vertices = secondVertices;
            secondMesh.triangles = triangles.ToArray();
            secondMesh.RecalculateNormals();
        }
        
        public static float GetBeatboardPoints(int index)
        {
            if (index < 0)
            {
                return 0f;
            }
            return Beatboards[index].GetComponent<BeatboardData>().points == 0f ? 360f : Beatboards[index].GetComponent<BeatboardData>().points;
        }

        public static float GetBeatboardSize(int index)
        {
            if (index < 0 || index >= Beatboards.Count)
            {
                return 0f;
            }
            return Beatboards[index].GetComponent<BeatboardData>().size;
        }

        public static Vector2 GetBeatboardPosition(int index)
        {
            if (index < 0 || index >= Beatboards.Count)
            {
                return Vector2.zero;
            }
            return Beatboards[index].GetComponent<BeatboardData>().position;
        }


        private IEnumerator UpdateBeatboard(int _currentPoints, int nextPoints, float currentSize, float nextSize, Vector2 position, int index)
        {
            int pointDiff = Math.Abs(_currentPoints - nextPoints);
            float sizeDiff = Math.Abs(currentSize - nextSize);
            if (_currentPoints > nextPoints)
            {
                for (float i = 0; i <= pointDiff*20; i += pointDiff)
                {
                    float size = sizeDiff == 0 ? nextSize : currentSize + sizeDiff / 20 * i;
                    CreateBeatboard(_currentPoints-(i/20), size, position, true, index);
                    yield return new WaitForSeconds(0f);
                }
            } else if (_currentPoints < nextPoints)
            {
                for (float i = 0; i <= pointDiff*20; i += pointDiff)
                {
                    float size = sizeDiff == 0 ? nextSize : currentSize + sizeDiff / 20 * i;
                    CreateBeatboard(_currentPoints+(i/20), size, position, true, index);
                    yield return new WaitForSeconds(0f);
                }
            } else
            {
                for (float i = 0; i <= 20; i ++)
                {
                    CreateBeatboard(_currentPoints, currentSize + sizeDiff / 20 * i, position, true, index);
                    yield return new WaitForSeconds(0f);
                }
            }
            CreateBeatboard(nextPoints, nextSize, position, false, index);
            UpdateBbIndex.Remove(index);
        }

        private void RemoveBeatboard(String category, GameObject thing, int index)
        {
            if (category == "update")
            {
                try
                {
                    int bbIndex = UpdateBbIndex.IndexOf(index);
                    Destroy(updateBeatboards[bbIndex]);
                    updateBeatboards.RemoveAt(bbIndex);
                }
                catch (Exception) {}

            } else if (category == "certain")
            {
                Beatboards.Remove(thing);
                Destroy(thing);
            } else if (category == "all")
            {
                while (updateBeatboards.Count > 0)
                {
                    GameObject beatboardToRemove = updateBeatboards[0];
                    updateBeatboards.RemoveAt(0);
                    Destroy(beatboardToRemove);
                }
                while (Beatboards.Count > 0)
                {
                    GameObject beatboardToRemove = Beatboards[0];
                    Beatboards.RemoveAt(0);
                    Destroy(beatboardToRemove);
                }
            }

        }

        public void ManageBeatboard(GameObject _gameObject, int _currentPoints, int _nextPoints, float currentSize, float nextSize,
            Vector2 position)
        {
            int gameObjectIndex = -1;
            try
            {
                gameObjectIndex = Beatboards.IndexOf(_gameObject);
            }
            catch (Exception)
            {
                // ignored
            }

            if (!UpdateBbIndex.Contains(gameObjectIndex) && _gameObject != null)
            {
                if (_nextPoints < 3)
                {
                    _nextPoints = 360;
                }
                Destroy(_gameObject);
                StartCoroutine(UpdateBeatboard(_currentPoints, _nextPoints, currentSize, nextSize, position, gameObjectIndex));
                currentPoints[gameObjectIndex] = _nextPoints;
            }
            else if (!UpdateBbIndex.Contains(gameObjectIndex) && _gameObject == null)
            {
                CreateBeatboard(_nextPoints, nextSize, position, false, -1);
                currentPoints.Add(_nextPoints);
            }
        }

        void Start()
        {
            //old way to create beatboard
            //CreateBeatboard(0f, 20f, new Vector2(0, 0), false, -1);
            //currentPoints.Add(0);
        }

        void Update()
        {
            if (MainGameManager.GameStarted)
            {
                for (int i = Beatboards.Count - 1; i >= 0; i--)
                {
                    if (Beatboards[i] == null)
                    {
                        continue;
                    }

                    Beatboards[i].transform.Rotate(Vector3.back * (RotationSpeed * Time.deltaTime), Space.Self);
                    Rotation = Beatboards[i].transform.rotation;
                }
            }
        }
    }
}