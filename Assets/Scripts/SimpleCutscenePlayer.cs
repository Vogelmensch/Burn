using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SimpleCutscenePlayer : MonoBehaviour
{
    [Tooltip("Das Video, das als Cutscene abgespielt werden soll")]
    public VideoClip cutsceneVideo;
    
    [Tooltip("Name der Szene, die nach der Cutscene geladen werden soll")]
    public string targetLevelName;
    
    [Tooltip("Optional: Referenz zur RawImage-Komponente, falls du nicht die ganze Kamera nutzen willst")]
    public UnityEngine.UI.RawImage videoImage;
    
    private VideoPlayer videoPlayer;
    private bool hasSkipped = false;

    void Start()
    {
        // VideoPlayer einrichten
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
        }
        
        // VideoPlayer konfigurieren
        videoPlayer.clip = cutsceneVideo;
        videoPlayer.playOnAwake = false;
        
        // Entscheide, wie das Video gerendert wird
        if (videoImage != null)
        {
            // Video auf eine RawImage-Komponente rendern
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = new RenderTexture((int)videoPlayer.clip.width, (int)videoPlayer.clip.height, 24);
            videoImage.texture = videoPlayer.targetTexture;
        }
        else
        {
            // Video auf die Kamera rendern
            videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
            videoPlayer.targetCamera = Camera.main;
        }
        
        // Event-Handler für das Video-Ende
        videoPlayer.loopPointReached += OnVideoFinished;
        
        // Video abspielen
        videoPlayer.Play();
    }

    void Update()
    {
        // Video mit ESC überspringen
        if (Input.GetKeyDown(KeyCode.Escape) && !hasSkipped)
        {
            SkipCutscene();
        }
    }
    
    void OnVideoFinished(VideoPlayer vp)
    {
        LoadTargetLevel();
    }
    
    public void SkipCutscene()
    {
        hasSkipped = true;
        videoPlayer.Stop();
        LoadTargetLevel();
    }
    
    void LoadTargetLevel()
    {
        SceneManager.LoadScene(targetLevelName);
    }
}