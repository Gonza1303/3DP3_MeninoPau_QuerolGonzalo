using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }
    
    [SerializeField] private AudioSource m_SFXObject;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void PlaySFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(m_SFXObject, spawnTransform.position, Quaternion.identity);
        
        audioSource.clip = clip;
        
        audioSource.volume = volume;
        
        audioSource.Play();
        
        float clipLength = clip.length;
        
        Destroy(audioSource.gameObject, clipLength);
    }

    public AudioSource PlayLoopingSFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(m_SFXObject, spawnTransform.position, Quaternion.identity);
        audioSource.transform.SetParent(spawnTransform);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.spatialBlend = 1.0f;
        audioSource.Play();

        return audioSource;
    }

    public void StopSFXClip(AudioSource audioSource)
    {
        if (audioSource == null)
        {
            return;
        }

        audioSource.Stop();
        Destroy(audioSource.gameObject);
    }
}
