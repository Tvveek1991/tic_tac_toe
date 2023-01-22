using System.Collections.Generic;

namespace _Project.Scripts.Services.InputResolver
{
  public class InputResolver : IInputResolver
  {
    private readonly List<INonUIInput> _inputModules = new List<INonUIInput>();

    public bool IsLocked { get; set; }

    public void Register(INonUIInput inputUnit)
    {
      inputUnit.InputResolver = this;
      
      if(!_inputModules.Contains(inputUnit))
        _inputModules.Add(inputUnit);
    }

    public void Enable() => 
      IsLocked = false;

    public void Disable() => 
      IsLocked = true;
  }
}