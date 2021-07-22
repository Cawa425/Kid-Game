using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class GameSetType : MonoBehaviour
{
    public abstract string Name { get; }
    public abstract IEnumerable<Sprite> Images { get;  }
}

public static class GameTypesFactory
{
    private static Dictionary<string, Type> _gameSetByName;
    private static bool IsInitialized => _gameSetByName != null;
    
    private static void InitializeFactory()
    {
        if (IsInitialized) return;
        var gameSetByType = Assembly
            .GetAssembly(typeof(GameSetType))
            .GetTypes()
            .Where(x =>
                x.IsClass &&
                !x.IsAbstract &&
                x.IsSubclassOf(typeof(GameSetType)));

        _gameSetByName = new Dictionary<string, Type>();
        foreach (var type in gameSetByType)
        {
            Activator.CreateInstance(type);
            _gameSetByName.Add(type.Name, type);
        }
    }

    public static GameSetType GetGameObject(string gmType)
    {
        InitializeFactory();
        if (!_gameSetByName.ContainsKey(gmType)) return null;
        var type = _gameSetByName[gmType];
        var gameSet = Activator.CreateInstance(type) as GameSetType;
        return gameSet;
    }

    public static IEnumerable<string> GetAllGameSetsName()
    {
        InitializeFactory();
        return _gameSetByName.Keys;
    }
}

public class Digitalises : GameSetType
{
    public override string Name => "Digitalis";
    public override IEnumerable<Sprite> Images => FindObjectOfType<Settings>().digitalImages;
}

public class Numbers : GameSetType
{
    public override string Name => "Numbers";
    public override IEnumerable<Sprite> Images => FindObjectOfType<Settings>().numberImages;

}