using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Services._Interfaces;
using _Project.Scripts.Services.Audio.Data;
using _Project.Scripts.Services.Audio.Enum;
using UnityEngine;
using AudioType = _Project.Scripts.Services.Audio.Enum.AudioType;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Services.Audio
{
  public class AudioService : IAudioService
  {
    private readonly IAssetProvider _assetProvider;

    private static readonly Dictionary<AudioAsset, AudioData> AudioSources =
      new Dictionary<AudioAsset, AudioData>();

    private bool _isMusicDisabled;
    private bool _isSoundDisabled;

    private readonly List<AudioAsset> _audio;

    public AudioService(IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;

      _audio = new List<AudioAsset>();
      for (int i = 0; i < System.Enum.GetNames(typeof(AudioId)).Length; i++) 
        _audio.Add(new AudioAsset((AudioId)i));
    }

    public void MuteMusic(bool isMusicDisabled)
    {
      _isMusicDisabled = isMusicDisabled;

      bool Predicate(KeyValuePair<AudioAsset, AudioData> kv) => kv.Value.Type == AudioType.Music;

      Dictionary<AudioAsset, AudioData> musicSources = AudioSources
        .Where(Predicate)
        .ToDictionary(
          kv => kv.Key,
          kv => kv.Value);

      foreach (AudioData audioData in musicSources.Values)
        audioData.Source.mute = _isMusicDisabled;
    }

    public void MuteSounds(bool isSoundDisabled)
    {
      _isSoundDisabled = isSoundDisabled;

      bool Predicate(KeyValuePair<AudioAsset, AudioData> kv) => kv.Value.Type == AudioType.Sound;

      Dictionary<AudioAsset, AudioData> soundSources = AudioSources
        .Where(Predicate)
        .ToDictionary(
          kv => kv.Key,
          kv => kv.Value);

      foreach (AudioData audioData in soundSources.Values) 
        audioData.Source.mute = _isSoundDisabled;
    }

    public void Play(AudioId audioId)
    {
      if (MusicAlreadyIsPlaying()) 
        return;

      AudioAsset audioAsset = GetAudioAsset(audioId);
      AudioData audioData = AudioSources.ContainsKey(audioAsset)
        ? AudioSources[audioAsset]
        : CreateAudioSource(audioAsset);

      // Debug.Log($"Audio ID {audioId.ToString()}");

      if (audioData.Type == AudioType.Music) 
        StopOtherMusic();

      audioData.Source.Play();

      void StopOtherMusic()
      {
        foreach (AudioData audio in AudioSources.Values)
        {
          if (audio.Type == AudioType.Music)
            audio.Source.Stop();
        }
      }

      bool MusicAlreadyIsPlaying()
      {
        AudioAsset data = AudioSources.Keys.FirstOrDefault(x => x.AudioId == audioId); 
        return data != null && AudioSources[data].Source.isPlaying && AudioSources[data].Type == AudioType.Music;
      }
    }

    public void Stop(AudioId audioId)
    {
      foreach (AudioAsset audio in AudioSources.Keys)
      {
         if(audio.AudioId == audioId)
           AudioSources[audio].Source.Stop();
      }
    }

    public bool MusicIsMute() =>
      _isMusicDisabled;

    public bool SoundsIsMute() =>
      _isSoundDisabled;

    public void Clean()
    {
      Debug.Log("Clean");
      
      foreach (AudioData value in AudioSources.Values) 
        Object.DestroyImmediate(value.gameObject);
      
      AudioSources.Clear();
    }

    private AudioData CreateAudioSource(AudioAsset audioAsset)
    {
      AudioData audioData = Instantiate(audioAsset);
      AudioSources.Add(audioAsset, audioData);

      audioData.Source.mute = audioData.Type switch 
      {
        AudioType.Sound => _isSoundDisabled,
        AudioType.Music => _isMusicDisabled,
      };

      return audioData;
    }

    private AudioData Instantiate(AudioAsset audioAsset)
    {
      AudioData original = _assetProvider.GetResource<AudioData>(audioAsset.AssetPath);
      AudioData instance = Object.Instantiate(original);
      return instance;
    }

    private AudioAsset GetAudioAsset(AudioId audioId) => 
      _audio.FirstOrDefault(a => a.AudioId == audioId);
  }
}