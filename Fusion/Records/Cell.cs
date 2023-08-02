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
    internal class CellPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("C.Acoustic,C.Climate,C.Encounter,C.ImageSpace,C.Light,C.LockList,C.Location,C.MiscFlags" +
                ",C.Music,C.Name,C.Owner,C.RecordFlags,C.Regions,C.Water");
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.Acoustic").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.Climate").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.Encounter").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.ImageSpace").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.Light").Contains(context.ModKey)))
                {
                    if ((!foundContext.Record.Lighting?.Equals(originalObject.Record.Lighting) ?? false)
                        || !foundContext.Record.LightingTemplate.Equals(originalObject.Record.LightingTemplate))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Lighting?.Equals(workingContext.Record.Lighting) ?? false) Change = true;
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.LockList").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.Location").Contains(context.ModKey)))
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
                if (Settings.TagCount("C.MiscFlags", out var FoundKeys) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys.Contains(context.ModKey) && (!context.Record.Flags.Equals(originalObject.Record.Flags)));
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.Music").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.Name").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.Owner").Contains(context.ModKey)))
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
                if (Settings.TagCount("C.RecordFlags", out var FoundKeys1) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys1.Contains(context.ModKey) && (!context.Record.MajorFlags.Equals(originalObject.Record.MajorFlags)));
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
                if (Settings.TagCount("C.Regions", out var FoundKeys2) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys2.Contains(context.ModKey) && ((!context.Record.Regions?.Equals(originalObject.Record.Regions) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<ICell, ICellGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Regions NewRegions = new(patchRecord?.Record.Regions, workingContext.Record.Regions);
                        foreach (var context in foundContext)
                            NewRegions.Add(context.Record.Regions, originalObject.Record.Regions);
                        foreach (var context in foundContext.Reverse())
                            NewRegions.Remove(context.Record.Regions, originalObject.Record.Regions);
                        if (NewRegions.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Regions?.SetTo(NewRegions.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Water
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("C.Water").Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Water.Equals(originalObject.Record.Water)
                        || !foundContext.Record.WaterHeight.Equals(originalObject.Record.WaterHeight)
                        || (!foundContext.Record.WaterNoiseTexture?.Equals(originalObject.Record.WaterNoiseTexture) ?? false)
                        || (!foundContext.Record.WaterEnvironmentMap?.Equals(originalObject.Record.WaterEnvironmentMap) ?? false))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Water.Equals(workingContext.Record.Water)) Change = true;
                        if (!foundContext.Record.WaterHeight.Equals(workingContext.Record.WaterHeight)) Change = true;
                        if (!foundContext.Record.WaterNoiseTexture?.Equals(workingContext.Record.WaterNoiseTexture) ?? false) Change = true;
                        if (!foundContext.Record.WaterEnvironmentMap?.Equals(workingContext.Record.WaterEnvironmentMap) ?? false) Change = true;

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
