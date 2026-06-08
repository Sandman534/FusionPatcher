using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class EXPL
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Enchantments, Tags.Graphics, Tags.Keywords, Tags.Names, Tags.ObjectBounds, Tags.Sound);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IExplosionGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Explosion");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IExplosion, IExplosionGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IExplosion? overrideObject = null;
                MappedTags mapped = new();

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Enchantments
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Enchantments, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.ObjectEffect,oContext.Record.ObjectEffect)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.ObjectEffect,wContext.Record.ObjectEffect,oContext.Record.ObjectEffect)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ObjectEffect.SetTo(fContext.Record.ObjectEffect);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Graphics, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Model,oContext.Record.Model)
                            || Compare.NotEqual(fContext.Record.ImageSpaceModifier,oContext.Record.ImageSpaceModifier)
                            || Compare.NotEqual(fContext.Record.Light,oContext.Record.Light)
                            || Compare.NotEqual(fContext.Record.ImpactDataSet,oContext.Record.ImpactDataSet)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChangeNull(fContext.Record.Model,wContext.Record.Model,oContext.Record.Model)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Model = fContext.Record.Model?.DeepCopy();
                                }

                                if (Utility.ShouldChange(fContext.Record.ImageSpaceModifier,wContext.Record.ImageSpaceModifier,oContext.Record.ImageSpaceModifier)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ImageSpaceModifier.SetTo(fContext.Record.ImageSpaceModifier);
                                }

                                if (Utility.ShouldChange(fContext.Record.Light,wContext.Record.Light,oContext.Record.Light)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Light.SetTo(fContext.Record.Light);
                                }

                                if (Utility.ShouldChange(fContext.Record.ImpactDataSet,wContext.Record.ImpactDataSet,oContext.Record.ImpactDataSet)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ImpactDataSet.SetTo(fContext.Record.ImpactDataSet);
                                }
                            }
                            mapped.SetMapped();
                        }
                    }

                    //==============================================================================================================
                    // Names
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Names, mapped, Settings, fContext))
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
                    // Sounds
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Sound, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Sound1,oContext.Record.Sound1)
                            || Compare.NotEqual(fContext.Record.Sound2,oContext.Record.Sound2)
                            || Compare.NotEqual(fContext.Record.SoundLevel,oContext.Record.SoundLevel)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Sound1,wContext.Record.Sound1,oContext.Record.Sound1)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Sound1.SetTo(fContext.Record.Sound1);
                                }

                                if (Utility.ShouldChange(fContext.Record.Sound2,wContext.Record.Sound2,oContext.Record.Sound2)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Sound2.SetTo(fContext.Record.Sound2);
                                }

                                if (Utility.ShouldChange(fContext.Record.SoundLevel,wContext.Record.SoundLevel,oContext.Record.SoundLevel)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SoundLevel = fContext.Record.SoundLevel;
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
