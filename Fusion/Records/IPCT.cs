using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class IPCT
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Graphics, Tags.Sound);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IImpactGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Effect Shader");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IImpact, IImpactGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IImpact? overrideObject = null;
                MappedTags mapped = new();

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach (var fContext in modContext)
                {
                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Graphics, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Model,oContext.Record.Model)
                            || Compare.NotEqual(fContext.Record.Duration,oContext.Record.Duration)
                            || Compare.NotEqual(fContext.Record.Orientation,oContext.Record.Orientation)
                            || Compare.NotEqual(fContext.Record.AngleThreshold,oContext.Record.AngleThreshold)
                            || Compare.NotEqual(fContext.Record.PlacementRadius,oContext.Record.PlacementRadius)
                            || Compare.NotEqual(fContext.Record.SkyrimMajorRecordFlags,oContext.Record.SkyrimMajorRecordFlags)
                            || Compare.NotEqual(fContext.Record.Result,oContext.Record.Result)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChangeNull(fContext.Record.Model,wContext.Record.Model,oContext.Record.Model)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Model = fContext.Record.Model?.DeepCopy();
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.Duration,wContext.Record.Duration,oContext.Record.Duration)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Duration = fContext.Record.Duration;
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.Orientation,wContext.Record.Orientation,oContext.Record.Orientation)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Orientation = fContext.Record.Orientation;
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.AngleThreshold,wContext.Record.AngleThreshold,oContext.Record.AngleThreshold)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AngleThreshold = fContext.Record.AngleThreshold;
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.PlacementRadius,wContext.Record.PlacementRadius,oContext.Record.PlacementRadius)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Duration = fContext.Record.PlacementRadius;
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.SkyrimMajorRecordFlags,wContext.Record.SkyrimMajorRecordFlags,oContext.Record.SkyrimMajorRecordFlags)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SkyrimMajorRecordFlags = fContext.Record.SkyrimMajorRecordFlags;
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.Result,wContext.Record.Result,oContext.Record.Result)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Result = fContext.Record.Result;
                                }
                            }
                            mapped.SetMapped();
                        }

                    }

                    //==============================================================================================================
                    // Sounds
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Sound, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.SoundLevel,oContext.Record.SoundLevel)
                            || Compare.NotEqual(fContext.Record.Sound1,oContext.Record.Sound1)
                            || Compare.NotEqual(fContext.Record.Sound2,oContext.Record.Sound2)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.SoundLevel,wContext.Record.SoundLevel,oContext.Record.SoundLevel)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SoundLevel = fContext.Record.SoundLevel;
                                }

                                if (Utility.ShouldChange(fContext.Record.Sound1,wContext.Record.Sound1,oContext.Record.Sound1)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Sound1.SetTo(fContext.Record.Sound1);
                                }

                                if (Utility.ShouldChange(fContext.Record.Sound2,wContext.Record.Sound2,oContext.Record.Sound2)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Sound2.SetTo(fContext.Record.Sound2);
                                }
                            }
                            mapped.SetMapped();
                        }

                    }
                }
            }
        }
    }
}
