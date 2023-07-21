using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace Fusion
{
    public class Settings
    {
        [SettingName("Armor")]
        public List<ArmorSettings> settingsArmor = new() { new ArmorSettings() };
        [SettingName("Books")]
        public List<BookSettings> settingsBooks = new() { new BookSettings() };
        [SettingName("Cells")]
        public List<CellSettings> settingsCells = new() { new CellSettings() };
        [SettingName("Containers")]
        public List<ContainerSettings> settingsContainers = new() { new ContainerSettings() };
        [SettingName("Factions")]
        public List<FactionSettings> settingsFactions = new() { new FactionSettings() };
        [SettingName("Locations")]
        public List<LocationSettings> settingsLocations = new() { new LocationSettings() };
        [SettingName("NPCs")]
        public List<NPCSettings> settingsNPCs = new() { new NPCSettings() };
        [SettingName("Perks")]
        public List<PerkSettings> settingsPerks = new() { new PerkSettings() };
        [SettingName("Quests")]
        public List<QuestSettings> settingsQuests = new() { new QuestSettings() };
        [SettingName("Weapons")]
        public List<WeaponSettings> settingsWeapons = new() { new WeaponSettings() };
    }

    public class Program
    {
        private static Lazy<Settings> _settings = null!;
        private static Settings Settings => _settings.Value;

        public static async Task<int> Main(string[] args)
        {

            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "OverridePatch.esp")
                .SetAutogeneratedSettings(nickname: "Settings", path: "settings.json", out _settings)
                .Run(args);
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {

            Console.WriteLine("Processing Armor");
            ArmorPatcher.Patch(state, Settings.settingsArmor[0]);

            Console.WriteLine("Processing Books");
            BookPatcher.Patch(state, Settings.settingsBooks[0]);

            Console.WriteLine("Processing Cells");
            CellPatcher.Patch(state, Settings.settingsCells[0]);

            Console.WriteLine("Processing Containers");
            ContainerPatcher.Patch(state, Settings.settingsContainers[0]);

            Console.WriteLine("Processing Factions");
            FactionPatcher.Patch(state, Settings.settingsFactions[0]);

            Console.WriteLine("Processing Locations");
            LocationPatcher.Patch(state, Settings.settingsLocations[0]);

            Console.WriteLine("Processing NPCs");
            NPCPatcher.Patch(state, Settings.settingsNPCs[0]);

            Console.WriteLine("Processing Perks");
            PerkPatcher.Patch(state, Settings.settingsPerks[0]);

            Console.WriteLine("Processing Quests");
            QuestPatcher.Patch(state, Settings.settingsQuests[0]);

            Console.WriteLine("Processing Weapons");
            WeaponPatcher.Patch(state, Settings.settingsWeapons[0]);
        }
    }
}