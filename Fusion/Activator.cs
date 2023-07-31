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
    public class ActivatorSettings
    {
        public List<ModKey> Destructible = new();
        public List<ModKey> Graphics = new();
        public List<ModKey> Keywords = new();
        public List<ModKey> Names = new();
        public List<ModKey> ObjectBounds = new();
        public List<ModKey> Sounds = new();
        public List<ModKey> Text = new();
    }
    internal class ActivatorPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, ActivatorSettings Settings)
        {
            List<ModKey> modList = new() { 
                Settings.Destructible, Settings.Graphics, Settings.Keywords, Settings.Names, Settings.ObjectBounds, Settings.Sounds,
                Settings.Text };
            HashSet<ModKey> workingModList = new(modList);

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
                foreach(var foundContext in modContext.Where(context => Settings.Destructible.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Destructible?.Equals(originalObject.Record.Destructible) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Destructible?.Equals(workingContext.Record.Destructible) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Destructible != null) overrideObject.Destructible?.DeepCopyIn(foundContext.Record.Destructible);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Graphics.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Model?.Equals(originalObject.Record.Model) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Model?.Equals(workingContext.Record.Model) ?? false) Change = true;
                        
                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Model != null) overrideObject.Model?.DeepCopyIn(foundContext.Record.Model);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.Keywords.Count > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => Settings.Keywords.Contains(context.ModKey) && ((!context.Record.Keywords?.Equals(originalObject.Record.Keywords) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<IActivator, IActivatorGetter>(workingContext.Record.FormKey, out var patchRecord);
                        Keywords NewKeywords = new(patchRecord?.Record.Keywords, workingContext.Record.Keywords);
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
                foreach(var foundContext in modContext.Where(context => Settings.Names.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Name?.Equals(originalObject.Record.Name) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Name?.Equals(workingContext.Record.Name) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Name != null) overrideObject.Name?.Set(foundContext.Record.Name.TargetLanguage, foundContext.Record.Name.String);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Object Bounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.ObjectBounds.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.ObjectBounds?.Equals(originalObject.Record.ObjectBounds) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.ObjectBounds?.Equals(workingContext.Record.ObjectBounds) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ObjectBounds != null) overrideObject.ObjectBounds?.DeepCopyIn(foundContext.Record.ObjectBounds);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Sounds
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Sounds.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.ActivationSound.Equals(originalObject.Record.ActivationSound)
                        || !foundContext.Record.LoopingSound.Equals(originalObject.Record.LoopingSound))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.ActivationSound.Equals(workingContext.Record.ActivationSound)) Change = true;
                        if (!foundContext.Record.LoopingSound.Equals(workingContext.Record.LoopingSound)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ActivationSound != null) overrideObject.ActivationSound.SetTo(foundContext.Record.ActivationSound);
                            if (foundContext.Record.LoopingSound != null) overrideObject.LoopingSound.SetTo(foundContext.Record.LoopingSound);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Text
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Text.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.ActivateTextOverride?.Equals(originalObject.Record.ActivateTextOverride) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.ActivateTextOverride?.Equals(workingContext.Record.ActivateTextOverride) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.ActivateTextOverride != null) overrideObject.ActivateTextOverride?.Set(foundContext.Record.ActivateTextOverride.TargetLanguage, foundContext.Record.ActivateTextOverride.String);
                        }
                        break;
                    }
                }
                
            }
        }
    }
}
