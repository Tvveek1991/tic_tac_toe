using _Project.Scripts.Services.Audio.Enum;

namespace _Project.Scripts.Services.Audio.Data
{
  public class AudioAsset
  {
    public readonly AudioId AudioId;

    public string AssetPath => "Sounds/" +  $"Audio-{AudioId.ToString()}";

    public AudioAsset(AudioId audioId) =>
      AudioId = audioId;
  }
}