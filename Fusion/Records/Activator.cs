using DynamicData;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System.Collections.Immutable;

namespace Fusion
{
    internal class ActivatorPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList("Destructible,Graphics,Keywords,Names,ObjectBounds,Sounds,Text");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Activator().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IActivator, IActivatorGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IActivator, IActivatorGetter>(workingContext.Record.FormKey).Last();

                //==============================================================================================================
                // Destructible
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Destructible").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Destructible,workingContext.Record.Destructible)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Destructible != null && Compare.NotEqual(foundContext.Record.Destructible,originalObject.Record.Destructible))
                                overrideObject.Destructible?.DeepCopyIn(foundContext.Record.Destructible);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Graphics").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Model,workingContext.Record.Model)) Change = true;
                        
                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Model != null && Compare.NotEqual(foundContext.Record.Model,originalObject.Record.Model))
                                overrideObject.Model?.DeepCopyIn(foundContext.Record.Model);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.HasTags("Keywords", out var FoundKeys))
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys.Contains(context.ModKey) && Compare.NotEqual(context.Record.Keywords,originalObject.Record.Keywords));
                    if (foundContext.Any())
                    {
                        Keywords NewKeywords = new(workingContext.Record.Keywords);
                        foreach (var context in foundContext)
                            NewKeywords.Add(context.Record.Keywords, originalObject.Record.Keywords);
                        foreach (var context in foundContext.Reverse())
                            NewKeywords.Remove(context.Record.Keywords, originalObject.Record.Keywords);
                        if (NewKeywords.Modified) {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Keywords?.SetTo(NewKeywords.OverrideObject);
                        }
                    }
                }

                //==============================================================================================================
                // Names
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Names").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.Name,workingContext.Record.Name)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Name != null && Compare.NotEqual(foundContext.Record.Name,originalObject.Record.Name))
                                overrideObject.Name?.Set(foundContext.Record.Name.TargetLanguage, foundContext.Record.Name.String);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Object Bounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("ObjectBounds").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.ObjectBounds,workingContext.Record.ObjectBounds)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ObjectBounds != null && Compare.NotEqual(foundContext.Record.ObjectBounds,originalObject.Record.ObjectBounds))
                                overrideObject.ObjectBounds?.DeepCopyIn(foundContext.Record.ObjectBounds);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Sounds").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.ActivationSound,originalObject.Record.ActivationSound)
                        || Compare.NotEqual(foundContext.Record.LoopingSound,originalObject.Record.LoopingSound))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.ActivationSound,workingContext.Record.ActivationSound)) Change = true;
                        if (Compare.NotEqual(foundContext.Record.LoopingSound,workingContext.Record.LoopingSound)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ActivationSound != null && Compare.NotEqual(foundContext.Record.ActivationSound,originalObject.Record.ActivationSound))
                                overrideObject.ActivationSound.SetTo(foundContext.Record.ActivationSound);
                            if (foundContext.Record.LoopingSound != null && Compare.NotEqual(foundContext.Record.LoopingSound,originalObject.Record.LoopingSound)) 
                                overrideObject.LoopingSound.SetTo(foundContext.Record.LoopingSound);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Text
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.TagList("Text").Contains(context.ModKey)))
                {
                    if (Compare.NotEqual(foundContext.Record.ActivateTextOverride,originalObject.Record.ActivateTextOverride))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (Compare.NotEqual(foundContext.Record.ActivateTextOverride,workingContext.Record.ActivateTextOverride)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ActivateTextOverride != null && Compare.NotEqual(foundContext.Record.ActivateTextOverride,originalObject.Record.ActivateTextOverride))
                                overrideObject.ActivateTextOverride?.Set(foundContext.Record.ActivateTextOverride.TargetLanguage, foundContext.Record.ActivateTextOverride.String);
                        }
                        break;
                    }
                }
                
            }
        }
    }
}
