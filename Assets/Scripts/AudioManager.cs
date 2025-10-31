using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Serializable]
    public class NamedClip
    {
        public string key;           // t.ex. "shoot"
        public AudioClip clip;
        public bool isMusic;         // markera musik
    }

    public static AudioManager I { get; private set; }

    [Header("Clips")]
    public List<NamedClip> clips = new List<NamedClip>();

    [Header("Mixer (valfritt)")]
    public AudioMixer mixer;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup uiGroup;

    [Header("Pool")]
    [SerializeField] int sfxPoolSize = 10;

    private readonly Dictionary<string, AudioClip> _map = new();
    private AudioSource _music;
    private List<AudioSource> _pool;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        foreach (var n in clips)
            if (n.clip && !string.IsNullOrWhiteSpace(n.key))
                _map[n.key] = n.clip;

        // Musik-källa
        _music = gameObject.AddComponent<AudioSource>();
        _music.loop = true;
        _music.playOnAwake = false;
        _music.spatialBlend = 0f;
        if (musicGroup) _music.outputAudioMixerGroup = musicGroup;

        // SFX-pool
        _pool = new List<AudioSource>(sfxPoolSize);
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var a = new GameObject($"SFX_{i}").AddComponent<AudioSource>();
            a.transform.SetParent(transform);
            a.playOnAwake = false;
            a.spatialBlend = 0f;              // 2D-ljud; sätt till 1f om du vill ha 3D
            if (sfxGroup) a.outputAudioMixerGroup = sfxGroup;
            _pool.Add(a);
        }
    }

    AudioSource GetFree()
    {
        foreach (var a in _pool) if (!a.isPlaying) return a;
        return _pool[0]; // fallback – skriv över äldsta
    }

    public static void Play(string key, float vol = 1f, float pitch = 1f)
    {
        if (I == null || !I._map.TryGetValue(key, out var clip) || clip == null) return;
        var a = I.GetFree();
        a.volume = vol;
        a.pitch = pitch;
        a.transform.position = Vector3.zero;
        a.spatialBlend = 0f;
        a.PlayOneShot(clip);
    }

    public static void PlayAt(string key, Vector3 pos, float vol = 1f, float pitch = 1f, float spatial = 1f)
    {
        if (I == null || !I._map.TryGetValue(key, out var clip) || clip == null) return;
        var a = I.GetFree();
        a.transform.position = pos;
        a.spatialBlend = Mathf.Clamp01(spatial); // 0=2D, 1=3D
        a.volume = vol;
        a.pitch = pitch;
        a.PlayOneShot(clip);
    }

    public static void PlayMusic(string key, float vol = 0.6f)
    {
        if (I == null || !I._map.TryGetValue(key, out var clip) || clip == null) return;
        I._music.clip = clip;
        I._music.volume = vol;
        I._music.Play();
    }

    public static void StopMusic() { if (I != null) I._music.Stop(); }

    // Volym-API om du använder mixer: ex. SetVolume("Master", -10f) i dB
    public static void SetVolume(string exposedParam, float dB)
    {
        if (I != null ? I.mixer : null) I.mixer.SetFloat(exposedParam, dB);
    }
}
