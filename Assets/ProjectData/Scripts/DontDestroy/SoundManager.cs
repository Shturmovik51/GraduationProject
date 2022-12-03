using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _menuBG;
    [SerializeField] private AudioSource _gameMainTheme;
    [SerializeField] private AudioSource _gameNearVictory;
    [SerializeField] private AudioSource _gameNearLose;
    [SerializeField] private AudioSource _click;
    [SerializeField] private AudioSource _onPointerEnter;

    private List<AnimatedButton> _menuButtons;
    private List<AnimatedButton> _gameButtons;
  
    public void PlayMenuSound()
    {
        _gameMainTheme.Stop();
        _menuBG.loop = true;
        _menuBG.Play();
    }

    public void PlayGameMainTheme()
    {
        _menuBG.Stop();
        _gameMainTheme.loop = true;
        _gameMainTheme.Play();
    }

    public void SubscribeMenuButtons()
    {
        _menuButtons = FindObjectsOfType<AnimatedButton>().ToList();
        foreach (var button in _menuButtons)
        {
            button.onClick.AddListener(() => _click.Play());
            button.OnPointerEnterEvent += () => _onPointerEnter.Play();
        }
        Debug.Log("Fiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiind menu buttons");
    }

    public void SubscribeGameButtons()
    {
        _gameButtons = FindObjectsOfType<AnimatedButton>().ToList();
        foreach (var button in _gameButtons)
        {
            button.onClick.AddListener(() => _click.Play());
            button.OnPointerEnterEvent += () => _onPointerEnter.Play();
        }
        Debug.Log("Fiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiind game buttons");
    }

    public void AddAudioSources()
    {

    }
}
