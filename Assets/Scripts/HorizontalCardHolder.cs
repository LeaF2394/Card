using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HorizontalCardHolder : MonoBehaviour
{
    public Transform CardSlotPrefab;
    public List<BaseCard> Cards = new List<BaseCard>();

    public int SpawnSlots = 5;
    
    private BaseCard HoveredCard;
    private BaseCard SelectedCard;
    
    private bool isSwaping;

    private void Start()
    {
        for (int i = 0; i < SpawnSlots; i++)
        {
            Transform cardSlot = Instantiate(CardSlotPrefab, transform);
            
            BaseCard card = cardSlot.GetComponentInChildren<BaseCard>();
            card.Setup(this);
            card.CardIndex = i;
            Cards.Add(card);
            
            cardSlot.gameObject.name = "Slot";
            card.gameObject.name     = i.ToString();
            
            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
        }

        for (int i = 0; i < Cards.Count; i++)
        {
            if (Cards[i].cardView != null)
                Cards[i].cardView.UpdateIndex(transform.childCount);
        }
    }

    private void BeginDrag(BaseCard card)
    {
        SelectedCard = card;
    }

    private void EndDrag(BaseCard card)
    {
        if (SelectedCard == null)
            return;
        
        SelectedCard.transform.DOLocalMove(Vector3.zero, .15f).SetEase(Ease.OutBack);
        
        SelectedCard = null;
    }

    private void CardPointerEnter(BaseCard card)
    {
        HoveredCard = card;
    }

    private void CardPointerExit(BaseCard card)
    {
        HoveredCard = null;
    }

    private void Update()
    {
        if(SelectedCard == null)
            return;
        if(isSwaping)
            return;
        
        for (int i = 0; i < Cards.Count; i++)
        {
            if (SelectedCard.transform.position.x - Cards[i].transform.position.x > 0.05f)
            {
                if (SelectedCard.ParentIndex() < Cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (SelectedCard.transform.position.x - Cards[i].transform.position.x < -0.05f)
            {
                if (SelectedCard.ParentIndex() > Cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    void Swap(int index)
    {
        isSwaping = true;

        Transform focusedParent = SelectedCard.transform.parent;
        Transform crossedParent = Cards[index].transform.parent;

        Cards[index].transform.SetParent(focusedParent);
        SelectedCard.transform.SetParent(crossedParent);
        
        Cards[index].transform.DOLocalMove(Vector3.zero, .15f).SetEase(Ease.OutBack);

        isSwaping = false;

        if (Cards[index].cardView == null)
            return;

        //Updated Visual Indexes
        foreach (BaseCard card in Cards)
        {
            card.cardView.UpdateIndex(transform.childCount);
        }
    }
}
