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
    internal class BookPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, SettingsUtility Settings)
        {
            HashSet<ModKey> workingModList = Settings.GetModList(@"Destructible,Graphics,Keywords,Names,ObjectBounds,Sounds,Stats,Text");
            foreach (var workingContext in state.LoadOrder.PriorityOrder.Book().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IBook, IBookGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                var originalObject = state.LinkCache.ResolveAllContexts<IBook, IBookGetter>(workingContext.Record.FormKey).Last();
                
                //==============================================================================================================
                // Destructible
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Destructible").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Graphics").Contains(context.ModKey)))
                {
                    if ((!foundContext.Record.Model?.Equals(originalObject.Record.Model) ?? false)
                        || !foundContext.Record.InventoryArt.Equals(originalObject.Record.InventoryArt)
                        || (!foundContext.Record.Icons?.Equals(originalObject.Record.Icons) ?? false))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Model?.Equals(workingContext.Record.Model) ?? false) Change = true;
                        if (!foundContext.Record.InventoryArt.Equals(workingContext.Record.InventoryArt)) Change = true;
                        if (!foundContext.Record.Icons?.Equals(workingContext.Record.Icons) ?? false) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Model != null) overrideObject.Model?.DeepCopyIn(foundContext.Record.Model);
                            if (foundContext.Record.InventoryArt != null) overrideObject.InventoryArt.SetTo(foundContext.Record.InventoryArt);
                            if (foundContext.Record.Icons != null) overrideObject.Icons?.DeepCopyIn(foundContext.Record.Icons);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Keywords
                //==============================================================================================================
                if (Settings.TagCount("Keywords", out var FoundKeys) > 0)
                {
                    // Get the last overriding context of our element
                    var foundContext = modContext.Where(context => FoundKeys.Contains(context.ModKey) && ((!context.Record.Keywords?.Equals(originalObject.Record.Keywords) ?? false)));
                    if (foundContext.Any())
                    {
                        state.LinkCache.TryResolveContext<IBook, IBookGetter>(workingContext.Record.FormKey, out var patchRecord);
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Names").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("ObjectBounds").Contains(context.ModKey)))
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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Sounds").Contains(context.ModKey)))
                {
                    if (!foundContext.Record.PickUpSound.Equals(originalObject.Record.PickUpSound)
                        || !foundContext.Record.PutDownSound.Equals(originalObject.Record.PutDownSound))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.PickUpSound.Equals(workingContext.Record.PickUpSound)) Change = true;
                        if (!foundContext.Record.PutDownSound.Equals(workingContext.Record.PutDownSound)) Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.PickUpSound != null) overrideObject.PickUpSound.SetTo(foundContext.Record.PickUpSound);
                            if (foundContext.Record.PutDownSound != null) overrideObject.PutDownSound.SetTo(foundContext.Record.PutDownSound);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Stats
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Text").Contains(context.ModKey)))
                {
                    if ((!foundContext.Record.EditorID?.Equals(originalObject.Record.EditorID) ?? false)
                        || !foundContext.Record.Value.Equals(originalObject.Record.Value)
                        || !foundContext.Record.Weight.Equals(originalObject.Record.Weight))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.EditorID?.Equals(workingContext.Record.EditorID) ?? false) Change = true;
                        if (!foundContext.Record.Value.Equals(workingContext.Record.Value))  Change = true;
                        if (!foundContext.Record.Weight.Equals(workingContext.Record.Weight))  Change = true;

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
                foreach(var foundContext in modContext.Where(context => Settings.HasTag("Text").Contains(context.ModKey)))
                {
                    if ((!foundContext.Record.Description?.Equals(originalObject.Record.Description) ?? false)
                        || !foundContext.Record.BookText.Equals(originalObject.Record.BookText))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Description?.Equals(workingContext.Record.Description) ?? false) Change = true;
                        if (!foundContext.Record.BookText.Equals(workingContext.Record.BookText))  Change = true;

                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            if (foundContext.Record.Description != null) overrideObject.Description?.Set(foundContext.Record.Description.TargetLanguage, foundContext.Record.Description.String);
                            if (foundContext.Record.BookText != null) overrideObject.BookText?.Set(foundContext.Record.BookText.TargetLanguage, foundContext.Record.BookText.String);
                        }
                        break;
                    }
                }
                
            }
        }
    }
}