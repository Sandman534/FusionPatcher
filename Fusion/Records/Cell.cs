using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class CELL
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.C_Acoustic, Tags.C_Climate, Tags.C_Encounter, Tags.C_ImageSpace, Tags.C_Light,
                Tags.C_LockList, Tags.C_Location, Tags.C_MiscFlags, Tags.C_Music, Tags.C_Name, Tags.C_Owner, Tags.C_RecordFlags, Tags.C_Regions,
                Tags.C_SkyLighting, Tags.C_Water);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<ICellGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Cell");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<ICell, ICellGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                ICell? overrideObject = null;
                MappedTags mapped = new();
                Flags<Cell.Flag> NewFlags = new(wContext.Record.Flags);
                Flags<Cell.MajorFlag> NewMajorFlags = new(wContext.Record.MajorFlags);
                Regions NewRegions = new(wContext.Record.Regions);

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Acoustic
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_Acoustic, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.AcousticSpace,oContext.Record.AcousticSpace)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.AcousticSpace,wContext.Record.AcousticSpace,oContext.Record.AcousticSpace)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AcousticSpace.SetTo(fContext.Record.AcousticSpace);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Climate
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_Climate, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.SkyAndWeatherFromRegion,oContext.Record.SkyAndWeatherFromRegion)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.SkyAndWeatherFromRegion,wContext.Record.SkyAndWeatherFromRegion,oContext.Record.SkyAndWeatherFromRegion)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkyAndWeatherFromRegion.SetTo(fContext.Record.SkyAndWeatherFromRegion);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Encounter Zone
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_Encounter, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.EncounterZone,oContext.Record.EncounterZone)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.EncounterZone,wContext.Record.EncounterZone,oContext.Record.EncounterZone)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EncounterZone.SetTo(fContext.Record.EncounterZone);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Image Space
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_ImageSpace, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.ImageSpace,oContext.Record.ImageSpace)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.ImageSpace,wContext.Record.ImageSpace,oContext.Record.ImageSpace)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ImageSpace.SetTo(fContext.Record.ImageSpace);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Lighting
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_Light, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Lighting,oContext.Record.Lighting)
                            || Compare.NotEqual(fContext.Record.LightingTemplate,oContext.Record.LightingTemplate)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Lighting,wContext.Record.Lighting,oContext.Record.Lighting)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Lighting = fContext.Record.Lighting?.DeepCopy();
                                }

                                if (Utility.ShouldChange(fContext.Record.LightingTemplate,wContext.Record.LightingTemplate,oContext.Record.LightingTemplate)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.LightingTemplate.SetTo(fContext.Record.LightingTemplate);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Lock List
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_LockList, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.LockList,oContext.Record.LockList)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.LockList,wContext.Record.LockList,oContext.Record.LockList)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.LockList.SetTo(fContext.Record.LockList);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Location
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_Location, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Location,oContext.Record.Location)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Location,wContext.Record.Location,oContext.Record.Location)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Location.SetTo(fContext.Record.Location);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Music
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_Music, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Music,oContext.Record.Music)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Music,wContext.Record.Music,oContext.Record.Music)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Music.SetTo(fContext.Record.Music);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Name
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_Name, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Name,oContext.Record.Name)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Name,wContext.Record.Name,oContext.Record.Name)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Name = Utility.NewString(fContext.Record.Name);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Owner
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_Owner, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Owner,oContext.Record.Owner)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Owner,wContext.Record.Owner,oContext.Record.Owner)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Owner.SetTo(fContext.Record.Owner);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Sky Lighting
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_SkyLighting, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.SkyAndWeatherFromRegion,oContext.Record.SkyAndWeatherFromRegion)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.SkyAndWeatherFromRegion,wContext.Record.SkyAndWeatherFromRegion,oContext.Record.SkyAndWeatherFromRegion)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkyAndWeatherFromRegion.SetTo(fContext.Record.SkyAndWeatherFromRegion);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Water
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.C_Water, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Water,oContext.Record.Water)
                            || Compare.NotEqual(fContext.Record.WaterHeight,oContext.Record.WaterHeight)
                            || Compare.NotEqual(fContext.Record.WaterNoiseTexture,oContext.Record.WaterNoiseTexture)
                            || Compare.NotEqual(fContext.Record.WaterEnvironmentMap,oContext.Record.WaterEnvironmentMap)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Water,wContext.Record.Water,oContext.Record.Water)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Water.SetTo(fContext.Record.Water);
                                }

                                if (Utility.ShouldChange(fContext.Record.WaterHeight,wContext.Record.WaterHeight,oContext.Record.WaterHeight)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.WaterHeight = fContext.Record.WaterHeight;
                                }

                                if (Utility.ShouldChange(fContext.Record.WaterNoiseTexture,wContext.Record.WaterNoiseTexture,oContext.Record.WaterNoiseTexture)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.WaterNoiseTexture = fContext.Record.WaterNoiseTexture;
                                }

                                if (Utility.ShouldChange(fContext.Record.WaterEnvironmentMap,wContext.Record.WaterEnvironmentMap,oContext.Record.WaterEnvironmentMap)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.WaterEnvironmentMap = fContext.Record.WaterEnvironmentMap;
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Misc Flag Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.C_MiscFlags).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Flags,oContext.Record.Flags))
                            NewFlags.Add(fContext.Record.Flags, oContext.Record.Flags);

                    //==============================================================================================================
                    // Record Flag Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.C_RecordFlags).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.MajorFlags,oContext.Record.MajorFlags))
                            NewMajorFlags.Add(fContext.Record.MajorFlags, oContext.Record.MajorFlags);

                    //==============================================================================================================
                    // Region Adds
                    //==============================================================================================================
                    if (Settings.TagList(Tags.C_Regions).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Regions,oContext.Record.Regions))
                            NewRegions.Add(fContext.Record.Regions, oContext.Record.Regions);
                    
                }

                //==============================================================================================================
                // Reverse Mod Lookup (Removes)
                //==============================================================================================================
                foreach(var fContext in modContext.Reverse())
                {
                    //==============================================================================================================
                    // Misc Flag Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.C_MiscFlags).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Flags,oContext.Record.Flags))
                            NewFlags.Remove(fContext.Record.Flags, oContext.Record.Flags);

                    //==============================================================================================================
                    // Record Flag Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.C_RecordFlags).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.MajorFlags,oContext.Record.MajorFlags))
                            NewMajorFlags.Remove(fContext.Record.MajorFlags, oContext.Record.MajorFlags);

                    //==============================================================================================================
                    // Region Removes
                    //==============================================================================================================
                    if (Settings.TagList(Tags.C_Regions).Contains(fContext.ModKey))
                        if (Compare.NotEqual(fContext.Record.Regions,oContext.Record.Regions))
                            NewRegions.Remove(fContext.Record.Regions, oContext.Record.Regions);
                }

                //==============================================================================================================
                // Finalize
                //==============================================================================================================
                if (NewFlags.Modified) 
                {
                    var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                    addedRecord.Flags = NewFlags.OverrideObject;
                }

                if (NewMajorFlags.Modified)
                {
                            var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.MajorFlags = NewMajorFlags.OverrideObject;
                }

                if (NewRegions.Modified)
                {
                            var addedRecord = wContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Regions = NewRegions.OverrideObject;
                }

            }
        }        
    }
}
