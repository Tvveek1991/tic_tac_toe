using _Project.Scripts.Services.Audio.Enum;

namespace _Project.Scripts.Services._Interfaces
{
  public interface IAudioService
  {
    bool MusicIsMute();
    bool SoundsIsMute();
    void MuteMusic(bool isMusicDisabled);
    void MuteSounds(bool isSoundDisabled);
    void Play(AudioId audioId);
    void Clean();
  }
}