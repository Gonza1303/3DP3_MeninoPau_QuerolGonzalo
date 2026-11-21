using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    public string m_SceneToLoad;
    public Transform m_SpawnPoint;

    private void Start()
    {
        GameController l_GameController = GameController.GetGameController();

        if (l_GameController != null && l_GameController.GetLastLevelPlayed() == m_SceneToLoad)
        {
            MarioController player = GameController.GetGameController().GetPlayer();
            if (player != null)
            {
                player.transform.position = m_SpawnPoint.position;
                player.transform.rotation = m_SpawnPoint.rotation;
            }

            l_GameController.SetLastLevelPlayed("");
        }
    }
}
