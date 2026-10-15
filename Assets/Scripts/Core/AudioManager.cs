using System.Collections.Generic;
using UnityEngine;

namespace AshenCitadel.Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        AudioSource _sfx;
        AudioSource _amb;
        readonly Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();
        public bool muted;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _sfx = gameObject.AddComponent<AudioSource>();
            _sfx.playOnAwake = false;
            _sfx.spatialBlend = 0f;

            _amb = gameObject.AddComponent<AudioSource>();
            _amb.playOnAwake = false;
            _amb.loop = true;
            _amb.volume = 0.12f;
            _amb.spatialBlend = 0f;

            BuildClips();
        }

        void Start()
        {
            if (!muted && _clips.ContainsKey("ambient"))
            {
                _amb.clip = _clips["ambient"];
                _amb.Play();
            }
        }

        void BuildClips()
        {
            _clips["click"] = Tone("click", 880, 0.05f, 0.2f, Wave.Square);
            _clips["upgrade"] = Chord("upgrade", new[] { 392f, 523f, 659f }, 0.18f, 0.35f);
            _clips["buy"] = Chord("buy", new[] { 523f, 659f, 784f }, 0.2f, 0.4f);
            _clips["win"] = Chord("win", new[] { 523f, 659f, 784f, 1046f }, 0.35f, 0.45f);
            _clips["lose"] = Tone("lose", 120, 0.4f, 0.5f, Wave.Saw);
            _clips["wave"] = Tone("wave", 200, 0.25f, 0.4f, Wave.Noise);
            _clips["hit"] = Tone("hit", 600, 0.04f, 0.25f, Wave.Square);
            _clips["notify"] = Tone("notify", 990, 0.08f, 0.22f, Wave.Sine);
            _clips["ambient"] = Noise("ambient", 2.5f, 0.08f);
        }

        public void Play(string id, float volumeScale = 1f)
        {
            if (muted || _sfx == null) return;
            if (!_clips.TryGetValue(id, out var clip) || clip == null) return;
            _sfx.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
        }

        public void SetMuted(bool m)
        {
            muted = m;
            if (_amb != null)
            {
                if (m) _amb.Pause();
                else if (!_amb.isPlaying && _amb.clip != null) _amb.Play();
            }
        }

        enum Wave { Sine, Square, Saw, Noise }

        static AudioClip Tone(string name, float freq, float duration, float vol, Wave wave)
        {
            int sampleRate = 22050;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float env = 1f - (float)i / samples;
                float v;
                switch (wave)
                {
                    case Wave.Square: v = Mathf.Sign(Mathf.Sin(2 * Mathf.PI * freq * t)); break;
                    case Wave.Saw: v = 2f * (t * freq - Mathf.Floor(0.5f + t * freq)); break;
                    case Wave.Noise: v = Random.Range(-1f, 1f); break;
                    default: v = Mathf.Sin(2 * Mathf.PI * freq * t); break;
                }
                data[i] = v * vol * env;
            }
            var clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        static AudioClip Chord(string name, float[] freqs, float duration, float vol)
        {
            int sampleRate = 22050;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float env = Mathf.Sin(Mathf.PI * (float)i / samples);
                float sum = 0f;
                foreach (var f in freqs) sum += Mathf.Sin(2 * Mathf.PI * f * t);
                data[i] = (sum / freqs.Length) * vol * env;
            }
            var clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        static AudioClip Noise(string name, float duration, float vol)
        {
            int sampleRate = 22050;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            var data = new float[samples];
            float prev = 0f;
            for (int i = 0; i < samples; i++)
            {
                float white = Random.Range(-1f, 1f);
                prev = (prev + 0.02f * white) / 1.02f;
                data[i] = prev * vol;
            }
            var clip = AudioClip.Create(name, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
