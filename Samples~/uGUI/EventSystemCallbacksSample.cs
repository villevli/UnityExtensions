using UnityEngine;
using UnityEngine.EventSystems;
using VLExtensions;

namespace VLExtensionsSamples
{
    public class EventSystemCallbacksSample : MonoBehaviour
    {
        private void OnEnable()
        {
            EventSystemCallbacks.onPointerClick += OnPointerClick;
        }

        private void OnDisable()
        {
            EventSystemCallbacks.onPointerClick -= OnPointerClick;
        }

        private void OnPointerClick(IPointerClickHandler handler, BaseEventData eventData)
        {
            Debug.Log($"EventSystemCallbacksSample OnPointerClick {handler} {eventData}");
        }
    }
}
