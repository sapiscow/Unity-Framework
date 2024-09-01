using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sapiscow.Editors
{
    /// <summary>
    /// Take screenshot of game view and save it to image file
    /// </summary>
    public static class GameViewScreenshoter
    {
        private const string _fileRoot = "Screenshots/";
        private const string _fileFormatName = "{0}{1} Screenshot [{2}].png";

        [MenuItem("Sapiscow/Screenshot Game _F12")]
        private static void TakeScreenshot()
        {
            if (!Directory.Exists(_fileRoot)) Directory.CreateDirectory(_fileRoot);

            string date = System.DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss");
            string path = string.Format(_fileFormatName, _fileRoot, Application.productName, date);

            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("Screenshot of Game View has been saved at <color=cyan>" + Path.GetFullPath(path) + "</color>");
        }
    }
}