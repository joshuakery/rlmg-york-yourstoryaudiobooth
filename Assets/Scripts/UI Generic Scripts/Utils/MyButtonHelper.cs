using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JoshKery.GenericUI
{
    [RequireComponent(typeof(Button))]
    public class MyButtonHelper : MonoBehaviour
    {
        private Button button;

        private static float notInteractableTimeout = 0.5f;

        [SerializeField]
        private bool doAutoSetInteractableAgain = true;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (button != null)
            {
                button.onClick.AddListener(BrieflySetNotInteractableButton);
            }
        }

        private void OnDisable()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(BrieflySetNotInteractableButton);
            }
        }

        private void BrieflySetNotInteractableButton()
        {
            StartCoroutine(BrieflySetNotInteractableButtonCo());
        }

        private IEnumerator BrieflySetNotInteractableButtonCo()
        {
            if (button != null)
            {
                button.interactable = false;

                yield return new WaitForSeconds(notInteractableTimeout);

                if (doAutoSetInteractableAgain)
                    button.interactable = true;
            }
        }

    }
}


