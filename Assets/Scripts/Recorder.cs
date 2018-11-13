#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class Recorder : MonoBehaviour {
    private static bool recording = false;
    private static bool screenshot = false;

    private static int shot;

    [MenuItem("Misc/Recording/Start")]
    public static void Start() {
        EnsureRecorderExists();

        EditorApplication.isPaused = false;
        Recorder.recording = true;
        Time.captureFramerate = 60;
        shot = 0;
    }

    [MenuItem("Misc/Recording/Stop")]
    public static void Stop() {
        Recorder.recording = false;
        Time.captureFramerate = 0;
        EditorApplication.isPaused = true;
    }

    [MenuItem("Misc/Screenshot %#R")]
    public static void Screenshot() {
        EnsureRecorderExists();

        Recorder.screenshot = true;
        EditorApplication.isPaused = false;
    }

    public void OnPostRender() {
        if(Recorder.screenshot) {
            ScreenCapture.CaptureScreenshot("recording/ss_" + shot.ToString("000") + ".png");
            shot++;
            Recorder.screenshot = false;
        }

        if(Recorder.recording) {
            ScreenCapture.CaptureScreenshot("recording/capture_" + shot.ToString("0000") + ".png");
            shot++;
        }
    }

    private static void EnsureRecorderExists() {
        if(GameObject.FindObjectOfType<Recorder>() == null) {
            Camera.main.gameObject.AddComponent<Recorder>();
        }
    }
} 

#endif