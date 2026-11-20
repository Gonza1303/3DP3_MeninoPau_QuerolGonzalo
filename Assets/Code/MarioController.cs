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
    
    [Header("Input")] 
    public Key m_RightKeyControl = Key.D;
    public Key m_LeftKeyControl = Key.A;
    public Key m_ForwardKeyControl = Key.W;
    public Key m_BackwardKeyControl = Key.S;
    public Key m_RunKeyControl = Key.LeftShift;
    
    void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        Vector3 l_Right = m_Camera.transform.right;
        Vector3 l_Forward = m_Camera.transform.forward;
        l_Right.y = 0.0f;
        l_Forward.y = 0.0f;
        l_Right.Normalize();
        l_Forward.Normalize();

        Vector3 l_Direction = Vector3.zero;
        
        if (Keyboard.current[m_RightKeyControl].isPressed)
            l_Direction = l_Right;
        else if (Keyboard.current[m_LeftKeyControl].isPressed)
            l_Direction = -l_Right;
        if(Keyboard.current[m_ForwardKeyControl].isPressed)
            l_Direction = l_Forward;
        else if (Keyboard.current[m_BackwardKeyControl].isPressed)
            l_Direction = -l_Forward;

        bool l_IsMoving = l_Direction.sqrMagnitude > 0.5f;
        float l_Speed = m_WalkSpeed;
        float l_AnimatorSpeed = 0.0f;
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
        }
        m_Animator.SetFloat("Speed", l_AnimatorSpeed);
        l_Direction.Normalize();

        l_Direction *= l_Speed * Time.deltaTime;
        m_CharacterController.Move(l_Direction);
    }
}
