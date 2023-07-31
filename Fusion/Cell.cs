using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System.Collections.Immutable;

namespace Fusion
{
    public class CellSettings
    {
        public List<ModKey> Acoustic = new();
        public List<ModKey> Climate = new();
        public List<ModKey> Encounter = new();
        public List<ModKey> ImageSpace = new();
        public List<ModKey> Light = new();
        public List<ModKey> LockList = new();
        public List<ModKey> Location = new();
        public List<ModKey> MiscFlags = new();
        public List<ModKey> Music = new();
        public List<ModKey> Names = new();
        public List<ModKey> Owner = new();
        public List<ModKey> RecordFlags = new();
        public List<ModKey> Regions = new();
        public List<ModKey> Water = new();
    }

    internal class CellPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, CellSettings Settings)
        {
            List<ModKey> modList = new() {
                Settings.Acoustic, Settings.Climate, Settings.Encounter, Settings.ImageSpace, Settings.Light, Settings.LockList, Settings.Location,
                Settings.MiscFlags,Settings.Music, Settings.Names, Settings.Owner, Settings.RecordFlags, Settings.Regions, Settings.Water };
            HashSet<ModKey> workingModList = new(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Cell().WinningContextOverrides(state.LinkCache))
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(workingContext.Record.FormKey).Last();
                
                //==============================================================================================================
                // Acoustic
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Acoustic.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.AcousticSpace.Equals(originalObject.Record.AcousticSpace))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.AcousticSpace.Equals(workingContext.Record.AcousticSpace)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.AcousticSpace != null) overrideObject.AcousticSpace.SetTo(foundContext.Record.AcousticSpace);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Climate
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Climate.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.SkyAndWeatherFromRegion.Equals(originalObject.Record.SkyAndWeatherFromRegion))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.SkyAndWeatherFromRegion.Equals(workingContext.Record.SkyAndWeatherFromRegion)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.SkyAndWeatherFromRegion != null) overrideObject.SkyAndWeatherFromRegion.SetTo(foundContext.Record.SkyAndWeatherFromRegion);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Encounter Zone
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Encounter.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.EncounterZone.Equals(originalObject.Record.EncounterZone))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.EncounterZone.Equals(workingContext.Record.EncounterZone)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.EncounterZone != null) overrideObject.EncounterZone.SetTo(foundContext.Record.EncounterZone);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Image Space
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.ImageSpace.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.ImageSpace.Equals(originalObject.Record.ImageSpace))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.ImageSpace.Equals(workingContext.Record.ImageSpace)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ImageSpace != null) overrideObject.ImageSpace.SetTo(foundContext.Record.ImageSpace);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Lighting
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Light.Contains(context.ModKey)))
                {
                    if ((!foundContext.Record.Lighting?.Equals(originalObject.Record.Lighting) ?? false)
                        || !foundContext.Record.LightingTemplate.Equals(originalObject.Record.LightingTemplate))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Lighting?.Equals(originalObject.Record.Lighting) ?? false) Change = true;
                        if (!foundContext.Record.LightingTemplate.Equals(workingContext.Record.LightingTemplate)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Lighting != null) overrideObject.Lighting?.DeepCopyIn(foundContext.Record.Lighting);
                            if (foundContext.Record.LightingTemplate != null) overrideObject.LightingTemplate.SetTo(foundContext.Record.LightingTemplate);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Lock List
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.LockList.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.LockList.Equals(originalObject.Record.LockList))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.LockList.Equals(workingContext.Record.LockList)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.LockList != null) overrideObject.LockList.SetTo(foundContext.Record.LockList);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Location
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Location.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Location.Equals(originalObject.Record.Location))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Location.Equals(workingContext.Record.Location)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Location != null) overrideObject.Location.SetTo(foundContext.Record.Location);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Misc Flags
                //==============================================================================================================
                if (Settings.MiscFlags.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.MiscFlags.Contains(context.ModKey) && (!context.Record.Flags.Equals(originalObject.Record.Flags)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        Cell.Flag overrideObject;
                        if (state.LinkCache.TryResolveContext<ICell, ICellGetter>(workingContext.Record.FormKey, out var patchRecord)) overrideObject = patchRecord.Record.Flags;
                        else overrideObject = workingContext.Record.Flags;

                        // Add Records
                        bool Change = false;
                        foreach (var context in foundContext)
                            foreach (var rec in Enums<Cell.Flag>.Values)
                                if (context.Record.Flags.HasFlag(rec) && !originalObject.Record.Flags.HasFlag(rec) && !overrideObject.HasFlag(rec))
                                {
                                    overrideObject |= rec;
                                    Change = true;
                                }

                        // Remove Records
                        foreach (var context in foundContext.Reverse())
                            foreach (var rec in Enums<Cell.Flag>.Values)
                                if (!context.Record.Flags.HasFlag(rec) && originalObject.Record.Flags.HasFlag(rec) && overrideObject.HasFlag(rec))
                                {
                                    overrideObject &= ~rec;
                                    Change = true;
                                }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            foreach (var rec in Enums<Cell.Flag>.Values)
                                if (overrideObject.HasFlag(rec) && !addedRecord.Flags.HasFlag(rec)) addedRecord.Flags |= rec;
                                else if (!overrideObject.HasFlag(rec) && addedRecord.Flags.HasFlag(rec)) addedRecord.Flags &= ~rec;
                        }
                    }
                }

                //==============================================================================================================
                // Music
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Music.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Music.Equals(originalObject.Record.Music))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Music.Equals(workingContext.Record.Music)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Music != null) overrideObject.Music.SetTo(foundContext.Record.Music);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Name
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Names.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Name?.Equals(originalObject.Record.Name) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Name?.Equals(workingContext.Record.Name) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Name != null) overrideObject.Name?.Set(foundContext.Record.Name.TargetLanguage, foundContext.Record.Name.String);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Owner
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Owner.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Owner.Equals(originalObject.Record.Owner))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Owner.Equals(workingContext.Record.Owner)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Name != null) overrideObject.Owner.SetTo(foundContext.Record.Owner);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Record Flags
                //==============================================================================================================
                if (Settings.RecordFlags.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.RecordFlags.Contains(context.ModKey) && (!context.Record.MajorFlags.Equals(originalObject.Record.MajorFlags)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        Cell.MajorFlag overrideObject;
                        if (state.LinkCache.TryResolveContext<ICell, ICellGetter>(workingContext.Record.FormKey, out var patchRecord))  overrideObject = patchRecord.Record.MajorFlags;
                        else overrideObject = workingContext.Record.MajorFlags;

                        // Add Records
                        bool Change = false;
                        foreach (var context in foundContext)
                            foreach (var rec in Enums<Cell.MajorFlag>.Values)
                                if (context.Record.MajorFlags.HasFlag(rec) && !originalObject.Record.MajorFlags.HasFlag(rec) && !overrideObject.HasFlag(rec))
                                {
                                    overrideObject |= rec;
                                    Change = true;
                                }

                        // Remove Records
                        foreach (var context in foundContext.Reverse())
                            foreach (var rec in Enums<Cell.MajorFlag>.Values)
                                if (!context.Record.MajorFlags.HasFlag(rec) && originalObject.Record.MajorFlags.HasFlag(rec) && overrideObject.HasFlag(rec))
                                {
                                    overrideObject &= ~rec;
                                    Change = true;
                                }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            foreach (var rec in Enums<Cell.MajorFlag>.Values)
                                if (overrideObject.HasFlag(rec) && !addedRecord.MajorFlags.HasFlag(rec)) addedRecord.MajorFlags |= rec;
                                else if (!overrideObject.HasFlag(rec) && addedRecord.MajorFlags.HasFlag(rec)) addedRecord.MajorFlags &= ~rec;
                        }
                    }
                }

                //==============================================================================================================
                // Regions
                //==============================================================================================================
                if (Settings.Regions.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.Regions.Contains(context.ModKey) && ((!context.Record.Regions?.Equals(originalObject.Record.Regions) ?? false)));
                    if (foundContext.Any())
                    {
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<IFormLinkGetter<IRegionGetter>> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<ICell, ICellGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Regions != null)
                            foreach (var rec in patchRecord.Record.Regions)
                                overrideObject.Add(rec);
                        else if (workingContext.Record.Regions != null)
                            foreach (var rec in workingContext.Record.Regions)
                                overrideObject.Add(rec);

                        // Add Records
                        bool Change = false;
                        foreach (var context in foundContext)
                        {
                            if (context.Record.Regions != null && context.Record.Regions.Count > 0)
                                foreach (var rec in context.Record.Regions)
                                    if (originalObject.Record.Regions != null && !originalObject.Record.Regions.Contains(rec) && !overrideObject.Contains(rec))
                                    {
                                        overrideObject.Add(rec);
                                        Change = true;
                                    }
                        }

                        // Remove Records
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Regions != null && context.Record.Regions.Count > 0)
                                if (originalObject.Record.Regions != null && originalObject.Record.Regions.Count > 0 && originalObject.Record.Regions?.Count > 0)
                                    foreach (var rec in originalObject.Record.Regions)
                                        if (!context.Record.Regions.Contains(rec) && overrideObject.Contains(rec))
                                        {
                                            overrideObject.Remove(rec);
                                            Change = true;
                                        }
                        }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Regions?.SetTo(overrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Water
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Owner.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Water.Equals(originalObject.Record.Water)
                        || !foundContext.Record.WaterHeight.Equals(originalObject.Record.WaterHeight)
                        || (!foundContext.Record.WaterNoiseTexture?.Equals(originalObject.Record.WaterNoiseTexture) ?? false)
                        || (!foundContext.Record.WaterEnvironmentMap?.Equals(originalObject.Record.WaterEnvironmentMap) ?? false))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Water.Equals(originalObject.Record.Water)) Change = true;
                        if (!foundContext.Record.WaterHeight.Equals(originalObject.Record.WaterHeight)) Change = true;
                        if (!foundContext.Record.WaterNoiseTexture?.Equals(originalObject.Record.WaterNoiseTexture) ?? false) Change = true;
                        if (!foundContext.Record.WaterEnvironmentMap?.Equals(originalObject.Record.WaterEnvironmentMap) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Water != null) overrideObject.Water.SetTo(foundContext.Record.Water);
                            if (foundContext.Record.WaterHeight != null) overrideObject.WaterHeight = foundContext.Record.WaterHeight;
                            if (foundContext.Record.WaterNoiseTexture != null) overrideObject.WaterNoiseTexture = foundContext.Record.WaterNoiseTexture;
                            if (foundContext.Record.WaterEnvironmentMap != null) overrideObject.WaterEnvironmentMap = foundContext.Record.WaterEnvironmentMap;
                        }
                        break;
                    }
                }

            }
        }        
    }
}
