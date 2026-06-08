using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using Mutagen.Bethesda.Plugins.Assets;

namespace Fusion
{
    internal class EFSH
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Graphics, Tags.Sound);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IEffectShaderGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Effect Shader");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IEffectShader, IEffectShaderGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IEffectShader? overrideObject = null;
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
                            Compare.NotEqual(fContext.Record.FillTexture,oContext.Record.FillTexture)
                            || Compare.NotEqual(fContext.Record.ParticleShaderTexture,oContext.Record.ParticleShaderTexture)
                            || Compare.NotEqual(fContext.Record.HolesTexture,oContext.Record.HolesTexture)
                            || Compare.NotEqual(fContext.Record.MembranePaletteTexture,oContext.Record.MembranePaletteTexture)
                            || Compare.NotEqual(fContext.Record.ParticlePaletteTexture,oContext.Record.ParticlePaletteTexture)
                            || EffectShaders.DATACheck(fContext.Record, oContext.Record)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChangeNull(fContext.Record.FillTexture,wContext.Record.FillTexture,oContext.Record.FillTexture)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.FillTexture?.SetPath(fContext.Record.FillTexture.DataRelativePath);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.ParticleShaderTexture,wContext.Record.ParticleShaderTexture,oContext.Record.ParticleShaderTexture)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ParticleShaderTexture?.SetPath(fContext.Record.ParticleShaderTexture.DataRelativePath);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.HolesTexture,wContext.Record.HolesTexture,oContext.Record.HolesTexture)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.HolesTexture?.SetPath(fContext.Record.HolesTexture.DataRelativePath);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.MembranePaletteTexture,wContext.Record.MembranePaletteTexture,oContext.Record.MembranePaletteTexture)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.MembranePaletteTexture?.SetPath(fContext.Record.MembranePaletteTexture.DataRelativePath);
                                }

                                if (Utility.ShouldChangeNull(fContext.Record.ParticlePaletteTexture,wContext.Record.ParticlePaletteTexture,oContext.Record.ParticlePaletteTexture)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ParticlePaletteTexture?.SetPath(fContext.Record.ParticlePaletteTexture.DataRelativePath);
                                }

                                if (EffectShaders.DATACheck(fContext.Record, oContext.Record) && EffectShaders.DATACheck(fContext.Record, wContext.Record)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    EffectShaders.DATADeepCopy(overrideObject, fContext.Record); 
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
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.AmbientSound,wContext.Record.AmbientSound,oContext.Record.AmbientSound)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.AmbientSound.SetTo(fContext.Record.AmbientSound);
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
