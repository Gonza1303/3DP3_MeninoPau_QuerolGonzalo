using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
    
public class MarioController : MonoBehaviour
{
    CharacterController m_CharacterController;
    Animator m_Animator;
    public Camera m_Camera;

    [Header("HorizontalMovement")]
    public float m_WalkSpeed = 4.0f;
    public float m_RunSpeed = 8.0f;
    public float m_RotationLerpPct = 10.0f;
    private Vector3 m_HorizontalVelocity = Vector3.zero;
    
    [Header("VerticalMovement")]
    public float m_VerticalSpeed ;
    public float m_JumpSpeed = 10.0f;
    public bool m_CanJump = true;
    public bool m_IsGrounded = true;
    public float m_AirControl = 2.0f;
    
    [Header("WallJump")]
    public bool m_WallJump;
    public float m_WallJumpForce = 6.0f;
    public float m_WallJumpDamping = 4.0f;
    public Vector3 m_WallNormal = Vector3.zero;
    public Vector3 m_WallJumpVelocity = Vector3.zero;
    
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
            if(Keyboard.current[m_RunKeyControl].isPressed && m_IsGrounded)
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
            
            if (m_WallJump)
            {
                m_WallJumpVelocity = m_WallNormal * m_WallJumpForce;
                transform.rotation = Quaternion.LookRotation(m_WallNormal); 
                m_WallJump = false;
            }
        }

        m_Animator.SetFloat("Speed", l_AnimatorSpeed);
        
        Vector3 l_DesiredVelocity =  l_Direction * l_Speed;

        if (m_IsGrounded)
            m_HorizontalVelocity = l_DesiredVelocity;
        else
            m_HorizontalVelocity = Vector3.Lerp(m_HorizontalVelocity, l_DesiredVelocity, m_AirControl * Time.deltaTime);
        
        Vector3 l_Movement = (m_HorizontalVelocity + m_WallJumpVelocity) * Time.deltaTime;
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;
        
        m_WallJumpVelocity = Vector3.Lerp(m_WallJumpVelocity, Vector3.zero, m_WallJumpDamping * Time.deltaTime);

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_VerticalSpeed = -1.0f;
            m_CanJump = true;
            m_WallJump = false;
            m_IsGrounded = true;
            l_AnimatorVerticalSpeed = 0.0f;
        }
        else
        {
            m_IsGrounded = false;
            m_CanJump = false;
            m_WallJump = false;
            
            if((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0)
                m_VerticalSpeed = 0.0f;
            
            if ((l_CollisionFlags & CollisionFlags.Sides) != 0)
            {
                m_CanJump = true;
                m_WallJump = true;
            }
            l_AnimatorVerticalSpeed = m_VerticalSpeed;
        }

        m_Animator.SetFloat("VerticalSpeed", l_AnimatorVerticalSpeed);
    }
    
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Mathf.Abs(hit.normal.y) < 0.5f)
        {
            m_WallNormal = hit.normal;
            m_WallNormal.y = 0.0f;
            m_WallNormal.Normalize();
        }
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
