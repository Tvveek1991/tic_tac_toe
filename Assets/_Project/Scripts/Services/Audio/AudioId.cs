namespace _Project.Scripts.Services.Audio
{
  public enum AudioId
  {
    SoundButton
  }

  public enum AudioType
  {
    Music,
    Sound,
  }

  public abstract class AudioAsset
  {
    public abstract AudioType AssetType { get; }
    public abstract string AssetPath { get; }
  }

  public class SoundButton : AudioAsset
  {
    public override AudioType AssetType => AudioType.Sound;
    public override string AssetPath => "Sounds/Buttons";
  }
}