using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class VideoTrigger : MonoBehaviour
{
    public VideoPlayer m_VideoPlayer;
    public string m_VideoFileName = "TetravexAiPs.mp4";
    
    public Key m_StartVideoKey = Key.Enter;
    public Key m_StopVideoKey = Key.Space;
    public Key m_ReturnToCastleKey = Key.Escape;
    
    private bool m_AlreadyEntered;
    private bool m_AlreadyActive;

    void Start()
    {
        if (m_VideoPlayer == null)
            m_VideoPlayer = GetComponent<VideoPlayer>();

        // 1. Construïm la ruta cap a Assets/StreamingAssets/TetravexAiPs.mp4
        string videoPath = Path.Combine(Application.streamingAssetsPath, m_VideoFileName);

        // 2. Configurem la URL i els esdeveniments
        m_VideoPlayer.source = VideoSource.Url;
        m_VideoPlayer.url = videoPath;
        
        m_VideoPlayer.loopPointReached += OnVideoFinished;

        // 3. Pre-carreguem els primers fotogrames abans d'engegar
        m_VideoPlayer.Prepare();
    }
    
    void Update()
    {
        if (Keyboard.current[m_ReturnToCastleKey].wasPressedThisFrame)
        {
            m_VideoPlayer.Pause();
            SetPlayerMovementDisabled(false);
            
            if (m_AlreadyActive)
            {
                m_AlreadyActive = false;
            }
            else
            {
                m_AlreadyEntered = false;
                m_AlreadyActive = false;
                ReturnToCastle();
            }
        }
        
        if (m_AlreadyEntered && !m_AlreadyActive && Keyboard.current[m_StartVideoKey].wasPressedThisFrame)
        {
            m_AlreadyActive = true;
            m_VideoPlayer.Play();
            SetPlayerMovementDisabled(true);
        }

        if (m_AlreadyActive && m_VideoPlayer.isPlaying)
        {
            if (Keyboard.current[m_StopVideoKey].wasPressedThisFrame)
            {
                m_VideoPlayer.Pause();
            }
        }
        
        else if (m_AlreadyActive && !m_VideoPlayer.isPlaying)
        {
            if (Keyboard.current[m_StopVideoKey].wasPressedThisFrame)
            {
                m_VideoPlayer.Play();
            }
        }
    }
    
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !m_AlreadyActive)
        {
            m_AlreadyEntered = true;
        }
    }
    
    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player") && !m_AlreadyActive)
        {
            m_AlreadyEntered = false;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        SetPlayerMovementDisabled(false);

        // Quan s'acaben els 35 minuts, tornem automàticament al castell
        ReturnToCastle();
    }

    private void SetPlayerMovementDisabled(bool disabled)
    {
        
        GameController.GetGameController().GetPlayer().DisableMovement(disabled);
    }
    
    bool IsVideoPlaying()
    {
        return m_VideoPlayer.isPlaying;
    }

    void ReturnToCastle()
    {
        // Fem servir el teu GameController existent perquè el Mario surti davant del quadre
        GameController gc = GameController.GetGameController();
        if (gc != null)
        {
            gc.ReturnToCastle( true);
        }
        else
        {
            // Fallback per si proves aquesta escena sola sense venir des del castell
            UnityEngine.SceneManagement.SceneManager.LoadScene("Castle");
        }
    }
}