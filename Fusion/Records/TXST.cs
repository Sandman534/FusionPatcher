using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using Mutagen.Bethesda.Plugins.Assets;

namespace Fusion
{
    internal class TXST
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Graphics, Tags.ObjectBounds);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<ITextureSetGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Texture Set");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<ITextureSet, ITextureSetGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                ITextureSet? overrideObject = null;
                MappedTags mapped = new();
                
                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Graphics, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Diffuse,oContext.Record.Diffuse)
                            || Compare.NotEqual(fContext.Record.NormalOrGloss,oContext.Record.NormalOrGloss)
                            || Compare.NotEqual(fContext.Record.EnvironmentMaskOrSubsurfaceTint,oContext.Record.EnvironmentMaskOrSubsurfaceTint)
                            || Compare.NotEqual(fContext.Record.GlowOrDetailMap,oContext.Record.GlowOrDetailMap)
                            || Compare.NotEqual(fContext.Record.Height,oContext.Record.Height)
                            || Compare.NotEqual(fContext.Record.Environment,oContext.Record.Environment)
                            || Compare.NotEqual(fContext.Record.Multilayer,oContext.Record.Multilayer)
                            || Compare.NotEqual(fContext.Record.BacklightMaskOrSpecular,oContext.Record.BacklightMaskOrSpecular)
                            || Compare.NotEqual(fContext.Record.Decal,oContext.Record.Decal)
                            || Compare.NotEqual(fContext.Record.Flags,oContext.Record.Flags)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Diffuse,wContext.Record.Diffuse,oContext.Record.Diffuse)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Diffuse?.SetPath(fContext.Record.Diffuse?.DataRelativePath);
                                }

                                if (Utility.ShouldChange(fContext.Record.NormalOrGloss,wContext.Record.NormalOrGloss,oContext.Record.NormalOrGloss)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.NormalOrGloss?.SetPath(fContext.Record.NormalOrGloss?.DataRelativePath);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.EnvironmentMaskOrSubsurfaceTint,wContext.Record.EnvironmentMaskOrSubsurfaceTint,oContext.Record.EnvironmentMaskOrSubsurfaceTint)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EnvironmentMaskOrSubsurfaceTint?.SetPath(fContext.Record.EnvironmentMaskOrSubsurfaceTint?.DataRelativePath);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.GlowOrDetailMap,wContext.Record.GlowOrDetailMap,oContext.Record.GlowOrDetailMap)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.GlowOrDetailMap?.SetPath(fContext.Record.GlowOrDetailMap?.DataRelativePath);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.Height,wContext.Record.Height,oContext.Record.Height)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Height?.SetPath(fContext.Record.Height?.DataRelativePath);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.Environment,wContext.Record.Environment,oContext.Record.Environment)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Environment?.SetPath(fContext.Record.Environment?.DataRelativePath);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.Multilayer,wContext.Record.Multilayer,oContext.Record.Multilayer)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Multilayer?.SetPath(fContext.Record.Multilayer?.GivenPath);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.BacklightMaskOrSpecular,wContext.Record.BacklightMaskOrSpecular,oContext.Record.BacklightMaskOrSpecular)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.BacklightMaskOrSpecular?.SetPath(fContext.Record.BacklightMaskOrSpecular?.DataRelativePath);
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.Decal,wContext.Record.Decal,oContext.Record.Decal)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Decal = fContext.Record.Decal?.DeepCopy();
                                }
                                
                                if (Utility.ShouldChange(fContext.Record.Flags,wContext.Record.Flags,oContext.Record.Flags)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Flags = fContext.Record.Flags;
                                }
                            }
                            mapped.SetMapped();
                        }  

                    }

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
                }                
            }
        }
    }
}