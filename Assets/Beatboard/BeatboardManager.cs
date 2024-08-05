using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Beatboard
{
    public class BeatboardManager : MonoBehaviour
    {
        public GameObject beatboardPrefab;
        public static List<GameObject> Beatboards = new List<GameObject>();
        public List<GameObject> updateBeatboards;
        public List<int> updateBbIndex;
        public Color beatboardColor = Color.white;
        public List<int> currentPoints;
        public const float RotationSpeed = 25f;
        public static Quaternion Rotation;
        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");

        private void CreateBeatboard(float points, float size, Vector2 position, Boolean update, int index)
        {
            RemoveBeatboard("update", null, index);
             {
                 if (points < 3)
                 {
                     points = 360f;
                 }
                // Instantiate the beatboard object
                GameObject beatboardObject = Instantiate(beatboardPrefab, position, Quaternion.identity, transform);
                beatboardObject.transform.rotation = Rotation;

                // Set beatboard data
                BeatboardData bbdata = beatboardObject.GetComponent<BeatboardData>();
                bbdata.points = points;
                bbdata.size = size;
                bbdata.position = position;
                if (update == true)
                {
            
                    updateBeatboards.Add(beatboardObject);
                    if (!updateBbIndex.Contains(index))
                    {
                        updateBbIndex.Add(index);
                    }
                    beatboardObject.name = "BeatboardUpdate" + index;
                }
                else
                {
                    if (index != -1)
                    {
                        Beatboards.RemoveAt(index);
                        Beatboards.Insert(index, beatboardObject);
                        beatboardObject.name = "Beatboard " + index;
                    }
                    else
                    {
                        Beatboards.Add(beatboardObject);
                        beatboardObject.name = "Beatboard " + Beatboards.Count;
                    }
                }
        
                // Add Mesh components
                MeshFilter meshFilter = beatboardObject.AddComponent<MeshFilter>();
                MeshRenderer meshRenderer = beatboardObject.AddComponent<MeshRenderer>();
                Mesh mesh = new Mesh();
                meshFilter.mesh = mesh;

                // Create a new material with the Standard shader and set its color
                Material beatboardMaterial = new Material(Shader.Find("Unlit/Color"));
                beatboardMaterial.color = beatboardColor; // Set color
                meshRenderer.material = beatboardMaterial;

                // Calculate number of vertices and triangles
                int numVertices = Mathf.CeilToInt(points) + 1;
                Vector3[] vertices = new Vector3[numVertices + 1]; // +1 for the center point
                List<int> triangles = new List<int>();

                float angleStep = 360f / points;
                vertices[0] = Vector3.zero; // Center point

                // Calculate vertices
                for (int i = 0; i < numVertices - 1; i++)
                {
                    float angle = (90f - i * angleStep) * Mathf.Deg2Rad;
                    vertices[i + 1] = new Vector3(size * Mathf.Cos(angle), size * Mathf.Sin(angle), 0f);
                }

                // Handle fractional part
                if (points % 1 > 0)
                {
                    float fractionalAngle = (90f - (numVertices - 1) * angleStep) * Mathf.Deg2Rad;
                    Vector3 lastVertex = new Vector3(size * Mathf.Cos(fractionalAngle), size * Mathf.Sin(fractionalAngle), 0f);
                    vertices[numVertices] = Vector3.Lerp(vertices[numVertices - 1], lastVertex, points % 1);
                }

                // Generate triangles
                for (int i = 1; i < numVertices; i++)
                {
                    triangles.Add(0);
                    triangles.Add(i);
                    triangles.Add((i % (numVertices - 1)) + 1);
                }

                // Set mesh data
                mesh.vertices = vertices;
                mesh.triangles = triangles.ToArray();
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();  // Recalculate bounds for correct rendering
            
                GameObject transparentOverlay = new GameObject("TransparentOverlay");
                transparentOverlay.transform.rotation = Rotation;
                transparentOverlay.transform.position = position;
                transparentOverlay.transform.parent = beatboardObject.transform;

                // Add MeshFilter and MeshRenderer components to the second shape
                MeshFilter secondMeshFilter = transparentOverlay.AddComponent<MeshFilter>();
                MeshRenderer secondMeshRenderer = transparentOverlay.AddComponent<MeshRenderer>();

                // Create a new material with alpha 0.5 for the second shape
                Material secondMaterial = new Material(Shader.Find("Standard"));
                secondMaterial.color = new Color(beatboardColor.r, beatboardColor.g, beatboardColor.b, 0.35f);
                secondMaterial.renderQueue = 3000; // Set the render queue to a value higher than the opaque objects
                secondMeshRenderer.material = secondMaterial;

                // Enable blending for the second shape's material
                secondMaterial.SetInt(SrcBlend, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                secondMaterial.SetInt(DstBlend, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                secondMaterial.SetInt(ZWrite, 0); // Disable writing to the depth buffer
                secondMaterial.DisableKeyword("_ALPHATEST_ON"); // Disable alpha testing
                secondMaterial.EnableKeyword("_ALPHABLEND_ON"); // Enable alpha blending
                secondMaterial.renderQueue = 0; // Set the render queue to a value higher than the opaque objects

                // Create a new mesh for the second shape with increased size
                Mesh secondMesh = new Mesh();
                secondMeshFilter.mesh = secondMesh;

                // Calculate vertices for the second shape with increased size
                Vector3[] secondVertices = new Vector3[numVertices + 1];
                secondVertices[0] = Vector3.zero; // Center point

                for (int i = 0; i < numVertices - 1; i++)
                {
                    float angle = (90f - i * angleStep) * Mathf.Deg2Rad;
                    secondVertices[i + 1] = new Vector3((size + size / 2) * Mathf.Cos(angle), (size + size / 2) * Mathf.Sin(angle), 0f);
                }

                // Handle fractional part for the second shape
                if (points % 1 > 0)
                {
                    float fractionalAngle = (90f - (numVertices - 1) * angleStep) * Mathf.Deg2Rad;
                    Vector3 lastVertex = new Vector3((size + size / 2) * Mathf.Cos(fractionalAngle), (size + size / 2) * Mathf.Sin(fractionalAngle), 0f);
                    secondVertices[numVertices] = Vector3.Lerp(secondVertices[numVertices - 1], lastVertex, points % 1);
                }

                // Set mesh data for the second shape
                secondMesh.vertices = secondVertices;
                secondMesh.triangles = triangles.ToArray();
                secondMesh.RecalculateNormals(); 
            }
        }

        public static float GetBeatboardPoints(int index)
        {
            if (index < 0 || index >= Beatboards.Count)
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


        private IEnumerator UpdateBeatboard(int currentPoints, int nextPoints, float size, Vector2 position, int index)
        {
            int diff = Math.Abs(currentPoints - nextPoints);
            if (currentPoints > nextPoints)
            {
                for (float i = 0; i <= diff*20; i += diff)
                {
                    CreateBeatboard(currentPoints-(i/20), size, position, true, index);
                    yield return new WaitForSeconds(0f);
                }
            } else if (currentPoints < nextPoints)
            {
                for (float i = 0; i <= diff*20; i += diff)
                {
                    CreateBeatboard(currentPoints+(i/20), size, position, true, index);
                    yield return new WaitForSeconds(0f);
                }
            }
            CreateBeatboard(nextPoints, size, position, false, index);
            updateBbIndex.Remove(index);
        }

        private void RemoveBeatboard(String category, GameObject thing, int index)
        {
            if (category == "update")
            {
                try
                {
                    int bbIndex = updateBbIndex.IndexOf(index);
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

        public void ManageBeatboard(GameObject gameObject, int _currentPoints, int _nextPoints, float size,
            Vector2 position)
        {
            int gameObjectIndex = -1;
            try
            {
                gameObjectIndex = Beatboards.IndexOf(gameObject);
            }
            catch (Exception)
            {
                // ignored
            }

            if (!updateBbIndex.Contains(gameObjectIndex) && gameObject != null)
            {
                if (_nextPoints < 3)
                {
                    _nextPoints = 360;
                }
                Destroy(gameObject);
                StartCoroutine(UpdateBeatboard(_currentPoints, _nextPoints, size, position, gameObjectIndex));
                currentPoints[gameObjectIndex] = _nextPoints;
            
            }
            else if (!updateBbIndex.Contains(gameObjectIndex) && gameObject == null)
            {
                CreateBeatboard(_nextPoints, size, position, false, -1);
                currentPoints.Add(_nextPoints);
            }
        }

        void Start()
        {
            //old way to create beatboard
            //CreateBeatboard(0f, 20f, new Vector2(0, 0), false, -1);
            //currentPoints.Add(0);
        }
    
        private KeyCode[] _keyCodes = {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Keypad1,
            KeyCode.Keypad2,
            KeyCode.Keypad3,
            KeyCode.Keypad4,
            KeyCode.Keypad5,
            KeyCode.Keypad6,
            KeyCode.Keypad7,
            KeyCode.Keypad8,
            KeyCode.Keypad9
        };
    
    
        void Update()
        {
            foreach (GameObject beatboardObject in Beatboards)
            {
                beatboardObject.transform.Rotate(Vector3.back * RotationSpeed * Time.deltaTime, Space.Self);
                Rotation = beatboardObject.transform.rotation;
            }
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(_keyCodes[i]))
                {
                    int nextPoints = i + 1;
                    ManageBeatboard(Beatboards[0], currentPoints[0], nextPoints, GetBeatboardSize(1), GetBeatboardPosition(0));
                
                }
            }
            for (int i = 0; i < 9; i++)
            {
                if (Input.GetKeyDown(_keyCodes[i+9]))
                {
                    int nextPoints = i + 1;
                    ManageBeatboard(Beatboards[1], currentPoints[1], nextPoints, GetBeatboardSize(1), GetBeatboardPosition(1));
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                RemoveBeatboard("all", null, -1);
            }
        }
    }
}
