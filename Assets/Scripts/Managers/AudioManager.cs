using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource pauseMusicSource;
    public AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Auto-setup AudioSources to prevent assignment errors
            AudioSource[] sources = GetComponents<AudioSource>();
            while (sources.Length < 3)
            {
                gameObject.AddComponent<AudioSource>();
                sources = GetComponents<AudioSource>();
            }

            if (musicSource == null) musicSource = sources[0];
            if (pauseMusicSource == null) pauseMusicSource = sources[1];
            if (sfxSource == null) sfxSource = sources[2];
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        Debug.Log("Intentando reproducir música: " + (clip != null ? clip.name : "Nulo"));
        
        if (clip == null) 
        {
            Debug.LogWarning("El clip de música es nulo.");
            return;
        }
        if (musicSource == null) 
        {
            Debug.LogWarning("No hay un AudioSource asignado en musicSource.");
            return;
        }
        
        if (musicSource.clip == clip && musicSource.isPlaying) 
        {
            Debug.Log("La pista ya se está reproduciendo.");
            return;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
        Debug.Log("Reproduciendo con éxito: " + clip.name);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PauseGameMusic(AudioClip pauseClip)
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Pause();
        }

        if (pauseMusicSource != null && pauseClip != null)
        {
            pauseMusicSource.clip = pauseClip;
            pauseMusicSource.loop = true;
            pauseMusicSource.Play();
        }
    }

    public void ResumeGameMusic()
    {
        if (pauseMusicSource != null)
        {
            pauseMusicSource.Stop();
        }

        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }

    public void StopAllMusic()
    {
        if (musicSource != null) musicSource.Stop();
        if (pauseMusicSource != null) pauseMusicSource.Stop();
    }
}
