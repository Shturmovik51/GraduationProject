using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
    [SerializeField] private AudioSource _endScreenSound;
    [SerializeField] private AudioSource _winTheme;
    [SerializeField] private AudioSource _loseTheme;

    private List<AudioSource> soundsBG = new List<AudioSource>();    

    private List<AnimatedButton> _menuButtons;
    private List<AnimatedButton> _gameButtons;

    private List<AudioSource> _shipsAudioSourses;
    private List<AudioSource> _cellsAudioSourses;

    private void Awake()
    {
        soundsBG = new List<AudioSource>()
        {
            _menuBG,
            _gameMainTheme,
            _gameNearVictory,
            _gameNearLose,   
            _winTheme,
            _loseTheme,
        };
    }

    public void PlayMenuSound()
    {
        ResetMusicBG();
        _menuBG.loop = true;
        _menuBG.Play();
    }

    public void PlayGameMainTheme()
    {
        ResetMusicBG();
        _gameMainTheme.loop = true;
        _gameMainTheme.Play();
    }

    public void PlayGameNearVictoryTheme()
    {
        ResetMusicBG();
        _gameNearVictory.loop = true;
        _gameNearVictory.Play();
    }

    public void PlayGameNearLoseTheme()
    {
        ResetMusicBG();
        _gameNearLose.loop = true;
        _gameNearLose.Play();
    }

    public void PlayVictoryTheme()
    {
        ResetMusicBG();
        _winTheme.loop = true;
        _winTheme.Play();
    }

    public void PlayLoseTheme()
    {
        ResetMusicBG();
        _loseTheme.loop = true;
        _loseTheme.Play();
    }

    public void PlayEndScreenSound()
    {
        ResetMusicBG();
        _endScreenSound.loop = true;
        _endScreenSound.Play();
    }

    private void ResetMusicBG()
    {
        foreach (var sours in soundsBG)
        {
            sours.Stop();
        }
    }

    public void SubscribeMenuButtons()
    {
        if (_menuButtons != null)
        {
            _menuButtons.Clear();
        }

        _menuButtons = FindObjectsOfType<AnimatedButton>().ToList();
        foreach (var button in _menuButtons)
        {
            button.onClick.AddListener(() => _click.Play());
            button.OnPointerEnterEvent += () => _onPointerEnter.Play();
        }
    }

    public void SubscribeGameButtons()
    {
        if(_gameButtons != null)
        {
            _gameButtons.Clear();
        }

        _gameButtons = FindObjectsOfType<AnimatedButton>().ToList();
        foreach (var button in _gameButtons)
        {
            button.onClick.AddListener(() => _click.Play());
            button.OnPointerEnterEvent += () => _onPointerEnter.Play();
        }
    }

    public void SetMusicValue(float value)
    {
        _menuBG.volume = value;
        _gameMainTheme.volume = value;
        _gameNearVictory.volume = value;
        _gameNearLose.volume = value;
        _winTheme.volume = value;
        _loseTheme.volume = value;
    }

    public void SetEffectsValue(float value)
    {
        _click.volume = value;
        _onPointerEnter.volume = value;
        _endScreenSound.volume = value;

        if(_shipsAudioSourses != null)
        {
            foreach (var shipSource in _shipsAudioSourses)
            {
                shipSource.volume = value;
            }
        }

        if (_cellsAudioSourses != null)
        {
            foreach (var cellSource in _cellsAudioSourses)
            {
                cellSource.volume = value;
            }
        }
    }

    public void AddShipsAudioSources()
    {
        if(_shipsAudioSourses != null)
        {
            _shipsAudioSourses.Clear();            
        }
        else
        {
            _shipsAudioSourses = new List<AudioSource>();
        }

        var ships = FindObjectsOfType<Ship>().ToList();

        foreach(var ship in ships)
        {
            _shipsAudioSourses.Add(ship.GetComponentInChildren<AudioSource>());
        }
    }

    public void AddCellsAudioSources()
    {
        if (_shipsAudioSourses != null)
        {
            _shipsAudioSourses.Clear();
        }
        else
        {
            _shipsAudioSourses = new List<AudioSource>();
        }

        var cells = FindObjectsOfType<FieldCell>().ToList();

        foreach (var cell in cells)
        {
            _shipsAudioSourses.Add(cell.GetComponentInChildren<AudioSource>());
        }
    }
}
