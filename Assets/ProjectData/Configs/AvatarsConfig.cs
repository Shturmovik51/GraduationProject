using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AvatarsConfig), menuName = "GameConfigs/AvatarsConfig")]
public class AvatarsConfig : ScriptableObject
{
    [SerializeField] private Sprite _playerAvatar1;
    [SerializeField] private Sprite _playerAvatar2;
    [SerializeField] private Sprite _playerAvatar3;
    [SerializeField] private Sprite _playerAvatar4;
    [SerializeField] private Sprite _playerAvatar5;
    [SerializeField] private Sprite _playerAvatar6;
    [SerializeField] private Sprite _playerAvatar7;
    [SerializeField] private Sprite _playerAvatar8;

    public Sprite GetAvatarByIndex(int index)
    {
        switch (index)
        {
            case 0: return _playerAvatar1;
            case 1: return _playerAvatar2;
            case 2: return _playerAvatar3;
            case 3: return _playerAvatar4;
            case 4: return _playerAvatar5;
            case 5: return _playerAvatar6;
            case 6: return _playerAvatar7;
            case 7: return _playerAvatar8;

            default: return null;                
        }
    }

    //public int GetIndexByAvatar(Sprite sprite)
    //{
    //    var f = 
    //}
}
