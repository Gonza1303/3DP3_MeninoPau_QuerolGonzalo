using System.Collections.Generic;
using UnityEngine;
using Cursor = UnityEngine.Cursor;

public class PortalController : MonoBehaviour
{
    public static PortalController Instance { get; private set; }

    public Transform m_PitchController;

    float m_Yaw;
    float m_Pitch;
	Vector3 m_Direction;
    public float m_YawSpeed;
    public float m_PitchSpeed;
    public bool m_InvertedYaw;
    public bool m_InvertedPitch;
    public float m_MinPitch;
    public float m_MaxPitch;

	public float m_TeleportPortalOffset = 1.0f;
	public float m_MaxValidAngle = 40.0f;

    CharacterController m_CharacterController;

    public float m_Speed;
    public float m_SprintSpeed;
    public float m_JumpSpeed;
    
    [Header("Player Movement")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_SprintKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    
    [Header("Player Start Position")]
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    float m_StartPitch;
    
    [Header("Checkpoint Positions")]
    public List<Transform> m_Checkpoints = new List<Transform>();
    public List<GameObject> m_CheckpointDoors = new List<GameObject>();
    private int m_CurrentCheckpointIndex = -1;
    
    [Header("Angle Locking")]
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.Y;
    public KeyCode m_DebugLockCursorKeyCode = KeyCode.O;
    
    public bool m_AngleLocked;
    
    [Header("Audio Clips")]
    public AudioClip m_CheckPointSFX;
    public AudioClip m_Teleport1SFX;
    public AudioClip m_Teleport2SFX;
    public List<AudioClip> m_LaserDeathSFXs;
    public List<AudioClip> m_LavaDeathsSFXs;

    float m_VerticalSpeed;
    bool m_GameOver;
    bool m_IsBeingKilled;

    private Transform m_StartPlayer;
    
    public LayerMask m_ShootLayerMask;

    public GameObject WinImage;
	public bool m_Win = false;
	private KeyCode m_QuitCode = KeyCode.Q;
    
    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
        m_CharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_Yaw = transform.rotation.eulerAngles.y;
        m_StartPitch = m_PitchController.localEulerAngles.x;
        m_Pitch = m_StartPitch;
        Cursor.lockState = CursorLockMode.Locked;
        //GameController.GetGameController().SetPlayer(this);
        WinImage.SetActive(false);
    }

    private void Update()
    {	
		if(m_Win && Input.GetKeyDown(m_QuitCode))
			Application.Quit();
			
        if (m_GameOver || m_IsBeingKilled || CantMove())
            return;

        if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
        {
            m_AngleLocked = !m_AngleLocked;
        }
        if (Input.GetKeyDown(m_DebugLockCursorKeyCode))
        {
            Cursor.lockState = Cursor.lockState != CursorLockMode.Locked ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");

        if(m_InvertedYaw)
            l_MouseX = -l_MouseX;
        if(m_InvertedPitch)
            l_MouseY = -l_MouseY;
        if (!m_AngleLocked)
        {
            m_Yaw = m_Yaw + l_MouseX * m_YawSpeed * Time.deltaTime;
            m_Pitch = m_Pitch + l_MouseY * m_PitchSpeed * Time.deltaTime;
        }

        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        m_Direction = Vector3.zero;

        if(Input.GetKey(m_RightKeyCode))
            m_Direction = transform.right;
        else if(Input.GetKey(m_LeftKeyCode))
            m_Direction = -transform.right;

        if(Input.GetKey(m_UpKeyCode))
            m_Direction += transform.forward;
        if(Input.GetKey(m_DownKeyCode))
            m_Direction -= transform.forward;

        float l_Speed = m_Speed;
        if(Input.GetKey(m_SprintKeyCode) && m_VerticalSpeed == 0.0f)
            l_Speed = m_SprintSpeed;
        if(Input.GetKey(m_JumpKeyCode) && m_VerticalSpeed == 0.0f)
            m_VerticalSpeed = m_JumpSpeed;

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;

        m_Direction.Normalize();
        Vector3 l_Direction = m_Direction * l_Speed * Time.deltaTime;
        l_Direction.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_collisionFlags = m_CharacterController.Move(l_Direction);

        if((l_collisionFlags & CollisionFlags.CollidedBelow) != 0)
            m_VerticalSpeed = 0.0f;
        else if((l_collisionFlags & CollisionFlags.CollidedAbove)!=0 && m_VerticalSpeed > 0)
            m_VerticalSpeed = 0.0f;
    }

    public void Kill(bool isLavaDeath)
    {
        if (m_IsBeingKilled || m_GameOver)
            return;
        
        if (isLavaDeath)
        {
            SFXManager.Instance.PlaySFXClip(m_LavaDeathsSFXs[UnityEngine.Random.Range(0, m_LavaDeathsSFXs.Count)], transform, 1.0f);
        }
        else
        {
            SFXManager.Instance.PlaySFXClip(m_LaserDeathSFXs[UnityEngine.Random.Range(0, m_LaserDeathSFXs.Count)], transform, 1.0f);
        }

        m_IsBeingKilled = true;
        SetGameOver(true);
        StartCoroutine(GameController.GetGameController().GetFade().FadeIn(() => {
            GameController.GetGameController().GetFade().gameObject.SetActive(false);
            GameController.GetGameController().SetGameOver();
        }));
    }

	private void OnTriggerEnter(Collider other)
	{
        /*
		if(other.CompareTag("Portal"))
		{
			Portal l_Portal = other.GetComponent<Portal>();
			if(l_Portal != null)
			{
				if(CanTeleport(l_Portal))
				{
					Teleport(l_Portal);
				}
			}
		}
		else if (other.CompareTag("Button"))
		{
			Button l_Button = other.GetComponent<CubeSpawnerButton>();
			if (l_Button != null)
			{
				l_Button.Press();
			}
		}*/
        
        if (other.CompareTag("DeadZone"))
        {
            Kill(true);
        }
        
        else if (other.CompareTag("Checkpoint"))
        {
            for (int i = 0; i < m_Checkpoints.Count; ++i)
            {
                Transform l_Checkpoint = m_Checkpoints[i];

                if (l_Checkpoint == null)
                {
                    continue;
                }

                if (other.transform == l_Checkpoint || other.transform.IsChildOf(l_Checkpoint))
                {
                    if (i > m_CurrentCheckpointIndex)
                    {
                        m_CurrentCheckpointIndex = i;
                        m_CheckpointDoors[i].SetActive(true);
                        SFXManager.Instance.PlaySFXClip(m_CheckPointSFX, transform, 1.0f);
                    }

                    break;
                }
            }

        }
        
        else if (other.CompareTag("Finish"))
        {
            WinImage.SetActive(true);
			m_Win = true;
        }
        
        else if (other.CompareTag("NextScene"))
        {
            SceneFlowManager.Instance.LoadNextScene();
        }
	}

    private void OnTriggerExit(Collider other)
    {
        /*
        if (other.CompareTag("Button"))
        {
            CubeSpawnerButton l_Button = other.GetComponent<CubeSpawnerButton>();
            if (l_Button != null)
            {
                l_Button.Release();
            }
        }*/
    }

    public void SetGameOver(bool gameOver)
    {
        m_GameOver = gameOver;
    }

    public bool GetGameOver()
    {
        return m_GameOver;
    }

    public bool IsDead()
    {
        return m_IsBeingKilled || m_GameOver;
    }

    public bool CantMove()
    {
        return false;
    }

	private void LimitScaleTransformation()
	{
		Vector3 l_Scale = new Vector3(2f, 2f, 2f);
		if(transform.localScale.x > l_Scale.x) transform.localScale = l_Scale;
		l_Scale = new Vector3(0.5f, 0.5f, 0.5f);
		if(transform.localScale.x < l_Scale.x) transform.localScale = l_Scale;
	}
    
    public void Restart()
    {
        m_IsBeingKilled = false;
        
        SetGameOver(false);
        m_CharacterController.enabled = false;
        
        if (m_CurrentCheckpointIndex >= 0 && m_CurrentCheckpointIndex < m_Checkpoints.Count)
        {
            Transform l_Checkpoint = m_Checkpoints[m_CurrentCheckpointIndex];

            if (l_Checkpoint != null)
            {
                transform.position = l_Checkpoint.position;
                transform.rotation = l_Checkpoint.rotation;
                m_Yaw = l_Checkpoint.rotation.eulerAngles.y;
            }
        }
        else
        {
            transform.position = m_StartPosition;
            transform.rotation = m_StartRotation;
            m_Yaw = m_StartRotation.eulerAngles.y;
        }
        
        m_Pitch = m_StartPitch;
        m_PitchController.localRotation = Quaternion.Euler(m_StartPitch, 0.0f, 0.0f);
        m_CharacterController.enabled = true;
    }
    
}
