using UnityEngine;
using UnityEngine.Serialization;

public class HitCollider : MonoBehaviour
{
    public enum THitColliderType
    {
        HEAD,
        HELIX,
        BODY
    }
    
    public THitColliderType m_Type;
    //public DroneEnemy m_DroneEnemy;

    public void Hit()
    {
        //m_DroneEnemy.Hit(m_Type);
    }
}
