using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public Transform m_DestroyObjects;
    public GameObject m_GameOverPanel;
    static GameController m_GameController = null;
    
    public Key m_RestartKeyCode = Key.F5;
    public Key m_GameOverRestartKeyCode = Key.Enter;
    
    string m_LastLevelPlayed;
    
    MarioController m_Player;
    bool m_GameOver;
    
    /*
    Turret m_CurrentTurretLaserAudio;
    WallLaser m_CurrentWallLaserAudio;
    
    List<Turret> m_Turrets = new List<Turret>();
    List<WallLaser> m_WallLasers = new List<WallLaser>();
    List<Companion> m_Companions = new List<Companion>();
    List<RefractionCube> m_RefractionCubes = new List<RefractionCube>();*/
    
    Fade m_Fade;
    
    //public AudioClip m_GameOverRestartSFX;

    private void Awake()
    {
        if (m_GameController == null)
        {
            m_GameController = this;
            DontDestroyOnLoad(gameObject);
            SetGameOverPanel(false);
        }
        else
            Destroy(gameObject);
    }
    
    public void Update()
    {
        if (m_GameOver)
        {
            if (Keyboard.current[m_GameOverRestartKeyCode].wasPressedThisFrame)
                Restart();

            return;
        }

        
        if(Keyboard.current[m_RestartKeyCode].wasPressedThisFrame)
            Restart();

        //UpdateLaserAudio();
    }
    
    public void SetPlayer(MarioController player)
    {
        m_Player = player;
    }

    public MarioController GetPlayer()
    {
        return m_Player;
    }
    
    static public GameController GetGameController()
    {
        return m_GameController;
    }
    
    public void EnterLevel(string sceneName)
    {
        m_LastLevelPlayed = sceneName;
        SceneManager.LoadScene(sceneName);
    }
    
    public void ReturnToCastle(bool levelCompleted)
    {
        /*
        if (levelCompleted && !string.IsNullOrEmpty(m_LastLevelPlayed))
        {
            if (!m_CompletedLevels.Contains(m_LastLevelPlayed))
            {
                m_CompletedLevels.Add(m_LastLevelPlayed);
                m_StarsCount++;
            }
        }*/

        m_GameOver = false;
        SceneManager.LoadScene("Castle");
    }

    public void Restart()
    {
        m_GameOver = false;
        SetGameOverPanel(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StartCoroutine(m_Fade.FadeOut());
    }

    /*
    private void UpdateLaserAudio()
    {
        if (m_Player == null)
        {
            StopCurrentLaserAudio();
            return;
        }

        Turret l_ClosestTurret = null;
        float l_ClosestTurretDistance = float.MaxValue;

        foreach (Turret l_Turret in m_Turrets)
        {
            if (l_Turret == null || !l_Turret.IsLaserActive)
            {
                continue;
            }

            float l_Distance = (l_Turret.transform.position - m_Player.transform.position).sqrMagnitude;

            if (l_Distance < l_ClosestTurretDistance)
            {
                l_ClosestTurretDistance = l_Distance;
                l_ClosestTurret = l_Turret;
            }
        }

        WallLaser l_ClosestWallLaser = null;
        float l_ClosestWallLaserDistance = float.MaxValue;

        foreach (WallLaser l_WallLaser in m_WallLasers)
        {
            if (l_WallLaser == null || !l_WallLaser.IsLaserActive)
            {
                continue;
            }

            float l_Distance =(l_WallLaser.transform.position - m_Player.transform.position).sqrMagnitude;

            if (l_Distance < l_ClosestWallLaserDistance)
            {
                l_ClosestWallLaserDistance = l_Distance;
                l_ClosestWallLaser = l_WallLaser;
            }
        }

        if (l_ClosestTurret != null && l_ClosestTurretDistance > l_ClosestTurret.LaserAudioMaxDistance * l_ClosestTurret.LaserAudioMaxDistance)
        {
            l_ClosestTurret = null;
        }

        if (l_ClosestWallLaser != null && l_ClosestWallLaserDistance > l_ClosestWallLaser.LaserAudioMaxDistance * l_ClosestWallLaser.LaserAudioMaxDistance)
        {
            l_ClosestWallLaser = null;
        }

        if (l_ClosestTurret != null && l_ClosestWallLaser != null)
        {
            if (l_ClosestWallLaserDistance < l_ClosestTurretDistance)
            {
                l_ClosestTurret = null;
            }
            else
            {
                l_ClosestWallLaser = null;
            }
        }

        if (l_ClosestTurret == null && l_ClosestWallLaser == null)
        {
            StopCurrentLaserAudio();
            return;
        }

        if (m_CurrentTurretLaserAudio == l_ClosestTurret && m_CurrentWallLaserAudio == l_ClosestWallLaser)
        {
            return;
        }

        StopCurrentLaserAudio();

        if (l_ClosestTurret != null)
        {
            m_CurrentTurretLaserAudio = l_ClosestTurret;
            l_ClosestTurret.StartLaserAudio();
        }
        else
        {
            m_CurrentWallLaserAudio = l_ClosestWallLaser;
            l_ClosestWallLaser.StartLaserAudio();
        }
    }

    private void StopCurrentLaserAudio()
    {
        if (m_CurrentTurretLaserAudio != null)
        {
            m_CurrentTurretLaserAudio.StopLaserAudio();
            m_CurrentTurretLaserAudio = null;
        }

        if (m_CurrentWallLaserAudio != null)
        {
            m_CurrentWallLaserAudio.StopLaserAudio();
            m_CurrentWallLaserAudio = null;
        }
    } */

    
    public void SetGameOver()
    {
        if (m_GameOver)
            return;

        m_GameOver = true;
        //StopCurrentLaserAudio();
        SetGameOverPanel(true);
        //m_Player.SetGameOver(true);

        /*
        foreach (Turret l_Turret in m_Turrets)
        {
            if (l_Turret != null)
            {
                l_Turret.StopLaserAudio();
            }
        }

        foreach (WallLaser l_WallLaser in m_WallLasers)
        {
            if (l_WallLaser != null)
            {
                l_WallLaser.StopLaserAudio();
            }
        }

        foreach (RefractionCube l_RefractionCube in m_RefractionCubes)
        {
            if (l_RefractionCube != null)
            {
                l_RefractionCube.StopRefractionAudio();
            }
        }*/
    }

    void SetGameOverPanel(bool active)
    {
        if (m_GameOverPanel != null)
            m_GameOverPanel.SetActive(active);
    }
    
    public string GetLastLevelPlayed()
    {
        return m_LastLevelPlayed;
    }
    
    public void SetLastLevelPlayed(string levelName)
    {
        m_LastLevelPlayed = levelName;
    }
    
    /*
    public void AddTurret(Turret turret)
    {
        m_Turrets.Add(turret);
    }

    public List<Turret> GetTurrets()
    {
        return m_Turrets;
    }

    public void AddWallLaser(WallLaser wallLaser)
    {
        m_WallLasers.Add(wallLaser);
    }

    public List<WallLaser> GetWallLasers()
    {
        return m_WallLasers;
    }
    
    public void AddCompanion(Companion companion)
    {
        m_Companions.Add(companion);
    }

    public List<Companion> GetCompanions()
    {
        return m_Companions;
    }
    
    public void AddRefractionCube(RefractionCube refractionCube)
    {
        m_RefractionCubes.Add(refractionCube);
    }
    
    public List<RefractionCube> GetRefractionCubes()
    {
        return m_RefractionCubes;
    }
    */
    
    public void SetFade(Fade fade)
    {
        m_Fade = fade;
    }
    
    public Fade GetFade()
    {
        return m_Fade;
    }
}
