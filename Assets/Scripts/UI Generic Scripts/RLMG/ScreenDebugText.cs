using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenDebugText : MonoBehaviour
{
    public Text text;

    public bool isVisible = false;
    public KeyCode toggleKey = KeyCode.BackQuote;

	void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            isVisible = !isVisible;


        if (text == null)
            text = GetComponent<Text>();

        if (text == null)
            return;

        text.enabled = isVisible;

        if (!text.isActiveAndEnabled)
            return;

        text.text = "Graphics Type: " + SystemInfo.graphicsDeviceType + "\n\n";

        text.text += "App: " + Screen.width + " x " + Screen.height + " (" + ((float)Screen.width / (float)Screen.height) + ")     \n\nDisplay: " + Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        //text.text += "App: " + Screen.width + " x " + Screen.height + " (" + ((float)Screen.width / (float)Screen.height) + ")     \n\nDisplay: " + Display.main.systemWidth + " x " + Display.main.systemHeight;


        Resolution[] resolutions = Screen.resolutions;

        if (resolutions != null && resolutions.Length > 0)
        {
            text.text += "\n\nSupported Resolutions:";

            foreach (var res in resolutions)
            {
                //Debug.Log(res.width + "x" + res.height + " : " + res.refreshRate);

                text.text += "\n" + res.width + "x" + res.height;
            }
        }
    }
}
