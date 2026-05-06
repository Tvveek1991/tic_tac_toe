using UnityEngine;
using AudioType = _Project.Scripts.Services.Audio.Enum.AudioType;

namespace _Project.Scripts.Services.Audio.Data
{
  [RequireComponent(typeof(AudioSource))]
  public class AudioData : MonoBehaviour
  {
    public AudioType Type;
    public AudioSource Source;

    private void OnValidate() =>
      Source = GetComponent<AudioSource>();

    private void Awake() =>
      Source ??= GetComponent<AudioSource>();
  }
}