using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Joystick : MonoBehaviour, IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    public RectTransform background;
    public RectTransform habdle;
    private float joystickRadius;

    public Vector2 Direction { get; private set; }

    public void Start()
    {
        joystickRadius = background.rect.width * 0.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position = RectTransformUtility.WorldToScreenPoint(null, background.position);

        Vector2 radius = eventData.position - position;

        Direction = radius.magnitude > joystickRadius ? radius.normalized : radius/joystickRadius;

        habdle.anchoredPosition = Direction * joystickRadius;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;
        habdle.anchoredPosition = Vector2.zero;
    }
}
