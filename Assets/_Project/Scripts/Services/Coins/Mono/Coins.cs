using TMPro;
using UnityEngine;

namespace _Project.Scripts.Services.Coins.Mono
{
  public class Coins : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI Counter;

    public void SetCounter(int coins) => 
      Counter.text = coins.ToString();
  }
}