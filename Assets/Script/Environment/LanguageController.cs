using System;
using UnityEngine;
using UnityEngine.UI;

public class LanguageController : MonoBehaviour
{
    [SerializeField]
    private Button englishBtn;

    [SerializeField]
    private Button portBtn;

    [SerializeField]
    private Button frenBtn;

    [SerializeField]
    private Button rsnBtn;

    private void Start()
    {
        englishBtn.onClick.AddListener(English);
        portBtn.onClick.AddListener(Portuguese);
        rsnBtn.onClick.AddListener(Russian);
        frenBtn.onClick.AddListener(French);
    }

    public void English()
    {
        ChangeLanguage("en");
    }

    public void Portuguese()
    {
        ChangeLanguage("pt");
    }

    public void Russian()
    {
        ChangeLanguage("rs");
    }

    public void French()
    {
        ChangeLanguage("fr");
    }

    public void ChangeLanguage(string lang)
    {
        LocalizationManager.Instance?.SetLanguage(lang);
        GameManager.instance.LanguageChange();
    }
}
