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
    public class IngestibleSettings
    {
        public List<ModKey> Destructible = new();
        public List<ModKey> Graphics = new();
        public List<ModKey> Keywords = new();
        public List<ModKey> Names = new();
        public List<ModKey> ObjectBounds = new();
        public List<ModKey> Sounds = new();
        public List<ModKey> Stats = new();
        public List<ModKey> Text = new();
    }

    internal class IngestiblePatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IngestibleSettings Settings)
        {
            List<ModKey> modList = new() {
                Settings.Destructible, Settings.Graphics, Settings.Keywords, Settings.Names, Settings.ObjectBounds, Settings.Sounds, Settings.Stats, Settings.Text };
            HashSet<ModKey> workingModList = new(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Ingestible().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IIngestible, IIngestibleGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IIngestible, IIngestibleGetter>(workingContext.Record.FormKey).Last();

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
                    if ((!foundContext.Record.Model?.Equals(originalObject.Record.Model) ?? false)
                        || (!foundContext.Record.Icons?.Equals(originalObject.Record.Icons) ?? false))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Model?.Equals(originalObject.Record.Model) ?? false) Change = true;
                        if (!foundContext.Record.Icons?.Equals(originalObject.Record.Icons) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Model != null) overrideObject.Model?.DeepCopyIn(foundContext.Record.Model);
                            if (foundContext.Record.Icons != null) overrideObject.Icons?.DeepCopyIn(foundContext.Record.Icons);
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
                        // Create list and fill it with Last Record or Patch Record
                        ExtendedList<IFormLinkGetter<IKeywordGetter>> overrideObject = new();
                        if (state.LinkCache.TryResolveContext<IIngestible, IIngestibleGetter>(workingContext.Record.FormKey, out var patchRecord) && patchRecord.Record.Keywords != null)
                            foreach (var rec in patchRecord.Record.Keywords)
                                overrideObject.Add(rec);
                        else if (workingContext.Record.Keywords != null)
                            foreach (var rec in workingContext.Record.Keywords)
                                overrideObject.Add(rec);

                        // Add Records
                        bool Change = false;
                        foreach (var context in foundContext)
                        {
                            if (context.Record.Keywords != null && context.Record.Keywords.Count > 0)
                                foreach (var rec in context.Record.Keywords)
                                    if (originalObject.Record.Keywords != null && !originalObject.Record.Keywords.Contains(rec) && !overrideObject.Contains(rec))
                                    {
                                        overrideObject.Add(rec);
                                        Change = true;
                                    }
                        }

                        // Remove Records
                        foreach (var context in foundContext.Reverse())
                        {
                            if (context.Record.Keywords != null && context.Record.Keywords.Count > 0)
                                if (originalObject.Record.Keywords != null && originalObject.Record.Keywords.Count > 0 && originalObject.Record.Keywords?.Count > 0)
                                    foreach (var rec in originalObject.Record.Keywords)
                                        if (!context.Record.Keywords.Contains(rec) && overrideObject.Contains(rec))
                                        {
                                            overrideObject.Remove(rec);
                                            Change = true;
                                        }
                        }

                        // If changes were made, override and write back
                        if (Change)
                        {
                            var addedRecord = workingContext.GetOrAddAsOverride(state.PatchMod);
                            addedRecord.Keywords?.SetTo(overrideObject);
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
                    if (!foundContext.Record.PickUpSound.Equals(originalObject.Record.PickUpSound)
                        || !foundContext.Record.PutDownSound.Equals(originalObject.Record.PutDownSound)
                        || !foundContext.Record.ConsumeSound.Equals(originalObject.Record.ConsumeSound))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.PickUpSound.Equals(workingContext.Record.PickUpSound)) Change = true;
                        if (!foundContext.Record.PutDownSound.Equals(workingContext.Record.PutDownSound)) Change = true;
                        if (!foundContext.Record.ConsumeSound.Equals(workingContext.Record.ConsumeSound)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.PickUpSound != null) overrideObject.PickUpSound.SetTo(foundContext.Record.PickUpSound);
                            if (foundContext.Record.PutDownSound != null) overrideObject.PutDownSound.SetTo(foundContext.Record.PutDownSound);
                            if (foundContext.Record.ConsumeSound != null) overrideObject.ConsumeSound.SetTo(foundContext.Record.ConsumeSound);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Stats
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Stats.Contains(context.ModKey)))
                {
                    if ((!foundContext.Record.EditorID?.Equals(originalObject.Record.EditorID) ?? false)
                        || !foundContext.Record.Value.Equals(originalObject.Record.Value)
                        || !foundContext.Record.Weight.Equals(originalObject.Record.Weight))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.EditorID?.Equals(originalObject.Record.EditorID) ?? false) Change = true;
                        if (!foundContext.Record.Value.Equals(originalObject.Record.Value)) Change = true;
                        if (!foundContext.Record.Weight.Equals(originalObject.Record.Weight)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.EditorID != null) overrideObject.EditorID = foundContext.Record.EditorID;
                            if (overrideObject.Value != foundContext.Record.Value) overrideObject.Value = foundContext.Record.Value;
                            if (overrideObject.Weight != foundContext.Record.Weight) overrideObject.Weight = foundContext.Record.Weight;
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Text
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Text.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Description?.Equals(originalObject.Record.Description) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Description?.Equals(workingContext.Record.Description) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Description != null) overrideObject.Description?.Set(foundContext.Record.Description.TargetLanguage, foundContext.Record.Description.String);
                        }
                        break;
                    }
                }

            }
        }
    }
}
