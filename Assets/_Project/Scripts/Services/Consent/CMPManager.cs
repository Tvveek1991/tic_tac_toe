using System;
using GoogleMobileAds.Ump.Api;
using UnityEngine; 
/*using GoogleMobileAds.Api.Mediation.Vungle;
using GoogleMobileAds.Mediation.AppLovin.Api;
using GoogleMobileAds.Mediation.IronSource.Api;
using GoogleMobileAds.Mediation.Pangle.Api;
using GoogleMobileAds.Mediation.UnityAds.Api;*/

namespace _Project.Scripts.Services.Consent
{
  public class CmpManager : ICmpManager
  {
    private const bool IsTest = false;

    private ConsentForm _consentForm;
    private Action _onEnd;

    public void ResetConsent() =>
      ConsentInformation.Reset();

    public void Request(Action onEnd)
    {
      _onEnd = onEnd;
      ConsentRequestParameters request;

      if (IsTest)
      {
        ConsentInformation.Reset();

        var debugSettings = new ConsentDebugSettings
        {
          DebugGeography = DebugGeography.EEA,
          TestDeviceHashedIds = {"912BC2D4EE14535578E59350261CAA5D"}
        };

        request = new ConsentRequestParameters
        {
          TagForUnderAgeOfConsent = false,
          ConsentDebugSettings = debugSettings,
        };
      }
      else
      {
        request = new ConsentRequestParameters
        {
          TagForUnderAgeOfConsent = false,
        };
      }

      ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    public bool IsFormAvailable() =>
      ConsentInformation.IsConsentFormAvailable();

    private void SetConsentForAds(bool hasConsent)
    {
      /*AppLovin.SetHasUserConsent(hasConsent);
      AppLovin.SetDoNotSell(!hasConsent);

      IronSource.SetConsent(hasConsent);
      IronSource.SetMetaData("do_not_sell", (!hasConsent).ToString());

      UnityAds.SetConsentMetaData("gdpr.consent", hasConsent);
      UnityAds.SetConsentMetaData("privacy.consent", hasConsent);

      Vungle.UpdateConsentStatus(hasConsent ? VungleConsentStatus.OPTED_IN : VungleConsentStatus.OPTED_OUT, "1.0.0");
      
      Pangle.SetGDPRConsent(hasConsent ? 1 : 0);
      Pangle.SetDoNotSell(hasConsent ? 0 : 1);*/
    }

    private void LoadConsentForm() =>
      ConsentForm.Load(OnLoadConsentForm);

    private void OnConsentInfoUpdated(FormError error)
    {
      if (HandleError(error)) return;

      if (ConsentInformation.IsConsentFormAvailable())
        LoadConsentForm();
      else
        _onEnd?.Invoke();
    }

    private void OnLoadConsentForm(ConsentForm consentForm, FormError error)
    {
      if (HandleError(error)) return;

      _consentForm = consentForm;

      if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
      {
        SetConsentForAds(false);
        _consentForm.Show(OnShowForm);

        Debug.Log("[Consent] Dont Have Consent");
      }
      else
      {
        SetConsentForAds(true);

        Debug.Log("[Consent] Has Consent");

        _onEnd?.Invoke();
      }
    }

    private void OnShowForm(FormError error)
    {
      if (HandleError(error)) return;

      LoadConsentForm();
    }

    private bool HandleError(FormError error)
    {
      if (error != null)
      {
        _onEnd?.Invoke();
        Debug.LogError(error);
        return true;
      }

      return false;
    }
  }
}