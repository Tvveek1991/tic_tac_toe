using _Project.Scripts.Services._Interfaces;

namespace _Project.Scripts.Services.InputResolver
{
  public interface INonUIInput
  { 
    IInputResolver InputResolver { get; set; }
  }
}