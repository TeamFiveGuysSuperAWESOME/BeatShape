using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beatboardManager : MonoBehaviour
{
    public GameObject beatboardPrefab;
    public List<GameObject> beatboards;
    public List<GameObject> updateBeatboards;
    public List<int> updateBbIndex;
    public Color beatboardColor = Color.white;
    public List<int> currentPoints;
    
    public void createBeatboard(float points, float size, Vector2 position, Boolean update, int index)
    {
        removeBeatboard("update", null, index);
        // Instantiate the beatboard object
        GameObject beatboardObject = Instantiate(beatboardPrefab, position, Quaternion.identity, transform);

        // Set beatboard data
        beatboardData bbdata = beatboardObject.GetComponent<beatboardData>();
        bbdata.points = points;
        bbdata.size = size;
        if (update == true)
        {
            
            updateBeatboards.Add(beatboardObject);
            updateBbIndex.Add(index);
            beatboardObject.name = "BeatboardUpdate";
        }
        else
        {
            if (index != -1)
            {
                beatboards.RemoveAt(index);
                beatboards.Insert(index, beatboardObject);
                beatboardObject.name = "Beatboard " + index;
            }
            else
            {
                beatboards.Add(beatboardObject);
                beatboardObject.name = "Beatboard " + beatboards.Count;
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
    }


    public IEnumerator updateBeatboard(int currentPoints, int nextPoints, float size, Vector2 position, int index)
    {
        int diff = Math.Abs(currentPoints - nextPoints);
        if (currentPoints > nextPoints)
        {
            for (float i = 0; i <= diff*20; i += diff)
            {
                createBeatboard(currentPoints-(i/20), size, position, true, index);
                yield return new WaitForSeconds(0f);
            }
        } else if (currentPoints < nextPoints)
        {
            for (float i = 0; i <= diff*20; i += diff)
            {
                createBeatboard(currentPoints+(i/20), size, position, true, index);
                yield return new WaitForSeconds(0f);
            }
        }
        createBeatboard(nextPoints, size, position, false, index);
    }

    public void removeBeatboard(String category, GameObject thing, int index)
    {
        if (category == "update")
        {
            try
            {
                int bbIndex = updateBbIndex.IndexOf(index);
                updateBbIndex.Remove(index);
                Destroy(updateBeatboards[bbIndex]);
                updateBeatboards.RemoveAt(bbIndex);
            }
            catch (Exception) {}
                
        } else if (category == "certain")
        {
            beatboards.Remove(thing);
            Destroy(thing);
        } else if (category == "all")
        {
            while (updateBeatboards.Count > 0)
            {
                GameObject beatboardToRemove = updateBeatboards[0];
                updateBeatboards.RemoveAt(0);
                Destroy(beatboardToRemove);
            }
            while (beatboards.Count > 0)
            {
                GameObject beatboardToRemove = beatboards[0];
                beatboards.RemoveAt(0);
                Destroy(beatboardToRemove);
            }
        }
        
    }

    public void manageBeatboard(GameObject gameObject, int currentPoints, int nextPoints, float size, Vector2 position)
    {
        int gameObjectIndex = -1;
        try
        {
            gameObjectIndex = beatboards.IndexOf(gameObject);
        } catch (Exception) {}
        if (updateBbIndex.Contains(gameObjectIndex) || gameObject != null)
        {
            Destroy(gameObject);
            StartCoroutine(updateBeatboard(currentPoints, nextPoints, size, position, gameObjectIndex));
        } else if (gameObject == null)
        {
            createBeatboard(nextPoints, size, position, false, -1);
            Debug.Log("create");
        }

    }

    void Start()
    {
        createBeatboard(4.5f, 20f, new Vector2(0, 0), false, -1);
        currentPoints.Add(4);
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
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(_keyCodes[i]))
            {
                int nextPoints = i+3;
                manageBeatboard(beatboards[0], currentPoints[0], nextPoints, 20f, new Vector2(0, 0));
                currentPoints[0] = nextPoints;
            }
        }
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(_keyCodes[i+9]))
            {
                int nextPoints = i+3;
                manageBeatboard(beatboards[1], currentPoints[1], nextPoints, 20f, new Vector2(60, 0));
                currentPoints[1] = nextPoints;
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            removeBeatboard("all", null, -1);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            manageBeatboard(null, 0, 12, 20f, new Vector2(60, 0));
            currentPoints.Add(12);
        }
    }
}
