using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;

namespace Fusion
{
    internal class CELL
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            Console.WriteLine("Processing Cell");
            HashSet<ModKey> workingModList = Settings.GetModList("C.Acoustic,C.Climate,C.Encounter,C.ImageSpace,C.Light,C.LockList,C.Location,C.MiscFlags" +
                ",C.Music,C.Name,C.Owner,C.RecordFlags,C.Regions,C.SkyLighting,C.Water");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Cell().WinningContextOverrides(state.LinkCache))
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (!modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(workingContext.Record.FormKey).Last();
                MappedTags mapped = new MappedTags();
                Flags<Cell.Flag> NewFlags = new(workingContext.Record.Flags);
                Flags<Cell.MajorFlag> NewMajorFlags = new(workingContext.Record.MajorFlags);
                Regions NewRegions = new(workingContext.Record.Regions);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Acoustic
                    //==============================================================================================================
                    if (mapped.NotMapped("C.Acoustic") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.AcousticSpace,originalObject.Record.AcousticSpace))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.AcousticSpace,workingContext.Record.AcousticSpace)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.AcousticSpace,originalObject.Record.AcousticSpace))
                                        overrideObject.AcousticSpace.SetTo(foundContext.Record.AcousticSpace);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Climate
                    //==============================================================================================================
                    if (mapped.NotMapped("C.Climate") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion,originalObject.Record.SkyAndWeatherFromRegion))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion,workingContext.Record.SkyAndWeatherFromRegion)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion,originalObject.Record.SkyAndWeatherFromRegion))
                                        overrideObject.SkyAndWeatherFromRegion.SetTo(foundContext.Record.SkyAndWeatherFromRegion);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Encounter Zone
                    //==============================================================================================================
                    if (mapped.NotMapped("C.Encounter") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.EncounterZone,originalObject.Record.EncounterZone))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else 
                            {
                                if (Compare.NotEqual(foundContext.Record.EncounterZone,workingContext.Record.EncounterZone)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.EncounterZone,originalObject.Record.EncounterZone))
                                        overrideObject.EncounterZone.SetTo(foundContext.Record.EncounterZone);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Image Space
                    //==============================================================================================================
                    if (mapped.NotMapped("C.ImageSpace") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.ImageSpace,originalObject.Record.ImageSpace))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) 
                                mapped.SetMapped();
                            else 
                            {
                                if (Compare.NotEqual(foundContext.Record.ImageSpace,workingContext.Record.ImageSpace)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.ImageSpace,originalObject.Record.ImageSpace))
                                        overrideObject.ImageSpace.SetTo(foundContext.Record.ImageSpace);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Lighting
                    //==============================================================================================================
                    if (mapped.NotMapped("C.Light") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Lighting,originalObject.Record.Lighting)
                            || Compare.NotEqual(foundContext.Record.LightingTemplate,originalObject.Record.LightingTemplate))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
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
                                mapped.SetMapped();
                            }
                            
                        }
                    }

                    //==============================================================================================================
                    // Lock List
                    //==============================================================================================================
                    if (mapped.NotMapped("C.LockList") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.LockList,originalObject.Record.LockList))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.LockList,workingContext.Record.LockList)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.LockList,originalObject.Record.LockList))
                                        overrideObject.LockList.SetTo(foundContext.Record.LockList);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Location
                    //==============================================================================================================
                    if (mapped.NotMapped("C.Location") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Location,originalObject.Record.Location))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Location,workingContext.Record.Location)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Location,originalObject.Record.Location))
                                        overrideObject.Location.SetTo(foundContext.Record.Location);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Music
                    //==============================================================================================================
                    if (mapped.NotMapped("C.Music") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Music,originalObject.Record.Music))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Music,workingContext.Record.Music)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Music,originalObject.Record.Music))
                                        overrideObject.Music.SetTo(foundContext.Record.Music);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Name
                    //==============================================================================================================
                    if (mapped.NotMapped("C.Name") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else {
                                if (Compare.NotEqual(foundContext.Record.Name,workingContext.Record.Name)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                        overrideObject.Name = Utility.NewString(foundContext.Record.Name);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Owner
                    //==============================================================================================================
                    if (mapped.NotMapped("C.Owner") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Owner,originalObject.Record.Owner))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Owner,workingContext.Record.Owner)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Owner,originalObject.Record.Owner))
                                        overrideObject.Owner.SetTo(foundContext.Record.Owner);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Sky Lighting
                    //==============================================================================================================
                    if (mapped.NotMapped("C.SkyLighting") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion, originalObject.Record.SkyAndWeatherFromRegion))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion, workingContext.Record.SkyAndWeatherFromRegion)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion, originalObject.Record.SkyAndWeatherFromRegion))
                                        overrideObject.SkyAndWeatherFromRegion.SetTo(foundContext.Record.SkyAndWeatherFromRegion);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Water
                    //==============================================================================================================
                    if (mapped.NotMapped("C.Water") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Water,originalObject.Record.Water)
                            || Compare.NotEqual(foundContext.Record.WaterHeight,originalObject.Record.WaterHeight)
                            || Compare.NotEqual(foundContext.Record.WaterNoiseTexture,originalObject.Record.WaterNoiseTexture)
                            || Compare.NotEqual(foundContext.Record.WaterEnvironmentMap,originalObject.Record.WaterEnvironmentMap))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) 
                                mapped.SetMapped();
                            else
                            {
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
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Misc Flag Adds
                    //==============================================================================================================
                    if (Settings.TagList("C.MiscFlags").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Flags,originalObject.Record.Flags))
                            NewFlags.Add(foundContext.Record.Flags, originalObject.Record.Flags);

                    //==============================================================================================================
                    // Record Flag Adds
                    //==============================================================================================================
                    if (Settings.TagList("C.RecordFlags").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.MajorFlags,originalObject.Record.MajorFlags))
                            NewMajorFlags.Add(foundContext.Record.MajorFlags, originalObject.Record.MajorFlags);

                    //==============================================================================================================
                    // Region Adds
                    //==============================================================================================================
                    if (Settings.TagList("C.Regions").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Regions,originalObject.Record.Regions))
                            NewRegions.Add(foundContext.Record.Regions, originalObject.Record.Regions);
                    
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var foundContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Misc Flag Removes
                    //==============================================================================================================
                    if (Settings.TagList("C.MiscFlags").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Flags,originalObject.Record.Flags))
                            NewFlags.Remove(foundContext.Record.Flags, originalObject.Record.Flags);

                    //==============================================================================================================
                    // Record Flag Removes
                    //==============================================================================================================
                    if (Settings.TagList("C.RecordFlags").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.MajorFlags,originalObject.Record.MajorFlags))
                            NewMajorFlags.Remove(foundContext.Record.MajorFlags, originalObject.Record.MajorFlags);

                    //==============================================================================================================
                    // Region Removes
                    //==============================================================================================================
                    if (Settings.TagList("C.Regions").Contains(foundContext.ModKey))
                        if (Compare.NotEqual(foundContext.Record.Regions,originalObject.Record.Regions))
                            NewRegions.Remove(foundContext.Record.Regions, originalObject.Record.Regions);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewFlags.Modified) 
                {
                    var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Flags = NewFlags.OverrideObject;
                }

                if (NewFlags.Modified)
                {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.MajorFlags = NewMajorFlags.OverrideObject;
                }

                if (NewRegions.Modified)
                {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Regions = NewRegions.OverrideObject;
                }

            }
        }        
    }
}
