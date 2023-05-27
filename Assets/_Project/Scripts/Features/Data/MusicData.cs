using _Project.Scripts.Services.Audio.Enum;
using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  [CreateAssetMenu(fileName = "Music Data", menuName = "Data/Playable/Music Data", order = 0)]
  public class MusicData : GameCustomData
  {
    [SerializeField] private AudioId _clipId;

    public AudioId ClipId => _clipId;
  }
}