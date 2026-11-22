using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    /*
    public static AudioManager Instance { get; private set; }
    
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
    }*/
    
    public static AudioManager Instance { get; private set; }

    [Header("---------- Audio Sources ----------")]
    [SerializeField] private AudioSource m_MusicSource;
    [SerializeField] private AudioSource m_SFXSource;
    [SerializeField] private float m_MusicFadeDuration = 1f;

    [Header("---------- Music ----------")]
    public AudioClip m_Music;
    
    private float m_MusicVolume = 1f;
    private Coroutine m_MusicFade;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    
    private void Start()
    {
        m_MusicVolume = m_MusicSource.volume;
        m_MusicSource.clip = m_Music;
        m_MusicSource.Play();
    }
    
    private void PlayMusic(AudioClip clip)
    {
        if (clip == null || m_MusicSource.clip == clip) return;

        if (m_MusicFade != null) StopCoroutine(m_MusicFade);
        m_MusicFade = StartCoroutine(FadeToClip(clip));
    }
    
    public void PlayMusicFrom(AudioClip clip, float startTime)
    {
        m_MusicSource.clip = clip;
        m_MusicSource.time = Mathf.Clamp(startTime, 0f, clip.length - 0.01f);
        m_MusicSource.Play();
    }
    
    public void StopMusic()
    {
        if (m_MusicFade != null)
            StopCoroutine(m_MusicFade);

        m_MusicSource.Stop();
    }

    private IEnumerator FadeToClip(AudioClip clip)
    {
        yield return Fade(m_MusicSource.volume, 0f);

        m_MusicSource.clip = clip;
        m_MusicSource.Play();
        PlayMusicFrom(clip, 31f);
        
        yield return Fade(0f, m_MusicVolume);

        m_MusicFade = null;
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < m_MusicFadeDuration)
        {
            t += Time.unscaledDeltaTime;
            m_MusicSource.volume = Mathf.Lerp(from, to, t / m_MusicFadeDuration);
            yield return null;
        }
        m_MusicSource.volume = to;
    }
    
    public void PlaySfx(AudioClip clip, float volume = 1f)
    {
        if (clip == null || m_SFXSource == null)
            return;

        m_SFXSource.PlayOneShot(clip, volume);
    }
    
    public AudioSource PlayLoopingSFXClip(AudioClip clip, Transform spawnTransform, float volume)
    {
        AudioSource audioSource = Instantiate(m_SFXSource, spawnTransform.position, Quaternion.identity);
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
