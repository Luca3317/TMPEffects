using TMPro;
using UnityEngine;

public abstract class TMPEffectComponent : MonoBehaviour
{
    public TMP_Text TextComponent => mediator.Text;

    [System.NonSerialized] protected TMPMediator mediator;

    public void SetText(string text)
    {
        mediator.Text.SetText(text);
    }

    public void UpdateMeshes(TMPro.TMP_VertexDataUpdateFlags flags = TMPro.TMP_VertexDataUpdateFlags.All)
    {
        mediator.Text.UpdateVertexData(flags);
    }

    protected void UpdateMediator()
    {
        mediator = TMPMediator.Create(gameObject);
    }
}
