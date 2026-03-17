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
    private Button spnBtn;

    [SerializeField]
    private Button frenBtn;

    private void Start()
    {
        englishBtn.onClick.AddListener(English);
        portBtn.onClick.AddListener(Portuguese);
        spnBtn.onClick.AddListener(Spanish);
        frenBtn.onClick.AddListener(French);
    }

    public void English()
    {
        GameManager.instance.LanguageChange("English");
    }

    public void Portuguese()
    {
        GameManager.instance.LanguageChange("Portuguese");
    }

    public void Spanish()
    {
        GameManager.instance.LanguageChange("Spanish");
    }

    public void French()
    {
        GameManager.instance.LanguageChange("French");
    }
}
