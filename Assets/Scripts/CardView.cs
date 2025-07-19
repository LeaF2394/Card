using System;
using Core.General;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class CardView : MonoBehaviour
{
    public BaseCard parentCard;
    public SpriteRenderer CardFront;
    public SpriteRenderer CardBack;
    public Transform CardShadow;

    public Transform AnimationParent;
    public Transform RotationParent;

    public bool isHovered;
    public bool isSelected;
    public bool isDragging;

    public float rotationSpeed = 10f;
    public float rotationAmount = 20f;
    public float rotationIntensity = 10f;
    public float DragOrSelectShadowPositionYOffset = 1;
    public float DragOrSelectShadowPositionXOffset = 1;

    private Vector3 startShadowPosition;
    private Vector3 rotationParentEuler;

    private Vector3 rotationDelta;
    private Vector3 movementDelta;

    private SortingGroup sortingGroup;

    [HideInInspector] public int SavedIndex = 0;

    public Card Card { get; private set; }

    private void Start()
    {
        startShadowPosition = CardShadow.localPosition;
        sortingGroup = GetComponent<SortingGroup>();
    }

    private void Update()
    {
        RotationParent.localRotation = Quaternion.Euler(rotationParentEuler);
        CardFront.material.SetVector("_Rotation", new Vector4(RotationParent.rotation.eulerAngles.x / 10f, RotationParent.rotation.eulerAngles.y / 10f));
        CardBack.material.SetVector("_Rotation", new Vector4(RotationParent.rotation.eulerAngles.x / 10f, RotationParent.rotation.eulerAngles.y / 10f));

        AnimationParent.localPosition = Vector3.Lerp(AnimationParent.localPosition, Vector3.zero, Time.deltaTime * 10f);
        AnimationParent.localRotation = Quaternion.Lerp(AnimationParent.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * 10f);

        Vector3 movement = (transform.position - parentCard.transform.position);
        movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
        Vector3 movementRotation = (isDragging ? movementDelta : movement) * rotationAmount;
        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));

        if (isSelected || isDragging)
        {
            CardShadow.localPosition = startShadowPosition - transform.up * DragOrSelectShadowPositionYOffset + transform.right * DragOrSelectShadowPositionXOffset;
        }
        else
        {
            CardShadow.localPosition = startShadowPosition;
        }

        if (isHovered || isSelected || isDragging)
        {
            AnimationParent.localScale = Vector3.Lerp(AnimationParent.localScale, Vector3.one * 1.05f, Time.deltaTime * 10f);
        }
        else
        {
            AnimationParent.localScale = Vector3.Lerp(AnimationParent.localScale, Vector3.one, Time.deltaTime * 10f);
        }

        if (isDragging)
        {
            sortingGroup.sortingOrder = 10;
        }
        else if (isSelected)
        {
            sortingGroup.sortingOrder = 5;
        }
        else if (isHovered)
        {
            sortingGroup.sortingOrder = 1;
        }
        else if (isDragging && isSelected && isHovered)
        {
            sortingGroup.sortingOrder = 5;
        }
        else if (isDragging && isHovered)
        {
            sortingGroup.sortingOrder = 5;
        }
        else
        {
            sortingGroup.sortingOrder = 0;
        }

        if (isHovered)
        {
            Vector3 mousePos = MouseUtils.GetMousePositionFromCamera(0f);
            Vector3 tilt = transform.position - mousePos;

            float X = tilt.y * -1;
            float Y = tilt.x;

            rotationParentEuler = new Vector3(X, Y, 0f) * rotationIntensity;
        }
        else
        {
            float sine = Mathf.Sin(Time.time + SavedIndex);
            float cosine = Mathf.Cos(Time.time + SavedIndex);

            rotationParentEuler = new Vector3(sine, cosine, 0f) * rotationIntensity;
        }

        UpdateView();
    }

    public void UpdateIndex(int length)
    {
        transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
    }

    public void Shake(float duration, float intensity = 1F, float rotIntensity = 5F, float scaleIntensity = 1F)
    {
        AnimationParent.DOPunchPosition(new Vector3(intensity, intensity, 0f), duration).SetAutoKill();
        AnimationParent.DOPunchRotation(new Vector3(0f, 0f, rotIntensity), duration).SetAutoKill();
        AnimationParent.DOPunchScale(new Vector3(scaleIntensity, scaleIntensity, 0f), duration).SetAutoKill();
    }

    public void ResetAnimation()
    {
        AnimationParent.localRotation = Quaternion.Euler(Vector3.zero);
        AnimationParent.localPosition = Vector3.zero;
        AnimationParent.localScale = Vector3.one;
    }
}
