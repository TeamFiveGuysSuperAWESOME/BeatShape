using UnityEngine;

public class Equalizer : MonoBehaviour
{
    MenuManager manager;
    public GameObject AP_obj;
    AudioPeer ap;
    RectTransform rt;

    public GameObject spectrum_prefab;
    GameObject[] spectrums;
    public int array = 40;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        ap = AP_obj.GetComponent<AudioPeer>();
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        spectrums = new GameObject[array*2];

        for(int i=0; i<array*2; i++) {
            spectrums[i] = Instantiate(spectrum_prefab, transform);
            spectrums[i].transform.localPosition = new Vector3(i, spectrums[i].transform.localPosition.y, spectrums[i].transform.localPosition.z);
            spectrums[i].GetComponent<SpriteRenderer>().color = manager.menuColor_dark;
        }
    }

    void Update()
    {
        for(int i=0; i<array; i++) {
            Vector3 targetScale = new Vector3(spectrums[i].transform.localScale.x, ap._samples[i]*2000, spectrums[i].transform.localScale.z);
            spectrums[i].transform.localScale = Vector3.Lerp(spectrums[i].transform.localScale, targetScale, 6f*Time.deltaTime);
        }
        for(int i=array; i<array*2; i++) {
            Vector3 targetScale = new Vector3(spectrums[i].transform.localScale.x, ap._samples[array*2-1 - i]*4000, spectrums[i].transform.localScale.z);
            spectrums[i].transform.localScale = Vector3.Lerp(spectrums[i].transform.localScale, targetScale, 6f*Time.deltaTime);
        }
    }
}
