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

    public GameObject thumb_obj;
    public GameObject lock_obj;
    public GameObject lockback_obj;
    public Material grayscale_mat;
    RawImage thumb_rawImage;

    void Awake()
    {
        manager = GameObject.FindWithTag("manager").GetComponent<MenuManager>();
        ls = GameObject.FindWithTag("levelselect").GetComponent<LevelSelectScene>();
        thumb_rawImage = thumb_obj.GetComponent<RawImage>();
        rawImage = GetComponent<RawImage>();
    }

    void Start()
    {
        rawImage.color = manager.menuColor_dark;
        //lock_obj.GetComponent<RawImage>().color = manager.menuColor_dark;
        lockback_obj.GetComponent<RawImage>().color = manager.menuColor_dark;
        ChangeThumbnail();
        if(stageNum != 0) {
            thumb_rawImage.material = grayscale_mat;
            lock_obj.SetActive(true);
        }
        else {
            thumb_rawImage.material = null;
            lock_obj.SetActive(false);
        }
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
