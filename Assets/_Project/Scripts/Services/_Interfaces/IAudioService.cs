namespace _Project.Scripts.Services._Interfaces
{
  public interface IAudioService
  {
    bool MusicIsMute();
    bool SoundsIsMute();
    void MuteMusic(bool isMusicDisabled);
    void MuteSounds(bool isSoundDisabled);
    void Play(Audio.AudioId audioId);
    void Clean();
  }
}