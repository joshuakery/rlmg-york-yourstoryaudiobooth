using UnityEngine;
using UnityEngine.UI;
using JoshKery.GenericUI.Events;

namespace JoshKery.GenericUI.AspectRatio
{
    // https://gamedev.stackexchange.com/questions/154046/rawimage-texture-change-event
    [RequireComponent(typeof(RawImage))]
    public class RawImageManager : MonoBehaviour
    {
        private RawImage _target;
        private RawImage target
        {
            get
            {
                if (_target == null)
                    _target = GetComponent<RawImage>();

                return _target;
            }
        }

        private FloatEvent _onAspectRatioChanged;

        public FloatEvent onAspectRatioChanged
        {
            get
            {
                if (_onAspectRatioChanged == null)
                    _onAspectRatioChanged = new FloatEvent();

                return _onAspectRatioChanged;
            }
        }

        private Vector2Event _onSizeChanged;

        public Vector2Event onSizeChanged
        {
            get
            {
                if (_onSizeChanged == null)
                    _onSizeChanged = new Vector2Event();

                return _onSizeChanged;
            }
        }

        public Texture texture
        {
            get { return target.texture; }
            set
            {
                target.texture = value;

                if (onAspectRatioChanged != null && value != null)
                    onAspectRatioChanged.Invoke((float)value.width / (float)value.height);

                if (onSizeChanged != null)
                {
                    if (value != null)
                        onSizeChanged.Invoke(new Vector2(value.width, value.height));
                    else
                        onSizeChanged.Invoke(new Vector2(0, 0));
                }
            }
        }
    }

}

