using _Project.Scripts.Services._Interfaces;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Services.Money.Mono
{
  public class MoneyCounter : MonoBehaviour
  {
    public TextMeshProUGUI MoneyCountInfo;

    private IMoneyService _moneyService;

    public void Construct(IMoneyService moneyService)
    {
      _moneyService = moneyService;
      _moneyService.OnCountChange += SetText;

      SetText(_moneyService.MoneyCount);
    }

    private void SetText(uint money)
    {
      MoneyCountInfo.text = "Money: " + money;
    }
  }
}