using UnityEngine;

public class LifeItem : Item
{
    public int m_LifePoints = 20;

    public override bool CanPick()
    {
        return PlayerHealthManager.Instance.CanAddLife(m_LifePoints);
    }

    public override void Pick()
    {
        if (PlayerHealthManager.Instance != null &&
            PlayerHealthManager.Instance.AddLife(m_LifePoints))
        {
            base.Pick();
            Debug.Log("Pick life item: " + m_LifePoints);
        }
    }
    
}
