using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager Instance { get; private set; }

    [SerializeField] private List<MinigameEntry> minigames;

    private Dictionary<MinigameType, MinigameBaseUI> lookup;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        lookup = new Dictionary<MinigameType, MinigameBaseUI>();

        foreach (var entry in minigames)
        {
            lookup[entry.type] = entry.minigame;
        }
    }

    public MinigameBaseUI Get(MinigameType type)
    {
        if (lookup.TryGetValue(type, out var game))
            return game;

        Debug.LogError($"Minigame {type} not found!");
        return null;
    }
}
