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
using Mutagen.Bethesda.Fallout4;
using Microsoft.CodeAnalysis.CSharp;

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

    public class SettingsUtility
    {
        public List<TagSetting> AllSettings;

        public SettingsUtility()
        {
            AllSettings = new();
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

        public HashSet<ModKey> HasTag(string BashTag)
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

        public int TagCount(string BashTag, out HashSet<ModKey> FoundKeys)
        {
            List<ModKey> ModList = new();
            foreach(var ts in AllSettings)
                if (BashTag == ts.BashTag)
                    ModList.Add(ts.Key);

            FoundKeys = new HashSet<ModKey>(ModList);
            return FoundKeys.Count;
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

        public void Process(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, Settings UserSettings)
        {
            // Process Bash Tags
            if (UserSettings.BashTags)
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
                        string FixedTag = bash.Replace("{BASH:", "");
                        
                        // Legacy Tags
                        foreach (var tag in LegacyTagFix(FixedTag))
                            AllSettings.Add(new TagSetting(tag, mod.ModKey));
                    }
                }
            }

            // Process Manual Settings
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