using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using System.Collections;
using System.Linq;
using UnityEngine.EventSystems;

namespace DuloGames.UI
{
	[AddComponentMenu("UI/Icon Slots/Item Slot", 12)]
	public class UIItemSlot : UISlotBase, ITooltipActivator {
		
		[Serializable] public class OnAssignEvent : UnityEvent<UIItemSlot> { }
		[Serializable] public class OnUnassignEvent : UnityEvent<UIItemSlot> { }
		
		[SerializeField] private UIItemSlot_Group m_SlotGroup = UIItemSlot_Group.None;
		[SerializeField] private int m_ID = 0;


        private bool tooltip = false;
        public bool ToolTipShow { get => tooltip; set => tooltip = value; }

      


        /// <summary>
        /// Gets or sets the slot group.
        /// </summary>
        /// <value>The slot group.</value>
        public UIItemSlot_Group slotGroup
		{
			get { return this.m_SlotGroup; }
			set { this.m_SlotGroup = value; }
		}
		
		/// <summary>
		/// Gets or sets the slot ID.
		/// </summary>
		/// <value>The I.</value>
		public int ID
		{
			get { return this.m_ID; }
			set { this.m_ID = value; }
		}
		
		/// <summary>
		/// The assigned item info.
		/// </summary>
		private Card m_ItemInfo;
		
		/// <summary>
		/// The assign event delegate.
		/// </summary>
		public OnAssignEvent onAssign = new OnAssignEvent();
		
		/// <summary>
		/// The unassign event delegate.
		/// </summary>
		public OnUnassignEvent onUnassign = new OnUnassignEvent();


   /*     protected override void OnEnable()
        {
            onAssign = new OnAssignEvent();
            onUnassign = new OnUnassignEvent();
        }*/
        /// <summary>
        /// Gets the item info of the item assigned to this slot.
        /// </summary>
        /// <returns>The spell info.</returns>
        /// 
        public Card GetItemInfo()
		{
			return this.m_ItemInfo;
		}

		/// <summary>
		/// Determines whether this slot is assigned.
		/// </summary>
		/// <returns><c>true</c> if this instance is assigned; otherwise, <c>false</c>.</returns>
		public override bool IsAssigned()
		{
			return (this.m_ItemInfo != null);
		}
		
		/// <summary>
		/// Assign the slot by item info.
		/// </summary>
		/// <param name="itemInfo">The item info.</param>
		public bool Assign(Card itemInfo)
		{
			if (itemInfo == null)
				return false;
			
			// Make sure we unassign first, so the event is called before new assignment
			this.Unassign();
			
			// Use the base class assign to set the icon
			this.Assign(itemInfo.sprite);
			
			// Set the spell info
			this.m_ItemInfo = itemInfo;
			
			// Invoke the on assign event
			if (this.onAssign != null)
				this.onAssign.Invoke(this);
			
			// Success
			return true;
		}
		
		/// <summary>
		/// Assign the slot by the passed source slot.
		/// </summary>
		/// <param name="source">Source.</param>
		public override bool Assign(Object source)
		{
			if (source is UIItemSlot)
			{
				UIItemSlot sourceSlot = source as UIItemSlot;
                if (sourceSlot != null)
                {
                    if (!sourceSlot.DragEnded) { sourceSlot.OnEndDrag(null); }
                    return this.Assign(sourceSlot.GetItemInfo());
                }
			}
			else if (source is UIEquipSlot)
			{
				UIEquipSlot sourceSlot = source as UIEquipSlot;
				
				if (sourceSlot != null)
					return this.Assign(sourceSlot.GetItemInfo());
			}
			
			// Default
			return false;
		}
		
		/// <summary>
		/// Unassign this slot.
		/// </summary>
		public override void Unassign()
		{
			// Remove the icon
			base.Unassign();
			
			// Invoke the on unassign event
			if (this.onUnassign != null)
				this.onUnassign.Invoke(this);
			// Clear the spell info
			this.m_ItemInfo = null;
			
		}
		
		/// <summary>
		/// Determines whether this slot can swap with the specified target slot.
		/// </summary>
		/// <returns><c>true</c> if this instance can swap with the specified target; otherwise, <c>false</c>.</returns>
		/// <param name="target">Target.</param>
		public override bool CanSwapWith(Object target)
		{
			if ((target is UIItemSlot) || (target is UIEquipSlot))
			{
				// Check if the equip slot accpets this item
				if (target is UIEquipSlot)
				{
					return (target as UIEquipSlot).CheckEquipType(this.GetItemInfo());
				}
				
				// It's an item slot
				return true;
			}
			
			// Default
			return false;
		}
		
		// <summary>
		/// Performs a slot swap.
		/// </summary>
		/// <returns><c>true</c>, if slot swap was performed, <c>false</c> otherwise.</returns>
		/// <param name="sourceSlot">Source slot.</param>
		public override bool PerformSlotSwap(Object sourceObject)
		{
			// Get the source slot
			UISlotBase sourceSlot = (sourceObject as UISlotBase);
			
			// Get the source item info
			Card sourceItemInfo = null;
			bool assign1 = false;
			
			// Check the type of the source slot
			if (sourceSlot is UIItemSlot)
			{
				sourceItemInfo = (sourceSlot as UIItemSlot).GetItemInfo();
				
				// Assign the source slot by this one
				assign1 = (sourceSlot as UIItemSlot).Assign(this.GetItemInfo());
			}
			else if (sourceSlot is UIEquipSlot)
			{
				sourceItemInfo = (sourceSlot as UIEquipSlot).GetItemInfo();
				
				// Assign the source slot by this one
				assign1 = (sourceSlot as UIEquipSlot).Assign(this.GetItemInfo());
			}
			
			// Assign this slot by the source slot
			bool assign2 = this.Assign(sourceItemInfo);
			
			// Return the status
			return (assign1 && assign2);
		}
		

		/// <summary>
		/// Prepares the tooltip with the specified item info.
		/// </summary>
		/// <param name="itemInfo">Item info.</param>
		public static void PrepareTooltip(Card itemInfo)
		{
		
		}


        public void OnToolTip(bool show)
        {
            if (ToggleTipWindow.Instance != null)
            {
                string description = this.m_ItemInfo.GetKeywordDescription();
                if (description != "")
                {
                    ToggleTipWindow.Instance.gameObject.SetActive(show);
                    ToggleTipWindow.Instance.GetComponent<RectTransform>().pivot = new Vector2(0F, 1f);
                    ToggleTipWindow.Instance.ToggleText.text = description;
                    ToggleTipWindow.Instance.transform.position = Input.mousePosition;
                    ToolTipShow = show;
                }
                else
                {
                    ToggleTipWindow.Instance.gameObject.SetActive(false);
                    ToolTipShow = false;

                }
            }
        }
        IEnumerator InternalShowToolTip()
        {
            yield return new WaitForSeconds(tooltipDelay);
            OnToolTip(true);
        }

        public override void UISlotBase_PartyMenuNotify(PartyMenu.SLOT_TRIGGER trigger)
        {
            switch (trigger)
            {
                case PartyMenu.SLOT_TRIGGER.SETUP:
                    ActiveAndAnimated = false;
                    glowingImage.color = new Color(1f, 1f, 1f, 0f);
                    break;
                case PartyMenu.SLOT_TRIGGER.SKILL:
                    if (!this.IsAssigned() && this.dropEnabled)
                    {
                        ActiveAndAnimated = true;
                        glowingImage.color = new Color(0f, 0.5f, 0f);
                    }
                    else if (this.dragEnabled)
                    {
                        ActiveAndAnimated = true;
                        if (PlayerInfos.Instance.persistentPartyDeck.GetCardSlots(new List<Unit> { PlayerInfos.Instance.unitsWindow.unit }) > 0 &&
                            m_ItemInfo != null)
                        {

                            Color fixedColor = PlayerInfos.Instance.unitsWindow.GetGlowingCardColor(m_ItemInfo);
                            glowingImage.color = fixedColor;
                        }

                        else
                        {
                            glowingImage.color = new Color(0.5f, 0f, 0f);
                        }

                    }
                    else
                    {
                        if (ActiveAndAnimated)
                        {
                            Debug.Log("LAST TICK ACTIVATED");
                            lastTick = true;
                        }
                    }
                    break;
                case PartyMenu.SLOT_TRIGGER.ON_DRAG:
                    if (!this.IsAssigned() && this.dropEnabled)
                    {
                        ActiveAndAnimated = true;
                        glowingImage.color = new Color(0f, 0.8f, 0f);
                    }
                    else if (!this.m_DragHasBegan)
                    {
                        if (ActiveAndAnimated) { lastTick = true; }
                    }
                    break;
            }



        }

        /// <summary>
        /// Raises the tooltip event.
        /// </summary>
        /// <param name="show">If set to <c>true</c> show.</param>
        public override void OnTooltip(bool show)
		{
			// Make sure we have spell info, otherwise game might crash
			if (this.m_ItemInfo == null)
				return;
			
			// If we are showing the tooltip
			if (show) 
            {
                this.OnToolTip(true);
                if (PlayerInfos.Instance == null)
                {
                    StarterDeckDisplay.Instance.ShowCardHolder(this.m_ItemInfo);
                }
                else
                {
                    PlayerInfos.Instance.unitsWindow.ShowCardHolder(this.m_ItemInfo, dragEnabled);
                }
			}
			else
			{
                // Hide the tooltip
                if (PlayerInfos.Instance == null)
                {
                    StarterDeckDisplay.Instance.HideCardHolder();
                }
                else
                {
                    PlayerInfos.Instance.unitsWindow.HideCardHolder();
                }
                this.OnToolTip(false);
                //UITooltip.Hide();
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (ActiveAndAnimated && m_ItemInfo != null && PlayerInfos.Instance != null)
            {
                glowingImage.color = PlayerInfos.Instance.unitsWindow.GetGlowingCardColor(m_ItemInfo);

            }
        }

        #region Static Methods
        /// <summary>
        /// Gets all the item slots.
        /// </summary>
        /// <returns>The slots.</returns>
        public static List<UIItemSlot> GetSlots()
		{
			List<UIItemSlot> slots = new List<UIItemSlot>();
			UIItemSlot[] sl = Resources.FindObjectsOfTypeAll<UIItemSlot>();
			
			foreach (UIItemSlot s in sl)
			{
				// Check if the slow is active in the hierarchy
				if (s.gameObject.activeInHierarchy)
					slots.Add(s);
			}
			
			return slots;
		}
		
		/// <summary>
		/// Gets all the item slots with the specified ID.
		/// </summary>
		/// <returns>The slots.</returns>
		/// <param name="ID">The slot ID.</param>
		public static List<UIItemSlot> GetSlotsWithID(int ID)
		{
			List<UIItemSlot> slots = new List<UIItemSlot>();
			UIItemSlot[] sl = Resources.FindObjectsOfTypeAll<UIItemSlot>();
			
			foreach (UIItemSlot s in sl)
			{
				// Check if the slow is active in the hierarchy
				if (s.gameObject.activeInHierarchy && s.ID == ID)
					slots.Add(s);
			}
			
			return slots;
		}
		
		/// <summary>
		/// Gets all the item slots in the specified group.
		/// </summary>
		/// <returns>The slots.</returns>
		/// <param name="group">The item slot group.</param>
		public static List<UIItemSlot> GetSlotsInGroup(UIItemSlot_Group group)
		{
			List<UIItemSlot> slots = new List<UIItemSlot>();
			UIItemSlot[] sl = Resources.FindObjectsOfTypeAll<UIItemSlot>();
			
			foreach (UIItemSlot s in sl)
			{
				// Check if the slow is active in the hierarchy
				if (s.gameObject.activeInHierarchy && s.slotGroup == group)
					slots.Add(s);
			}
			
			return slots;
		}
		
		/// <summary>
		/// Gets the slot with the specified ID and Group.
		/// </summary>
		/// <returns>The slot.</returns>
		/// <param name="ID">The slot ID.</param>
		/// <param name="group">The slot Group.</param>
		public static UIItemSlot GetSlot(int ID, UIItemSlot_Group group)
		{
			UIItemSlot[] sl = Resources.FindObjectsOfTypeAll<UIItemSlot>();
			
			foreach (UIItemSlot s in sl)
			{
				// Check if the slow is active in the hierarchy
				if (s.gameObject.activeInHierarchy && s.ID == ID && s.slotGroup == group)
					return s;
			}
			
			return null;
		}
		#endregion
	}
}