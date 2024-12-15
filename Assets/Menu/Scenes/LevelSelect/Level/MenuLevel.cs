using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuLevel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int stageNum;
    public Vector3 targetScale;
    public Texture[] thumbnails;

    MenuManager manager;
    LevelSelectScene ls;
    RawImage rawImage;

    public GameObject obj;
    public Material grayscale_mat;
    RawImage thumb_rawImage;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        ls = GameObject.FindWithTag("levelselect").GetComponent<LevelSelectScene>();
        thumb_rawImage = obj.GetComponent<RawImage>();
        rawImage = GetComponent<RawImage>();
    }

    void Start()
    {
        rawImage.color = manager.menuColor_dark;
        ChangeThumbnail();
        if(stageNum != 0) {thumb_rawImage.material = grayscale_mat;}
    }

    public void ChangeThumbnail()
    {
        if(stageNum < thumbnails.Length) {thumb_rawImage.texture = thumbnails[stageNum];}
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(stageNum+1 == MenuManager.levelNumber) {ls.Input_spaceDown();}
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if(stageNum+1 == MenuManager.levelNumber) {ls.Input_spaceUp();}
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 6f);
    }
}
