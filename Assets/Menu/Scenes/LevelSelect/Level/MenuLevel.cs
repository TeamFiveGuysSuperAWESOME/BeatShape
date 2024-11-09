using UnityEngine;
using UnityEngine.UI;

public class MenuLevel : MonoBehaviour
{
    public int stageNum;
    public Vector3 targetScale;
    public Texture[] thumbnails;

    MenuManager manager;
    RawImage rawImage;
    public GameObject obj;
    RawImage thumb_rawImage;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        thumb_rawImage = obj.GetComponent<RawImage>();
        rawImage = GetComponent<RawImage>();
    }

    void Start()
    {
        rawImage.color = manager.menuColor_dark;
        ChangeThumbnail();
    }

    public void ChangeThumbnail()
    {
        if(stageNum < thumbnails.Length) {thumb_rawImage.texture = thumbnails[stageNum];}
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 6f);
    }
}
