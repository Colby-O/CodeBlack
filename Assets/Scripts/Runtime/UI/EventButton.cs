using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBlack.UI
{
    public class EventButton : Button, IPointerEnterHandler, IPointerExitHandler
    {
        [Serializable]
        public class ButtonHoverEvent : UnityEvent { }

        [SerializeField] private bool _enableSfXOnLoad = true;
        [SerializeField] private ButtonHoverEvent _onHover;

        public ButtonHoverEvent onHover { get => _onHover; }

        protected override void Awake()
        {
            base.Awake();

            Navigation nav = navigation;
            nav.mode = Navigation.Mode.None;
            navigation = nav;

            if (_enableSfXOnLoad)
            {
                onClick.AddListener(() => { if (interactable) ButtonActionHandler.CallEvent(ButtonActions.OnClick); });
                onHover.AddListener(() => { if (interactable) ButtonActionHandler.CallEvent(ButtonActions.OnHover); });
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            onHover?.Invoke();
        }
    }
}
