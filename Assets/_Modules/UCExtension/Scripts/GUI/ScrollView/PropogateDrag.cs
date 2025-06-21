using System.Collections;
using System.Collections.Generic;
using UCExtension.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UCExtension.GUI
{
    [RequireComponent(typeof(EventTrigger))]
    public class PropogateDrag : MonoBehaviour
    {
        public ScrollRect horizontalScrollView;
        public ScrollSnapRect scrollSnapRect;
        public ScrollRect verticalScrollView;
        // Start is called before the first frame update

        bool isDragHorizontal = false;
        void Start()
        {
            RegisterEvent();
        }
        void RegisterEvent()
        {
            EventTrigger trigger = GetComponent<EventTrigger>();
            EventTrigger.Entry entryBegin = new EventTrigger.Entry();
            EventTrigger.Entry entryDrag = new EventTrigger.Entry();
            EventTrigger.Entry entryEnd = new EventTrigger.Entry();
            EventTrigger.Entry entrypotential = new EventTrigger.Entry();
            EventTrigger.Entry entryScroll = new EventTrigger.Entry();
            EventTrigger.Entry entryClick = new EventTrigger.Entry();
            EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
            //
            entryBegin.eventID = EventTriggerType.BeginDrag;
            entryBegin.callback.AddListener(OnBeginDrag);
            trigger.triggers.Add(entryBegin);
            //
            entryDrag.eventID = EventTriggerType.Drag;
            entryDrag.callback.AddListener(OnDrag);
            trigger.triggers.Add(entryDrag);
            //
            entryEnd.eventID = EventTriggerType.EndDrag;
            entryEnd.callback.AddListener(OnEndDrag);
            trigger.triggers.Add(entryEnd);
            //
            entrypotential.eventID = EventTriggerType.InitializePotentialDrag;
            entrypotential.callback.AddListener(OnInitializePotentialDrag);
            trigger.triggers.Add(entrypotential);
            //
            entryScroll.eventID = EventTriggerType.Scroll;
            entryScroll.callback.AddListener(OnScroll);
            trigger.triggers.Add(entryScroll);
            //
            entryClick.eventID = EventTriggerType.PointerClick;
            trigger.triggers.Add(entryClick);
            //
            entryPointerDown.eventID = EventTriggerType.PointerDown;
            entryPointerDown.callback.AddListener(OnPointerDown);
            trigger.triggers.Add(entryPointerDown);

        }

        void OnBeginDrag(BaseEventData data)
        {
            var eventData = (PointerEventData)data;
            if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
            {
                isDragHorizontal = true;
            }
            else
            {
                isDragHorizontal = false;
            }
            if (isDragHorizontal)
            {
                if (scrollSnapRect)
                {
                    scrollSnapRect.OnBeginDrag(eventData);
                }
                horizontalScrollView.OnBeginDrag((PointerEventData)data);
            }
            else
            {
                verticalScrollView.OnBeginDrag((PointerEventData)data);
            }
        }
        void OnDrag(BaseEventData data)
        {
            if (isDragHorizontal)
            {
                if (scrollSnapRect)
                {
                    scrollSnapRect.OnDrag((PointerEventData)data);
                }
                horizontalScrollView.OnDrag((PointerEventData)data);
            }
            else
            {
                verticalScrollView.OnDrag((PointerEventData)data);
            }
        }
        void OnEndDrag(BaseEventData data)
        {
            if (isDragHorizontal)
            {
                if (scrollSnapRect)
                {
                    scrollSnapRect.OnEndDrag((PointerEventData)data);
                }
                horizontalScrollView.OnEndDrag((PointerEventData)data);
            }
            else
            {
                verticalScrollView.OnEndDrag((PointerEventData)data);
            }
        }
        void OnInitializePotentialDrag(BaseEventData data)
        {
            if (isDragHorizontal)
            {
                horizontalScrollView.OnInitializePotentialDrag((PointerEventData)data);
            }
            else
            {
                verticalScrollView.OnInitializePotentialDrag((PointerEventData)data);
            }
        }
        void OnScroll(BaseEventData data)
        {
            if (isDragHorizontal)
            {
                horizontalScrollView.OnScroll((PointerEventData)data);
            }
            else
            {
                verticalScrollView.OnScroll((PointerEventData)data);
            }
        }
        void OnPointerDown(BaseEventData data)
        {
            if (isDragHorizontal)
            {
            }
            else
            {
                verticalScrollView.OnScroll((PointerEventData)data);
            }
        }

        public void SetVerticalScroll(ScrollRect horizontalScroll)
        {
            verticalScrollView = horizontalScroll;
        }
        public void SetHorizontalScroll(ScrollRect horizontalScroll)
        {
            horizontalScrollView = horizontalScroll;
        }
    }
}