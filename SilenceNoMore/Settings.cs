using FFXIVClientStructs.FFXIV.Client.Game;
using SilenceNoMore.Hooking.Enums;

namespace SilenceNoMore;

internal static unsafe class Settings
{
    public static readonly TerritoryIntendedUseEnum[] AllowedZones =
    [
        TerritoryIntendedUseEnum.Dungeon,
        TerritoryIntendedUseEnum.Variant_Dungeon,
        TerritoryIntendedUseEnum.Quest_Area,
        TerritoryIntendedUseEnum.Alliance_Raid,
        TerritoryIntendedUseEnum.Quest_Battle,
        TerritoryIntendedUseEnum.Trial,
        TerritoryIntendedUseEnum.Quest_Area_2,
        TerritoryIntendedUseEnum.Residential_Area,
        TerritoryIntendedUseEnum.Housing_Instances,
        TerritoryIntendedUseEnum.Quest_Area_3,
        TerritoryIntendedUseEnum.Raid,
        TerritoryIntendedUseEnum.Raid_2,
        TerritoryIntendedUseEnum.Chocobo_Square,
        TerritoryIntendedUseEnum.Restoration_Event,
        TerritoryIntendedUseEnum.Sanctum,
        TerritoryIntendedUseEnum.Gold_Saucer,
        TerritoryIntendedUseEnum.Lord_of_Verminion,
        TerritoryIntendedUseEnum.Diadem,
        TerritoryIntendedUseEnum.Hall_of_the_Novice,
        TerritoryIntendedUseEnum.Quest_Battle_2,
        TerritoryIntendedUseEnum.Barracks,
        TerritoryIntendedUseEnum.Deep_Dungeon,
        TerritoryIntendedUseEnum.Seasonal_Event,
        TerritoryIntendedUseEnum.Treasure_Map_Duty,
        TerritoryIntendedUseEnum.Seasonal_Event_Duty,
        TerritoryIntendedUseEnum.Battlehall,
        TerritoryIntendedUseEnum.Diadem_2,
        TerritoryIntendedUseEnum.Eureka,
        TerritoryIntendedUseEnum.Seasonal_Event_2,
        TerritoryIntendedUseEnum.Leap_of_Faith,
        TerritoryIntendedUseEnum.Masked_Carnivale,
        TerritoryIntendedUseEnum.Ocean_Fishing,
        TerritoryIntendedUseEnum.Diadem_3,
        TerritoryIntendedUseEnum.Bozja,
        TerritoryIntendedUseEnum.Island_Sanctuary,
        TerritoryIntendedUseEnum.Battlehall_2,
        TerritoryIntendedUseEnum.Battlehall_3,
        TerritoryIntendedUseEnum.Large_Scale_Raid,
        TerritoryIntendedUseEnum.Large_Scale_Savage_Raid,
        TerritoryIntendedUseEnum.Quest_Area_4,
        TerritoryIntendedUseEnum.Tribal_Instance,
        TerritoryIntendedUseEnum.Criterion_Duty,
        TerritoryIntendedUseEnum.Criterion_Savage_Duty,
        TerritoryIntendedUseEnum.Blunderville,
        TerritoryIntendedUseEnum.Occult_Crescent,             
    ];

    public static bool CurrentTerritoryIsAllowed()
    {
        if (GameMain.Instance() == null)
        {
            return false;
        }

        byte currentId = GameMain.Instance()->CurrentTerritoryIntendedUseId;

        int arrayLength = AllowedZones.Length;

        for (int i = 0; i < arrayLength; i++)
        {
            TerritoryIntendedUseEnum enumValue = AllowedZones[i];

            if ((int)enumValue != currentId)
            {
                continue;
            }

            return true;
        }

        return false;
    }
}
