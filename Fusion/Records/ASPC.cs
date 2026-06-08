using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class ASPC
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.ObjectBounds, Tags.Sound);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IAcousticSpaceGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Acoustic Space");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IAcousticSpace, IAcousticSpaceGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var oContext = allContexts[^1];
                var wContext = allContexts[0];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IAcousticSpace? overrideObject = null;
                MappedTags mapped = new();

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Object Bounds
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.ObjectBounds, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.ObjectBounds,oContext.Record.ObjectBounds)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.ObjectBounds,wContext.Record.ObjectBounds,oContext.Record.ObjectBounds)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ObjectBounds.DeepCopyIn(fContext.Record.ObjectBounds);
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
                            Compare.NotEqual(fContext.Record.AmbientSound,oContext.Record.AmbientSound)
                            || Compare.NotEqual(fContext.Record.UseSoundFromRegion,oContext.Record.UseSoundFromRegion)
                            || Compare.NotEqual(fContext.Record.EnvironmentType,oContext.Record.EnvironmentType)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.AmbientSound,wContext.Record.AmbientSound,oContext.Record.AmbientSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AmbientSound.SetTo(fContext.Record.AmbientSound);
                                }

                                if (Utility.ShouldChange(fContext.Record.UseSoundFromRegion,wContext.Record.UseSoundFromRegion,oContext.Record.UseSoundFromRegion)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.UseSoundFromRegion.SetTo(fContext.Record.UseSoundFromRegion);
                                }

                                if (Utility.ShouldChange(fContext.Record.EnvironmentType,wContext.Record.EnvironmentType,oContext.Record.EnvironmentType)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EnvironmentType.SetTo(fContext.Record.EnvironmentType);
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
