using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Fusion
{
    public class Settings
    {
        [SettingName("Use Bash Tags")]
        public bool BashTags = true;

        [SettingName("Use Bash Tags from LOOT")]
        public bool BashTagsLoot = true;

        [SettingName("Loot Master List Location")]
        public string BashTagsLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LOOT\\games\\Skyrim Special Edition\\masterlist.yaml";

        [SettingName("No Merge")]
        public List<ModKey> settingsNoMerge = new();

        [SettingName("Actors")]
        public List<ModKey> settingsActors = new();

        [SettingName("Actor Tags")]
        public List<ActorSettings> granularActors = new() { new ActorSettings() };

        [SettingName("Cells")]
        public List<ModKey> settingsCells = new();

        [SettingName("Cell Tags")]
        public List<CellSettings> granularCells = new() { new CellSettings() };

        [SettingName("Destuctibles")]
        public List<ModKey> settingsDestructibles = new();

        [SettingName("Enchantments")]
        public List<ModKey> settingsEnchantments = new();

        [SettingName("Graphics")]
        public List<ModKey> settingsGraphics = new();

        [SettingName("Inventory \\ Container")]
        public List<ModKey> settingsInventory = new();

        [SettingName("Keywords")]
        public List<ModKey> settingsKeywords = new();

        [SettingName("Leveled Lists")]
        public List<ModKey> settingsLeveled = new();

        [SettingName("Names")]
        public List<ModKey> settingsNames = new();

        [SettingName("Outfits")]
        public List<ModKey> settingsOutfits = new List<ModKey>();

        [SettingName("Race")]
        public List<ModKey> settingsRace = new List<ModKey>();

        [SettingName("References")]
        public List<ModKey> settingsRefs = new();

        [SettingName("Reference Tags")]
        public List<RefSettings> granularRefs = new() { new RefSettings() };

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

    public class TagSetting
    {        
        public string BashTag { get; set; }
        public ModKey Key { get; set; }

        public TagSetting(string pBashTag, ModKey pKey)
        {
            BashTag = pBashTag;
            Key = pKey;
        }

        public void SetKey(ModKey pKey)
        {
            Key = pKey;
        }
    }

    public class CellSettings
    {
        [SettingName("Acoustic")]
        public List<ModKey> cellAcoustic = new();
        [SettingName("Climate")]
        public List<ModKey> cellClimate = new();
        [SettingName("Encounter")]
        public List<ModKey> cellEncounter = new();
        [SettingName("Image Space")]
        public List<ModKey> cellImageSpace = new();
        [SettingName("Light")]
        public List<ModKey> cellLight = new();
        [SettingName("Lock List")]
        public List<ModKey> cellLockList = new();
        [SettingName("Location")]
        public List<ModKey> cellLocation = new();
        [SettingName("Misc Flags")]
        public List<ModKey> cellMiscFlags = new();
        [SettingName("Music")]
        public List<ModKey> cellMusic = new();
        [SettingName("Name")]
        public List<ModKey> cellName = new();
        [SettingName("Owner")]
        public List<ModKey> cellOwner = new();
        [SettingName("Record Flags")]
        public List<ModKey> cellRecordFlags = new();
        [SettingName("Regions")]
        public List<ModKey> cellRegions = new();
        [SettingName("Sky Lighting")]
        public List<ModKey> cellSkyLighting = new();
        [SettingName("Water")]
        public List<ModKey> cellWater = new();
    }

    public class ActorSettings
    {
        [SettingName("ACBS")]
        public List<ModKey> actorACBS = new();
        [SettingName("AI Data")]
        public List<ModKey> actorAIData = new();
        [SettingName("AI Packages")]
        public List<ModKey> actorAIPackages = new();
        [SettingName("AI Package Overrides")]
        public List<ModKey> actorAIPackageOverride = new();
        [SettingName("AttackRace")]
        public List<ModKey> actorAttackRace = new();
        [SettingName("Class")]
        public List<ModKey> actorClass = new();
        [SettingName("Combat Style")]
        public List<ModKey> actorCombatStyle = new();
        [SettingName("Crime Faction")]
        public List<ModKey> actorCrimeFaction = new();
        [SettingName("DeathItem")]
        public List<ModKey> actorDeathItem = new();
        [SettingName("Default Outfit")]
        public List<ModKey> actorOutfit = new();
        [SettingName("Full Face Import")]
        public List<ModKey> actorFullFace = new();
        [SettingName("Race")]
        public List<ModKey> actorRace = new();
        [SettingName("Voice")]
        public List<ModKey> actorVoice = new();
    }

    public class RefSettings
    {
        [SettingName("F.Base")]
        public List<ModKey> refBase = new();
        [SettingName("F.EnableParent")]
        public List<ModKey> refEnableParent = new();
        [SettingName("F.LocationReference")]
        public List<ModKey> refLocationReference = new();
    }

    public class LootTag 
    {
        public string ModName {get; set;}
        public List<string> TagList {get; set;}

        public LootTag(string Name)
        {
            ModName = Name;
            TagList = new List<string>();
        }
    }

    public class SettingsUtility
    {
        public List<TagSetting> AllSettings;
        public List<LootTag> LootTags;

        public SettingsUtility()
        {
            AllSettings = new();
            LootTags = new();
        }

        public HashSet<ModKey> GetModList(params string[] bashTags)
        {
            HashSet<ModKey> modList = [];

            foreach (var ts in AllSettings)
            {
                if (bashTags.Any(ts.BashTag.Contains))
                    modList.Add(ts.Key);
            }

            return modList;
        }

        public List<string> GetLootList(string FileName)
        {
            List<string> ReturnList = new();

            foreach(LootTag s in LootTags)
                if (s.ModName == FileName)
                {
                    foreach(string tag in s.TagList)
                        ReturnList.Add(tag);

                    break;
                }

            return ReturnList;
        }

        public bool HasTags(string BashTag)
        {
            List<string> tagList = BashTag.Split(',').ToList();
            List<ModKey> ModList = new();
            foreach(var ts in AllSettings)
                if (tagList.Contains(ts.BashTag))
                    ModList.Add(ts.Key);

            return ModList.Any();
        }

        public bool HasTags(string BashTag, out HashSet<ModKey> TagList)
        {
            List<ModKey> ModList = new();
            foreach(var ts in AllSettings)
                if (BashTag == ts.BashTag)
                    ModList.Add(ts.Key);

            TagList = new HashSet<ModKey>(ModList);
            return TagList.Any();
        }

        public HashSet<ModKey> TagList(string BashTag)
        {
            List<ModKey> ModList = new();
            foreach(var ts in AllSettings)
                if (BashTag == ts.BashTag)
                    ModList.Add(ts.Key);

            return new HashSet<ModKey>(ModList);
        }

        public int TagCount(string BashTag)
        {
            HashSet<ModKey> ModList = new();
            foreach(var ts in AllSettings)
                if (BashTag == ts.BashTag)
                    ModList.Add(ts.Key);

            return ModList.Count;
        }

        private void ProcessUserSetting(List<ModKey> ModKeys, params string[] bashTags)
        {
            foreach (var key in ModKeys)
            {
                foreach(var tag in bashTags)
                    if (!AllSettings.Where(x => x.BashTag.Equals(tag) && x.Key.Equals(key)).Any())
                        AllSettings.Add(new TagSetting(tag, key));
            }
        }

        private static List<string> LegacyTagFix(string pBashTag)
        {
            // Fix Legacy Tags
            List<string> FixedTagList = new();
            if (pBashTag == "Factions")
                FixedTagList.Add(Tags.Actors_Factions);
            else if (pBashTag == "NpcFaces")
                FixedTagList.Add(Tags.NpcFacesForceFullImport);
            else if (pBashTag == "Invent" || pBashTag == "InventOnly")
            {
                FixedTagList.Add(Tags.Invent_Add);
                FixedTagList.Add(Tags.Invent_Change);
                FixedTagList.Add(Tags.Invent_Remove);
            }
            else if (pBashTag == "Body-F")
                FixedTagList.Add(Tags.R_Body_F);
            else if (pBashTag == "Body-M")
                FixedTagList.Add(Tags.R_Body_M);
            else if (pBashTag == "Body-Size-F")
                FixedTagList.Add(Tags.R_Body_Size_F);
            else if (pBashTag == "Body-Size-M")
                FixedTagList.Add(Tags.R_Body_Size_M);
            else if (pBashTag == "Eyes" || pBashTag == "Eyes-D" || pBashTag == "Eyes-E" || pBashTag == "Eyes-R")
                FixedTagList.Add(Tags.R_Eyes);
            else if (pBashTag == "Hair")
                FixedTagList.Add(Tags.R_Hair);
            else if (pBashTag == "Voice-F")
                FixedTagList.Add(Tags.R_Voice_F);
            else if (pBashTag == "Voice-M")
                FixedTagList.Add(Tags.R_Voice_M);
            else if (pBashTag == "R.Relations")
            {
                FixedTagList.Add(Tags.R_Relations_Add);
                FixedTagList.Add(Tags.R_Relations_Change);
                FixedTagList.Add(Tags.R_Relations_Remove);
            }
            else if (pBashTag == "Relations")
            {
                FixedTagList.Add(Tags.Relations_Add);
                FixedTagList.Add(Tags.Relations_Change);
            }
            else if (pBashTag == "Derel")
                FixedTagList.Add(Tags.Relations_Remove);
            else
                FixedTagList.Add(pBashTag);

            return FixedTagList;
        }

        private void ProcessLOOTMaster(string TagLocation)
        {
            // Get LOOT AppData Folder
            if (!File.Exists(TagLocation)) {
                Console.WriteLine("Unable to find LOOT Master List");
                return;
            }
            Console.WriteLine("Processing LOOT Master List");

            // Process the YAML to JSON for easier serializaiton
            using StreamReader reader = File.OpenText(TagLocation);
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(new MergingParser(new Parser(reader)));

            // If there is no YAML file
            if (yamlObject is not Dictionary<object, object> root) return;

            // root = (Dictionary<object, object>)yamlObject;
            foreach (var kvp in root) {
                if (root.TryGetValue("plugins", out var pluginsObj)) {
                    foreach (Dictionary<object, object> plugin in (List<object>)pluginsObj) {
                        if (!plugin.TryGetValue("name", out var nameObj))
                            continue;

                        LootTag NewTag = new(nameObj.ToString()!);
                        string testTag = "";
                        HashSet<string> tags = [];

                        if (plugin.TryGetValue("tag", out var tagObj)) {
                            foreach (var tag in (List<object>)tagObj) {
                                NewTag.TagList.Add(tag.ToString()!);
                                testTag += "," + tag.ToString()!;
                            }
                            LootTags.Add(NewTag);
                        }
                    }
                }
            }

            Console.WriteLine("Processed {0} MasterList Records", LootTags.Count);
        }

        public void Process(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, Settings UserSettings)
        {
            // Process LOOT Master List
            if (UserSettings.BashTagsLoot)
                ProcessLOOTMaster(UserSettings.BashTagsLocation);
                

            // Process Bash Tags
            foreach(var mod in state.LoadOrder.ListedOrder)
            {
                // Skip Mod
                if (UserSettings.settingsNoMerge.Contains(mod.ModKey))
                    continue;

                // Bash Tags in Mod Descriptions
                if (UserSettings.BashTags)
                {
                    // Get Description
                    string modDescr = mod.Mod?.ModHeader.Description ?? "";

                    // No Tags Found
                    if (modDescr.Contains("BASH:"))
                    {

                        // Process Bash Tags
                        int start = modDescr.IndexOf("{{BASH:") + 1;
                        int end = modDescr.IndexOf("}}", start);

                        // Check length for Substring
                        if (start >= 0 && end >= 0)
                        {
                            string bashTags = modDescr[start..end];
                            string[] bashArray = bashTags.Split(',');

                            foreach (var bash in bashArray)
                            {
                                // Fix BASH: Showing in tag
                                string FixedTag = bash.Replace("{BASH:", "");
                                
                                // Legacy Tags
                                foreach (var tag in LegacyTagFix(FixedTag))
                                    AllSettings.Add(new TagSetting(tag, mod.ModKey));
                            }
                        }
                    }
                }

                // Process LOOT Master List
                if (UserSettings.BashTagsLoot)
                {
                    List<string> tags = GetLootList(mod.ModKey.FileName.ToString());
                    foreach(string tag in tags)
                        foreach (var fixedtag in LegacyTagFix(tag.Replace("-",string.Empty)))
                            AllSettings.Add(new TagSetting(fixedtag, mod.ModKey));
                }
            }

            // Process Cell Settings
            foreach(var setting in UserSettings.granularCells)
            {
                ProcessUserSetting(setting.cellAcoustic,Tags.C_Acoustic);
                ProcessUserSetting(setting.cellClimate,Tags.C_Climate);
                ProcessUserSetting(setting.cellEncounter,Tags.C_Encounter);
                ProcessUserSetting(setting.cellImageSpace,Tags.C_ImageSpace);
                ProcessUserSetting(setting.cellLight,Tags.C_Light);
                ProcessUserSetting(setting.cellLockList,Tags.C_LockList);
                ProcessUserSetting(setting.cellLocation,Tags.C_Location);
                ProcessUserSetting(setting.cellMiscFlags,Tags.C_MiscFlags);
                ProcessUserSetting(setting.cellMusic,Tags.C_Music);
                ProcessUserSetting(setting.cellName,Tags.C_Name);
                ProcessUserSetting(setting.cellOwner,Tags.C_Owner);
                ProcessUserSetting(setting.cellRecordFlags,Tags.C_RecordFlags);
                ProcessUserSetting(setting.cellRegions,Tags.C_Regions);
                ProcessUserSetting(setting.cellSkyLighting,Tags.C_SkyLighting);
                ProcessUserSetting(setting.cellWater,Tags.C_Water);
            }

            // Process Actor Settings
            foreach (var setting in UserSettings.granularActors)
            {
                ProcessUserSetting(setting.actorACBS, Tags.Actors_ACBS);
                ProcessUserSetting(setting.actorAIData, Tags.Actors_AIData);
                ProcessUserSetting(setting.actorAIPackageOverride, Tags.Actors_AIPackagesForceAdd);
                ProcessUserSetting(setting.actorAIPackages, Tags.NPC_AIPackageOverrides);
                ProcessUserSetting(setting.actorAttackRace, Tags.NPC_AttackRace);
                ProcessUserSetting(setting.actorClass, Tags.NPC_Class);
                ProcessUserSetting(setting.actorCombatStyle, Tags.Actors_CombatStyle);
                ProcessUserSetting(setting.actorCrimeFaction, Tags.NPC_CrimeFaction);
                ProcessUserSetting(setting.actorDeathItem, Tags.Actors_DeathItem);
                ProcessUserSetting(setting.actorOutfit, Tags.NPC_DefaultOutfit);
                ProcessUserSetting(setting.actorFullFace, Tags.NpcFacesForceFullImport);
                ProcessUserSetting(setting.actorRace, Tags.NPC_Race);
                ProcessUserSetting(setting.actorVoice, Tags.Actors_Voice);
            }

            // Process Ref Settings
            foreach (var setting in UserSettings.granularRefs)
            {
                ProcessUserSetting(setting.refBase, Tags.F_Base);
                ProcessUserSetting(setting.refEnableParent, Tags.F_EnableParent);
                ProcessUserSetting(setting.refLocationReference, Tags.F_LocationReference);
            }

            // Process Bulk Settings
            ProcessUserSetting(UserSettings.settingsActors, Tags.Actors_ACBS, Tags.Actors_AIData, Tags.Actors_AIPackages, Tags.Actors_AIPackagesForceAdd,
                Tags.Actors_CombatStyle, Tags.Actors_DeathItem, Tags.Actors_Factions, Tags.Actors_Perks_Add, Tags.Actors_Perks_Change, Tags.Actors_Perks_Remove,
                Tags.Actors_RecordFlags, Tags.Actors_Skeleton, Tags.Actors_Spells, Tags.Actors_SpellsForceAdd, Tags.Actors_Stats, Tags.Actors_Voice,
                Tags.NPC_AIPackageOverrides, Tags.NPC_AttackRace, Tags.NPC_Class, Tags.NPC_Class, Tags.NPC_CrimeFaction, Tags.NPC_DefaultOutfit, Tags.NPC_Race,
                Tags.NpcFacesForceFullImport);
            ProcessUserSetting(UserSettings.settingsCells, Tags.C_Acoustic, Tags.C_Climate, Tags.C_Encounter, Tags.C_ImageSpace, Tags.C_Light, Tags.C_LockList,
                Tags.C_Location, Tags.C_MiscFlags, Tags.C_Music, Tags.C_Name, Tags.C_Owner, Tags.C_RecordFlags, Tags.C_Regions, Tags.C_SkyLighting, Tags.C_Water);
            ProcessUserSetting(UserSettings.settingsDestructibles, Tags.Destructible);
            ProcessUserSetting(UserSettings.settingsEnchantments, Tags.EffectStats, Tags.Enchantments, Tags.EnchantmentStats);
            ProcessUserSetting(UserSettings.settingsGraphics, Tags.Graphics);
            ProcessUserSetting(UserSettings.settingsInventory, Tags.Invent_Add, Tags.Invent_Change, Tags.Invent_Remove);
            ProcessUserSetting(UserSettings.settingsKeywords, Tags.Keywords);
            ProcessUserSetting(UserSettings.settingsLeveled,Tags.Delev, Tags.Relev);
            ProcessUserSetting(UserSettings.settingsNames,Tags.Names);
            ProcessUserSetting(UserSettings.settingsOutfits,Tags.Outfits_Add, Tags.Outfits_Remove);
            ProcessUserSetting(UserSettings.settingsRace, Tags.R_AddSpells, Tags.R_Body_F, Tags.R_Body_M, Tags.R_Body_Size_F, Tags.R_Body_Size_M, Tags.R_ChangeSpells,
                Tags.R_Description, Tags.R_Ears, Tags.R_Eyes, Tags.R_Hair, Tags.R_Head, Tags.R_Mouth, Tags.R_Skills, Tags.R_Teeth, Tags.R_Voice_F, Tags.R_Voice_M);
            ProcessUserSetting(UserSettings.settingsRefs, Tags.F_Base, Tags.F_EnableParent, Tags.F_LocationReference);
            ProcessUserSetting(UserSettings.settingsRelations, Tags.R_Relations_Add, Tags.R_Relations_Change, Tags.R_Relations_Remove, Tags.Relations_Add, 
                Tags.Relations_Change, Tags.Relations_Remove);
            ProcessUserSetting(UserSettings.settingsScripts, Tags.Scripts);
            ProcessUserSetting(UserSettings.settingsSounds, Tags.Sound);
            ProcessUserSetting(UserSettings.settingsStats, Tags.ObjectBounds, Tags.SpellStats, Tags.Stats);
            ProcessUserSetting(UserSettings.settingsText, Tags.Text);
        }

    }
    
}