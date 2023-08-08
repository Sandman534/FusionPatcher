using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using Newtonsoft.Json;
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

        [SettingName("No Merge")]
        public List<ModKey> settingsNoMerge = new();

        [SettingName("Actors")]
        public List<ModKey> settingsActors = new();

        [SettingName("Cells")]
        public List<ModKey> settingsCells = new();

        [SettingName("Cell Tags")]
        public List<CellSettings> granularCells = new() { new CellSettings()};

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
        [SettingName("C.Acoustic")]
        public List<ModKey> cellAcoustic = new();
        [SettingName("C.Climate")]
        public List<ModKey> cellClimate = new();
        [SettingName("C.Encounter")]
        public List<ModKey> cellEncounter = new();
        [SettingName("C.ImageSpace")]
        public List<ModKey> cellImageSpace = new();
        [SettingName("C.Light")]
        public List<ModKey> cellLight = new();
        [SettingName("C.LockList")]
        public List<ModKey> cellLockList = new();
        [SettingName("C.Location")]
        public List<ModKey> cellLocation = new();
        [SettingName("C.MiscFlags")]
        public List<ModKey> cellMiscFlags = new();
        [SettingName("C.Music")]
        public List<ModKey> cellMusic = new();
        [SettingName("C.Name")]
        public List<ModKey> cellName = new();
        [SettingName("C.Owner")]
        public List<ModKey> cellOwner = new();
        [SettingName("C.RecordFlags")]
        public List<ModKey> cellRecordFlags = new();
        [SettingName("C.Regions")]
        public List<ModKey> cellRegions = new();
        [SettingName("C.SkyLighting")]
        public List<ModKey> cellSkyLighting = new();
        [SettingName("C.Water")]
        public List<ModKey> cellWater = new();
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

        public HashSet<ModKey> GetModList(string BashTags)
        {
            List<ModKey> ModList = new();
            string[] TagList = BashTags.Split(",");
            foreach(var ts in AllSettings)
                if (TagList.Any(ts.BashTag.Contains))
                    ModList.Add(ts.Key);

            return new HashSet<ModKey>(ModList);
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

        private void ProcessUserSetting(List<ModKey> ModKeys, string BashTags)
        {
            foreach (var key in ModKeys)
            {
                string[] tags = BashTags.Split(',');
                foreach(var tag in tags)
                    if (!AllSettings.Where(x => x.BashTag.Equals(tag) && x.Key.Equals(key)).Any())
                        AllSettings.Add(new TagSetting(tag, key));
            }
        }

        private static List<string> LegacyTagFix(string pBashTag)
        {
            // Fix Legacy Tags
            List<string> FixedTagList = new();
            if (pBashTag == "Factions")
                FixedTagList.Add("Actors.Factions");
            else if (pBashTag == "NpcFaces")
                FixedTagList.Add("NpcFacesForceFullImport");
            else if (pBashTag == "Invent" || pBashTag == "InventOnly")
            {
                FixedTagList.Add("Invent.Add");
                FixedTagList.Add("Invent.Change");
                FixedTagList.Add("Invent.Remove");
            }
            else if (pBashTag == "Body-F")
                FixedTagList.Add("R.Body-F");
            else if (pBashTag == "Body-M")
                FixedTagList.Add("R.Body-M");
            else if (pBashTag == "Body-Size-F")
                FixedTagList.Add("Body-Size-F");
            else if (pBashTag == "Body-Size-M")
                FixedTagList.Add("Body-Size-M");
            else if (pBashTag == "Eyes" || pBashTag == "Eyes-D" || pBashTag == "Eyes-E" || pBashTag == "Eyes-R")
                FixedTagList.Add("R.Eyes");
            else if (pBashTag == "Hair")
                FixedTagList.Add("R.Hair");
            else if (pBashTag == "Voice-F")
                FixedTagList.Add("R.Voice-F");
            else if (pBashTag == "Voice-M")
                FixedTagList.Add("R.Voice-M");
            else if (pBashTag == "R.Relations")
            {
                FixedTagList.Add("R.Relations.Add");
                FixedTagList.Add("R.Relations.Change");
                FixedTagList.Add("R.Relations.Remove");
            }
            else if (pBashTag == "Relations")
            {
                FixedTagList.Add("Relations.Add");
                FixedTagList.Add("Relations.Change");
            }
            else if (pBashTag == "Derel")
                FixedTagList.Add("Relations.Remove");
            else
                FixedTagList.Add(pBashTag);

            return FixedTagList;
        }

        private void ProcessLOOTMaster()
        {
            // Get LOOT AppData Folder
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LOOT\\Skyrim Special Edition\\masterlist.yaml";
            if (!File.Exists(path)) return;

            // Process the YAML to JSON for easier serializaiton
            using StreamReader reader = File.OpenText(path);
            var deserializer = new DeserializerBuilder().Build();
            var yamlObject = deserializer.Deserialize(new MergingParser(new Parser(reader)));
            var serializer = new SerializerBuilder().JsonCompatible().Build();
            var json = serializer.Serialize(yamlObject);
            Rootobject? LOOTList = JsonConvert.DeserializeObject<Rootobject>(json);

            // Process the List
            if (LOOTList?.plugins != null)
                foreach (var plugin in LOOTList.plugins)
                    if (plugin.tag != null && plugin.name != null)
                    {
                        LootTag NewTag = new(plugin.name);
                        foreach (var tag in plugin.tag)
                            if (tag != null)
                                NewTag.TagList.Add(tag.ToString() ?? "");

                        LootTags.Add(NewTag);
                    }
        }

        public void Process(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, Settings UserSettings)
        {
            // Process LOOT Master List
            if (UserSettings.BashTagsLoot)
                ProcessLOOTMaster();

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
                ProcessUserSetting(setting.cellAcoustic,"C.Acoustic");
                ProcessUserSetting(setting.cellClimate,"C.Climate");
                ProcessUserSetting(setting.cellEncounter,"C.Encounter");
                ProcessUserSetting(setting.cellImageSpace,"C.ImageSpace");
                ProcessUserSetting(setting.cellLight,"C.Light");
                ProcessUserSetting(setting.cellLockList,"C.LockList");
                ProcessUserSetting(setting.cellLocation,"C.Location");
                ProcessUserSetting(setting.cellMiscFlags,"C.MiscFlags");
                ProcessUserSetting(setting.cellMusic,"C.Music");
                ProcessUserSetting(setting.cellName,"C.Name");
                ProcessUserSetting(setting.cellOwner,"C.Owner");
                ProcessUserSetting(setting.cellRecordFlags,"C.RecordFlags");
                ProcessUserSetting(setting.cellRegions,"C.Regions");
                ProcessUserSetting(setting.cellSkyLighting,"C.SkyLighting");
                ProcessUserSetting(setting.cellWater,"C.Water");
            }

            // Process Bulk Settings
            ProcessUserSetting(UserSettings.settingsActors,"Actors.ACBS,Actors.AIData,Actors.AIPackages,Actors.AIPackagesForceAdd,Actors.CombatStyle" +
                ",Actors.DeathItem,Actors.Factions,Actors.Perks.Add,Actors.Perks.Change,Actors.Perks.Remove,Actors.RecordFlags,Actors.Skeleton" + 
                ",Actors.Spells,Actors.SpellsForceAdd,Actors.Stats,Actors.Voice,NPC.AIPackageOverrides,NPC.AttackRace,NPC.Class,NPC.CrimeFaction" +
                ",NPC.DefaultOutfit,NPC.Race,NpcFacesForceFullImport");
            ProcessUserSetting(UserSettings.settingsCells,"C.Acoustic,C.Climate,C.Encounter,C.ImageSpace,C.Light,C.LockList,C.Location,C.MiscFlags" +
                ",C.Music,C.Name,C.Owner,C.RecordFlags,C.Regions,C.SkyLighting,C.Water");
            ProcessUserSetting(UserSettings.settingsDestructibles,"Destructible");
            ProcessUserSetting(UserSettings.settingsEnchantments,"EffectStats,Enchantments,EnchantmentStats");
            ProcessUserSetting(UserSettings.settingsGraphics,"Graphics");
            ProcessUserSetting(UserSettings.settingsInventory,"Invent.Add,Invent.Change,Invent.Remove");
            ProcessUserSetting(UserSettings.settingsKeywords,"Keywords");
            ProcessUserSetting(UserSettings.settingsLeveled,"Delev,Relev");
            ProcessUserSetting(UserSettings.settingsNames,"Names");
            // ProcessUserSetting(UserSettings.settingsOutfits,"Outfits.Add,Outfits.Remove");
            // ProcessUserSetting(UserSettings.settingsRace,"R.AddSpells,R.Body-F,R.Body-M,R.Body-Size-F,R.Body-Size-M,R.ChangeSpells,R.Description" +
            //     ",R.Ears,R.Eyes,R.Hair,R.Head,R.Mouth,R.Skills,R.Teeth,R.Voice-F,R.Voice-M");
            ProcessUserSetting(UserSettings.settingsRelations,"R.Relations.Add,R.Relations.Change,R.Relations.Remove,Relations.Add,Relations.Change,Relations.Remove");
            ProcessUserSetting(UserSettings.settingsScripts,"Scripts");
            ProcessUserSetting(UserSettings.settingsSounds,"Sound");
            ProcessUserSetting(UserSettings.settingsStats,"ObjectBounds,SpellStats,Stats");
            ProcessUserSetting(UserSettings.settingsText,"Text");
        }

    }
    
}