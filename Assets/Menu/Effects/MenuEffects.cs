using UnityEngine;

public class MenuEffects : MonoBehaviour
{
    public GameObject[] effects_obj;

    /// <summary>Square Effect</summary>
    /// <param name="state">position(Vector2), prescale(Vector2), scale(Vector2), length(float), color(Color)</param>
    public void NewSquare(Vector2 pos, Vector2 prescale, Vector2 scale, float len, Color col)
    {
        GameObject effect_obj = Instantiate(effects_obj[0], transform);
        Effect_Square effect = effect_obj.GetComponent<Effect_Square>();
        effect.position = pos;
        effect.startScale = prescale;
        effect.targetScale = scale;
        effect.length = len;
        effect.color = col;
    }
}
