using System;
using UnityEngine;
using UnityEngine.Serialization;

public class TCDebugTimeScaler : MonoBehaviour
{
    public KeyCode m_AccelerateKeyCode = KeyCode.RightControl;
    public KeyCode m_DecelerateKeyCode = KeyCode.RightShift;
    
#if UNITY_EDITOR
    private void Update()
    {
#if TC_MY_PRECOMPILED_SYMBOL
        Debug.Log("Hola sergio");
#endif

        if (Input.GetKey(m_AccelerateKeyCode)) {
            if (Time.timeScale == 1.0f)
                Time.timeScale = 10.0f;
            else 
                Time.timeScale = 1.0f;
        }
        else if (Input.GetKey(m_DecelerateKeyCode))
        {
            if (Time.timeScale == 1.0f)
                Time.timeScale = 0.1f;
            else 
                Time.timeScale = 1.0f;
        }
        
    }
#endif
}
