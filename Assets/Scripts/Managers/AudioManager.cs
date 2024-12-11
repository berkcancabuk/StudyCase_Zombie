using UnityEngine;
using System.Collections.Generic;

public enum SoundType
{
    BackgroundMusic,
    ZombieAttack,
    PlayerAttack
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public SoundType type;
        public AudioClip clip;
        public bool loop;
        [Range(0f, 1f)] public float volume = 1f;
        public AudioSource source;
    }

    public List<Sound> sounds = new();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
        }
    }

    public void Play(SoundType type)
    {
        Sound sound = sounds.Find(s => s.type == type);
        if (sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning($"Sound {type} not found!");
        }
    }

    public void Stop(SoundType type)
    {
        Sound sound = sounds.Find(s => s.type == type);
        if (sound != null)
        {
            sound.source.Stop();
        }
    }

    public void PlayRandomZombieGroan()
    {
        Sound zombieSound = sounds.Find(s => s.type == SoundType.ZombieAttack);
        if (zombieSound != null)
        {
            zombieSound.source.pitch = Random.Range(0.8f, 1.2f);
            zombieSound.source.Play();
        }
    }

    // Volume Settings
    public void SetMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}