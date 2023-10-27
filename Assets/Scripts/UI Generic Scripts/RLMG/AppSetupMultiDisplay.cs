using System.Collections;
using System.Collections.Generic;
using rlmg.logging;
using UnityEngine;

public class AppSetupMultiDisplay : MonoBehaviour
{
    [SerializeField]
    private bool doEnableMultitouch = false;

    void Start()
    {
        RLMGLogger.Instance.Log("Application Awake - version #" + Application.version, MESSAGETYPE.INFO);

        if (!Application.isEditor)
            Cursor.visible = false;

        // if (setResolution)
        //     Screen.SetResolution(targetScreenWidth, targetScreenHeight, true);

        //Display.displays[0].Activate();
        //Display.displays[0].SetParams(1920, 1080, 0, 0);

        string primaryDisplayDebugInfo = "Primary Display    App (screen dims): " + Screen.width + " x " + Screen.height + " (" + ((float)Screen.width / (float)Screen.height) + ")";
        primaryDisplayDebugInfo += "   \"Current Resolution\": " + Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        primaryDisplayDebugInfo += "   Rendering Res: " + Display.main.renderingWidth + " x " + Display.main.renderingHeight + "   System Res: " + Display.main.systemWidth + " x " + Display.main.systemHeight;

        RLMGLogger.Instance.Log(primaryDisplayDebugInfo, MESSAGETYPE.INFO);

        Input.multiTouchEnabled = doEnableMultitouch;
    }

    #region Start Helper Methods
    private string AddSupportedResolutionsToLog(string log)
    {
        Resolution[] supportedResolutions = Screen.resolutions;

        if (supportedResolutions != null && supportedResolutions.Length > 0)
        {
            log += "\n\nSupported Resolutions:";

            foreach (var res in supportedResolutions)
                log += "\n" + res.width + "x" + res.height;
        }

        return log;
    }

    private void ActivateOtherDisplays()
    {
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            //Display.displays[1].SetParams(1080, 1920, 0, 0);

            RLMGLogger.Instance.Log("Activated second display.   Rendering Res: " + Display.displays[1].renderingWidth + " x " + Display.displays[1].renderingHeight + "   System Res: " + Display.displays[1].systemWidth + " x " + Display.displays[1].systemHeight, MESSAGETYPE.INFO);
        }

        //Display.displays[0] is the primary, default display and is //always ON, so start at index 1.
        for (int i = 1; i < Display.displays.Length; i++) 
        {
            Display.displays[i].Activate();
        }

    }
    #endregion

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            RLMGLogger.Instance.Log("Quit application via 'Escape' key.", MESSAGETYPE.INFO);

            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Cursor.visible = !Cursor.visible;
        }
    }

    void OnApplicationQuit()
    {
        RLMGLogger.Instance.Log("Application Quit", MESSAGETYPE.INFO);
    }
}
