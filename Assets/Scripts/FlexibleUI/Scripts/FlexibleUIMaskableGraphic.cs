using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.FlexibleUI
{
    [RequireComponent(typeof(MaskableGraphic))]
    public class FlexibleUIMaskableGraphic : FlexibleUIBase
    {
        MaskableGraphic graphic;
        public ColorChoice colorChoice;

        public override void Awake()
        {
            graphic = GetComponent<MaskableGraphic>();

            base.Awake();
        }

        protected override void OnSkinUI()
        {
            if (skinData == null) return;

            if (colorChoice != ColorChoice.Custom)
                graphic.color = skinData.GetColor(colorChoice);

            base.OnSkinUI();
        }
    }
}


