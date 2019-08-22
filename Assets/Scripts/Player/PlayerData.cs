using System;
using UnityEngine;

public interface IPlayerData
{
    void Clear();
}

public abstract class PlayerData : IPlayerData
{
    public abstract void Clear();
    
    protected static void Save()
    {
        PlayerPrefs.Save();
    }

    protected static void Delete(string withKey)
    {
        PlayerPrefs.DeleteKey(withKey);
    }
    
    protected static T GetEnum<T>(string withKey, T andDefaultValue) where T : struct, IConvertible
    {
        return (T)(object)PlayerPrefs.GetInt(withKey,Convert.ToInt32(andDefaultValue));
    }

    private static T GetEnum<T>(string withKey) where T : struct, IConvertible
    {
        return (T)(object)GetInt(withKey, 0);
    }

    protected static void SetEnum<T>(string withKey, T andValue) where T : struct, IConvertible
    {
        SetInt(withKey, Convert.ToInt32(andValue));
    }

    protected static bool GetBool(string withKey, bool andDefaultValue = false)
    {
        return GetInt(withKey, andDefaultValue ? 1 : 0) == 1;
    }

    protected static void SetBool(string withKey, bool andValue)
    {
        SetInt(withKey, andValue ? 1 : 0);
    }
    
    protected static int GetInt(string withKey, int andDefaultValue)
    {
        return PlayerPrefs.GetInt(withKey, andDefaultValue);
    }

    protected static void SetInt(string withKey, int andValue)
    {
        PlayerPrefs.SetInt(withKey, andValue);
    }
}