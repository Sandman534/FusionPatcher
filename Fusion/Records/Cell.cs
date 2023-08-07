using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Strings;
using Noggog;

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
                foreach(var foundContext in modContext.Where(context => Settings.TagList("C.Acoustic").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.AcousticSpace,originalObject.Record.AcousticSpace))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.AcousticSpace,workingContext.Record.AcousticSpace)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.AcousticSpace,originalObject.Record.AcousticSpace))
                                overrideObject.AcousticSpace.SetTo(foundContext.Record.AcousticSpace);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Climate
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("C.Climate").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion,originalObject.Record.SkyAndWeatherFromRegion))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion,workingContext.Record.SkyAndWeatherFromRegion)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion,originalObject.Record.SkyAndWeatherFromRegion))
                                overrideObject.SkyAndWeatherFromRegion.SetTo(foundContext.Record.SkyAndWeatherFromRegion);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Encounter Zone
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("C.Encounter").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.EncounterZone,originalObject.Record.EncounterZone))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.EncounterZone,workingContext.Record.EncounterZone)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.EncounterZone,originalObject.Record.EncounterZone))
                                overrideObject.EncounterZone.SetTo(foundContext.Record.EncounterZone);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Image Space
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("C.ImageSpace").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.ImageSpace,originalObject.Record.ImageSpace))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.ImageSpace,workingContext.Record.ImageSpace)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.ImageSpace,originalObject.Record.ImageSpace))
                                overrideObject.ImageSpace.SetTo(foundContext.Record.ImageSpace);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Lighting
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("C.Light").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Lighting,originalObject.Record.Lighting)
                        || Compare.NotEqual(foundContext.Record.LightingTemplate,originalObject.Record.LightingTemplate))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Lighting,workingContext.Record.Lighting)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.LightingTemplate,workingContext.Record.LightingTemplate)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Lighting,originalObject.Record.Lighting))
                                overrideObject.Lighting = foundContext.Record.Lighting?.DeepCopy();
                            if (Compare.NotEqual(foundContext.Record.LightingTemplate,originalObject.Record.LightingTemplate))
                                overrideObject.LightingTemplate.SetTo(foundContext.Record.LightingTemplate);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Lock List
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("C.LockList").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.LockList,originalObject.Record.LockList))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.LockList,workingContext.Record.LockList)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.LockList,originalObject.Record.LockList))
                                overrideObject.LockList.SetTo(foundContext.Record.LockList);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Location
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("C.Location").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Location,originalObject.Record.Location))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Location,workingContext.Record.Location)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Location,originalObject.Record.Location))
                                overrideObject.Location.SetTo(foundContext.Record.Location);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Misc Flags
                //==============================================================================================================
                if (Settings.HasTags("C.MiscFlags", out var MisFlags))
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => MisFlags.Contains(context.ModKey) && Compare.NotEqual(context.Record.Flags,originalObject.Record.Flags));
                    if (foundContext.Any())
                    {
                        Flags<Cell.Flag> NewFlags = new(workingContext.Record.Flags);
                        foreach (var context in foundContext)
                            NewFlags.Add(context.Record.Flags, originalObject.Record.Flags);
                        foreach (var context in foundContext.Reverse())
                            NewFlags.Remove(context.Record.Flags, originalObject.Record.Flags);
                        if (NewFlags.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Flags = NewFlags.OverrideObject;
                        }
                    }
                }

                //==============================================================================================================
                // Music
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("C.Music").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Music,originalObject.Record.Music))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Music,workingContext.Record.Music)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Music,originalObject.Record.Music))
                                overrideObject.Music.SetTo(foundContext.Record.Music);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Name
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Names").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Name,workingContext.Record.Name)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                overrideObject.Name = Utility.NewString(foundContext.Record.Name);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Owner
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("C.Owner").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Owner,originalObject.Record.Owner))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Owner,workingContext.Record.Owner)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Owner,originalObject.Record.Owner))
                                overrideObject.Owner.SetTo(foundContext.Record.Owner);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Record Flags
                //==============================================================================================================
                if (Settings.HasTags("C.RecordFlags", out var RecordFlags))
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => RecordFlags.Contains(context.ModKey) && Compare.NotEqual(context.Record.MajorFlags,originalObject.Record.MajorFlags));
                    if (foundContext.Any())
                    {
                        Flags<Cell.MajorFlag> NewFlags = new(workingContext.Record.MajorFlags);
                        foreach (var context in foundContext)
                            NewFlags.Add(context.Record.MajorFlags, originalObject.Record.MajorFlags);
                        foreach (var context in foundContext.Reverse())
                            NewFlags.Remove(context.Record.MajorFlags, originalObject.Record.MajorFlags);
                        if (NewFlags.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.MajorFlags = NewFlags.OverrideObject;
                        }
                    }
                }

                //==============================================================================================================
                // Regions
                //==============================================================================================================
                if (Settings.HasTags("C.Regions", out var Regions))
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Regions.Contains(context.ModKey) && Compare.NotEqual(context.Record.Regions,originalObject.Record.Regions));
                    if (foundContext.Any())
                    {
                        Regions NewRegions = new(workingContext.Record.Regions);
                        foreach (var context in foundContext)
                            NewRegions.Add(context.Record.Regions, originalObject.Record.Regions);
                        foreach (var context in foundContext.Reverse())
                            NewRegions.Remove(context.Record.Regions, originalObject.Record.Regions);
                        if (NewRegions.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Regions = NewRegions.OverrideObject;
                        }
                    }
                }

                //==============================================================================================================
                // Water
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("C.Water").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Water,originalObject.Record.Water)
                        || Compare.NotEqual(foundContext.Record.WaterHeight,originalObject.Record.WaterHeight)
                        || Compare.NotEqual(foundContext.Record.WaterNoiseTexture,originalObject.Record.WaterNoiseTexture)
                        || Compare.NotEqual(foundContext.Record.WaterEnvironmentMap,originalObject.Record.WaterEnvironmentMap))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Water,workingContext.Record.Water)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.WaterHeight,workingContext.Record.WaterHeight)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.WaterNoiseTexture,workingContext.Record.WaterNoiseTexture)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.WaterEnvironmentMap,workingContext.Record.WaterEnvironmentMap)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (Compare.NotEqual(foundContext.Record.Water,originalObject.Record.Water))
                                overrideObject.Water.SetTo(foundContext.Record.Water);
                            if (Compare.NotEqual(foundContext.Record.WaterHeight,originalObject.Record.WaterHeight))
                                overrideObject.WaterHeight = foundContext.Record.WaterHeight;
                            if (Compare.NotEqual(foundContext.Record.WaterNoiseTexture,originalObject.Record.WaterNoiseTexture))
                                overrideObject.WaterNoiseTexture = foundContext.Record.WaterNoiseTexture;
                            if (Compare.NotEqual(foundContext.Record.WaterEnvironmentMap,originalObject.Record.WaterEnvironmentMap))
                                overrideObject.WaterEnvironmentMap = foundContext.Record.WaterEnvironmentMap;
                        }
                        break;
                    }
                }

            }
        }        
    }
}
