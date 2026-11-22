using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
    
public class MarioController : MonoBehaviour
{
    CharacterController m_CharacterController;
    Animator m_Animator;
    public Camera m_Camera;

    public float m_WalkSpeed = 4.0f;
    public float m_RunSpeed = 8.0f;
    public float m_RotationLerpPct = 10.0f;
    public float m_VerticalSpeed ;
    public float m_JumpSpeed = 10.0f;
    public bool m_CanJump = true;
    
    [Header("Input")] 
    public Key m_RightKeyControl = Key.D;
    public Key m_LeftKeyControl = Key.A;
    public Key m_ForwardKeyControl = Key.W;
    public Key m_BackwardKeyControl = Key.S;
    public Key m_RunKeyControl = Key.LeftShift;
    public Key m_JumpKeyControl = Key.Space;
    
    [Header("Angle Locking")]
    public Key m_DebugLockAngleKeyCode = Key.I;
    public Key m_DebugLockCursorKeyCode = Key.O;
    public bool m_AngleLocked;
    
    bool m_GameOver;
    bool m_IsBeingKilled;
    bool m_MovementDisabled;
    
    void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    void Start()
    {
        GameController.GetGameController().SetPlayer(this);
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        if (CantMove())
            return;
        
        if (Keyboard.current[m_DebugLockAngleKeyCode].wasPressedThisFrame)
        {
            m_AngleLocked = !m_AngleLocked;
        }
        if (Keyboard.current[m_DebugLockCursorKeyCode].wasPressedThisFrame)
        {
            Cursor.lockState = Cursor.lockState != CursorLockMode.Locked ? CursorLockMode.Locked : CursorLockMode.None;
        }
        
        Vector3 l_Right = m_Camera.transform.right;
        Vector3 l_Forward = m_Camera.transform.forward;
        l_Right.y = 0.0f;
        l_Forward.y = 0.0f;
        l_Right.Normalize();
        l_Forward.Normalize();

        Vector3 l_Direction = Vector3.zero;
        
        if (Keyboard.current[m_RightKeyControl].isPressed)
            l_Direction += l_Right;
        else if (Keyboard.current[m_LeftKeyControl].isPressed)
            l_Direction += -l_Right;
        if(Keyboard.current[m_ForwardKeyControl].isPressed)
            l_Direction += l_Forward;
        else if (Keyboard.current[m_BackwardKeyControl].isPressed)
            l_Direction += -l_Forward;
        
        bool l_IsMoving = l_Direction.sqrMagnitude > 0.5f;
        float l_Speed = m_WalkSpeed;
        float l_AnimatorSpeed = 0.0f;
        float l_AnimatorVerticalSpeed = 0.0f;
        l_Direction.Normalize();
        
        if (l_IsMoving)
        {
            if(Keyboard.current[m_RunKeyControl].isPressed)
            {
                l_Speed = m_RunSpeed;
                l_AnimatorSpeed = 1.0f;
            }
            else
            {
                l_AnimatorSpeed = 0.3f;
            }
            Quaternion l_DesiredRotation = Quaternion.LookRotation(l_Direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, l_DesiredRotation, m_RotationLerpPct * Time.deltaTime);
        }
        if(Keyboard.current[m_JumpKeyControl].wasPressedThisFrame && m_CanJump)
        {
            m_VerticalSpeed = m_JumpSpeed;
            l_AnimatorVerticalSpeed = m_JumpSpeed;
            m_CanJump = false;
        }

        m_Animator.SetFloat("Speed", l_AnimatorSpeed);

        l_Direction *= l_Speed * Time.deltaTime;
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Direction.y = m_VerticalSpeed * Time.deltaTime;
        
        if(m_VerticalSpeed < 0)
        {
            m_CanJump = false;
            l_AnimatorVerticalSpeed = m_VerticalSpeed;
            //m_Animator.SetFloat("VerticalSpeed", l_AnimatorVerticalSpeed);
        }

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Direction);
        if((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = -1.0f;
            m_CanJump = true;
            l_AnimatorVerticalSpeed = 0.0f;
        }
        else if((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0)
            m_VerticalSpeed = 0.0f;

        m_Animator.SetFloat("VerticalSpeed", l_AnimatorVerticalSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeadZone"))
        {
            //Kill(true);
        }
        /*
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
        }*/
        
        else if (other.CompareTag("ScenePortal"))
        {
            string l_sceneName = other.gameObject.GetComponent<LevelPortal>().m_SceneToLoad;
            if (l_sceneName != "")
            {
                GameController.GetGameController().EnterLevel(l_sceneName);
            }
        }
    }
    
    public void SetGameOver(bool gameOver)
    {
        m_GameOver = gameOver;
    }

    public bool GetGameOver()
    {
        return m_GameOver;
    }

    public void DisableMovement (bool disable)
    {
        m_MovementDisabled = disable;
    }
    
    public bool CantMove()
    {
        return m_GameOver || m_IsBeingKilled || m_MovementDisabled;
    }
}
