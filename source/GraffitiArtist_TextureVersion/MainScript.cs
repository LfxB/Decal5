using GTA; // This is a reference that is needed! do not edit this
using GTA.Native; // This is a reference that is needed! do not edit this
using GTA.Math;
using System; // This is a reference that is needed! do not edit this
using System.Windows.Forms; // This is a reference that is needed! do not edit this
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using GTA.UI;
using GTAMath;
using SimpleUI;
using ScriptCommunicatorHelper;
using GTAVFunctions;
using ModSettings;

namespace GraffitiArtist
{
    public class GraffitiArtistScript : Script // declare Modname as a script
    {
        ScriptCommunicator ScriptComm = new ScriptCommunicator("GraffitiArtist");
        
        bool menuInit;

        string drawMenuSearchbarString = "";

        string savedOutfitSearchbarString = "";

        public GraffitiArtistScript() // main function
        {
            ScriptComm.CreateSCModFile(@"scripts\GraffitiArtist.scmod", "Graffiti Script", "There are no mistakes, only happy accidents.");
            GraffitiMethods.InitDecalTypes();
            GraffitiMethods.LoadAddonGraffitiFromFile(@"scripts\Graffiti Mod\" + GraffitiMethods.TextureListFilename);
            GraffitiMethods.LoadWorldGraffitiFromFile();
            //GraffitiMethods.LoadAllVehicleGraffiiFromFile();
            GraffitiMethods.LoadAllVehicleOutfitsFromFolder();

            Tick += OnTick;
            KeyDown += OnKeyDown;
            KeyUp += OnKeyUp;
            Aborted += OnAbort;

            Interval = 0;
        }

        private void OnAbort(object sender, EventArgs e)
        {
            GTAFunction.ClearAllHelpMessages();

            ScriptComm.UnblockScriptCommunicatorModMenu();

            if (GraffitiMethods.AllGraffitiInMap != null && GraffitiMethods.AllGraffitiInMap.Count > 0)
            {
                foreach (var graf in GraffitiMethods.AllGraffitiInMap)
                {
                    if (graf.Exists())
                    {
                        graf.Remove();
                    }
                }
            }

            if (GraffitiMethods.LoadedVehicleOutfits != null && GraffitiMethods.LoadedVehicleOutfits.Count > 0)
            {
                foreach (var outfit in GraffitiMethods.LoadedVehicleOutfits)
                {
                    foreach (var decal in outfit.DecalList)
                    {
                        if (decal.Exists())
                        {
                            decal.Remove();
                        }
                    }
                }
            }
        }

        bool MissingTextureExistsInMapOrOutfit()
        {
            bool textureMissing = false;
            if (GraffitiMethods.AllGraffitiInMap != null && GraffitiMethods.AllGraffitiInMap.Count > 0)
            {
                foreach (var graf in GraffitiMethods.AllGraffitiInMap.ToList())
                {
                    if (!GraffitiMethods.AllAddonGraffiti.Any(ag => ag.TextureName == graf.Graffiti.TextureName && ag.TextureDictionary == graf.Graffiti.TextureDictionary))
                    {
                        textureMissing = true;
                    }
                }
            }

            if (GraffitiMethods.AllVehicleGraffitiOutfits != null && GraffitiMethods.AllVehicleGraffitiOutfits.Count > 0)
            {
                foreach (var outfit in GraffitiMethods.AllVehicleGraffitiOutfits.ToList())
                {
                    foreach (var decal in outfit.DecalList.ToList())
                    {
                        if (!GraffitiMethods.AllAddonGraffiti.Any(ag => ag.TextureName == decal.Graffiti.TextureName && ag.TextureDictionary == decal.Graffiti.TextureDictionary))
                        {
                            textureMissing = true;
                        }
                    }
                }
            }

            return textureMissing;
        }

        void RemoveMissingTextures() // Removes graffiti and decals that do not exist in the addon graffiti list.
        {
            bool worldGrafMissing = false;
            //bool outfitDecalMissing = false;
            if (GraffitiMethods.AllGraffitiInMap != null && GraffitiMethods.AllGraffitiInMap.Count > 0)
            {
                foreach (var graf in GraffitiMethods.AllGraffitiInMap.ToList())
                {
                    if (!GraffitiMethods.AllAddonGraffiti.Any(ag => ag.TextureName == graf.Graffiti.TextureName && ag.TextureDictionary == graf.Graffiti.TextureDictionary))
                    {
                        GraffitiMethods.AllGraffitiInMap.Remove(graf);
                        worldGrafMissing = true;
                    }
                }

                if (worldGrafMissing)
                GraffitiMethods.SaveWorldGraffitiToFile();
            }

            if (GraffitiMethods.AllVehicleGraffitiOutfits != null && GraffitiMethods.AllVehicleGraffitiOutfits.Count > 0)
            {
                foreach (var outfit in GraffitiMethods.AllVehicleGraffitiOutfits.ToList())
                {
                    foreach (var decal in outfit.DecalList.ToList())
                    {
                        if (!GraffitiMethods.AllAddonGraffiti.Any(ag => ag.TextureName == decal.Graffiti.TextureName && ag.TextureDictionary == decal.Graffiti.TextureDictionary))
                        {
                            outfit.DecalList.Remove(decal);
                            //outfitDecalMissing = true;
                        }
                    }

                    if (outfit.DecalList.Count <= 0)
                    {
                        GraffitiMethods.AllVehicleGraffitiOutfits.Remove(outfit);
                        GraffitiMethods.DeleteSingleVehicleOutfit(outfit);
                    }
                    else
                    {
                        GraffitiMethods.SaveSingleVehicleOutfit(outfit);
                    }
                }

                //if (outfitDecalMissing)
                //GraffitiMethods.SaveAllVehicleGraffitiToFile();
            }
        }

        MenuPool _menuPool;
        UIMenu MainMenu;
        UIMenu DrawMenu;
        UIMenuItem drawMenuSearchbar;
        UIMenuItem ItemDeleteMode;
        UIMenu ExistingArtMenu;
        UIMenu VehDecalOutfitMenu;
        UIMenuItem SavedOutfitsSearchbar;
        UIMenuItem ItemRemoveOutfit;
        void  SetupMenu()
        {
            _menuPool = new MenuPool();

            MainMenu = new UIMenu("DECAL5");
            _menuPool.AddMenu(MainMenu);
            MainMenu.TitleColor = Color.FromArgb(255, 237, 90, 90);
            MainMenu.TitleBackgroundColor = Color.FromArgb(240, 0, 0, 0);
            MainMenu.TitleUnderlineColor = Color.FromArgb(255, 237, 90, 90);
            MainMenu.DefaultBoxColor = Color.FromArgb(160, 0, 0, 0);
            MainMenu.DefaultTextColor = Color.FromArgb(230, 255, 255, 255);
            MainMenu.HighlightedBoxColor = Color.FromArgb(130, 237, 90, 90);
            MainMenu.HighlightedItemTextColor = Color.FromArgb(255, 255, 255, 255);
            MainMenu.DescriptionBoxColor = Color.FromArgb(255, 0, 0, 0);
            MainMenu.DescriptionTextColor = Color.FromArgb(255, 255, 255, 255);

            DrawMenu = new UIMenu("Draw");
            _menuPool.AddSubMenu(DrawMenu, MainMenu, DrawMenu.Title, "Select a texture and start tagging!");
            SetupDrawMenu();

            ExistingArtMenu = new UIMenu("Existing Graffiti Manager");
            _menuPool.AddSubMenu(ExistingArtMenu, MainMenu, ExistingArtMenu.Title, "Manage existing graffiti in the world. Excludes vehicle decals.");
            SetupExistingArtMenu();

            VehDecalOutfitMenu = new UIMenu("Saved Vehicle Decal Outfits");
            _menuPool.AddSubMenu(VehDecalOutfitMenu, MainMenu, VehDecalOutfitMenu.Title, "Manage your saved vehicle outfits.");
            SetupVehicleSavedOutfitMenu();

            GraffitiMethods.MyScriptSettings = new GraffitiScriptSettings(@"scripts\Graffiti Mod\Settings.ini");
            GraffitiMethods.MyScriptSettings.Init();
            GraffitiMethods.MyScriptSettings.AddToMenu(MainMenu, _menuPool);
        }

        void SetupDrawMenu()
        {
            #region SEARCH_MENU

            drawMenuSearchbar = new UIMenuItem("Search ~r~:~s~ ", null, "Start typing. Use DELETE for backspace.");
            DrawMenu.AddMenuItem(drawMenuSearchbar);

            DrawMenu.WhileItemHighlight += (s, selItem, selIndex) =>
            {
                if (selItem == drawMenuSearchbar)
                {
                    Input.DisableAllButCamera(0);
                }
            };

            DrawMenu.OnItemLeftRight += (s, selItem, selIndex, direction) =>
            {
                if (direction == UIMenu.Direction.Right)
                {
                    var list = DrawMenu.UIMenuItemList.Where(i => i.PersistentIndex > selIndex).ToList(); // Get a list of items under the current highlighted item.

                    bool nextSubsectionExists = list.Any(i => i is UIMenuSubsectionItem);

                    if (nextSubsectionExists)
                    {
                        var item = list.First(i => i is UIMenuSubsectionItem); // Get the next subsection item under the current highlighted item.
                        DrawMenu.SetIndexPosition(DrawMenu.UIMenuItemList.IndexOf(item) + 1); // If a next subsection exists, go to it.
                    }
                    else
                    {
                        DrawMenu.SetIndexPosition(DrawMenu.UIMenuItemList.Count - 1); // If a next subsection does not exist, go to the end of the list.
                    }
                }
                else
                {
                    var list = DrawMenu.UIMenuItemList.Where(i => i.PersistentIndex < (selIndex - 1)).ToList(); // Get a list of items one item above the current highlighted item (in case it is a subsection item above the current item)

                    bool previousSubsectionExists = list.Any(i => i is UIMenuSubsectionItem);

                    if (previousSubsectionExists)
                    {
                        var item = list.Last(i => i is UIMenuSubsectionItem); // Get the next subsection item above the current highlighted item.
                        DrawMenu.SetIndexPosition(DrawMenu.UIMenuItemList.IndexOf(item) + 1); // If a previous subsection exists, go to it.
                    }
                    else
                    {
                        DrawMenu.SetIndexPosition(0); // If a previous subsection does not exist, go to the top of the list.
                    }
                }
            };

            /*UIMenu searchMenu = new UIMenu("Search...");
            _menuPool.AddSubMenu(searchMenu, DrawMenu, searchMenu.Title);*/

            //DrawMenu.OnItemSelect += (s, selItem, selIndex) =>
            //{
            //    /*if (selItem == searchMenu.ParentItem)
            //    {
            //        string searchString = Game.GetUserInput(999);
            //        if (String.IsNullOrWhiteSpace(searchString)) return;

            //        while (true)
            //        {
            //            _menuPool.CloseAllMenus();
            //            searchMenu.UnsubscribeAll_OnItemSelect();
            //            searchMenu.UnsubscribeAll_WhileItemHighlight();

            //            if (searchMenu.BindedList.Count > 0)
            //            {
            //                foreach (var bind in searchMenu.BindedList)
            //                {
            //                    bind.BindedSubmenu.Dispose();
            //                }
            //            }

            //            searchMenu.UIMenuItemList.Clear();
            //            searchMenu.BindedList.Clear();
            //            searchMenu.SaveIndexPositionFromOutOfBounds();

            //            foreach (var graf in GraffitiMethods.AllAddonGraffiti)
            //            {
            //                if (graf.TextureName.Contains(searchString) || graf.TextureDictionary.Contains(searchString))
            //                {
            //                    UIMenuItem grafItem = new UIMenuItem(graf.TextureName, null, "Loaded from ~b~" + graf.TextureDictionary);
            //                    searchMenu.AddMenuItem(grafItem);
            //                    searchMenu.WhileItemHighlight += (ss, sselItem, sselindex) => DrawmenuHighlight_WhileItemHighlight(ss, sselItem, sselindex, grafItem, graf);
            //                    searchMenu.OnItemSelect += (sss, ssselItem, ssselindex) => DrawmenuHighlight_OnItemSelect(sss, ssselItem, ssselindex, grafItem, graf);
            //                }
            //            }

            //            searchMenu.IsVisible = true;
            //            break;
            //        }
            //    }*/
            //};

            #endregion

            #region Deletion Mode

            ItemDeleteMode = new UIMenuItem("Deletion Mode", null, "Allows you to delete map graffiti by pointing at them. Does not affect vehicle decals.");
            DrawMenu.AddMenuItem(ItemDeleteMode);
            SetupDeleteMode();

            #endregion

            string lastTextureDict = "";
            foreach (var graf in GraffitiMethods.AllAddonGraffiti)
            {
                if (string.IsNullOrWhiteSpace(lastTextureDict) || lastTextureDict != graf.TextureDictionary) // Create subsections based on Texture Dictionary.
                {
                    UIMenuSubsectionItem subsection = new UIMenuSubsectionItem("––– " + graf.TextureDictionary + " –––");
                    DrawMenu.AddMenuItem(subsection);
                    lastTextureDict = graf.TextureDictionary;
                }

                UIMenuItem grafItem = new UIMenuItem(graf.TextureName, null, "Loaded from ~b~" + graf.TextureDictionary);
                DrawMenu.AddMenuItem(grafItem);
                DrawMenu.WhileItemHighlight += (s, selItem, selindex) => DrawmenuHighlight_WhileItemHighlight(s, selItem, selindex, grafItem, graf);
                DrawMenu.OnItemSelect += (s, selItem, selindex) => DrawmenuHighlight_OnItemSelect(s, selItem, selindex, grafItem, graf);

                DecalHelper.RequestStreamedTextureDict(graf.TextureDictionary);
            }
        }

        private void DrawmenuHighlight_WhileItemHighlight(UIMenu s, UIMenuItem selectedItem, int selindex, UIMenuItem itemToControl, Graffiti graf)
        {
            if (selectedItem == itemToControl)
            {
                Vector2 size = graf.TextureWidth > graf.TextureHeight ? GraffitiMethods.ResizedWidthHeightBasedOnNewWidth(graf.TextureWidth, graf.TextureHeight, 200f)
                    : GraffitiMethods.ResizedWidthHeightBasedOnNewHeight(graf.TextureWidth, graf.TextureHeight, 200f);

                GTA.UI.Sprite sprite = new GTA.UI.Sprite(graf.TextureDictionary, graf.TextureName,
                    new Size((int) size.X, (int) size.Y),
                    new Point(((int) GTA.UI.Screen.Width / 2) - ((int) size.X / 2),
                        ((int) GTA.UI.Screen.Height / 2) - ((int) size.Y) / 2));
                sprite.Enabled = true;
                sprite.Draw();
            }
        }

        private void DrawmenuHighlight_OnItemSelect(UIMenu s, UIMenuItem selectedItem, int selindex, UIMenuItem itemToControl, Graffiti graf)
        {
            // Go into tagging mode
            if (selectedItem == itemToControl)
            {
                GraffitiMethods.LastSelectedGraffiti = graf;

                HelpText.DrawingInstructions();

                GraffitiMethods.IsInTaggingMode = true;
                Input.SetInputWait(150);
                //_menuPool.OpenCloseLastMenu();
                _menuPool.CloseAllMenus();
            }
        }

        void SetupDeleteMode()
        {
            DrawMenu.OnItemSelect += (s, selItem, selIndex) =>
            {
                if (selItem == ItemDeleteMode)
                {
                    Input.SetInputWait(100);
                    bool confirmation = false;
                    ExistingGraffitiInMap chosenGraf = null;
                    HelpText.DeletionInstructions();

                    bool inDeletionMode = true;
                    while (inDeletionMode)
                    {
                        WorldGraffitiController();
                        GTA.UI.Hud.ShowComponentThisFrame(HudComponent.Reticle);

                        if (Input.AcceptPressed())
                        {
                            Wait(100);

                            RaycastResult ray = World.Raycast(GameplayCamera.Position,
                                GameplayCamera.Position + GameplayCamera.Direction * 2000,
                                IntersectFlags.Map | IntersectFlags.Objects, Game.Player.Character);

                            if (!ray.DidHit) return;

                            float dist = float.MaxValue;
                            foreach (var graf in GraffitiMethods.AllGraffitiInMap)
                            {
                                float currDist = World.GetDistance(graf.Location, ray.HitPosition);
                                if (currDist <= 10f && currDist < dist)
                                {
                                    chosenGraf = graf;
                                    dist = currDist;
                                }
                            }

                            if (chosenGraf != null)
                            {
                                confirmation = true;
                                GTAFunction.ClearAllHelpMessages();
                                HelpText.DeletionConfirmation();
                            }
                            else
                            {
                                GTA.UI.Screen.ShowSubtitle("You are not pointing at a graffiti. Try pointing at the center of the image for best results.");
                            }
                        }
                        else if (Input.CancelPressed() || Game.Player.Character.IsDead)
                        {
                            inDeletionMode = false;
                            GTAFunction.ClearAllHelpMessages();
                            Wait(100);
                            break;
                        }

                        while (confirmation)
                        {
                            TextElement info = new TextElement("Texture: " + chosenGraf.Graffiti.TextureName
                                                                           + "\nDictionary: " +
                                                                           chosenGraf.Graffiti.TextureDictionary
                                                                           + "\nStreet: " +
                                                                           World.GetStreetName(chosenGraf.Location)
                                , GTA.UI.Screen.WorldToScreen(chosenGraf.Location), 0.40f, Color.White,
                                GTA.UI.Font.ChaletComprimeCologne, Alignment.Center);
                            info.Enabled = true;
                            info.Draw();

                            if (Input.AcceptPressed())
                            {
                                while (true)
                                {
                                    // The index of the graf in AllGraffitiInMap should match its corresponding UIMenuItem in ExistingArtMenu.UIMenuItemList at the same index
                                    int index = GraffitiMethods.AllGraffitiInMap.IndexOf(chosenGraf);

                                    GraffitiMethods.AllGraffitiInMap.Remove(chosenGraf);
                                    chosenGraf.Remove();
                                    GraffitiMethods.SaveWorldGraffitiToFile();

                                    /*DisposeMenusInExistingArtMenu();
                                    SetupExistingArtMenu();*/
                                    DeleteExistingGraffitiMenuFromParentMenu(ExistingArtMenu.UIMenuItemList[index].SubmenuWithin);
                                    break;
                                }

                                confirmation = false;
                                GTAFunction.ClearAllHelpMessages();
                                HelpText.DeletionInstructions();
                                Wait(100);
                            }
                            else if (Input.CancelPressed() || Game.Player.Character.IsDead)
                            {
                                confirmation = false;
                                GTAFunction.ClearAllHelpMessages();
                                HelpText.DeletionInstructions();
                                Wait(100);
                                break;
                            }
                            Yield();
                        }

                        Yield();
                    }
                }
            };
        }

        void SetupExistingArtMenu()
        {
            if (GraffitiMethods.AllGraffitiInMap == null) return;
            foreach (var graffiti in GraffitiMethods.AllGraffitiInMap)
            {
                AddSavedExistingGraffitiToExistingArtMenu(graffiti);
            }
        }

        void AddSavedExistingGraffitiToExistingArtMenu(ExistingGraffitiInMap graffiti)
        {
            UIMenu grafMenu = new UIMenu(graffiti.Graffiti.TextureName + " | " + World.GetStreetName(graffiti.Location));
            _menuPool.AddSubMenu(grafMenu, ExistingArtMenu, grafMenu.Title, "Edit this graffiti art.");

            UIMenuItem teleportTo = new UIMenuItem("Teleport To Graffiti Art", null, "Go to art's location.");
            grafMenu.AddMenuItem(teleportTo);

            UIMenuItem deleteArt = new UIMenuItem("Delete This Graffiti Art", null, "This is permanent.");
            grafMenu.AddMenuItem(deleteArt);

            ItemHighlightEvent onHighlightHandler = null;
            onHighlightHandler = (s, selItem, selIndex) =>
            {
                TextElement info = new TextElement(
                    graffiti.Graffiti.TextureName + " | " + World.GetStreetName(graffiti.Location),
                    GTA.UI.Screen.WorldToScreen(graffiti.Location), 0.40f, System.Drawing.Color.White,
                    GTA.UI.Font.ChaletComprimeCologne, Alignment.Center);
                info.Enabled = true;
                info.Draw();
            };
            grafMenu.WhileItemHighlight += onHighlightHandler;

            ItemSelectEvent OnSelectHandler = null;
            OnSelectHandler = (s, selItem, selIndex) =>
            {
                if (selItem == teleportTo)
                {
                    Vector3 safeLoc = World.GetSafeCoordForPed(graffiti.Location, false);
                    while (safeLoc.Z == 0.0f)
                    {
                        Game.Player.Character.Position = graffiti.Location;
                        safeLoc = World.GetSafeCoordForPed(graffiti.Location, false);
                        Yield();
                    }
                    Game.Player.Character.Position = safeLoc;
                    Game.Player.Character.Rotation = MathHelper.DirectionToRotation((graffiti.Location - Game.Player.Character.Position).Normalized, 0f);
                }

                if (selItem == deleteArt)
                {
                    while (true)
                    {
                        grafMenu.OnItemSelect -= OnSelectHandler;
                        grafMenu.WhileItemHighlight -= onHighlightHandler;

                        GTA.UI.Screen.ShowSubtitle("Deleted " + grafMenu.Title);
                        _menuPool.CloseAllMenus();

                        // The index of the graf in AllGraffitiInMap should match its corresponding UIMenuItem in ExistingArtMenu.UIMenuItemList at the same index
                        int index = GraffitiMethods.AllGraffitiInMap.IndexOf(graffiti);

                        GraffitiMethods.AllGraffitiInMap.Remove(graffiti);
                        graffiti.Remove();
                        GraffitiMethods.SaveWorldGraffitiToFile();

                        /*DisposeMenusInExistingArtMenu();
                        SetupExistingArtMenu();*/
                        DeleteExistingGraffitiMenuFromParentMenu(ExistingArtMenu.UIMenuItemList[index].SubmenuWithin);

                        MainMenu.IsVisible = true;
                        break;
                    }
                }
            };
            grafMenu.OnItemSelect += OnSelectHandler;
        }

        void DisposeMenusInExistingArtMenu()
        {
            // Reset Existing Art Menu
            if (ExistingArtMenu.BindedList.Count > 0)
            {
                foreach (var bind in ExistingArtMenu.BindedList)
                {
                    bind.BindedSubmenu.Dispose();
                }
            }

            ExistingArtMenu.UIMenuItemList.Clear();
            ExistingArtMenu.BindedList.Clear();
            ExistingArtMenu.SaveIndexPositionFromOutOfBounds();
        }

        void DeleteExistingGraffitiMenuFromParentMenu(UIMenu existingGrafMenu)
        {
            ExistingArtMenu.UIMenuItemList.Remove(existingGrafMenu.ParentItem);
            existingGrafMenu.ParentItem.Dispose(); // Required to remove reference to the submenu to be deleted.
            existingGrafMenu.Dispose();
            ExistingArtMenu.SaveIndexPositionFromOutOfBounds();
        }

        void SetupVehicleSavedOutfitMenu()
        {
            SavedOutfitsSearchbar = new UIMenuItem("Search ~r~:~s~ ", null, "Start typing. Use DELETE for backspace.");
            VehDecalOutfitMenu.AddMenuItem(SavedOutfitsSearchbar);

            ItemRemoveOutfit = new UIMenuItem("Remove Outfit from Vehicle", null, "The vehicle in the center of your screen will be stripped of its outfit.");
            VehDecalOutfitMenu.AddMenuItem(ItemRemoveOutfit);

            VehDecalOutfitMenu.WhileItemHighlight += (s, selItem, selIndex) =>
            {
                GraffitiMethods.DisplayAllLoadedVehicleInfo();

                if (selItem == SavedOutfitsSearchbar)
                {
                    Input.DisableAllButCamera(0);
                }
            };

            VehDecalOutfitMenu.OnItemSelect += (s, selItem, selIndex) =>
            {
                if (selItem == ItemRemoveOutfit)
                {
                    RaycastResult ray = World.Raycast(GameplayCamera.Position,
                        GameplayCamera.Position + GameplayCamera.Direction * 50f,
                        IntersectFlags.Map | IntersectFlags.MissionEntities, Game.Player.Character);

                    if (ray.DidHit)
                    {
                        Vehicle closeVeh = null;
                        if (ray.HitEntity != null && ray.HitEntity.EntityIsAVehicle())
                        {
                            closeVeh = (Vehicle)ray.HitEntity;
                        }
                        else
                        {
                            closeVeh = World.GetClosestVehicle(ray.HitPosition, 10f);
                        }

                        if (closeVeh == null) { GTA.UI.Screen.ShowSubtitle("No vehicle detected. Point under vehicle for best results."); return; }

                        if (GraffitiMethods.LoadedVehicleOutfits.Any(lo => lo.CurrentVehicle == closeVeh)) // If there is already a loaded outfit applied to this vehicle, remove it first.
                        {
                            foreach (var outfit in GraffitiMethods.LoadedVehicleOutfits.Where(o => o.CurrentVehicle == closeVeh).ToList())
                            {
                                GraffitiMethods.LoadedVehicleOutfits.Remove(outfit);
                                outfit.RemoveFromVehicle();
                                outfit.DecalList.Clear();

                                GTA.UI.Screen.ShowSubtitle("Outfit ~y~" + outfit.OutfitName + " ~s~removed from vehicle");
                            }
                        }
                        else
                        {
                            GTA.UI.Screen.ShowSubtitle("Detected vehicle does not have an outfit.");
                        }
                    }
                    else
                    {
                        GTA.UI.Screen.ShowSubtitle("No vehicle detected. Point under vehicle for best results.");
                    }
                }
            };

            if (GraffitiMethods.AllVehicleGraffitiOutfits == null) return;
            foreach (var savedOutfit in GraffitiMethods.AllVehicleGraffitiOutfits)
            {
                AddSavedOutfitToVehicleSavedOutfitMenu(savedOutfit);
            }
        }

        void AddSavedOutfitToVehicleSavedOutfitMenu(DecaledVehicleOutfit savedOutfit)
        {
            ItemSelectEvent selectEventHandler = null;

            UIMenu outfitMenu = new UIMenu(savedOutfit.OutfitName);
            _menuPool.AddSubMenu(outfitMenu, VehDecalOutfitMenu, savedOutfit.OutfitName + " | ~y~Model: " + (savedOutfit.FriendlyName == null || savedOutfit.FriendlyName.Equals("NULL") ? savedOutfit.DisplayName : savedOutfit.FriendlyName), "Manage this outfit");

            UIMenuItem spawnVeh = new UIMenuItem("Spawn Vehicle With Outfit", null, savedOutfit.FriendlyName + " | " + savedOutfit.DisplayName + " | " + savedOutfit.VehicleModelHash);
            outfitMenu.AddMenuItem(spawnVeh);

            UIMenuItem applyToVeh = new UIMenuItem("Apply to Vehicle", null, "Applies this decal to the vehicle you are pointing at, assuming the vehicle model matches. Point under vehicle for best results.");
            outfitMenu.AddMenuItem(applyToVeh);

            UIMenuItem changeName = new UIMenuItem("Change Outfit Name");
            outfitMenu.AddMenuItem(changeName);

            UIMenu decalList = new UIMenu("Decal List");
            _menuPool.AddSubMenu(decalList, outfitMenu, decalList.Title);

            foreach (var decal in savedOutfit.DecalList)
            {

                UIMenu singleDecalMenu = new UIMenu(decal.Graffiti.TextureName + " | " + GTAFunction.RoundVector3D(decal.Location, 3));
                _menuPool.AddSubMenu(singleDecalMenu, decalList, singleDecalMenu.Title);
                
                decalList.WhileItemHighlight += (s, selItem, selIndex) =>
                {
                    //GraffitiMethods.DisplayAllLoadedVehicleInfo();
                    
                    if (selItem == singleDecalMenu.ParentItem)
                    {
                        GraffitiMethods.DisplaySingleVehicleDecalInfo(savedOutfit, decal);
                    }
                };

                UIMenuItem mirrorDecal = new UIMenuItem("Mirror this decal: ", "< " + decal.MirrorFlip.ToString() + " >", "Toggle whether to mirror this decal to the other side of the vehicle or not. Only does left/right mirroring.");
                singleDecalMenu.AddMenuItem(mirrorDecal);

                UIMenuItem redRGB = new UIMenuItem("Red Value", "< " + decal.RGB_Red + " >", "Red value of the decal. Range: 0 - 255. Default is 255.");
                singleDecalMenu.AddMenuItem(redRGB);

                UIMenuItem greenRGB = new UIMenuItem("Green Value", "< " + decal.RGB_Green + " >", "Green value of the decal. Range: 0 - 255. Default is 255.");
                singleDecalMenu.AddMenuItem(greenRGB);

                UIMenuItem blueRGB = new UIMenuItem("Blue Value", "< " + decal.RGB_Blue + " >", "Blue value of the decal. Range: 0 - 255. Default is 255.");
                singleDecalMenu.AddMenuItem(blueRGB);

                UIMenuItem alphaRGB = new UIMenuItem("Alpha Value", "< " + decal.RGB_Alpha + " >", "Alpha value of the decal. Range: 0 - 255. Default is 255.");
                singleDecalMenu.AddMenuItem(alphaRGB);

                UIMenuItem finetuneX = new UIMenuItem("Finetune Left/Right Position", "< " + decal.Location.X.ShowRounded(3) + " >", "The decal may disappear when positioned too far, be careful.");
                singleDecalMenu.AddMenuItem(finetuneX);

                UIMenuItem finetuneY = new UIMenuItem("Finetune Front/Back Position", "< " + decal.Location.Y.ShowRounded(3) + " >", "The decal may disappear when positioned too far, be careful.");
                singleDecalMenu.AddMenuItem(finetuneY);

                UIMenuItem finetuneZ = new UIMenuItem("Finetune Up/Down Position", "< " + decal.Location.Z.ShowRounded(3) + " >", "The decal may disappear when positioned too far, be careful.");
                singleDecalMenu.AddMenuItem(finetuneZ);

                UIMenuItem finetuneRot = new UIMenuItem("Finetune Rotation Angle", "< " + decal.RotationAngle + " >", "Between 0 and 359");
                singleDecalMenu.AddMenuItem(finetuneRot);
                
                UIMenuItem deleteDecal = new UIMenuItem("Delete this decal", null, "Select twice to confirm deletion.");
                singleDecalMenu.AddMenuItem(deleteDecal);
                
                singleDecalMenu.OnItemLeftRight += (s, selItem, selIndex, direction) =>
                {
                    if (selItem == mirrorDecal)
                    {
                        decal.MirrorFlip = direction == UIMenu.Direction.Left ? decal.MirrorFlip.Previous() : decal.MirrorFlip.Next();

                        mirrorDecal.Value = "< " + decal.MirrorFlip.ToString() + " >";
                        while (true)
                        {
                            foreach (var loadedOutfit in GraffitiMethods.LoadedVehicleOutfits)
                            {
                                if (savedOutfit.OutfitName.Equals(loadedOutfit.OutfitName) && savedOutfit.VehicleModelHash == loadedOutfit.VehicleModelHash)
                                {
                                    foreach (var d in loadedOutfit.DecalList)
                                    {
                                        d.MirrorFlip = savedOutfit.DecalList[loadedOutfit.DecalList.IndexOf(d)].MirrorFlip;
                                        d.Remove();
                                    }
                                }
                            }
                            break;
                        }
                        GraffitiMethods.SaveSingleVehicleOutfit(savedOutfit);
                        //GraffitiMethods.RefreshAllGraffiti();
                    }
                    else if (selItem == redRGB || selItem == greenRGB || selItem == blueRGB || selItem == alphaRGB)
                    {
                        singleDecalMenu.ControlFloatValue(ref decal.RGB_Red, redRGB, direction, 1f, 10f, 0, true, 0f, 255f);
                        singleDecalMenu.ControlFloatValue(ref decal.RGB_Green, greenRGB, direction, 1f, 10f, 0, true, 0f, 255f);
                        singleDecalMenu.ControlFloatValue(ref decal.RGB_Blue, blueRGB, direction, 1f, 10f, 0, true, 0f, 255f);
                        singleDecalMenu.ControlFloatValue(ref decal.RGB_Alpha, alphaRGB, direction, 1f, 10f, 0, true, 0f, 255f);

                        while (true)
                        {
                            foreach (var loadedOutfit in GraffitiMethods.LoadedVehicleOutfits)
                            {
                                if (savedOutfit.OutfitName.Equals(loadedOutfit.OutfitName) && savedOutfit.VehicleModelHash == loadedOutfit.VehicleModelHash)
                                {
                                    foreach (var d in loadedOutfit.DecalList)
                                    {
                                        var baseDecal = savedOutfit.DecalList[loadedOutfit.DecalList.IndexOf(d)];
                                        d.RGB_Red = baseDecal.RGB_Red;
                                        d.RGB_Green = baseDecal.RGB_Green;
                                        d.RGB_Blue = baseDecal.RGB_Blue;
                                        d.RGB_Alpha = baseDecal.RGB_Alpha;
                                        d.Remove();
                                    }
                                }
                            }
                            break;
                        }

                        GraffitiMethods.SaveSingleVehicleOutfit(savedOutfit);
                        //GraffitiMethods.RefreshAllGraffiti();
                    }
                    else if (selItem == finetuneX || selItem == finetuneY || selItem == finetuneZ)
                    {
                        float x2 = decal.Location_ForDirectionPurpose.X, y2 = decal.Location_ForDirectionPurpose.Y, z2 = decal.Location_ForDirectionPurpose.Z;
                        singleDecalMenu.ControlFloatValue(ref x2, finetuneX, direction, 0.001f, 0.01f, 3);
                        singleDecalMenu.ControlFloatValue(ref y2, finetuneY, direction, 0.001f, 0.01f, 3);
                        singleDecalMenu.ControlFloatValue(ref z2, finetuneZ, direction, 0.001f, 0.01f, 3);

                        float x1 = decal.Location.X, y1 = decal.Location.Y, z1 = decal.Location.Z;
                        singleDecalMenu.ControlFloatValue(ref x1, finetuneX, direction, 0.001f, 0.01f, 3);
                        singleDecalMenu.ControlFloatValue(ref y1, finetuneY, direction, 0.001f, 0.01f, 3);
                        singleDecalMenu.ControlFloatValue(ref z1, finetuneZ, direction, 0.001f, 0.01f, 3);

                        decal.Location = new Vector3(x1, y1, z1);
                        decal.Location_ForDirectionPurpose = new Vector3(x2, y2, z2);

                        while (true)
                        {
                            foreach (var loadedOutfit in GraffitiMethods.LoadedVehicleOutfits)
                            {
                                if (savedOutfit.OutfitName.Equals(loadedOutfit.OutfitName) && savedOutfit.VehicleModelHash == loadedOutfit.VehicleModelHash)
                                {
                                    foreach (var d in loadedOutfit.DecalList)
                                    {
                                        var baseDecal = savedOutfit.DecalList[loadedOutfit.DecalList.IndexOf(d)];
                                        d.Location = baseDecal.Location;
                                        d.Location_ForDirectionPurpose = baseDecal.Location_ForDirectionPurpose;
                                        d.Remove();
                                    }
                                }
                            }
                            break;
                        }

                        GraffitiMethods.SaveSingleVehicleOutfit(savedOutfit);
                        //GraffitiMethods.RefreshAllGraffiti();
                    }
                    else if (selItem == finetuneRot)
                    {
                        float tempRot = decal.RotationAngle;
                        singleDecalMenu.ControlFloatValue(ref tempRot, finetuneRot, direction, 0.5f, 5f, 1, true, 0f, 359.5f);
                        decal.RotationAngle = tempRot;

                        while (true)
                        {
                            foreach (var loadedOutfit in GraffitiMethods.LoadedVehicleOutfits)
                            {
                                if (savedOutfit.OutfitName.Equals(loadedOutfit.OutfitName) && savedOutfit.VehicleModelHash == loadedOutfit.VehicleModelHash)
                                {
                                    foreach (var d in loadedOutfit.DecalList)
                                    {
                                        var baseDecal = savedOutfit.DecalList[loadedOutfit.DecalList.IndexOf(d)];
                                        d.RotationAngle = baseDecal.RotationAngle;
                                        d.Remove();
                                    }
                                }
                            }
                            break;
                        }

                        GraffitiMethods.SaveSingleVehicleOutfit(savedOutfit);
                        //GraffitiMethods.RefreshAllGraffiti();
                    }
                };

                singleDecalMenu.OnItemSelect += (s, selItem, selIndex) =>
                {
                    if (selItem == deleteDecal)
                    {
                        Wait(150);

                        while (true)
                        {
                            GTAFunction.ClearAllHelpMessages();
                            HelpText.DeletionInstructions();

                            if (Input.AcceptPressed())
                            {
                                DeleteVehicleDecals_SelectEvent(s, selItem, selIndex, deleteDecal, decal, savedOutfit);
                                GTAFunction.ClearAllHelpMessages();
                                Wait(300);
                                break;
                            }
                            else if (Input.CancelPressed())
                            {
                                GTAFunction.ClearAllHelpMessages();
                                Wait(300);
                                break;
                            }

                            Script.Yield();
                        }
                    }
                };
            }

            UIMenuItem deleteOutfit = new UIMenuItem("Delete Outfit", null, "Select twice to confirm deletion.");
            outfitMenu.AddMenuItem(deleteOutfit);

            selectEventHandler = (s, selItem, selIndex) =>
            {
                if (selItem == spawnVeh)
                {
                    while (true)
                    {
                        Model model = new Model(savedOutfit.VehicleModelHash);
                        model.Request(250);

                        if (model.IsInCdImage && model.IsValid)
                        {
                            int retries = 0;
                            while (!model.IsLoaded && retries <= 10)
                            {
                                retries++;
                                Script.Wait(50);
                            }

                            if (!model.IsLoaded)
                            {
                                GTA.UI.Screen.ShowSubtitle("Vehicle model is taking long to load.\nTry again.");
                                break;
                            }

                            Vector3 gameCamDir = GameplayCamera.Direction;
                            gameCamDir.Z = 0f;
                            Vector3 spawnPoint = Game.Player.Character.Position + gameCamDir * 5f;
                            Vehicle newVeh = World.CreateVehicle(model, Game.Player.Character.GetOffsetPosition(new Vector3(0f, 0f, 50f)));
                            GTAFunction.Teleport(newVeh, spawnPoint, false);
                            newVeh.IsPersistent = false;
                            DecaledVehicleOutfit newLoadedOutfit = savedOutfit.Clone();
                            newLoadedOutfit.ApplyToVehicle(newVeh);
                            GraffitiMethods.LoadedVehicleOutfits.Add(newLoadedOutfit);
                        }
                        else
                        {
                            GTA.UI.Screen.ShowSubtitle("Vehicle Model is not valid.");
                        }
                        break;
                    }
                }
                else if (selItem == applyToVeh)
                {
                    RaycastResult ray = World.Raycast(GameplayCamera.Position,
                        GameplayCamera.Position + GameplayCamera.Direction * 50f,
                        IntersectFlags.Map | IntersectFlags.MissionEntities, Game.Player.Character);

                    if (ray.DidHit)
                    {
                        Vehicle closeVeh = null;
                        if (ray.HitEntity != null && ray.HitEntity.EntityIsAVehicle())
                        {
                            closeVeh = (Vehicle)ray.HitEntity;
                        }
                        else
                        {
                            closeVeh = World.GetClosestVehicle(ray.HitPosition, 10f);
                        }

                        if (closeVeh == null) { GTA.UI.Screen.ShowSubtitle("No vehicle detected. Point under vehicle for best results."); return; }

                        if (closeVeh.Model.Hash == savedOutfit.VehicleModelHash)
                        {
                            if (GraffitiMethods.LoadedVehicleOutfits.Any(lo => lo.CurrentVehicle == closeVeh)) // If there is already a loaded outfit applied to this vehicle, remove it first.
                            {
                                foreach (var outfit in GraffitiMethods.LoadedVehicleOutfits.Where(o => o.CurrentVehicle == closeVeh).ToList())
                                {
                                    GraffitiMethods.LoadedVehicleOutfits.Remove(outfit);
                                    outfit.RemoveFromVehicle();
                                    outfit.DecalList.Clear();
                                }
                            }

                            DecaledVehicleOutfit newLoadedOutfit = savedOutfit.Clone();
                            newLoadedOutfit.ApplyToVehicle(closeVeh);
                            GraffitiMethods.LoadedVehicleOutfits.Add(newLoadedOutfit);

                            GTA.UI.Screen.ShowSubtitle("Outfit Applied.");
                        }
                        else
                        {
                            GTA.UI.Screen.ShowSubtitle("Vehicle does not match this outfit's vehicle model.");
                        }
                    }
                    else
                    {
                        GTA.UI.Screen.ShowSubtitle("No vehicle detected. Point under vehicle for best results.");
                    }
                }
                else if (selItem == changeName)
                {
                    string newOutfitName = Game.GetUserInput();
                    if (String.IsNullOrWhiteSpace(newOutfitName)) return;

                    while (true)
                    {
                        foreach (var loadedOutfit in GraffitiMethods.LoadedVehicleOutfits)
                        {
                            if (savedOutfit.OutfitName.Equals(loadedOutfit.OutfitName) && savedOutfit.VehicleModelHash == loadedOutfit.VehicleModelHash)
                            {
                                loadedOutfit.OutfitName = newOutfitName;
                            }
                        }

                        GraffitiMethods.DeleteSingleVehicleOutfit(savedOutfit);

                        savedOutfit.OutfitName = newOutfitName;
                        outfitMenu.Title = newOutfitName;
                        outfitMenu.ParentItem.Text = newOutfitName + " | ~y~Model: " + (savedOutfit.FriendlyName == null || savedOutfit.FriendlyName.Equals("NULL") ? savedOutfit.DisplayName : savedOutfit.FriendlyName);

                        //GraffitiMethods.SaveAllVehicleGraffitiToFile();
                        GraffitiMethods.SaveSingleVehicleOutfit(savedOutfit);
                        break;
                    }

                }
                else if (selItem == deleteOutfit)
                {
                    Wait(150);

                    while (true)
                    {
                        GTAFunction.ClearAllHelpMessages();
                        HelpText.DeletionInstructions();

                        if (Input.AcceptPressed())
                        {
                            DeleteSavedOutfit(savedOutfit, outfitMenu);
                            GTAFunction.ClearAllHelpMessages();
                            Wait(300);
                            break;
                        }
                        else if (Input.CancelPressed())
                        {
                            GTAFunction.ClearAllHelpMessages();
                            Wait(300);
                            break;
                        }

                        Script.Yield();
                    }
                }
            };
            outfitMenu.OnItemSelect += selectEventHandler;
            
            outfitMenu.WhileItemHighlight += (s, selItem, selIndex) =>
            {
                GraffitiMethods.DisplayAllLoadedVehicleInfo();
            };
        }

        private void DeleteVehicleDecals_SelectEvent(UIMenu s, UIMenuItem selItem, int selIndex, UIMenuItem decalMenuItem, SingleVehicleDecal decalToDelete, DecaledVehicleOutfit savedOutfit)
        {
            if (selItem == decalMenuItem)
            {
                if (savedOutfit.DecalList.Any(d => d == decalToDelete))
                {
                    if (savedOutfit.DecalList.Count > 1)
                    {
                        while (true)
                        {
                            savedOutfit.DecalList.Remove(decalToDelete);
                            decalToDelete.Remove();
                            decalMenuItem.Text = "~c~" + decalMenuItem.Text + " (Removed)~s~";
                            s.Title = decalMenuItem.Text;
                            s.ParentItem.Text = decalMenuItem.Text;

                            foreach (var loadedOutfit in GraffitiMethods.LoadedVehicleOutfits)
                            {
                                if (savedOutfit.OutfitName.Equals(loadedOutfit.OutfitName) && savedOutfit.VehicleModelHash == loadedOutfit.VehicleModelHash)
                                {
                                    foreach (var loadedDecal in loadedOutfit.DecalList.ToList())
                                    {
                                        if (decalToDelete.Location == loadedDecal.Location)
                                        {
                                            loadedOutfit.DecalList.Remove(loadedDecal);
                                            loadedDecal.Remove();
                                        }
                                    }
                                }
                            }
                            GraffitiMethods.SaveSingleVehicleOutfit(savedOutfit);
                            break;
                        }
                    }
                    else
                    {
                        DeleteSavedOutfit(savedOutfit, s.ParentMenu.ParentMenu);
                    }
                }
            }
        }

        void DeleteSavedOutfit(DecaledVehicleOutfit savedOutfit, UIMenu outfitMenu)
        {
            while (true)
            {
                GTA.UI.Screen.ShowSubtitle("Deleted " + outfitMenu.Title);
                _menuPool.CloseAllMenus();

                GraffitiMethods.AllVehicleGraffitiOutfits.Remove(savedOutfit);
                savedOutfit.RemoveFromVehicle();

                if (GraffitiMethods.LoadedVehicleOutfits != null && GraffitiMethods.LoadedVehicleOutfits.Count > 0)
                {
                    foreach (var loadedOutfit in GraffitiMethods.LoadedVehicleOutfits.ToList())
                    {
                        if (savedOutfit.OutfitName.Equals(loadedOutfit.OutfitName) && savedOutfit.VehicleModelHash == loadedOutfit.VehicleModelHash)
                        {
                            GraffitiMethods.LoadedVehicleOutfits.Remove(loadedOutfit);
                            loadedOutfit.RemoveFromVehicle();
                            loadedOutfit.DecalList.Clear();
                        }
                    }
                }

                GraffitiMethods.DeleteSingleVehicleOutfit(savedOutfit);
                //GraffitiMethods.SaveAllVehicleGraffitiToFile();

                /*DisposeMenusInVehicleSavedOutfitMenu();
                SetupVehicleSavedOutfitMenu();*/
                DeleteSavedOutfitMenuFromParentMenu(outfitMenu);

                MainMenu.IsVisible = true;
                break;
            }
        }

        void DeleteSavedOutfitMenuFromParentMenu(UIMenu outfitMenu)
        {
            VehDecalOutfitMenu.UIMenuItemList.Remove(outfitMenu.ParentItem);
            outfitMenu.ParentItem.Dispose(); // Required to remove reference to the submenu to be deleted.

            foreach (var bind_l1 in outfitMenu.BindedList) // Dispose submenus under outfitMenu
            {
                foreach (var bind_l2 in bind_l1.BindedSubmenu.BindedList) // Dispose submenus within (i.e. decals submenus)
                {
                    bind_l2.BindedSubmenu.Dispose();
                }

                bind_l1.BindedSubmenu.Dispose();
            }

            outfitMenu.Dispose();
            VehDecalOutfitMenu.SaveIndexPositionFromOutOfBounds();
        }

        UIMenu GetOutfitMenuFromSavedOutfit(DecaledVehicleOutfit savedOutfit)
        {
            List<UIMenuItem> fullList = new List<UIMenuItem>();
            fullList.AddRange(VehDecalOutfitMenu.UIMenuItemList.Where((x, i) => x.PersistentIndex >= 2));
            fullList.AddRange(VehDecalOutfitMenu.DisabledList);

            return fullList.First(o => o.Text.Contains(" | ~y~Model: ")
            && savedOutfit.OutfitName.Equals(o.Text.GetUntilOrEmpty(" | ~y~Model: "))).SubmenuWithin;
        }

        /*void DisposeMenusInVehicleSavedOutfitMenu()
        {
            // Reset Vehicle Saved Outfit Menu
            if (VehDecalOutfitMenu.BindedList.Count > 0)
            {
                foreach (var bindLevelOne in VehDecalOutfitMenu.BindedList) // Bind Level One pertains to each Outfit submenu.
                {
                    if (bindLevelOne.BindedSubmenu.BindedList.Count > 0)
                    {
                        foreach (var bindLevelTwo in bindLevelOne.BindedSubmenu.BindedList) // Bind Level Two pertains to each Delete Decal submenu within an Outfit submenu.
                        {
                            bindLevelTwo.BindedSubmenu.Dispose();
                        }
                    }

                    bindLevelOne.BindedSubmenu.Dispose();
                }
            }

            VehDecalOutfitMenu.UIMenuItemList.Clear();
            VehDecalOutfitMenu.BindedList.Clear();
            VehDecalOutfitMenu.SaveIndexPositionFromOutOfBounds();
        }*/

        void MenuControl()
        {
            if (ScriptComm.IsEventTriggered())
            {
                _menuPool.OpenCloseLastMenu();

                ScriptComm.BlockScriptCommunicatorModMenu();
                ScriptComm.ResetEvent();
                Wait(300);
            }

            _menuPool.ProcessMenus();

            if (_menuPool.IsMenuDrawAllowed())
            {
                if (_menuPool.LastUsedMenu.IsVisible)
                {
                    ScriptComm.BlockScriptCommunicatorModMenu();
                }
                else
                {
                    if (ScriptComm.ScriptCommunicatorMenuIsBlocked())
                    {
                        //Wait(300);
                        ScriptComm.UnblockScriptCommunicatorModMenu();
                    }
                }
            }
        }

        bool welcomeMsgShown;
        void ManageOnGameInit()
        {
            while (!welcomeMsgShown)
            {
                if (Game.Player.CanControlCharacter)
                {
                    GTA.UI.Notification.Show("~r~Loaded \"Graffiti Mod\" by ~s~stillhere");
                    welcomeMsgShown = true;
                    break;
                }

                Yield();
            }

            if (!menuInit)
            {
                GraffitiMethods.ImportFromImportsFolder();

                bool missingTextures = MissingTextureExistsInMapOrOutfit();

                while (missingTextures)
                {
                    HelpText.MissingTextureQuestion();

                    if (Input.AcceptPressed())
                    {
                        RemoveMissingTextures();
                        GTAFunction.ClearAllHelpMessages();
                        break;
                    }
                    else if (Input.CancelPressed())
                    {
                        GTAFunction.ClearAllHelpMessages();
                        break;
                    }

                    Script.Yield();
                }

                //GraffitiMethods.InitGraffitiReferences();

                SetupMenu();

                menuInit = true;
            }
        }

        void OnTick(object sender, EventArgs e) // This is where most of your script goes
        {
            ManageOnGameInit();

            MenuControl();

            TaggingMode();

            WorldGraffitiController();
        }

        void TaggingMode()
        {
            while (GraffitiMethods.IsInTaggingMode)
            {
                Input.DisableControlsWhileTagging();

                WorldGraffitiController();

                GraffitiMethods.SeeInfoMode = true;

                if (Input.CancelPressed() || Game.Player.Character.IsDead)
                {
                    GraffitiMethods.IsInTaggingMode = false;
                    GraffitiMethods.SeeInfoMode = false;
                    GTAFunction.ClearAllHelpMessages();
                    Wait(100);
                    _menuPool.OpenCloseLastMenu();
                    break;
                }

                if (!GraffitiMethods.DisplayTemporaryGraffiti()) return;

                if (Input.MoveRotationClockwise())
                {
                    Input.SetChangeUnits(1f, 5f, 20f);
                    GraffitiMethods.RotationAngle = MathHelper.IncreaseNumber(GraffitiMethods.RotationAngle, GraffitiMethods.ChangeUnit, 360f, true);
                }
                else if (Input.MoveRotationCounterClockwise())
                {
                    Input.SetChangeUnits(1f, 5f, 20f);
                    GraffitiMethods.RotationAngle = MathHelper.DecreaseNumber(GraffitiMethods.RotationAngle, GraffitiMethods.ChangeUnit, 0f, true);
                }
                else if (Input.IncreaseSize())
                {
                    Input.SetChangeUnits(0.01f, 0.1f, 1f);
                    GraffitiMethods.WidthHeightSizeCoeff = MathHelper.IncreaseNumber(GraffitiMethods.WidthHeightSizeCoeff, GraffitiMethods.ChangeUnit, 30.0f, false, 0f, 2);
                }
                else if (Input.DecreaseSize())
                {
                    Input.SetChangeUnits(0.01f, 0.1f, 1f);
                    GraffitiMethods.WidthHeightSizeCoeff = MathHelper.DecreaseNumber(GraffitiMethods.WidthHeightSizeCoeff, GraffitiMethods.ChangeUnit, 0.01f, false, 0f, 2);
                }

                if (Input.AcceptPressed())
                {
                    Wait(150);
                    GraffitiMethods.IsInFinalDecisionMode = true;

                    if (GraffitiMethods.LastHitVehicle == null)
                    {
                        if (GraffitiMethods.AllGraffitiInMap == null) { GraffitiMethods.AllGraffitiInMap = new List<ExistingGraffitiInMap>(); }
                        GraffitiMethods.AllGraffitiInMap.Add(new ExistingGraffitiInMap(
                            GraffitiMethods.AllAddonGraffiti.Any(g => g.TextureDictionary == GraffitiMethods.LastSelectedGraffiti.TextureDictionary && g.TextureName == GraffitiMethods.LastSelectedGraffiti.TextureName) ?
                            GraffitiMethods.AllAddonGraffiti.First(g => g.TextureDictionary == GraffitiMethods.LastSelectedGraffiti.TextureDictionary && g.TextureName == GraffitiMethods.LastSelectedGraffiti.TextureName) : GraffitiMethods.LastSelectedGraffiti,
                            // new Graffiti(GraffitiMethods.LastSelectedGraffiti.TextureDictionary, GraffitiMethods.LastSelectedGraffiti.TextureName, GraffitiMethods.LastSelectedGraffiti.TextureWidth, GraffitiMethods.LastSelectedGraffiti.TextureHeight),
                            GraffitiMethods.LastHitLocation, GraffitiMethods.LastHitDirection, GraffitiMethods.LastHitRotationAngle, GraffitiMethods.LastHitGrafSize.X, GraffitiMethods.LastHitGrafSize.Y));
                    }
                    else
                    {
                        GraffitiMethods.CurrentEditVehicle = GraffitiMethods.LastHitVehicle;
                        Vector3 pos1 = GraffitiMethods.LastHitVehicle.GetPositionOffset(GraffitiMethods.LastHitLocation);
                        Vector3 pos2 = GraffitiMethods.LastHitVehicle.GetPositionOffset(GraffitiMethods.LastHitLocation - GraffitiMethods.LastHitDirection * 0.1f);
                        GraffitiMethods.CurrentEditOutfit.DecalList.Add(new SingleVehicleDecal(GraffitiMethods.LastHitVehicle, GraffitiMethods.LastSelectedGraffiti, pos1, pos2, GraffitiMethods.LastHitRotationAngle, GraffitiMethods.LastHitGrafSize.X, GraffitiMethods.LastHitGrafSize.Y));
                    }

                    GTAFunction.ClearAllHelpMessages();
                    HelpText.DrawingConfirmation();
                }

                while (GraffitiMethods.IsInFinalDecisionMode)
                {
                    WorldGraffitiController();
                    GraffitiMethods.SeeInfoMode = false;

                    if (Input.AcceptPressed())
                    {
                        if (GraffitiMethods.LastHitVehicle == null)
                        {
                            GraffitiMethods.SaveWorldGraffitiToFile();
                            while (true)
                            {
                                //DisposeMenusInExistingArtMenu(); // Gonna re-do these two, no need to reset the whole menu!
                                //SetupExistingArtMenu();
                                AddSavedExistingGraffitiToExistingArtMenu(GraffitiMethods.AllGraffitiInMap.Last());
                                break;
                            }
                        }
                        else
                        {
                            int savedOutfitIndex;
                            bool outfitIsNew;
                            GraffitiMethods.SaveCurrentEditOutfitPermanently(out savedOutfitIndex, out outfitIsNew);
                            while (true)
                            {
                                /*DisposeMenusInVehicleSavedOutfitMenu();
                                SetupVehicleSavedOutfitMenu();*/

                                AddSavedOutfitToVehicleSavedOutfitMenu(GraffitiMethods.AllVehicleGraffitiOutfits[savedOutfitIndex]);

                                if (outfitIsNew) break; // Only delete the outfit at the savedOutfitIndex if editing an existing outfit (this is required because a brand new outfit menu was just added regardless of whether the outfit is new or not)
                                
                                DeleteSavedOutfitMenuFromParentMenu(GetOutfitMenuFromSavedOutfit(GraffitiMethods.AllVehicleGraffitiOutfits[savedOutfitIndex]));
                                break;
                            }
                        }
                        
                        GraffitiMethods.IsInFinalDecisionMode = false;
                        GraffitiMethods.IsInTaggingMode = false;
                        GTAFunction.ClearAllHelpMessages();
                        Wait(100);
                        _menuPool.OpenCloseLastMenu();
                        break;
                    }
                    if (Input.CancelPressed() || Game.Player.Character.IsDead)
                    {
                        while (true)
                        {
                            if (GraffitiMethods.LastHitVehicle == null)
                            {
                                GraffitiMethods.AllGraffitiInMap.Last().Remove();
                                GraffitiMethods.AllGraffitiInMap.Remove(GraffitiMethods.AllGraffitiInMap.Last());
                            }
                            else
                            {
                                if (GraffitiMethods.CurrentEditOutfit.DecalList.Count > 1)
                                {
                                    GraffitiMethods.CurrentEditOutfit.DecalList.Last().Remove(); // Remove physical decal first
                                    GraffitiMethods.CurrentEditOutfit.DecalList.Remove(GraffitiMethods.CurrentEditOutfit.DecalList.Last()); // Remove decal from list
                                }
                                else
                                {
                                    GraffitiMethods.LoadedVehicleOutfits.Last().RemoveFromVehicle(); // Remove all decals and set the vehicle to null
                                    GraffitiMethods.LoadedVehicleOutfits.Last().DecalList.Clear();
                                    GraffitiMethods.LoadedVehicleOutfits.Remove(GraffitiMethods.LoadedVehicleOutfits.Last()); // Remove entire outfit from loaded outfits
                                }
                            }

                            GraffitiMethods.RefreshAllGraffiti();
                            break;
                        }
                        GraffitiMethods.IsInFinalDecisionMode = false;
                        GTAFunction.ClearAllHelpMessages();
                        HelpText.DrawingInstructions();
                        Wait(100);
                        break;
                    }

                    Script.Yield();
                }

                Script.Yield();
            }
        }

        DateTime refreshAllTimer = DateTime.Now.AddMilliseconds(20000d);
        void WorldGraffitiController()
        {
            if (Game.Player.IsAlive)
            {
                if (GraffitiMethods.AllGraffitiInMap != null && GraffitiMethods.AllGraffitiInMap.Count > 0)
                {
                    foreach (var graf in GraffitiMethods.AllGraffitiInMap.ToList())
                    {
                        graf.ManageInWorld();
                    }
                }

                if (GraffitiMethods.LoadedVehicleOutfits != null && GraffitiMethods.LoadedVehicleOutfits.Count > 0)
                {
                    foreach (var outfit in GraffitiMethods.LoadedVehicleOutfits.ToList())
                    {
                        outfit.ManageOutfit();
                    }
                }

                if (refreshAllTimer < DateTime.Now)
                {
                    GraffitiMethods.RemoveUnusedLoadedOutfits();
                    GraffitiMethods.FreeAnyUnusedDecalTypes();
                    if (GraffitiMethods.SeeInfoMode)
                    {
                        GTA.UI.Screen.ShowSubtitle(
                            "Report: " + GraffitiMethods.UsableDecalTypes.Where(g => !g.Available).ToList().Count() +
                            "/" + GraffitiMethods.UsableDecalTypes.Count + " decalTypes used in this area.", 10000);
                    }
                    refreshAllTimer = DateTime.Now.AddMilliseconds(20000d);
                }
            }
        }

        string AddKeyCodeToString(string str, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q:
                case Keys.W:
                case Keys.E:
                case Keys.R:
                case Keys.T:
                case Keys.Y:
                case Keys.U:
                case Keys.I:
                case Keys.O:
                case Keys.P:
                case Keys.A:
                case Keys.S:
                case Keys.D:
                case Keys.F:
                case Keys.G:
                case Keys.H:
                case Keys.J:
                case Keys.K:
                case Keys.L:
                case Keys.Z:
                case Keys.X:
                case Keys.C:
                case Keys.V:
                case Keys.B:
                case Keys.N:
                case Keys.M:
                    str += (new KeysConverter()).ConvertToString(e.KeyCode);
                    break;
                case Keys.Delete:
                    if (str.Length > 0) str = str.Remove(str.Length - 1);
                    break;
                //case Keys.NumPad0:
                case Keys.D0:
                    str += e.Shift ? ")" : "0";
                    break;
                //case Keys.NumPad1:
                case Keys.D1:
                    str += e.Shift ? "!" : "1";
                    break;
                //case Keys.NumPad2:
                case Keys.D2:
                    str += e.Shift ? "@" : "2";
                    break;
                //case Keys.NumPad3:
                case Keys.D3:
                    str += e.Shift ? "#" : "3";
                    break;
                //case Keys.NumPad4:
                case Keys.D4:
                    str += e.Shift ? "$" : "4";
                    break;
                //case Keys.NumPad5:
                case Keys.D5:
                    str += e.Shift ? "%" : "5";
                    break;
                //case Keys.NumPad6:
                case Keys.D6:
                    str += e.Shift ? "^" : "6";
                    break;
                //case Keys.NumPad7:
                case Keys.D7:
                    str += e.Shift ? "&" : "7";
                    break;
                //case Keys.NumPad8:
                case Keys.D8:
                    str += e.Shift ? "*" : "8";
                    break;
                //case Keys.NumPad9:
                case Keys.D9:
                    str += e.Shift ? "(" : "9";
                    break;
                case Keys.Oemcomma:
                    str += e.Shift ? "<" : ",";
                    break;
                case Keys.OemPeriod:
                case Keys.Decimal:
                    str += e.Shift ? ">" : ".";
                    break;
                case Keys.Space:
                    str += " ";
                    break;
                case Keys.Subtract:
                    str += "-";
                    break;
                case Keys.OemMinus:
                    str += e.Shift ? "_" : "-";
                    break;
                case Keys.Add:
                case Keys.Oemplus:
                    str += e.Shift ? "+" : "=";
                    break;
                case Keys.OemQuestion:
                case Keys.Divide:
                    str += e.Shift ? "?" : "/";
                    break;
                case Keys.Multiply:
                    str += "*";
                    break;
                case Keys.Oem1: // is the ; and : key
                    str += e.Shift ? ":" : ";";
                    break;
                case Keys.OemOpenBrackets: // is the [ and { key
                    str += e.Shift ? "{" : "[";
                    break;
                case Keys.Oem5: // is the \ and | key
                    str += e.Shift ? "\\" : "|";
                    break;
                case Keys.Oem6: // is the ] and } key
                    str += e.Shift ? "}" : "]";
                    break;
                case Keys.Oem7: // is the " and ' key
                    str += e.Shift ? "\"" : "'";
                    break;
                case Keys.Oemtilde: // is the ` and ~ key
                    str += e.Shift ? "~" : "`";
                    break;
                default:
                    return str;
            }
            return str;
        }

        void AdjustMenuToSearchbar(UIMenu menu, UIMenuItem searchbar, ref string searchbarString, KeyEventArgs e, bool menuNeverChanges)
        {
            string oldString = searchbarString;
            searchbarString = AddKeyCodeToString(searchbarString, e);

            if (oldString.Equals(searchbarString)) return;

            if (string.IsNullOrWhiteSpace(searchbarString))
            {
                searchbar.Text = "Search ~r~:~s~ ";
                menu.ReenableAllItems();

                if (!menuNeverChanges)
                {
                    menu.ResetOriginalOrder();
                }
                else
                {
                    menu.SortMenuItemsByOriginalOrder();
                }
            }
            else
            {
                searchbar.Text = "Search ~r~:~s~ " + searchbarString;

                List<UIMenuItem> enabledList = new List<UIMenuItem>();
                enabledList.AddRange(menu.UIMenuItemList.Where((x, i) => x.PersistentIndex >= 2));
                var disabledList = menu.DisabledList.ToList();

                if (enabledList != null && enabledList.Count > 0)
                {
                    foreach (UIMenuItem eItem in enabledList)
                    {
                        if ((!string.IsNullOrWhiteSpace(eItem.Text) && eItem.Text.Contains(searchbarString, StringComparison.OrdinalIgnoreCase)) || (!string.IsNullOrWhiteSpace(eItem.Description) && eItem.Description.Contains(searchbarString, StringComparison.OrdinalIgnoreCase))) continue;
                        menu.DisableItem(eItem);
                    }
                }

                foreach (var dItem in disabledList)
                {
                    if ((!string.IsNullOrWhiteSpace(dItem.Text) && dItem.Text.Contains(searchbarString, StringComparison.OrdinalIgnoreCase)) || (!string.IsNullOrWhiteSpace(dItem.Description) && dItem.Description.Contains(searchbarString, StringComparison.OrdinalIgnoreCase)))
                        menu.ReenableItem(dItem);
                }

                menu.SortMenuItemsByOriginalOrder();
            }
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
        }

        void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!menuInit) return;

            if (e.KeyCode == GraffitiMethods.MyScriptSettings.MenuKey && !ScriptComm.ScriptCommunicatorMenuDllExists())
            {
                _menuPool.OpenCloseLastMenu();
            }
            
            if (DrawMenu.IsVisible && drawMenuSearchbar == DrawMenu.SelectedItem)
            {
                AdjustMenuToSearchbar(DrawMenu, drawMenuSearchbar, ref drawMenuSearchbarString, e, true);
            }

            if (VehDecalOutfitMenu.IsVisible && SavedOutfitsSearchbar == VehDecalOutfitMenu.SelectedItem)
            {
                AdjustMenuToSearchbar(VehDecalOutfitMenu, SavedOutfitsSearchbar, ref savedOutfitSearchbarString, e, false);
            }
        }
    }

    public static class HelpText
    {
        public static void DrawingInstructions()
        {
            GTAFunction.DisplayHelpTextThisFrame("~h~Controls~h~"
                    + "\nRotation " + GTAFunction.InputString(GTA.Control.PhoneLeft) + " " + GTAFunction.InputString(GTA.Control.PhoneRight)
                    + "\nSize " + GTAFunction.InputString(GTA.Control.PhoneUp) + " " + GTAFunction.InputString(GTA.Control.PhoneDown)
                    + "\nSpeed modifier " + (Input.IsKeyboard() ? "~y~" + Keys.ShiftKey.ToString() + " ~s~or ~y~" + Keys.Space.ToString() + "~s~" : GTAFunction.InputString(GTA.Control.Aim) + " " + GTAFunction.InputString(GTA.Control.Attack)) + " (hold)"
                    , true, false);
        }

        public static void DrawingConfirmation()
        {
            GTAFunction.DisplayHelpTextThisFrame("Place Decal " + GTAFunction.InputString(GTA.Control.PhoneSelect)
                        + "\nCancel " + GTAFunction.InputString(GTA.Control.PhoneCancel)
                        , true, false);
        }

        public static void DeletionInstructions()
        {
            GTAFunction.DisplayHelpTextThisFrame("Delete " + GTAFunction.InputString(GTA.Control.PhoneSelect)
                + "\nExit" + GTAFunction.InputString(GTA.Control.PhoneCancel)
                , true, false);
        }

        public static void DeletionConfirmation()
        {
            GTAFunction.DisplayHelpTextThisFrame("Confirm Deletion " + GTAFunction.InputString(GTA.Control.PhoneSelect)
                + "\nCancel Deletion" + GTAFunction.InputString(GTA.Control.PhoneCancel)
                , true, false);
        }

        public static void MissingTextureQuestion()
        {
            GTAFunction.DisplayHelpTextThisFrame("Some decals in your map and/or vehicle outfits no longer exists.\nWould you like to remove the missing decals?"
                + "\nYes " + GTAFunction.InputString(GTA.Control.PhoneSelect)
                + "\nNo" + GTAFunction.InputString(GTA.Control.PhoneCancel)
                , true, false);
        }
    }

    public static class Input
    {
        public static List<GTA.Control> ControlsToEnable = new List<GTA.Control>
            {
                GTA.Control.MoveUpDown,
                GTA.Control.MoveLeftRight,
                GTA.Control.Sprint,
                //GTA.Control.Jump,
                //GTA.Control.Enter,
                //GTA.Control.VehicleExit,
                //GTA.Control.VehicleAccelerate,
                //GTA.Control.VehicleBrake,
                //GTA.Control.VehicleMoveLeftRight,
                //GTA.Control.VehicleFlyYawLeft,
                //GTA.Control.FlyLeftRight,
                //GTA.Control.FlyUpDown,
                //GTA.Control.VehicleFlyYawRight,
                //GTA.Control.VehicleHandbrake,
                GTA.Control.NextCamera,
                GTA.Control.LookUpDown,
                GTA.Control.LookLeftRight
            };

        public static void DisableControlsWhileTagging()
        {
            Game.DisableAllControlsThisFrame();
            
            foreach (var con in ControlsToEnable)
            {
                Game.EnableControlThisFrame(con);
            }

            //DisableCameraControlsWhenAppropriate();
        }

        private static void DisableCameraControlsWhenAppropriate()
        {
            if (!IsHoldingRotationModifier())
                return;

            Game.DisableControlThisFrame(GTA.Control.LookUpDown);
            Game.DisableControlThisFrame(GTA.Control.LookLeftRight);
        }

        public static void DisableAllButCamera(int index)
        {
            Game.DisableAllControlsThisFrame();
            Game.EnableControlThisFrame(GTA.Control.LookUpDown);
            Game.EnableControlThisFrame(GTA.Control.LookLeftRight);
        }

        public static bool IsKeyboard()
        {
            return Game.LastInputMethod == InputMethod.MouseAndKeyboard;
        }

        public static DateTime Timer = DateTime.Now;

        public static bool CanAcceptInput()
        {
            return Timer < DateTime.Now;
        }

        public static void SetInputWait(int milliseconds)
        {
            Timer = DateTime.Now.AddMilliseconds(milliseconds);
        }

        public static bool IsHoldingRotationModifier()
        {
            return !IsKeyboard() && Game.IsControlPressed(GTA.Control.PhoneDown);
        }

        public static bool IsHoldingSpeedup()
        {
            return Game.IsKeyPressed(Keys.ShiftKey) || Game.IsControlPressed(GTA.Control.Aim);
        }

        public static bool IsHoldingSuperSpeedup()
        {
            return Game.IsKeyPressed(Keys.Space) || Game.IsControlPressed(GTA.Control.Attack);
        }

        public static void SetChangeUnits(float normal, float fast, float superfast)
        {
            if (IsHoldingSuperSpeedup())
            {
                GraffitiMethods.ChangeUnit = superfast;
            }
            else if (IsHoldingSpeedup())
            {
                GraffitiMethods.ChangeUnit = fast;
            }
            else
            {
                GraffitiMethods.ChangeUnit = normal;
            }
        }

        public static bool MoveRotationClockwise()
        {
            if (CanAcceptInput())
            {
                if (IsKeyboard())
                {
                    if (Game.IsKeyPressed(Keys.Right))
                    {
                        SetInputWait(100);
                        return true;
                    }
                }
                else
                {
                    if (/*IsHoldingRotationModifier() && */Game.IsControlPressed(GTA.Control.PhoneRight))
                    {
                        SetInputWait(100);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool MoveRotationCounterClockwise()
        {
            if (CanAcceptInput())
            {
                if (IsKeyboard())
                {
                    if (Game.IsKeyPressed(Keys.Left))
                    {
                        SetInputWait(100);
                        return true;
                    }
                }
                else
                {
                    if (/*IsHoldingRotationModifier() && */Game.IsControlPressed(GTA.Control.PhoneLeft))
                    {
                        SetInputWait(100);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool IncreaseSize()
        {
            if (CanAcceptInput())
            {
                if (IsKeyboard())
                {
                    if (Game.IsKeyPressed(Keys.Up))
                    {
                        SetInputWait(100);
                        return true;
                    }
                }
                else
                {
                    if (/*IsHoldingRotationModifier() && */Game.IsControlPressed(GTA.Control.PhoneUp))
                    {
                        SetInputWait(100);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool DecreaseSize()
        {
            if (CanAcceptInput())
            {
                if (IsKeyboard())
                {
                    if (Game.IsKeyPressed(Keys.Down))
                    {
                        SetInputWait(100);
                        return true;
                    }
                }
                else
                {
                    if (/*IsHoldingRotationModifier() && */Game.IsControlPressed(GTA.Control.PhoneDown))
                    {
                        SetInputWait(100);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CancelPressed()
        {
            if (CanAcceptInput())
            {
                if (IsKeyboard())
                {
                    if (Game.IsKeyPressed(Keys.Back) || Game.IsKeyPressed(Keys.Escape) ||
                        Game.IsControlPressed(GTA.Control.CursorCancel))
                    {
                        SetInputWait(100);
                        return true;
                    }
                }
                else
                {
                    if (Game.IsControlPressed(GTA.Control.PhoneCancel))
                    {
                        SetInputWait(100);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool AcceptPressed()
        {
            if (CanAcceptInput())
            {
                if (Game.IsControlPressed(GTA.Control.PhoneSelect))
                {
                    SetInputWait(100);
                    return true;
                }
            }
            return false;
        }
    }
}