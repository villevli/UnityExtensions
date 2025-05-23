#if USE_UGUI
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

namespace VLExtensions
{
    /// <summary>
    /// Global callbacks just before any EventSystem event is executed.
    /// </summary>
    public static class EventSystemCallbacks
    {
        public delegate void PointerEventFunction<T>(T handler, PointerEventData eventData);

        /// <summary>Global callback before any <see cref="IPointerEnterHandler.OnPointerEnter"/> is called.</summary>
        public static event PointerEventFunction<IPointerEnterHandler> onPointerEnter;

        /// <summary>Global callback before any <see cref="IPointerExitHandler.OnPointerExit"/> is called.</summary>
        public static event PointerEventFunction<IPointerExitHandler> onPointerExit;

        /// <summary>Global callback before any <see cref="IPointerDownHandler.OnPointerDown"/> is called.</summary>
        public static event PointerEventFunction<IPointerDownHandler> onPointerDown;

        /// <summary>Global callback before any <see cref="IPointerUpHandler.OnPointerUp"/> is called.</summary>
        public static event PointerEventFunction<IPointerUpHandler> onPointerUp;

        /// <summary>Global callback before any <see cref="IPointerClickHandler.OnPointerClick"/> is called.</summary>
        public static event PointerEventFunction<IPointerClickHandler> onPointerClick;

        /// <summary>Global callback before any <see cref="IInitializePotentialDragHandler.OnInitializePotentialDrag"/> is called.</summary>
        public static event PointerEventFunction<IInitializePotentialDragHandler> onInitializePotentialDrag;

        /// <summary>Global callback before any <see cref="IBeginDragHandler.OnBeginDrag"/> is called.</summary>
        public static event PointerEventFunction<IBeginDragHandler> onBeginDrag;

        /// <summary>Global callback before any <see cref="IDragHandler.OnDrag"/> is called.</summary>
        public static event PointerEventFunction<IDragHandler> onDrag;

        /// <summary>Global callback before any <see cref="IEndDragHandler.OnEndDrag"/> is called.</summary>
        public static event PointerEventFunction<IEndDragHandler> onEndDrag;

        /// <summary>Global callback before any <see cref="IDropHandler.OnDrop"/> is called.</summary>
        public static event PointerEventFunction<IDropHandler> onDrop;

        /// <summary>Global callback before any <see cref="IScrollHandler.OnScroll"/> is called.</summary>
        public static event PointerEventFunction<IScrollHandler> onScroll;

        static EventSystemCallbacks()
        {
            ReplaceExecuteEventsHandlers();
        }

        private static void ReplaceExecuteEventsHandlers()
        {
            static void Replace<T>(string name, ExecuteEvents.EventFunction<T> method)
            {
                typeof(ExecuteEvents).GetField(name, BindingFlags.NonPublic | BindingFlags.Static)
                                     .SetValue(null, method);
            }

            Replace<IPointerEnterHandler>("s_PointerEnterHandler", Execute);
            Replace<IPointerExitHandler>("s_PointerExitHandler", Execute);
            Replace<IPointerDownHandler>("s_PointerDownHandler", Execute);
            Replace<IPointerUpHandler>("s_PointerUpHandler", Execute);
            Replace<IPointerClickHandler>("s_PointerClickHandler", Execute);
            Replace<IInitializePotentialDragHandler>("s_InitializePotentialDragHandler", Execute);
            Replace<IBeginDragHandler>("s_BeginDragHandler", Execute);
            Replace<IDragHandler>("s_DragHandler", Execute);
            Replace<IEndDragHandler>("s_EndDragHandler", Execute);
            Replace<IDropHandler>("s_DropHandler", Execute);
            Replace<IScrollHandler>("s_ScrollHandler", Execute);
        }

        private static void Execute(IPointerEnterHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onPointerEnter?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnPointerEnter(pointerEventData);
        }

        private static void Execute(IPointerExitHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onPointerExit?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnPointerExit(pointerEventData);
        }

        private static void Execute(IPointerDownHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onPointerDown?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnPointerDown(pointerEventData);
        }

        private static void Execute(IPointerUpHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onPointerUp?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnPointerUp(pointerEventData);
        }

        private static void Execute(IPointerClickHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onPointerClick?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnPointerClick(pointerEventData);
        }

        private static void Execute(IInitializePotentialDragHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onInitializePotentialDrag?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnInitializePotentialDrag(pointerEventData);
        }

        private static void Execute(IBeginDragHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onBeginDrag?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnBeginDrag(pointerEventData);
        }

        private static void Execute(IDragHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onDrag?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnDrag(pointerEventData);
        }

        private static void Execute(IEndDragHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onEndDrag?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnEndDrag(pointerEventData);
        }

        private static void Execute(IDropHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onDrop?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnDrop(pointerEventData);
        }

        private static void Execute(IScrollHandler handler, BaseEventData eventData)
        {
            var pointerEventData = ExecuteEvents.ValidateEventData<PointerEventData>(eventData);
            try
            {
                onScroll?.Invoke(handler, pointerEventData);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            handler.OnScroll(pointerEventData);
        }
    }
}
#endif
