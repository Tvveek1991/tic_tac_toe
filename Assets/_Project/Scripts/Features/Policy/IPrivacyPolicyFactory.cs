namespace _Project.Scripts.Features.Policy
{
  public interface IPrivacyPolicyFactory
  {
    PrivacyPolicy SpawnPrivacyPolicy();
    void Clear();
  }
}