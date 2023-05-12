using System.Collections.Generic;
using UnityEngine;

namespace _Project.Scripts.Features.ShopScreen.Content
{
  public class StageDisplayContent : BaseDisplayContent
  {
    public override void Display(List<string> displayData) => 
      ChangeCanvasGroupsValue(displayData, 1);

    public override void Hide(List<string> displayData) => 
      ChangeCanvasGroupsValue(displayData, 0);

    private void ChangeCanvasGroupsValue(List<string> displayData, float value)
    {
      foreach (string data in displayData)
      {
        GameObject stagePart = GameObject.Find(data);
        
        if (!stagePart)
          continue;
        
        var canvasGroup = stagePart.GetComponent<CanvasGroup>();
        canvasGroup.alpha = value;
      }
    }
  }
}