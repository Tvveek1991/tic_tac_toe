using UnityEngine;

namespace _Project.Scripts.Features.Data
{
  [CreateAssetMenu(fileName = "Tutorial Data", menuName = "Data/Tutorial/Tutorial Data", order = 0)]
  public class TutorialData : ScriptableObject
  {
    [SerializeField] private string _descriptionKey;

    public string DescriptionKey => _descriptionKey;
  }
}