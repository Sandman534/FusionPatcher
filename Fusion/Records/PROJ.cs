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
    internal class PROJ
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            Console.WriteLine("Processing Projectile");
            HashSet<ModKey> workingModList = Settings.GetModList("Destructible,Graphics,Names,ObjectBounds,Sound,Stats");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Projectile().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IProjectile, IProjectileGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                //==============================================================================================================
                // Initial Settings
                //==============================================================================================================
                var originalObject = state.LinkCache.ResolveAllContexts<IProjectile, IProjectileGetter>(workingContext.Record.FormKey).Last();
                MappedTags mapped = new MappedTags();

                //==============================================================================================================
                // Mod Lookup
                //==============================================================================================================
                foreach(var foundContext in modContext)
                {
                    //==============================================================================================================
                    // Destructible
                    //==============================================================================================================
                    if (mapped.NotMapped("Destructible") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Destructible,workingContext.Record.Destructible)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                                        overrideObject.Destructible = foundContext.Record.Destructible?.DeepCopy();
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Graphics
                    //==============================================================================================================
                    if (mapped.NotMapped("Graphics") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Model,workingContext.Record.Model)) Change = true;
                                
                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                                        overrideObject.Model = foundContext.Record.Model?.DeepCopy();
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Names
                    //==============================================================================================================
                    if (mapped.NotMapped("Names") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.Name, workingContext.Record.Name)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.Name, originalObject.Record.Name))
                                        overrideObject.Name = Utility.NewString(foundContext.Record.Name);
                                }
                            }
                        }
                    }

                    //==============================================================================================================
                    // Object Bounds
                    //==============================================================================================================
                    if (mapped.NotMapped("ObjectBounds") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.ObjectBounds,workingContext.Record.ObjectBounds)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (foundContext.Record.ObjectBounds != null && Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                        overrideObject.ObjectBounds.DeepCopyIn(foundContext.Record.ObjectBounds);
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Sounds
                    //==============================================================================================================
                    if (mapped.NotMapped("Sound") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.SoundLevel,originalObject.Record.SoundLevel))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else {
                                if (Compare.NotEqual(foundContext.Record.SoundLevel, workingContext.Record.SoundLevel)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.SoundLevel, originalObject.Record.SoundLevel))
                                        overrideObject.SoundLevel = foundContext.Record.SoundLevel;
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                    //==============================================================================================================
                    // Stats
                    //==============================================================================================================
                    if (mapped.NotMapped("Stats") && Settings.TagList(mapped.GetTag()).Contains(foundContext.ModKey))
                    {
                        if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID)
                            || Compare.NotEqual(foundContext.Record.Speed,originalObject.Record.Speed)
                            || Compare.NotEqual(foundContext.Record.Gravity,originalObject.Record.Gravity)
                            || Compare.NotEqual(foundContext.Record.Range,originalObject.Record.Range)
                            || Compare.NotEqual(foundContext.Record.Type, originalObject.Record.Type)
                            || Compare.NotEqual(foundContext.Record.FadeDuration, originalObject.Record.FadeDuration)
                            || Compare.NotEqual(foundContext.Record.ImpactForce, originalObject.Record.ImpactForce)
                            || Compare.NotEqual(foundContext.Record.CollisionRadius, originalObject.Record.CollisionRadius)
                            || Compare.NotEqual(foundContext.Record.Lifetime, originalObject.Record.Lifetime))
                        {
                            // Checks
                            bool Change = false;
                            if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey)
                                mapped.SetMapped();
                            else
                            {
                                if (Compare.NotEqual(foundContext.Record.EditorID,workingContext.Record.EditorID)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Speed, originalObject.Record.Speed)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Gravity, originalObject.Record.Gravity)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Range, originalObject.Record.Range)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Type, originalObject.Record.Type)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.FadeDuration, originalObject.Record.FadeDuration)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.ImpactForce, originalObject.Record.ImpactForce)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.CollisionRadius, originalObject.Record.CollisionRadius)) Change = true;
                                if (Compare.NotEqual(foundContext.Record.Lifetime, originalObject.Record.Lifetime)) Change = true;

                                // Copy Records
                                if (Change)
                                {
                                    var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                                    if (Compare.NotEqual(foundContext.Record.EditorID,originalObject.Record.EditorID))
                                        overrideObject.EditorID = foundContext.Record.EditorID;
                                    if (Compare.NotEqual(foundContext.Record.Speed, originalObject.Record.Speed))
                                        overrideObject.Speed = foundContext.Record.Speed;
                                    if (Compare.NotEqual(foundContext.Record.Gravity, originalObject.Record.Gravity))
                                        overrideObject.Gravity = foundContext.Record.Gravity;
                                    if (Compare.NotEqual(foundContext.Record.Range, originalObject.Record.Range))
                                        overrideObject.Range = foundContext.Record.Range;
                                    if (Compare.NotEqual(foundContext.Record.Type, originalObject.Record.Type))
                                        overrideObject.Type = foundContext.Record.Type;
                                    if (Compare.NotEqual(foundContext.Record.FadeDuration, originalObject.Record.FadeDuration))
                                        overrideObject.FadeDuration = foundContext.Record.FadeDuration;
                                    if (Compare.NotEqual(foundContext.Record.ImpactForce, originalObject.Record.ImpactForce))
                                        overrideObject.ImpactForce = foundContext.Record.ImpactForce;
                                    if (Compare.NotEqual(foundContext.Record.CollisionRadius, originalObject.Record.CollisionRadius))
                                        overrideObject.CollisionRadius = foundContext.Record.CollisionRadius;
                                    if (Compare.NotEqual(foundContext.Record.Lifetime, originalObject.Record.Lifetime))
                                        overrideObject.Lifetime = foundContext.Record.Lifetime;
                                }
                                mapped.SetMapped();
                            }
                        }
                    }

                }   
            }
        }
    }
}
