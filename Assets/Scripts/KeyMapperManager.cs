using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyMapperManager : MonoBehaviour
{
    public static KeyMapperManager Instance { get; private set; }

    private Dictionary<string, KeyCode> keyMappings;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadKeyMappings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadKeyMappings()
    {
        keyMappings = new Dictionary<string, KeyCode>
        {
            { "MoveForward", (KeyCode)PlayerPrefs.GetInt("MoveForward", (int)KeyCode.W) },
            { "MoveBackward", (KeyCode)PlayerPrefs.GetInt("MoveBackward", (int)KeyCode.S) },
            { "MoveLeft", (KeyCode)PlayerPrefs.GetInt("MoveLeft", (int)KeyCode.A) },
            { "MoveRight", (KeyCode)PlayerPrefs.GetInt("MoveRight", (int)KeyCode.D) },
            { "Jump", (KeyCode)PlayerPrefs.GetInt("Jump", (int)KeyCode.Space) },
            {"Inventory", (KeyCode)PlayerPrefs.GetInt("Inventory", (int)KeyCode.I) },
            { "UseItem", (KeyCode)PlayerPrefs.GetInt("UseItem", (int)KeyCode.E) },
            { "PauseGame", (KeyCode)PlayerPrefs.GetInt("PauseGame", (int)KeyCode.Tab) }
        };
    }

    public void SetKeyMapping(string action, KeyCode key)
    {
        keyMappings[action] = key;
        PlayerPrefs.SetInt(action, (int)key);
    }

    public KeyCode GetKeyMapping(string action)
    {
        return keyMappings.ContainsKey(action) ? keyMappings[action] : KeyCode.None;
    }
}
