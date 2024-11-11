using UnityEngine;
using UnityEngine.UI;

public class Effect_Square : MonoBehaviour
{
    public GameObject mask;
    public Color color;
    public float length;
    public Vector2 startScale;
    public Vector2 targetScale;
    public Vector2 position;

    SpriteRenderer sr;

    float timer;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        transform.localScale = new Vector3(startScale.x,startScale.y,1);
        mask.transform.localScale = new Vector3(startScale.x,startScale.y,1);
        sr.color = color;
        transform.position = position;
    }

    void Update()
    {
        timer += Time.deltaTime;
        
        transform.localScale = new Vector3(startScale.x + (targetScale.x-startScale.x)*Easing.OutQuint(timer/length), startScale.y + (targetScale.y-startScale.y)*Easing.OutQuint(timer/length),1);
        mask.transform.localScale = new Vector3((startScale.x + (targetScale.x-startScale.x)*Easing.OutCubic(timer/length))/transform.localScale.x, (startScale.y + (targetScale.y-startScale.y)*Easing.OutCubic(timer/length))/transform.localScale.y,1);

        if(timer >= length) {Destroy(gameObject);}
    }
}
