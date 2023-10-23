using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHelper : MonoBehaviour
{
    [Header("These events are called from animation clips")]
    public UnityEvent[] animationEvents;

    public void OnAnimationEvent(int eventId)
    {
        if (eventId < 0) return;
        if (eventId < animationEvents.Length) animationEvents[eventId]?.Invoke();
    }
}
