using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerPrefValues
{

    public const string SFX = "SFX";
    public const string Music = "Music";
    public const string StartupAudio = "StartupAudio";
    public const string BonusCoin = "BonusCoinScore";
    public const string Character = "Character";

    public const string Coin = "Coin";

    public const string Moon = "Moon";
    public const string Rocket = "Rocket";

    public static Config.Types.Collectible.CollectibleLevel GetCollectiblLevel(Config.Types.Collectible.CollectibleType collectibleType)
    {
        return (Config.Types.Collectible.CollectibleLevel)PlayerPrefs.GetInt(collectibleType.ToString() + "-level", (int)Config.Types.Collectible.CollectibleLevel.Level1);
    }

    public static void SetCollectibleLevel(Config.Types.Collectible.CollectibleType collectibleType, Config.Types.Collectible.CollectibleLevel level)
    {
        PlayerPrefs.SetInt(collectibleType.ToString() + "-level", (int)level);
    }


}
