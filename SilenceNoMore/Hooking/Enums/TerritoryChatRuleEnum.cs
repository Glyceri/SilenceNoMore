namespace SilenceNoMore.Hooking.Enums;

internal enum TerritoryChatRuleEnum : byte
{
    CityErea                    = 0,  // Public: NoRestriction        Shout: NoRestriction        DutyTell: Restricted        PublicTell: NoRestriction        Party: NoRestriction       Global: NoRestriction       Pvp: NoRestriction
    BattleHall                  = 1,  // Public: Restricted           Shout: Restricted           DutyTell: Restricted        PublicTell: Restricted           Party: Restricted          Global: Restricted          Pvp: Restricted
    CrystallineConflict         = 2,  // Public: Restricted           Shout: Restricted           DutyTell: Restricted        PublicTell: Restricted           Party: Restricted          Global: Speical             Pvp: Special
    CrystallineConflictPremade  = 3,  // Public: Restricted           Shout: NoRestriction        DutyTell: Restricted        PublicTell: Restricted           Party: NoRestriction       Global: NoRestriction       Pvp: NoRestriction
    Gaol                        = 4,  // Public: NoRestriction        Shout: Restricted           DutyTell: Restricted        PublicTell: Special              Party: Restricted          Global: Restricted          Pvp: Restricted
    NormalContent               = 5,  // Public: NoRestriction        Shout: NoRestriction        DutyTell: Restricted        PublicTell: Restricted           Party: NoRestriction       Global: NoRestriction       Pvp: NoRestriction
    Frontline                   = 6,  // Public: NoRestriction        Shout: NoRestriction        DutyTell: Restricted        PublicTell: Restricted           Party: NoRestriction       Global: NoRestriction       Pvp: NoRestriction
    RivalWings                  = 7,  // Public: NoRestriction        Shout: NoRestriction        DutyTell: Restricted        PublicTell: Restricted           Party: NoRestriction       Global: NoRestriction       Pvp: NoRestriction
    Eureka                      = 8,  // Public: NoRestriction        Shout: NoRestriction        DutyTell: NoRestriction     PublicTell: Restricted           Party: NoRestriction       Global: NoRestriction       Pvp: NoRestriction
    CrystalTowerTraining        = 9,  // Public: Restricted           Shout: Restricted           DutyTell: Restricted        PublicTell: Restricted           Party: NoRestriction       Global: Special             Pvp: NoRestriction
    Blunderville                = 10  // Public: Restricted           Shout: Restricted           DutyTell: Restricted        PublicTell: Restricted           Party: NoRestriction       Global: NoRestriction       Pvp: NoRestriction
}
