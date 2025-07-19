using System;
using Core.General;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BaseCard : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Selectable selectable;
    
    [SerializeField] private bool SpawnVisual;
    [SerializeField] private Transform CardPrefab;
    private Transform card;
    [HideInInspector] public CardView cardView;
    
    public float MoveSpeed = 20f;
    public float SelectedPositionYOffset = 50f;
    public float DragOrSelectPositionZOffset = 2.5f;
    
    private Vector3 localPos;
    private Vector3 targetPosition;
    
    private bool isSelected;
    private bool isHovering;
    private bool isDragging;
    
    public bool selected => isSelected;
    
    [Header("Events")]
    [HideInInspector] public UnityEvent<BaseCard> PointerEnterEvent;
    [HideInInspector] public UnityEvent<BaseCard> PointerExitEvent;
    [HideInInspector] public UnityEvent<BaseCard, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<BaseCard> PointerDownEvent;
    [HideInInspector] public UnityEvent<BaseCard> BeginDragEvent;
    [HideInInspector] public UnityEvent<BaseCard> EndDragEvent;
    [HideInInspector] public UnityEvent<BaseCard, bool> SelectEvent;
    
    private HorizontalCardHolder CardHolder;
    
    [HideInInspector] public int CardIndex;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
        
        if (SpawnVisual)
        {
            card = Instantiate(CardPrefab, Vector3.zero, Quaternion.identity);
            cardView = card.GetComponent<CardView>();
            cardView.parentCard = this;
        }
    }

    public void Setup(HorizontalCardHolder cardHolder)
    {
        CardHolder = cardHolder;
    }

    private void Update()
    {
        targetPosition      = transform.position + localPos;
        transform.rotation  = Quaternion.Euler(Vector3.zero);
        cardView.SavedIndex = CardIndex;
        if (SpawnVisual)
        {
            card.transform.localPosition = Vector3.Lerp(card.transform.position, targetPosition, Time.deltaTime * MoveSpeed);
            
            cardView.isSelected = isSelected;
            cardView.isHovered = isHovering;
            cardView.isDragging = isDragging;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = MouseUtils.GetMousePositionFromCamera();
        localPos.z         = -DragOrSelectPositionZOffset;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent?.Invoke(this);
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging              = false;
        isSelected              = false;
        
        // selectable.interactable = false;
        // Collider2D hitColl = Physics2D.OverlapPoint(targetPosition);
        // selectable.interactable = true;
        // if (hitColl != null && hitColl.TryGetComponent(out ICardDropArea dropArea))
        // {
        //     dropArea.OnCardDrop(this);
        // }
        // else
        // {
        //     if(isSelected)
        //     {
        //         transform.localPosition = Vector3.zero;
        //         localPos                = Vector3.zero;
        //         localPos.y              = SelectedPositionYOffset;
        //         // localPos.z              =  -DragOrSelectPositionZOffset;
        //         // transform.localPosition += localPos;
        //     }
        //     else
        //     {
        //         transform.localPosition = Vector3.zero;
        //         localPos                = Vector3.zero;
        //         // transform.localPosition += localPos;
        //     }
        // }
        
        EndDragEvent?.Invoke(this);
        // localPos                = Vector3.zero;
        // transform.localPosition = new Vector3(localPos.x, localPos.y, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        cardView.Shake(0.2f, 0f, 8f, 0f);
        PointerEnterEvent?.Invoke(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        PointerExitEvent?.Invoke(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cardView.ResetAnimation();
        cardView.Shake(0.25f, 0f, 10f, 0.25f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isSelected = !isSelected;
        
        SelectEvent?.Invoke(this, isSelected);
        
        if(isSelected)
        {
            transform.localPosition =  Vector3.zero;
            localPos                =  Vector3.zero;
            localPos.y              =  SelectedPositionYOffset;
            // localPos.z              =  -DragOrSelectPositionZOffset;
            // transform.localPosition += localPos;
        }
        else
        {
            transform.localPosition =  Vector3.zero;
            localPos                =  Vector3.zero;
            // transform.localPosition += localPos;
        }
    }

    public int ParentIndex()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
    }
}
