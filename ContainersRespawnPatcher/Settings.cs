using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainersRespawnPatcher
{
    public class Containers
    {
        [SettingName("Container EditorIDs")]
        [Tooltip("All the types of containers that are safe to make 'no respawn' copies of")]
        public List<string> ContainerEditorIDs = new()
        {
            "BarrelFish01",
            "BarrelFood01",
            "BarrelFood01_Snow",
            "BarrelIngredientCommon01",
            "BarrelIngredientCommon01_Snow",
            "BarrelIngredientUncommon01",
            "BarrelIngredientUncommon01_Snow",
            "BarrelMeat01",
            "BeeHive",
            "BeeHiveVacant",
            "BlackBriarMeadBarrel01",
            "CommonCoffin01",
            "CommonWardrobe01",
            "CompanionsKodlakNightTable01",
            "Cupboard01",
            "DesecratedImperial",
            "DesecratedStormCloak",
            "Dresser01",
            "DweDresser01",
            "EndTable01",
            "HonningbrewMeadBarrel01",
            "MammothContainer01",
            "MarkarthBurialUrn",
            "MarkarthCoffin01container",
            "MarkarthCoffin02container",
            "MeadBarrel02",
            "MiscSack02Large",
            "MiscSack02LargeFlat",
            "MiscSack02Small",
            "MiscSack02SmallFlat",
            "MiscSackLarge",
            "MiscSackLargeFlat01",
            "MiscSackLargeFlat02",
            "MiscSackLargeFlat03",
            "MiscSackSmall",
            "NobleChest01",
            "NobleChestDrawers01",
            "NobleChestDrawers02",
            "NobleChestDrawers02NoName",
            "NobleCupboard01",
            "NobleCupboard02",
            "NobleNightTable01",
            "NobleWardrobe01",
            "OrcDresser01",
            "OrcEndTable01",
            "PersonalChestSmall",
            "PlayerBookShelfContainer",
            "PlayerHouseChest",
            "PlayerPotionRackContainer",
            "PlayerWerewolfStorage",
            "RTCoffin01",
            "SafewithLock",
            "SBurialUrn01",
            "SCoffin01",
            "SCoffinPoor01",
            "SkyHavenArmoryChest",
            "SovBarrel01",
            "SpitPotClosed01",
            "SpitPotClosed01AlchemyCommon",
            "SpitPotClosed02",
            "SpitPotClosedLoose01",
            "SpitPotClosedLoose01AlchemyCommon",
            "StrongBox",
            "UnownedChest",
            "UpperCupboard01",
            "UpperDresser01",
            "UpperEndTable01",
            "UpperEndTable02",
            "UpperWardrobe01",
            "VendorMiscChestSmall",
            "WE19BanditLootChest",
            "WHcoffin01",
            "WinhelmBurialUrn",
            "WinterholdBookCase01",
            "wispCorpseContainer",
            "WRBurialUrn01",
            "WRCoffin01",
            "WRinteractiveBookshelfContainer"
        };
    }

    public class Cells
    {
        [SettingName("EditorIDs of Cells without respawn")]
        [Tooltip("All the cells that should not have respawning containers (player homes, ...)")]
        public List<string> CellNoRespawnEditorIDs = new ()
        {
            "WhiterunJorrvaskrBasement",
            "WhiterunJorrvaskr",
            "WinterholdCollegeHallofAttainment",
            "WinterholdCollegeArchMageQuarters",
            "RiftenThievesGuildHeadquarters",
            "SolitudeProudspireManor",
            "WhiterunBreezehome",
            "WindhelmHjerim",
            "RiftenHoneyside",
            "MarkarthVlindrelHall",
            "DLC1DawnguardHQ01",
            "DLC1VampireCastleGuildhall",
            "BYOHHouse1Falkreath",
            "BYOHHouse1FalkreathBasement",
            "BYOHHouse2Hjaalmarch",
            "BYOHHouse2HjaalmarchBasement",
            "BYOHHouse3Pale",
            "BYOHHouse3PaleBasement",
            "DLC2RRSeverinHouse"
        };
}

    public class Settings
    {
        public Containers SafeContainersSettings { get; set; } = new();

        public Cells CellsNotRespawningSettings { get; set; } = new();

        [SettingName("Debug")]
        [Tooltip("Activate all the debug messages")]
        public bool debug = false;
    }
}