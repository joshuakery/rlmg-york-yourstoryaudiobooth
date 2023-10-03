namespace JoshKery.GenericUI.Events
{
    [System.Serializable]
    public class IntEvent : UnityEngine.Events.UnityEvent<int> { };

    [System.Serializable]
    public class ListIntEvent : UnityEngine.Events.UnityEvent<System.Collections.Generic.List<int>> { };

    [System.Serializable]
    public class StringEvent : UnityEngine.Events.UnityEvent<string> { };

    [System.Serializable]
    public class FloatEvent : UnityEngine.Events.UnityEvent<float> { };

    [System.Serializable]
    public class Vector2Event : UnityEngine.Events.UnityEvent<UnityEngine.Vector2> { };


}
