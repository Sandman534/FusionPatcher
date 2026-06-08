using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;

namespace Fusion
{
    internal class PROJ
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            // Get the working mod lists
            HashSet<ModKey> workingModList = Settings.GetModList(Tags.Destructible, Tags.Graphics, Tags.Names, Tags.ObjectBounds, Tags.Sound, Tags.Stats);
            HashSet<FormKey> affectedFormKeys = Utility.GetAffectedFormKeys<IProjectileGetter>(state, workingModList);
            Utility.RecordCountMessage(affectedFormKeys.Count, "Projectile");

            // Loop through the 
            foreach (var formKey in affectedFormKeys)
            {
                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                // Get all the contexts, and leave if there is none
                var allContexts = state.LinkCache.ResolveAllContexts<IProjectile, IProjectileGetter>(formKey).ToList();
                if (allContexts.Count < 2) continue;

                // Get the last context, as well as the mods context
                var wContext = allContexts[0];
                var oContext = allContexts[^1];
                var modContext = allContexts.Where(x => workingModList.Contains(x.ModKey));

                // Tracking Tags
                IProjectile? overrideObject = null;
                MappedTags mapped = new();

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var fContext in modContext)
                {
                    //==============================================================================================================
                    // Destructible
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Destructible, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.Destructible,oContext.Record.Destructible)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Destructible,wContext.Record.Destructible,oContext.Record.Destructible)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Destructible = fContext.Record.Destructible?.DeepCopy();
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
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.Model,wContext.Record.Model,oContext.Record.Model)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Model = fContext.Record.Model?.DeepCopy();
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
                            Compare.NotEqual(fContext.Record.SoundLevel,oContext.Record.SoundLevel)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.SoundLevel,wContext.Record.SoundLevel,oContext.Record.SoundLevel)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.SoundLevel = fContext.Record.SoundLevel;
                                }
                            }
                            mapped.SetMapped();
                        }

                    }

                    //==============================================================================================================
                    // Stats
                    //==============================================================================================================
                    if (Utility.TagCheck(Tags.Stats, mapped, Settings, fContext))
                    {
                        if (
                            Compare.NotEqual(fContext.Record.EditorID,oContext.Record.EditorID)
                            || Compare.NotEqual(fContext.Record.Speed,oContext.Record.Speed)
                            || Compare.NotEqual(fContext.Record.Gravity,oContext.Record.Gravity)
                            || Compare.NotEqual(fContext.Record.Range,oContext.Record.Range)
                            || Compare.NotEqual(fContext.Record.Type,oContext.Record.Type)
                            || Compare.NotEqual(fContext.Record.FadeDuration,oContext.Record.FadeDuration)
                            || Compare.NotEqual(fContext.Record.ImpactForce,oContext.Record.ImpactForce)
                            || Compare.NotEqual(fContext.Record.CollisionRadius,oContext.Record.CollisionRadius)
                            || Compare.NotEqual(fContext.Record.Lifetime,oContext.Record.Lifetime)
                        ){
                            if (Utility.CheckContext(fContext, wContext, oContext)) {
                                if (Utility.ShouldChange(fContext.Record.EditorID,wContext.Record.EditorID,oContext.Record.EditorID)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.EditorID = fContext.Record.EditorID;
                                }

                                if (Utility.ShouldChange(fContext.Record.Speed,wContext.Record.Speed,oContext.Record.Speed)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Speed = fContext.Record.Speed;
                                }

                                if (Utility.ShouldChange(fContext.Record.Gravity,wContext.Record.Gravity,oContext.Record.Gravity)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Gravity = fContext.Record.Gravity;
                                }

                                if (Utility.ShouldChange(fContext.Record.Range,wContext.Record.Range,oContext.Record.Range)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Range = fContext.Record.Range;
                                }

                                if (Utility.ShouldChange(fContext.Record.Type,wContext.Record.Type,oContext.Record.Type)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Type = fContext.Record.Type;
                                }

                                if (Utility.ShouldChange(fContext.Record.FadeDuration,wContext.Record.FadeDuration,oContext.Record.FadeDuration)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.FadeDuration = fContext.Record.FadeDuration;
                                }

                                if (Utility.ShouldChange(fContext.Record.ImpactForce,wContext.Record.ImpactForce,oContext.Record.ImpactForce)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.ImpactForce = fContext.Record.ImpactForce;
                                }

                                if (Utility.ShouldChange(fContext.Record.CollisionRadius,wContext.Record.CollisionRadius,oContext.Record.CollisionRadius)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.CollisionRadius = fContext.Record.CollisionRadius;
                                }

                                if (Utility.ShouldChange(fContext.Record.Lifetime,wContext.Record.Lifetime,oContext.Record.Lifetime)) {
                                    overrideObject ??= wContext.GetOrAddAsOverride(state.PatchMod);
                                    overrideObject.Lifetime = fContext.Record.Lifetime;
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
