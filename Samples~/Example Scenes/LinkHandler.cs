using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class LinkHandler : MonoBehaviour, IPointerClickHandler
{

    private TMP_Text text;
    private Canvas canvas;
    [SerializeField] private Camera cameraoUse;


    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            cameraoUse = null;
        else
            cameraoUse = canvas.worldCamera;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 mousePos = new Vector3(eventData.position.x, eventData.position.y, 0);

        var linktaggedtext = TMP_TextUtilities.FindIntersectingLink(text, mousePos, cameraoUse);

        if (linktaggedtext == -1) return;

        TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linktaggedtext];

        string selectedLink = linkInfo.GetLinkID();
        if (!string.IsNullOrEmpty(selectedLink))
        {
            //Debug.LogFormat("Open link {0}", selectedLink);
            Application.OpenURL(selectedLink);
        }
    }
}
