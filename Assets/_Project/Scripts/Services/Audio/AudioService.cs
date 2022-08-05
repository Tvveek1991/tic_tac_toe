using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Services._Interfaces;
using UnityEngine;

namespace _Project.Scripts.Services.Audio
{
  public static class AudioMapping
  {
    private static readonly Dictionary<AudioId, AudioAsset> IdToAssetMapping =
      new Dictionary<AudioId, AudioAsset>()
      {
        {AudioId.SoundButton, new SoundButton()}
      };

    public static AudioAsset GetAudioAsset(AudioId audioId)
    {
      return IdToAssetMapping[audioId];
    }
  }

  public class AudioService : IAudioService
  {
    private readonly IAssetProvider _assetProvider;

    private static readonly Dictionary<AudioAsset, AudioSource> AudioSources =
      new Dictionary<AudioAsset, AudioSource>();

    private bool _isMusicDisabled;
    private bool _isSoundDisabled;

    public AudioService(IAssetProvider assetProvider)
    {
      _assetProvider = assetProvider;
    }

    public void MuteMusic(bool isMusicDisabled)
    {
      _isMusicDisabled = isMusicDisabled;

      bool Predicate(KeyValuePair<AudioAsset, AudioSource> kv) => kv.Key.AssetType == AudioType.Music;

      Dictionary<AudioAsset, AudioSource> musicSources = AudioSources
        .Where(Predicate)
        .ToDictionary(
          kv => kv.Key,
          kv => kv.Value);

      foreach (AudioSource audioSource in musicSources.Values) 
        audioSource.mute = _isMusicDisabled;
    }

    public void MuteSounds(bool isSoundDisabled)
    {
      _isSoundDisabled = isSoundDisabled;

      bool Predicate(KeyValuePair<AudioAsset, AudioSource> kv) => kv.Key.AssetType == AudioType.Sound;

      Dictionary<AudioAsset, AudioSource> soundSources = AudioSources
        .Where(Predicate)
        .ToDictionary(
          kv => kv.Key,
          kv => kv.Value);

      foreach (var audioSource in soundSources.Values)
      {
        audioSource.mute = _isSoundDisabled;
      }
    }

    public void Play(AudioId audioId)
    {
      var audioAsset = AudioMapping.GetAudioAsset(audioId);
      var audioSource = AudioSources.ContainsKey(audioAsset)
        ? AudioSources[audioAsset]
        : CreateAudioSource(audioAsset);

      audioSource.Play();
    }

    public bool MusicIsMute() =>
      _isMusicDisabled;

    public bool SoundsIsMute() =>
      _isSoundDisabled;

    public void Clean() => 
      AudioSources.Clear();

    private AudioSource CreateAudioSource(AudioAsset audioAsset)
    {
      var audioSource = Instantiate(audioAsset);
      AudioSources.Add(audioAsset, audioSource);

      audioSource.mute = audioAsset.AssetType switch
      {
        AudioType.Sound => _isSoundDisabled,
        AudioType.Music => _isMusicDisabled,
      };

      return audioSource;
    }

    private AudioSource Instantiate(AudioAsset audioAsset)
    {
      var original = _assetProvider.GetResource<AudioSource>(audioAsset.AssetPath);
      var instance = Object.Instantiate(original);
      return instance;
    }
  }
}