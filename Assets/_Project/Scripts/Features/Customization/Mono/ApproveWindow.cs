using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Features.Customization.Mono
{
  public class ApproveWindow : MonoBehaviour
  {
    [SerializeField] private Button _yesButton;
    [SerializeField] private Button _noButton;

    private Action _onApprove;

    private void Start() => 
      Initialize();

    public void Activate(Action onApprove)
    {
      _onApprove = onApprove;
    
      Show();
    }

    private void ClickYes()
    {
      _onApprove?.Invoke();
    
      Hide();
    }

    private void ClickNo() => 
      Hide();

    private void Show() => 
      gameObject.SetActive(true);

    private void Hide() => 
      gameObject.SetActive(false);

    private void Initialize()
    {
      _yesButton.onClick.AddListener(ClickYes);
      _noButton.onClick.AddListener(ClickNo);
    
      Hide();
    }
  }
}