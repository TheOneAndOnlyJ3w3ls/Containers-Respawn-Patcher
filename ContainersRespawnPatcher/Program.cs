using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using Noggog;


namespace ContainersRespawnPatcher
{
	public class Program
    {
        /*private static readonly HashSet<FormLink<IContainerGetter>> BaseContainerGetters = new()
        {
            Skyrim.Container.BarrelFish01,
            Skyrim.Container.BarrelFood01,
            Skyrim.Container.BarrelFood01_Snow,
            Skyrim.Container.BarrelIngredientCommon01,
            Skyrim.Container.BarrelIngredientCommon01_Snow,
            Skyrim.Container.BarrelIngredientUncommon01,
            Skyrim.Container.BarrelIngredientUncommon01_Snow,
            Skyrim.Container.BarrelMeat01,
            Skyrim.Container.BeeHive,
            Skyrim.Container.BeeHiveVacant,
            Skyrim.Container.BlackBriarMeadBarrel01,
            Skyrim.Container.CommonCoffin01,
            Skyrim.Container.CommonWardrobe01,
            Skyrim.Container.CompanionsKodlakNightTable01,
            Skyrim.Container.Cupboard01,
            Skyrim.Container.DesecratedImperial,
            Skyrim.Container.DesecratedStormCloak,
            Skyrim.Container.Dresser01,
            Skyrim.Container.DweDresser01,
            Skyrim.Container.EndTable01,
            Skyrim.Container.HonningbrewMeadBarrel01,
            Skyrim.Container.MammothContainer01,
            Skyrim.Container.MarkarthBurialUrn,
            Skyrim.Container.MarkarthCoffin01container,
            Skyrim.Container.MarkarthCoffin02container,
            Skyrim.Container.MeadBarrel02,
            Skyrim.Container.MiscSack02Large,
            Skyrim.Container.MiscSack02LargeFlat,
            Skyrim.Container.MiscSack02Small,
            Skyrim.Container.MiscSack02SmallFlat,
            Skyrim.Container.MiscSackLarge,
            Skyrim.Container.MiscSackLargeFlat01,
            Skyrim.Container.MiscSackLargeFlat02,
            Skyrim.Container.MiscSackLargeFlat03,
            Skyrim.Container.MiscSackSmall,
            Skyrim.Container.NobleChest01,
            Skyrim.Container.NobleChestDrawers01,
            Skyrim.Container.NobleChestDrawers02,
            Skyrim.Container.NobleChestDrawers02NoName,
            Skyrim.Container.NobleCupboard01,
            Skyrim.Container.NobleCupboard02,
            Skyrim.Container.NobleNightTable01,
            Skyrim.Container.NobleWardrobe01,
            Skyrim.Container.OrcDresser01,
            Skyrim.Container.OrcEndTable01,
            Skyrim.Container.PersonalChestSmall,
            Skyrim.Container.PlayerBookShelfContainer,
            Skyrim.Container.PlayerHouseChest,
            Skyrim.Container.PlayerPotionRackContainer,
            Skyrim.Container.PlayerWerewolfStorage,
            Skyrim.Container.RTCoffin01,
            Skyrim.Container.SafewithLock,
            Skyrim.Container.SBurialUrn01,
            Skyrim.Container.SCoffin01,
            Skyrim.Container.SCoffinPoor01,
            Skyrim.Container.SkyHavenArmoryChest,
            Skyrim.Container.SovBarrel01,
            Skyrim.Container.SpitPotClosed01,
            Skyrim.Container.SpitPotClosed01AlchemyCommon,
            Skyrim.Container.SpitPotClosed02,
            Skyrim.Container.SpitPotClosedLoose01,
            Skyrim.Container.SpitPotClosedLoose01AlchemyCommon,
            Skyrim.Container.StrongBox,
            Skyrim.Container.UnownedChest,
            Skyrim.Container.UpperCupboard01,
            Skyrim.Container.UpperDresser01,
            Skyrim.Container.UpperEndTable01,
            Skyrim.Container.UpperEndTable02,
            Skyrim.Container.UpperWardrobe01,
            Skyrim.Container.VendorMiscChestSmall,
            Skyrim.Container.WE19BanditLootChest,
            Skyrim.Container.WHcoffin01,
            Skyrim.Container.WinhelmBurialUrn,
            Skyrim.Container.WinterholdBookCase01,
            Skyrim.Container.wispCorpseContainer,
            Skyrim.Container.WRBurialUrn01,
            Skyrim.Container.WRCoffin01,
            Skyrim.Container.WRinteractiveBookshelfContainer
        };*/

        internal static List<Container> containersRespawn = new();
        internal static List<Container> containersNoRespawn = new();

        internal static List<string> containersRespawnEID = new();
        internal static List<string> containersNoRespawnEID = new();

        /*internal static Dictionary<IFormLinkGetter<ISkyrimMajorRecordGetter>, Container> allRespawnContainers = new();
        internal static Dictionary<IFormLinkGetter<ISkyrimMajorRecordGetter>, Container> allNoRespawnContainers = new();*/

        public static Lazy<Settings> _settings = null!;
        public static Settings Settings => _settings.Value;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings("Settings", "settings.json", out _settings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynthesisContainers.esp")
                .Run(args);
        }


        public static void CreateNewContainers(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            System.Console.WriteLine("Creating new 'No Respawn' containers!");

            // Counter
            int nbCont = 0;

            foreach (var containerGetter in state.LoadOrder.PriorityOrder.WinningOverrides<IContainerGetter>())
            {
                // Skip null container EditorID
                if (containerGetter.EditorID is null) continue;

                // If the EditorID of the container is found in the settings
                if (Settings.SafeContainersSettings.ContainerEditorIDs.Contains(containerGetter.EditorID))
                {
                    if(Settings.debug)
                        System.Console.WriteLine("Container found: " + containerGetter.EditorID);

                    // Check if the container already exists
                    state.LinkCache.TryResolve<IContainer>(containerGetter.EditorID + "_NoRespawn", out var existing);
                    if (existing is not null)
                    {
                        if (Settings.debug)
                            System.Console.WriteLine("   > Container _NoRespawn already exists: " + existing?.EditorID);

                        continue;
                    }

                    // Duplicate the record
                    Container contNew = state.PatchMod.Containers.DuplicateInAsNewRecord<Container, IContainerGetter>(containerGetter);

                    // Get the current record
                    var contOld = state.PatchMod.Containers.GetOrAddAsOverride(containerGetter);

                    // Skip null
                    if (contOld.EditorID is null || contNew.EditorID is null) continue;

                    // If the container has a flag already, duplicate it and add no respawn
                    if (containerGetter.Flags.HasFlag(Container.Flag.Respawns))
                    {
                        // Name the new container NORESPAWN & remove the respawn flag 
                        contNew.EditorID = containerGetter.EditorID + "_NoRespawn";
                        contNew.Flags.SetFlag(Container.Flag.Respawns, false);

                        if (Settings.debug)
                            System.Console.WriteLine("   > Created new container: " + contNew.EditorID);

                        //Count
                        nbCont++;
                    }
                    // The container does not have the flag yet
                    else
                    {
                        // Duplicate the record, add the NORESPAWN text to the copy
                        contNew!.EditorID = contNew.EditorID + "_NoRespawn";

                        // Add the flag to the original container
                        contOld.Flags |= Container.Flag.Respawns;

                        if (Settings.debug)
                        {
                            System.Console.WriteLine("   > Created new container: " + contNew.EditorID);
                            System.Console.WriteLine("   > Added Respawn flag to container: " + contOld.EditorID);
                        }
                        
                        // Count
                        nbCont++;
                    }

                    // Add the containers to the lists
                    containersRespawn.Add(contOld);
                    containersNoRespawn.Add(contNew);

                    // Add the editor ID to the lists (for convenience)
                    containersRespawnEID.Add(contOld.EditorID);
                    containersNoRespawnEID.Add(contNew.EditorID);
                }
                else
                {
                    // Do nothing
                    //System.Console.WriteLine("Container not found: " + containerGetter.EditorID);
                }
            }

            System.Console.WriteLine("Created " + nbCont + " new containers!");
        }



        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            // Create a link cache
            ILinkCache cache = state.LinkCache;

            // Counter
            int nbContTotal = 0;

            /*
             * Swap the placed container in that cell with a No Respawn version
             * 
             */
            void DoContainerSwap(IModContext<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter> placed, IModContext<ICellGetter> cell)
            {
                // Find the base record
                placed.Record.Base.TryResolve(cache, out var baseObject);
                if (baseObject is null || baseObject.EditorID is null) return;

                // If the containers have this editor ID, it is a container
                if (containersRespawnEID.Contains(baseObject.EditorID))
                {
                    if (Settings.debug)
                        System.Console.WriteLine("Swapping object:" + baseObject.EditorID + " in cell: " + cell.Record.EditorID);

                    // Swap the container base
                    var placedCopy = placed.GetOrAddAsOverride(state.PatchMod);
                    placedCopy.Base.SetTo(containersNoRespawn[containersRespawnEID.IndexOf(baseObject.EditorID)]);

                    nbContTotal++;

                    // Set the parent object
                    var parent = (ICellGetter?)placed.Parent?.Record;

                    // Handle ownership properly
                    if (parent?.Ownership is null) return;
                    if (placedCopy.Ownership is null) return;

                    placedCopy.Ownership.Owner = parent.Ownership.Owner.AsNullable();
                }
                // Container already flagged as "No Respawn"
                else if (containersNoRespawnEID.Contains(baseObject.EditorID))
                {
                    // Nothing to do!
                }
                // Not a container
                else
                {
                    return;
                }
            }

            System.Console.WriteLine("Doing settings checks...");

            if(Settings.CellsNotRespawningSettings.CellNoRespawnEditorIDs.Count == 0)
            {
                System.Console.WriteLine("WARNING: NO SAFE PLAYER HOME CONTAINERS SET");
            }
            if(Settings.SafeContainersSettings.ContainerEditorIDs.Count == 0)
            {
                System.Console.WriteLine("ERROR: NO CONTAINERS SET, ABORTING! THIS WILL CAUSE MAJOR ISSUES IN YOUR GAME!");
                throw new Exception("Invalid settings!");
            }

            System.Console.WriteLine("Settings seem valid, starting!");
            
            // Create the No Respawn containers
            CreateNewContainers(state);

            // Check all placed objects
            foreach (var placed in state.LoadOrder.PriorityOrder.PlacedObject().WinningContextOverrides(state.LinkCache))
            {
                // Get parent cell
                placed.TryGetParentSimpleContext<ICellGetter>(out var cell);

                // Ignore null
                if (cell is null || cell.Record is null || cell.Record.EditorID is null) continue;
                if (placed is null || cell.Record is null || cell.Record.EditorID is null) continue;

                // If the placed object is in the "No respawn" locations
                if (Settings.CellsNotRespawningSettings.CellNoRespawnEditorIDs.Contains(cell.Record.EditorID))
                {
                    DoContainerSwap(placed, cell);
                }
                else
                {
                    // Cell is owned by the Player
                    if (cell.Record?.Ownership?.Owner is not null && cell.Record.Ownership.Owner.FormKey.IDString() == "00000DB1")
                    {
                        DoContainerSwap(placed, cell);
                    }
                }
            }


            System.Console.WriteLine("Swapped " + nbContTotal + " containers for a safe No Respawn one!");




            /*foreach (ICellGetter? cellContext in state.LoadOrder.PriorityOrder.WinningOverrides<ICellGetter>())
            {
                 if (cellContext is null || cellContext.EditorID is null) continue;

                // If it is one of the "player homes" or zones with no respawn
                if (NoRespawnLocations.Contains(cellContext.EditorID))
                {
                    var tempObjects = cellContext.Temporary;
                    if (tempObjects is null) continue;


                    //if (!cellContext.TryResolveContext<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(state.LinkCache, out var winningContainerContext)) continue;

                    //tempObjects.;

                   // var query = cellContext.Temporary.SelectWhere(x => x.); //.Select(obj => obj.FormKey);
                    *//*foreach (var f in query)
                    {
                        allNoRespawnContainers.TryGetValue(f, out var found);
                    }*//*

                    

                    System.Console.WriteLine("Editing cell: " + cellContext.EditorID);
                    foreach (IPlacedGetter obj in cellContext.Temporary)
                    {
                        if (obj is null || obj is null ) continue;

                        //if(obj.EnableParent.GetType())
                        //System.Console.WriteLine("      " + obj.FormKey);

                        //var cont = state.PatchMod.Cells.GetOrAddAsOverride(cellContext);
                        //obj.FormKey

                        //allNoRespawnContainers.TryGetValue(obj, out var found);


                        //obj.ContainedFormLinks.

                        //state.PatchMod.Containers.GetOrAddAsOverride(obj);
                        //Container v = obj.AsLink().TryResolve<Container>(cache);
                        //if (v is null) continue;


                        //if (obj.Base == )
                        //;

                        *//*Crates.Select(x =>
                        {
                            List<Container> toAdd = AddContainers(state, x);
                            return (x, toAdd);
                        }).ForEach(tuple => crateContainers.Add(tuple.x, tuple.toAdd));*//*

                    }
                }
            }*/



            System.Console.WriteLine("All done!");
        }
    }
}
