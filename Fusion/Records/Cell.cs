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
    internal class CellPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
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
                bool[] mapped = new bool[20];
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
                    if (Settings.TagList("C.Acoustic").Contains(foundContext.ModKey) && !mapped[0]) 
                    {
                        if (Compare.NotEqual(foundContext.Record.AcousticSpace,originalObject.Record.AcousticSpace))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[0] = true;
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
                                mapped[0] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Climate
                    //==============================================================================================================
                    if (Settings.TagList("C.Climate").Contains(foundContext.ModKey) && !mapped[1])
                    {
                        if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion,originalObject.Record.SkyAndWeatherFromRegion))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[1] = true;
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
                                mapped[1] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Encounter Zone
                    //==============================================================================================================
                    if (Settings.TagList("C.Encounter").Contains(foundContext.ModKey) && !mapped[2])
                    {
                        if (Compare.NotEqual(foundContext.Record.EncounterZone,originalObject.Record.EncounterZone))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[2] = true;
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
                                mapped[2] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Image Space
                    //==============================================================================================================
                    if (Settings.TagList("C.ImageSpace").Contains(foundContext.ModKey) && !mapped[3]) 
                    {
                        if (Compare.NotEqual(foundContext.Record.ImageSpace,originalObject.Record.ImageSpace))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) 
                                mapped[3] = true;
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
                                mapped[3] = true;
                            }
                        }
                    }
                
                    //==============================================================================================================
                    // Lighting
                    //==============================================================================================================
                    if (Settings.TagList("C.Light").Contains(foundContext.ModKey) && !mapped[4]) 
                    {
                        if (Compare.NotEqual(foundContext.Record.Lighting,originalObject.Record.Lighting)
                            || Compare.NotEqual(foundContext.Record.LightingTemplate,originalObject.Record.LightingTemplate))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[4] = true;
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
                                mapped[4] = true;
                            }
                            
                        }
                    }

                    //==============================================================================================================
                    // Lock List
                    //==============================================================================================================
                    if (Settings.TagList("C.LockList").Contains(foundContext.ModKey) && !mapped[5])
                    {
                        if (Compare.NotEqual(foundContext.Record.LockList,originalObject.Record.LockList))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[5] = true;
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
                                mapped[5] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Location
                    //==============================================================================================================
                    if (Settings.TagList("C.Location").Contains(foundContext.ModKey) && !mapped[6])
                    {
                        if (Compare.NotEqual(foundContext.Record.Location,originalObject.Record.Location))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[6] = true;
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
                                mapped[6] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Music
                    //==============================================================================================================
                    if (Settings.TagList("C.Music").Contains(foundContext.ModKey) && !mapped[7])
                    {
                        if (Compare.NotEqual(foundContext.Record.Music,originalObject.Record.Music))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[7] = true;
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
                                mapped[7] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Name
                    //==============================================================================================================
                    if (Settings.TagList("C.Name").Contains(foundContext.ModKey) && !mapped[8])
                    {
                        if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[8] = true;
                            else {
                                if (Compare.NotEqual(foundContext.Record.Name,workingContext.Record.Name)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                        overrideObject.Name = Utility.NewString(foundContext.Record.Name);
                                }
                                mapped[8] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Owner
                    //==============================================================================================================
                    if (Settings.TagList("C.Owner").Contains(foundContext.ModKey) && !mapped[9])
                    {
                        if (Compare.NotEqual(foundContext.Record.Owner,originalObject.Record.Owner))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[9] = true;
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
                                mapped[9] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Sky Lighting
                    //==============================================================================================================
                    if (Settings.TagList("C.SkyLighting").Contains(foundContext.ModKey) && !mapped[10])
                    {
                        if (Compare.NotEqual(foundContext.Record.SkyAndWeatherFromRegion, originalObject.Record.SkyAndWeatherFromRegion))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped[9] = true;
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
                                mapped[10] = true;
                            }
                        }
                    }

                    //==============================================================================================================
                    // Water
                    //==============================================================================================================
                    if (Settings.TagList("C.Water").Contains(foundContext.ModKey) && !mapped[11])
                    {
                        if (Compare.NotEqual(foundContext.Record.Water,originalObject.Record.Water)
                            || Compare.NotEqual(foundContext.Record.WaterHeight,originalObject.Record.WaterHeight)
                            || Compare.NotEqual(foundContext.Record.WaterNoiseTexture,originalObject.Record.WaterNoiseTexture)
                            || Compare.NotEqual(foundContext.Record.WaterEnvironmentMap,originalObject.Record.WaterEnvironmentMap))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) 
                                mapped[10] = true;
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
                                mapped[11] = true;
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
