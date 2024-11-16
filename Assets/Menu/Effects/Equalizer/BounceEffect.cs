using UnityEngine;

public class BounceEffect : MonoBehaviour
{
    public Vector2 standardScale;
    public float strength;
    public int array;

    public GameObject AP_obj;
    AudioPeer ap;

    void Awake()
    {
        ap = AP_obj.GetComponent<AudioPeer>();
    }

    void Update()
    {
        float value = ap._samples[array]*100*strength;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(standardScale.x+value,standardScale.y+value,1), Time.deltaTime*6f);
    }
}
