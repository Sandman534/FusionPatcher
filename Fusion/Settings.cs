using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using System.Collections.Immutable;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda;
using System.Security.Policy;
using Noggog;

namespace Fusion
{
    public class Settings
    {
        [SettingName("Use Bash Tags")]
        public bool BashTags = true;

        [SettingName("No Merge")]
        public List<ModKey> settingsNoMerge = new();

        [SettingName("Actors")]
        public List<ModKey> settingsActors = new();

        [SettingName("Cells")]
        public List<ModKey> settingsCells = new();

        [SettingName("Destuctibles")]
        public List<ModKey> settingsDestructibles = new();

        [SettingName("Enchantments")]
        public List<ModKey> settingsEnchantments = new();

        [SettingName("Graphics")]
        public List<ModKey> settingsGraphics = new();

        [SettingName("Inventory")]
        public List<ModKey> settingsInventory = new();

        [SettingName("Keywords")]
        public List<ModKey> settingsKeywords = new();

        [SettingName("Leveled Lists")]
        public List<ModKey> settingsLeveled = new();

        [SettingName("Names")]
        public List<ModKey> settingsNames = new();

        //[SettingName("Outfits")]
        //public List<ModKey> settingsOutfits = new List<ModKey>();

        //[SettingName("Race")]
        //public List<ModKey> settingsRace = new List<ModKey>();

        [SettingName("Relations")]
        public List<ModKey> settingsRelations = new();

        [SettingName("Scripts")]
        public List<ModKey> settingsScripts = new();

        [SettingName("Sounds")]
        public List<ModKey> settingsSounds = new();

        [SettingName("Stats")]
        public List<ModKey> settingsStats = new();

        [SettingName("Text")]
        public List<ModKey> settingsText = new();
    }

    public class SettingsUtility
    {
        public ActivatorSettings SettingsActivator { get; set; }
        public ArmorSettings SettingsArmor { get; set; }
        public BookSettings SettingsBooks { get; set; }
        public CellSettings SettingsCells { get; set; }
        public ContainerSettings SettingsContainers { get; set; }
        public FactionSettings SettingsFactions { get; set; }
        public IngestibleSettings SettingsIngestibles { get; set; }
        public LeveledItemSettings SettingsLeveledItems { get; set; }
        public LeveledSpellSettings SettingsLeveledSpells { get; set; }
        public LeveledNPCSettings SettingsLeveledNPCs { get; set; }
        public LocationSettings SettingsLocations { get; set; }
        public NPCSettings SettingsNPCs { get; set; }
        public PerkSettings SettingsPerks { get; set; }
        public QuestSettings SettingsQuests { get; set; }
        public WeaponSettings SettingsWeapons { get; set; }

        public SettingsUtility()
        {
            SettingsActivator = new ActivatorSettings();
            SettingsArmor = new ArmorSettings();
            SettingsBooks = new BookSettings();
            SettingsCells = new CellSettings();
            SettingsContainers = new ContainerSettings();
            SettingsFactions = new FactionSettings();
            SettingsIngestibles = new IngestibleSettings();
            SettingsLeveledItems = new LeveledItemSettings();
            SettingsLeveledSpells = new LeveledSpellSettings();
            SettingsLeveledNPCs = new LeveledNPCSettings();
            SettingsLocations = new LocationSettings();
            SettingsNPCs = new NPCSettings();
            SettingsPerks = new PerkSettings();
            SettingsQuests = new QuestSettings();
            SettingsWeapons = new WeaponSettings();
        }

        public void ProcessBashTags(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, Settings UserSettings)
        {
            foreach(var mod in state.LoadOrder.ListedOrder)
            {
                // Skip Mod
                if (UserSettings.settingsNoMerge.Contains(mod.ModKey))
                    continue;

                // Get Description
                string modDescr = mod.Mod?.ModHeader.Description ?? "";

                // No Tags Found
                if (!modDescr.Contains("BASH:"))
                    continue;

                // Process Bash Tags
                int start = modDescr.IndexOf("{{BASH:") + 1;
                int end = modDescr.IndexOf("}}", start);
                string bashTags = modDescr[start..end];
                string[] bashArray = bashTags.Split(',');

                foreach (var bash in bashArray)
                {
                    // Fix BASH: Showing in tag
                    string fixedTag = bash.Replace("{BASH:", "");

                    #region Actors
                    if (fixedTag == "Actors.ACBS")
                        SettingsNPCs.ACBS.Add(mod.ModKey);
                    else if (fixedTag == "Actors.AIData")
                        SettingsNPCs.AIData.Add(mod.ModKey);
                    else if (fixedTag == "Actors.AIPackages")
                        SettingsNPCs.AIPackages.Add(mod.ModKey);
                    else if (fixedTag == "Actors.AIPackagesForceAdd")
                        SettingsNPCs.AIPackagesForceAdd.Add(mod.ModKey);
                    else if (fixedTag == "Actors.CombatStyle")
                        SettingsNPCs.CombatStyle.Add(mod.ModKey);
                    else if (fixedTag == "Actors.DeathItem")
                        SettingsNPCs.DeathItem.Add(mod.ModKey);
                    else if (fixedTag == "Actors.Factions" || fixedTag == "Factions")
                        SettingsNPCs.Factions.Add(mod.ModKey);
                    else if (fixedTag == "Actors.Perks.Add")
                        SettingsNPCs.PerksAdd.Add(mod.ModKey);
                    else if (fixedTag == "Actors.Perks.Change")
                        SettingsNPCs.PerksChange.Add(mod.ModKey);
                    else if (fixedTag == "Actors.Perks.Remove")
                        SettingsNPCs.PerksRemove.Add(mod.ModKey);
                    else if (fixedTag == "Actors.RecordFlags")
                        SettingsNPCs.RecordFlags.Add(mod.ModKey);
                    else if (fixedTag == "Actors.Stats")
                        SettingsNPCs.Stats.Add(mod.ModKey);
                    else if (fixedTag == "Actors.Voice")
                        SettingsNPCs.Voice.Add(mod.ModKey);
                    else if (fixedTag == "NPC.AIPackageOverrides")
                        SettingsNPCs.AIPackagesOverrides.Add(mod.ModKey);
                    else if (fixedTag == "NPC.AttackRace")
                        SettingsNPCs.AttackRace.Add(mod.ModKey);
                    else if (fixedTag == "NPC.Class")
                        SettingsNPCs.Class.Add(mod.ModKey);
                    else if (fixedTag == "NPC.CrimeFaction")
                        SettingsNPCs.CrimeFaction.Add(mod.ModKey);
                    else if (fixedTag == "NPC.DefaultOutfit")
                        SettingsNPCs.DefaultOutfit.Add(mod.ModKey);
                    else if (fixedTag == "NPC.Race")
                        SettingsNPCs.Race.Add(mod.ModKey);
                    else if (fixedTag == "NpcFacesForceFullImport")
                        SettingsNPCs.NpcFacesForceFullImport.Add(mod.ModKey);
                    #endregion

                    #region Cells
                    else if (fixedTag == "C.Acoustic")
                        SettingsCells.Acoustic.Add(mod.ModKey);
                    else if (fixedTag == "C.Climate")
                        SettingsCells.Climate.Add(mod.ModKey);
                    else if (fixedTag == "C.Encounter")
                        SettingsCells.Encounter.Add(mod.ModKey);
                    else if (fixedTag == "C.ImageSpace")
                        SettingsCells.ImageSpace.Add(mod.ModKey);
                    else if (fixedTag == "C.Light")
                        SettingsCells.Light.Add(mod.ModKey);
                    else if (fixedTag == "C.LockList")
                        SettingsCells.LockList.Add(mod.ModKey);
                    else if (fixedTag == "C.Location")
                        SettingsCells.Location.Add(mod.ModKey);
                    else if (fixedTag == "C.MiscFlags" || fixedTag == "C.SkyLighting")
                        SettingsCells.MiscFlags.Add(mod.ModKey);
                    else if (fixedTag == "C.Music")
                        SettingsCells.Music.Add(mod.ModKey);
                    else if (fixedTag == "C.Name")
                        SettingsCells.Names.Add(mod.ModKey);
                    else if (fixedTag == "C.Owner")
                        SettingsCells.Owner.Add(mod.ModKey);
                    else if (fixedTag == "C.RecordFlags")
                        SettingsCells.RecordFlags.Add(mod.ModKey);
                    else if (fixedTag == "C.Regions")
                        SettingsCells.Regions.Add(mod.ModKey);
                    else if (fixedTag == "C.Water")
                        SettingsCells.Water.Add(mod.ModKey);
                    #endregion

                    #region Destructables
                    else if (fixedTag == "Destructible")
                    {
                        SettingsActivator.Destructible.Add(mod.ModKey);
                        SettingsArmor.Destructible.Add(mod.ModKey);
                        SettingsBooks.Destructible.Add(mod.ModKey);
                        SettingsContainers.Destructible.Add(mod.ModKey);
                        SettingsIngestibles.Destructible.Add(mod.ModKey);
                        SettingsWeapons.Destructible.Add(mod.ModKey);
                    }
                    #endregion

                    #region Enchantments
                    else if (fixedTag == "Enchantments")
                    {
                        SettingsArmor.Enchantments.Add(mod.ModKey);
                        SettingsWeapons.Enchantments.Add(mod.ModKey);
                    }
                    else if (fixedTag == "EffectStats")
                    {
                    }
                    else if (fixedTag == "EnchantmentStats")
                    {
                    }
                    #endregion

                    #region Graphics
                    else if (fixedTag == "Graphics")
                    {
                        SettingsActivator.Graphics.Add(mod.ModKey);
                        SettingsArmor.Graphics.Add(mod.ModKey);
                        SettingsBooks.Graphics.Add(mod.ModKey);
                        SettingsContainers.Graphics.Add(mod.ModKey);
                        SettingsIngestibles.Graphics.Add(mod.ModKey);
                        SettingsWeapons.Graphics.Add(mod.ModKey);
                    }
                    #endregion

                    #region Inventory
                    else if (fixedTag == "Invent.Add")
                    {
                        SettingsNPCs.InventAdd.Add(mod.ModKey);
                        SettingsContainers.InventAdd.Add(mod.ModKey);
                    }
                    else if (fixedTag == "Invent.Change")
                    {
                        SettingsNPCs.InventChange.Add(mod.ModKey);
                        SettingsContainers.InventChange.Add(mod.ModKey);
                    }
                    else if (fixedTag == "Invent.Remove")
                    {
                        SettingsNPCs.InventRemove.Add(mod.ModKey);
                        SettingsContainers.InventRemove.Add(mod.ModKey);
                    }
                    else if (fixedTag == "Invent")
                    {
                        SettingsNPCs.InventAdd.Add(mod.ModKey);
                        SettingsContainers.InventAdd.Add(mod.ModKey);
                        SettingsNPCs.InventChange.Add(mod.ModKey);
                        SettingsContainers.InventChange.Add(mod.ModKey);
                        SettingsNPCs.InventRemove.Add(mod.ModKey);
                        SettingsContainers.InventRemove.Add(mod.ModKey);
                    }
                    #endregion

                    #region Keywords
                    else if (fixedTag == "Keywords")
                    {
                        SettingsActivator.Keywords.Add(mod.ModKey);
                        SettingsArmor.Keywords.Add(mod.ModKey);
                        SettingsBooks.Keywords.Add(mod.ModKey);
                        SettingsIngestibles.Keywords.Add(mod.ModKey);
                        SettingsLocations.Keywords.Add(mod.ModKey);
                        SettingsNPCs.Keywords.Add(mod.ModKey);
                        SettingsWeapons.Keywords.Add(mod.ModKey);
                    }
                    #endregion

                    #region Leveled Lists
                    else if (fixedTag == "Delev")
                    {
                        SettingsLeveledItems.Delev.Add(mod.ModKey);
                        SettingsLeveledSpells.Delev.Add(mod.ModKey);
                        SettingsLeveledNPCs.Delev.Add(mod.ModKey);
                    }
                    else if (fixedTag == "Relev")
                    {
                        SettingsLeveledItems.Relev.Add(mod.ModKey);
                        SettingsLeveledSpells.Relev.Add(mod.ModKey);
                        SettingsLeveledNPCs.Relev.Add(mod.ModKey);
                    }
                    #endregion

                    #region Names
                    else if (fixedTag == "Names")
                    {
                        SettingsActivator.Names.Add(mod.ModKey);
                        SettingsArmor.Names.Add(mod.ModKey);
                        SettingsBooks.Names.Add(mod.ModKey);
                        SettingsContainers.Names.Add(mod.ModKey);
                        SettingsFactions.Names.Add(mod.ModKey);
                        SettingsIngestibles.Names.Add(mod.ModKey);
                        SettingsLocations.Names.Add(mod.ModKey);
                        SettingsNPCs.Names.Add(mod.ModKey);
                        SettingsPerks.Names.Add(mod.ModKey);
                        SettingsQuests.Names.Add(mod.ModKey);
                        SettingsWeapons.Names.Add(mod.ModKey);
                    }
                    #endregion

                    #region Object Bounds
                    if (fixedTag == "ObjectBounds")
                    {
                        SettingsActivator.ObjectBounds.Add(mod.ModKey);
                        SettingsBooks.ObjectBounds.Add(mod.ModKey);
                        SettingsContainers.ObjectBounds.Add(mod.ModKey);
                        SettingsIngestibles.ObjectBounds.Add(mod.ModKey);
                        SettingsLeveledItems.ObjectBounds.Add(mod.ModKey);
                        SettingsLeveledSpells.ObjectBounds.Add(mod.ModKey);
                        SettingsLeveledNPCs.ObjectBounds.Add(mod.ModKey);
                        SettingsWeapons.ObjectBounds.Add(mod.ModKey);
                    }
                    #endregion

                    #region Outfits
                    else if (fixedTag == "Outfits.Add")
                    {
                    }
                    else if (fixedTag == "Outfits.Remove")
                    {
                    }
                    #endregion

                    #region Race
                    else if (fixedTag == "R.AddSpells")
                    {
                    }
                    else if (fixedTag == "R.Body-F" || fixedTag == "Body-F")
                    {
                    }
                    else if (fixedTag == "R.Body-M" || fixedTag == "Body-M")
                    {
                    }
                    else if (fixedTag == "R.Body-Size-F" || fixedTag == "Body-Size-F")
                    {
                    }
                    else if (fixedTag == "R.Body-Size-M" || fixedTag == "Body-Size-M")
                    {
                    }
                    else if (fixedTag == "R.ChangeSpells")
                    {
                    }
                    else if (fixedTag == "R.Description")
                    {
                    }
                    else if (fixedTag == "R.Ears")
                    {
                    }
                    else if (fixedTag == "R.Eyes" || fixedTag == "Eyes" || fixedTag == "Eyes - D" || fixedTag == "Eyes - E" || fixedTag == "Eyes - R")
                    {
                    }
                    else if (fixedTag == "R.Hair" || fixedTag == "Hair")
                    {
                    }
                    else if (fixedTag == "R.Head")
                    {
                    }
                    else if (fixedTag == "R.Mouth")
                    {
                    }
                    else if (fixedTag == "R.Relations.Add")
                    {
                    }
                    else if (fixedTag == "R.Relations.Change" || fixedTag == "R.Relations")
                    {
                    }
                    else if (fixedTag == "R.Relations.Remove")
                    {
                    }
                    else if (fixedTag == "R.Skills")
                    {
                    }
                    else if (fixedTag == "R.Teeth")
                    {
                    }
                    else if (fixedTag == "R.Voice-F" || fixedTag == "Voice-F")
                    {
                    }
                    else if (fixedTag == "R.Voice-M" || fixedTag == "Voice-M")
                    {
                    }
                    #endregion

                    #region Relations
                    else if (fixedTag == "Relations.Add")
                        SettingsFactions.RelationsAdd.Add(mod.ModKey);
                    else if (fixedTag == "Relations.Change" || fixedTag == "Relations")
                        SettingsFactions.RelationsChange.Add(mod.ModKey);
                    else if (fixedTag == "Relations.Remove" || fixedTag == "Derel")
                        SettingsFactions.RelationsRemove.Add(mod.ModKey);
                    #endregion

                    #region Scripts
                    else if (fixedTag == "Scripts")
                        SettingsContainers.Scripts.Add(mod.ModKey);
                    #endregion

                    #region Sound
                    else if (fixedTag == "Sound")
                    {
                        SettingsActivator.Sounds.Add(mod.ModKey);
                        SettingsArmor.Sounds.Add(mod.ModKey);
                        SettingsBooks.Sounds.Add(mod.ModKey);
                        SettingsContainers.Sounds.Add(mod.ModKey);
                        SettingsIngestibles.Sounds.Add(mod.ModKey);
                        SettingsLocations.Sounds.Add(mod.ModKey);
                        SettingsWeapons.Sounds.Add(mod.ModKey);
                    }
                    #endregion

                    #region Stats
                    else if (fixedTag == "Stats")
                    {
                        SettingsArmor.Stats.Add(mod.ModKey);
                        SettingsBooks.Stats.Add(mod.ModKey);
                        SettingsIngestibles.Stats.Add(mod.ModKey);
                        SettingsWeapons.Stats.Add(mod.ModKey);
                    }
                    else if (fixedTag == "SpellStats")
                    { 
                    }
                    #endregion

                    #region Text
                    else if (fixedTag == "Text")
                    {
                        SettingsArmor.Text.Add(mod.ModKey);
                        SettingsBooks.Text.Add(mod.ModKey);
                        SettingsIngestibles.Text.Add(mod.ModKey);
                        SettingsPerks.Text.Add(mod.ModKey);
                        SettingsQuests.Text.Add(mod.ModKey);
                        SettingsWeapons.Text.Add(mod.ModKey);
                    }
                    #endregion
                }
            }
        }

        public void ProcessManualTags(Settings UserSettings)
        {
            #region Actors
            SettingsNPCs.ACBS.Add(UserSettings.settingsActors);
            SettingsNPCs.AIData.Add(UserSettings.settingsActors);
            SettingsNPCs.AIPackages.Add(UserSettings.settingsActors);
            SettingsNPCs.AIPackagesForceAdd.Add(UserSettings.settingsActors);
            SettingsNPCs.CombatStyle.Add(UserSettings.settingsActors);
            SettingsNPCs.DeathItem.Add(UserSettings.settingsActors);
            SettingsNPCs.Factions.Add(UserSettings.settingsActors);
            SettingsNPCs.PerksAdd.Add(UserSettings.settingsActors);
            SettingsNPCs.PerksChange.Add(UserSettings.settingsActors);
            SettingsNPCs.PerksRemove.Add(UserSettings.settingsActors);
            SettingsNPCs.RecordFlags.Add(UserSettings.settingsActors);
            SettingsNPCs.Stats.Add(UserSettings.settingsActors);
            SettingsNPCs.Voice.Add(UserSettings.settingsActors);
            SettingsNPCs.AIPackagesOverrides.Add(UserSettings.settingsActors);
            SettingsNPCs.AttackRace.Add(UserSettings.settingsActors);
            SettingsNPCs.Class.Add(UserSettings.settingsActors);
            SettingsNPCs.CrimeFaction.Add(UserSettings.settingsActors);
            SettingsNPCs.DefaultOutfit.Add(UserSettings.settingsActors);
            SettingsNPCs.Race.Add(UserSettings.settingsActors);
            SettingsNPCs.NpcFacesForceFullImport.Add(UserSettings.settingsActors);
            #endregion

            #region Cells
            SettingsCells.Acoustic.Add(UserSettings.settingsCells);
            SettingsCells.Climate.Add(UserSettings.settingsCells);
            SettingsCells.Encounter.Add(UserSettings.settingsCells);
            SettingsCells.ImageSpace.Add(UserSettings.settingsCells);
            SettingsCells.Light.Add(UserSettings.settingsCells);
            SettingsCells.LockList.Add(UserSettings.settingsCells);
            SettingsCells.Location.Add(UserSettings.settingsCells);
            SettingsCells.MiscFlags.Add(UserSettings.settingsCells);
            SettingsCells.Music.Add(UserSettings.settingsCells);
            SettingsCells.Names.Add(UserSettings.settingsCells);
            SettingsCells.Owner.Add(UserSettings.settingsCells);
            SettingsCells.RecordFlags.Add(UserSettings.settingsCells);
            SettingsCells.Regions.Add(UserSettings.settingsCells);
            SettingsCells.Water.Add(UserSettings.settingsCells);
            #endregion

            #region Destructables
            SettingsActivator.Destructible.Add(UserSettings.settingsDestructibles);
            SettingsArmor.Destructible.Add(UserSettings.settingsDestructibles);
            SettingsBooks.Destructible.Add(UserSettings.settingsDestructibles);
            SettingsContainers.Destructible.Add(UserSettings.settingsDestructibles);
            SettingsIngestibles.Destructible.Add(UserSettings.settingsDestructibles);
            SettingsWeapons.Destructible.Add(UserSettings.settingsDestructibles);
            #endregion

            #region Enchantments
            SettingsArmor.Enchantments.Add(UserSettings.settingsEnchantments);
            SettingsWeapons.Enchantments.Add(UserSettings.settingsEnchantments);
            #endregion

            #region Graphics
            SettingsActivator.Graphics.Add(UserSettings.settingsGraphics);
            SettingsArmor.Graphics.Add(UserSettings.settingsGraphics);
            SettingsBooks.Graphics.Add(UserSettings.settingsGraphics);
            SettingsContainers.Graphics.Add(UserSettings.settingsGraphics);
            SettingsIngestibles.Graphics.Add(UserSettings.settingsGraphics);
            SettingsWeapons.Graphics.Add(UserSettings.settingsGraphics);
            #endregion

            #region Inventory
            SettingsNPCs.InventAdd.Add(UserSettings.settingsInventory);
            SettingsNPCs.InventRemove.Add(UserSettings.settingsInventory);
            SettingsNPCs.InventChange.Add(UserSettings.settingsInventory);
            SettingsContainers.InventAdd.Add(UserSettings.settingsInventory);
            SettingsContainers.InventRemove.Add(UserSettings.settingsInventory);
            SettingsContainers.InventChange.Add(UserSettings.settingsInventory);
            #endregion

            #region Keywords
            SettingsActivator.Keywords.Add(UserSettings.settingsKeywords);
            SettingsArmor.Keywords.Add(UserSettings.settingsKeywords);
            SettingsBooks.Keywords.Add(UserSettings.settingsKeywords);
            SettingsLocations.Keywords.Add(UserSettings.settingsKeywords);
            SettingsNPCs.Keywords.Add(UserSettings.settingsKeywords);
            SettingsIngestibles.Keywords.Add(UserSettings.settingsKeywords);
            SettingsWeapons.Keywords.Add(UserSettings.settingsKeywords);
            #endregion

            #region Leveled Lists
            SettingsLeveledItems.Delev.Add(UserSettings.settingsNames);
            SettingsLeveledSpells.Delev.Add(UserSettings.settingsNames);
            SettingsLeveledNPCs.Delev.Add(UserSettings.settingsNames);
            SettingsLeveledItems.Relev.Add(UserSettings.settingsNames);
            SettingsLeveledSpells.Relev.Add(UserSettings.settingsNames);
            SettingsLeveledNPCs.Relev.Add(UserSettings.settingsNames);
            #endregion

            #region Names
            SettingsActivator.Names.Add(UserSettings.settingsNames);
            SettingsArmor.Names.Add(UserSettings.settingsNames);
            SettingsBooks.Names.Add(UserSettings.settingsNames);
            SettingsContainers.Names.Add(UserSettings.settingsNames);
            SettingsFactions.Names.Add(UserSettings.settingsNames);
            SettingsIngestibles.Names.Add(UserSettings.settingsNames);
            SettingsLocations.Names.Add(UserSettings.settingsNames);
            SettingsNPCs.Names.Add(UserSettings.settingsNames);
            SettingsPerks.Names.Add(UserSettings.settingsNames);
            SettingsQuests.Names.Add(UserSettings.settingsNames);
            SettingsWeapons.Names.Add(UserSettings.settingsNames);
            #endregion

            #region Object Bounds
            SettingsActivator.ObjectBounds.Add(UserSettings.settingsStats);
            SettingsBooks.ObjectBounds.Add(UserSettings.settingsStats);
            SettingsContainers.ObjectBounds.Add(UserSettings.settingsStats);
            SettingsIngestibles.ObjectBounds.Add(UserSettings.settingsStats);
            SettingsLeveledItems.ObjectBounds.Add(UserSettings.settingsNames);
            SettingsLeveledSpells.ObjectBounds.Add(UserSettings.settingsNames);
            SettingsLeveledNPCs.ObjectBounds.Add(UserSettings.settingsNames);
            SettingsWeapons.ObjectBounds.Add(UserSettings.settingsStats);
            #endregion

            #region Outfits

            #endregion

            #region Race

            #endregion

            #region Relations
            SettingsFactions.RelationsChange.Add(UserSettings.settingsRelations);
            #endregion

            #region Scripts
            SettingsContainers.Scripts.Add(UserSettings.settingsScripts);
            #endregion

            #region Sound
            SettingsActivator.Sounds.Add(UserSettings.settingsSounds);
            SettingsArmor.Sounds.Add(UserSettings.settingsSounds);
            SettingsBooks.Sounds.Add(UserSettings.settingsSounds);
            SettingsContainers.Sounds.Add(UserSettings.settingsSounds);
            SettingsIngestibles.Sounds.Add(UserSettings.settingsSounds);
            SettingsLocations.Sounds.Add(UserSettings.settingsSounds);
            SettingsWeapons.Sounds.Add(UserSettings.settingsSounds);
            #endregion

            #region Stats
            SettingsArmor.Stats.Add(UserSettings.settingsStats);
            SettingsBooks.Stats.Add(UserSettings.settingsStats);
            SettingsIngestibles.Stats.Add(UserSettings.settingsStats);
            SettingsWeapons.Stats.Add(UserSettings.settingsStats);
            #endregion

            #region Text
            SettingsArmor.Text.Add(UserSettings.settingsText);
            SettingsBooks.Text.Add(UserSettings.settingsText);
            SettingsIngestibles.Text.Add(UserSettings.settingsText);
            SettingsPerks.Text.Add(UserSettings.settingsText);
            SettingsQuests.Text.Add(UserSettings.settingsText);
            SettingsWeapons.Text.Add(UserSettings.settingsText);
            #endregion

        }
    }


}
