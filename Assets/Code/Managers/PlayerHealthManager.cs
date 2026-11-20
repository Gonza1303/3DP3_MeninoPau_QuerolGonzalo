using System;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    public static PlayerHealthManager Instance { get; private set; }
    
    public int m_MaxHealth = 100;
    private int m_CurrentHealth;
    private bool m_IsDead;
    
    public Action<int> OnHealthUpdated;

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
    }
    
    void Start()
    {
        m_CurrentHealth = m_MaxHealth;
        OnHealthUpdated?.Invoke(m_CurrentHealth);
    }

    public void TakeDamage(int damage)
    {
        m_CurrentHealth -= damage;
        OnHealthUpdated?.Invoke(m_CurrentHealth);
        
        if (m_CurrentHealth <= 0)
        {
            m_IsDead = true;
            PortalController.Instance.Kill(true);
        }
    }
    
    public bool AddLife(int life)
    {
        if (m_CurrentHealth + life > m_MaxHealth)
            m_CurrentHealth = m_MaxHealth;
        else
            m_CurrentHealth += life;

        OnHealthUpdated?.Invoke(m_CurrentHealth);
        return true;
    }
    
    public bool CanAddLife(int life)
    {
        return m_CurrentHealth < m_MaxHealth;
    }
    
    
    public void RestartHealth()
    {
        m_CurrentHealth = m_MaxHealth;
        OnHealthUpdated?.Invoke(m_CurrentHealth);
    }

    public bool IsDead()
    {
        return m_IsDead;
    }
}
