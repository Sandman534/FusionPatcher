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
    public class PerkSettings
    {
        public List<ModKey> Graphics = new();
        public List<ModKey> Names = new();
        public List<ModKey> Text = new();
        
    }

    internal class PerkPatcher
    {
        public static void Patch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, PerkSettings Settings)
        {
            List<ModKey> modList = new() { 
                Settings.Graphics, Settings.Names, Settings.Text };
            HashSet<ModKey> workingModList = new(modList);

            foreach (var workingContext in state.LoadOrder.PriorityOrder.Perk().WinningContextOverrides())
            {
                // Skip record if its not in one of our overwrite mods
                var modContext = state.LinkCache.ResolveAllContexts<IPerk, IPerkGetter>(workingContext.Record.FormKey).Where(context => workingModList.Contains(context.ModKey));
                if (modContext == null || !modContext.Any()) continue;

                // Get the base record
                var originalObject = state.LinkCache.ResolveAllContexts<IPerk, IPerkGetter>(workingContext.Record.FormKey).Last();
                
                //==============================================================================================================
                // Graphics
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Graphics.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Icons?.Equals(originalObject.Record.Icons) ?? false)
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (!foundContext.Record.Icons?.Equals(workingContext.Record.Icons) ?? false) Change = true;
                        
                        // Copy Records
                        if (Change)
                        {
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            var lastObject = foundContext.Record;
                            if (lastObject.Icons != null) overrideObject.Icons?.DeepCopyIn(lastObject.Icons);
                        }
                        break;
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
                            var lastObject = foundContext.Record;
                            if (lastObject.Name != null) overrideObject.Name?.Set(lastObject.Name.TargetLanguage, lastObject.Name.String);
                        }
                        break;
                    }
                }

                //==============================================================================================================
                // Description
                //==============================================================================================================
                foreach(var foundContext in modContext.Where(context => Settings.Text.Contains(context.ModKey)))
                {
                    if (!foundContext.Record.Description.Equals(originalObject.Record.Description))
                    {
                        // Checks
                        bool Change = false;
                        if (foundContext.ModKey == workingContext.ModKey || foundContext.ModKey == originalObject.ModKey) break;
                        if (foundContext.Record.Description.Equals(workingContext.Record.Description)) Change = true;

                        // Copy Records
                        if (Change)
                        {   
                            var overrideObject = workingContext.GetOrAddAsOverride(state.PatchMod);
                            var lastObject = foundContext.Record;
                            overrideObject.Description.Set(lastObject.Description.TargetLanguage, lastObject.Description.String);
                        }
                        break;
                    }
                }

            }
        }
    }
}
