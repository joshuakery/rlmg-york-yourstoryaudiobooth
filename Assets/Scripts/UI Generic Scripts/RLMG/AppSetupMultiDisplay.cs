using System.Collections;
using System.Collections.Generic;
using rlmg.logging;
using UnityEngine;

public class AppSetupMultiDisplay : MonoBehaviour 
{
    // public bool setResolution = true;
    // public int targetScreenWidth = 1920;
    // public int targetScreenHeight = 1080;

    void Start()
    {
        RLMGLogger.Instance.Log("Application Awake - version #" + Application.version, MESSAGETYPE.INFO);

        if (!Application.isEditor)
            Cursor.visible = false;

        // if (setResolution)
        //     Screen.SetResolution(targetScreenWidth, targetScreenHeight, true);

        //Display.displays[0].Activate();
        //Display.displays[0].SetParams(1920, 1080, 0, 0);

        //string primaryDisplayDebugInfo = "Graphics Type: " + SystemInfo.graphicsDeviceType + "\n\n";
        string primaryDisplayDebugInfo = "Primary Display    App (screen dims): " + Screen.width + " x " + Screen.height + " (" + ((float)Screen.width / (float)Screen.height) + ")";
        primaryDisplayDebugInfo += "   \"Current Resolution\": " + Screen.currentResolution.width + " x " + Screen.currentResolution.height;
        primaryDisplayDebugInfo += "   Rendering Res: " + Display.main.renderingWidth + " x " + Display.main.renderingHeight + "   System Res: " + Display.main.systemWidth + " x " + Display.main.systemHeight;
        
        RLMGLogger.Instance.Log(primaryDisplayDebugInfo, MESSAGETYPE.INFO);


//         Resolution[] supportedResolutions = Screen.resolutions;
// ​
//         if (supportedResolutions != null && supportedResolutions.Length > 0)
//         {
//             primaryDisplayDebugInfo += "\n\nSupported Resolutions:";
// ​
//             foreach (var res in supportedResolutions)
//             {
//                 //Debug.Log(res.width + "x" + res.height + " : " + res.refreshRate);
// ​
//                 primaryDisplayDebugInfo += "\n" + res.width + "x" + res.height;
//             }
//         }

        //if (Display.displays.Length > 1)
        //{
        //    Display.displays[1].Activate();
        //    //Display.displays[1].SetParams(1080, 1920, 0, 0);

        //    RLMGLogger.Instance.Log("Activated second display.   Rendering Res: " + Display.displays[1].renderingWidth + " x " + Display.displays[1].renderingHeight + "   System Res: " + Display.displays[1].systemWidth + " x " + Display.displays[1].systemHeight, MESSAGETYPE.INFO);
        //}

        // for (int i = 1; i < Display.displays.Length; i++)  //Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // {
        //     Display.displays[i].Activate();
        // }
    }

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
