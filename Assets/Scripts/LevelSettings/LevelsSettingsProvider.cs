using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(menuName = "ScriptableObjects/LevelsSettingsProvider", fileName = "LevelsSettingsProvider", order = 51)]
public class LevelsSettingsProvider : ScriptableObject
{
    [SerializeField] 
    private LevelSettings[] _levelSettings;
    
    private Dictionary<int, LevelSettings> _levelSettingsByNumber;

    public void Initialize()
    {
        _levelSettingsByNumber = new();

        for (var i = 0; i < _levelSettings.Length; i++)
        {
            var levelSetting = _levelSettings[i];
            _levelSettingsByNumber[i + 1] = levelSetting;
        }
    }

    public int GetItemTypesCount(int levelNumber)
    {
        var levelSettings = _levelSettingsByNumber[levelNumber];
        Assert.IsNotNull(levelSettings, $"LevelSettings is null, please check level number {levelNumber} in " +
                                        "LevelsSettingsProvider");

        return levelSettings.ItemTypesNumber;
    }
}