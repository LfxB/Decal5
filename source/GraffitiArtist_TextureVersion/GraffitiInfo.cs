using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GTA;
using GTA.Math;
using GTAMath;
using ModSettings;

namespace GraffitiArtist
{
    public class AvailableDecalType
    {
        public int DecalTypeID { get; set; }
        public bool Available = true;

        public AvailableDecalType(int type)
        {
            DecalTypeID = type;
        }
    }

    public static class GraffitiMethods
    {
        const string GraffitiModPath = @"scripts\Graffiti Mod\";

        const string ImportsPath = GraffitiModPath + "Imports\\";

        public const string ExportsPath = GraffitiModPath + "VehicleOutfits\\";

        #region ADDON_GRAFFITI_SAVING_AND_LOADING

        public const string TextureListFilename = "GraffitiTextureList.xml";

        public static List<Graffiti> AllAddonGraffiti = new List<Graffiti>();

        public static void SaveAddonGraffitiToFile(string path = GraffitiModPath + TextureListFilename)
        {
            XMLHelper.SaveObjectToXML(AllAddonGraffiti, path);
        }

        public static void LoadAddonGraffitiFromFile(string path = GraffitiModPath + TextureListFilename)
        {
            AllAddonGraffiti = XMLHelper.LoadXMLToObject(AllAddonGraffiti, path);
        }

        #endregion

        #region EXISTING_GRAFFITI_SAVING_AND_LOADING

        public const string GraffitiPlacementListFilename = "GraffitiPlacementList.xml";

        public static List<ExistingGraffitiInMap> AllGraffitiInMap = new List<ExistingGraffitiInMap>();

        public static void SaveWorldGraffitiToFile(string path = GraffitiModPath + GraffitiPlacementListFilename)
        {
            XMLHelper.SaveObjectToXML(AllGraffitiInMap, path);
        }

        public static void LoadWorldGraffitiFromFile(string path = GraffitiModPath + GraffitiPlacementListFilename)
        {
            AllGraffitiInMap = XMLHelper.LoadXMLToObject(AllGraffitiInMap, path);
        }

        #endregion

        #region VEHICLE_GRAFFITI_SAVING_AND_LOADING

        public static List<DecaledVehicleOutfit> AllVehicleGraffitiOutfits = new List<DecaledVehicleOutfit>();

        public static List<DecaledVehicleOutfit> LoadedVehicleOutfits = new List<DecaledVehicleOutfit>();

        public static void LoadAllVehicleOutfitsFromFolder()
        {
            string[] vehicleOutfitFilePaths = System.IO.Directory.GetFiles(ExportsPath, "*.xml");

            foreach (string outfitFilePath in vehicleOutfitFilePaths) // Example of outfitFilePath: scripts/Graffiti Mod/Imports/Itasha Red.outfit
            {
                DecaledVehicleOutfit outfit = XMLHelper.LoadXMLToObject(new DecaledVehicleOutfit(), outfitFilePath); // outfit would be the object deserialized from Itasha Red.outfit

                if (outfit == null || outfit == default(DecaledVehicleOutfit)) continue; // Skip if the outfit is null or default

                if (AllVehicleGraffitiOutfits.Any(existing => existing.OutfitName.Equals(outfit.OutfitName))) continue; // Skip if the oufit already exists

                AllVehicleGraffitiOutfits.Add(outfit); // Add outfit to main outfit list
            }
        }
        
        public static void DeleteSingleVehicleOutfit(DecaledVehicleOutfit outfit)
        {
            string fileName = outfit.OutfitName;
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }

            string filepath = ExportsPath + fileName + ".xml";

            if (System.IO.File.Exists(filepath))
            {
                System.IO.File.Delete(filepath);
            }
        }

        public static void SaveSingleVehicleOutfit(DecaledVehicleOutfit outfit)
        {
            string fileName = outfit.OutfitName;
            foreach (var c in System.IO.Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(c, '-');
            }

            string filepath = ExportsPath + fileName + ".xml";

            XMLHelper.SaveObjectToXML(outfit, filepath); // If a file already exists, it will be overwritten.
            //UI.ShowSubtitle("Outfit Exported to: \n~y~" + filepath);
        }

        #endregion

        #region IMPORT_MAP_AND_OUTFIT_DECALS

        public static void ImportFromImportsFolder()
        {
            bool somethingAdded = false;

            /*string[] vehicleOutfitFilePaths = System.IO.Directory.GetFiles(ImportsPath, "*.outfit");

            foreach (string outfitFilePath in vehicleOutfitFilePaths) // Example of outfitFilePath: scripts/Graffiti Mod/Imports/Itasha Red.outfit
            {
                DecaledVehicleOutfit outfit = XMLHelper.LoadXMLToObject(new DecaledVehicleOutfit(), outfitFilePath); // outfit would be the object deserialized from Itasha Red.outfit

                if (outfit == null || outfit == default(DecaledVehicleOutfit)) continue; // Skip if the outfit is null or default

                if (AllVehicleGraffitiOutfits.Any(existing => existing.OutfitName.Equals(outfit.OutfitName))) continue; // Skip if the oufit already exists

                AllVehicleGraffitiOutfits.Add(outfit); // Add outfit to main outfit list
                somethingAdded = true;

                try
                {
                    System.IO.File.Move(outfitFilePath, outfitFilePath + ".added"); // Adds '.added' to the end of the file name so the outfit will be ignored next time around. Ex: Itasha Red.outfit.added
                }
                catch
                {
                    System.IO.File.Move(outfitFilePath, outfitFilePath + ".added." + DateTime.Now.ToString("MMMM dd',' yyyy hh'-'mm'-'ss")); // Ex: Itasha Red.outfit.added.January 01, 2018, 01-27-40
                }
            }*/
            
            string[] mapGraffitiList = System.IO.Directory.GetFiles(ImportsPath, "*.xml");

            foreach (string pathOfMapXml in mapGraffitiList) // Example of pathOfMapXml: scripts/Graffiti Mod/Imports/CoolGraffitiList.xml
            {
                List<ExistingGraffitiInMap> singleMapList = XMLHelper.LoadXMLToObject(AllGraffitiInMap, pathOfMapXml); // singleMapList would be the object deserialized from CoolGraffitiList.xml

                if (singleMapList == null || singleMapList == default(List<ExistingGraffitiInMap>)) continue;

                singleMapList.ForEach(l => AllGraffitiInMap.Add(l)); // Add each ExistingGraffitiInMap object from the imported list (Ex: CoolGraffitiList.xml) to the main AllGraffitiInMap list (GraffitiPlacementList.xml)
                somethingAdded = true;

                try
                {
                    System.IO.File.Move(pathOfMapXml, pathOfMapXml + ".added"); // Adds '.added' to the end of the file name so the outfit will be ignored next time around. Ex: CoolGraffitiList.xml.added
                }
                catch
                {
                    System.IO.File.Move(pathOfMapXml, pathOfMapXml + ".added." + DateTime.Now.ToString("MMMM dd',' yyyy hh'-'mm'-'ss")); // Ex: CoolGraffitiList.xml.added.January 01, 2018, 01-27-40
                }
            }

            if (somethingAdded)
            {
                SaveWorldGraffitiToFile();
                //SaveAllVehicleGraffitiToFile();
            }
        }

        #endregion
        
        public static readonly List<AvailableDecalType> UsableDecalTypes = new List<AvailableDecalType>()
        {
        };

        /// <summary>
        /// Creates the list of available decal types the script can use. Defined in 'Grand Theft Auto V\mods\update\update.rpf\common\data\effects\decals.dat'
        /// Must be called at the start of the script.
        /// </summary>
        public static void InitDecalTypes()
        {
            if (System.IO.File.Exists(GraffitiModPath + "decalids.txt"))
            {
                var lines = System.IO.File.ReadLines(GraffitiModPath + "decalids.txt");

                foreach (string line in lines)
                {
                    int id;
                    if (int.TryParse(line, out id))
                    {
                        if (id != 9007)
                            UsableDecalTypes.Add(new AvailableDecalType(id));
                    }
                }
            }
            else
            {
                for (int i = 10000; i < 10048; i++)
                {
                    UsableDecalTypes.Add(new AvailableDecalType(i));
                }

                UsableDecalTypes.Add(new AvailableDecalType(9100));
                UsableDecalTypes.Add(new AvailableDecalType(9101));
                UsableDecalTypes.Add(new AvailableDecalType(9102));
                UsableDecalTypes.Add(new AvailableDecalType(9103));
                UsableDecalTypes.Add(new AvailableDecalType(9104));
                UsableDecalTypes.Add(new AvailableDecalType(9106));
                UsableDecalTypes.Add(new AvailableDecalType(9107));
                UsableDecalTypes.Add(new AvailableDecalType(9108));
                UsableDecalTypes.Add(new AvailableDecalType(9110));
                UsableDecalTypes.Add(new AvailableDecalType(9111));
                UsableDecalTypes.Add(new AvailableDecalType(9112));
                UsableDecalTypes.Add(new AvailableDecalType(9115));
                UsableDecalTypes.Add(new AvailableDecalType(9116));
                UsableDecalTypes.Add(new AvailableDecalType(9117));
                UsableDecalTypes.Add(new AvailableDecalType(9118));
                UsableDecalTypes.Add(new AvailableDecalType(9119));
                UsableDecalTypes.Add(new AvailableDecalType(9123));

                UsableDecalTypes.Add(new AvailableDecalType(9002));
                UsableDecalTypes.Add(new AvailableDecalType(9004));
                UsableDecalTypes.Add(new AvailableDecalType(9006));
                //UsableDecalTypes.Add(new AvailableDecalType(9007)); // Reserved for temporary decal draw.
                UsableDecalTypes.Add(new AvailableDecalType(9008));
                UsableDecalTypes.Add(new AvailableDecalType(9009));
            }

        }

        public static bool AvailableDecalTypeExists()
        {
            /*foreach (AvailableDecalType type in UsableDecalTypes)
            {
                if (type.Available == true)
                {
                    indexOfType = UsableDecalTypes.IndexOf(type);
                    type.Available = false;
                    return true;
                }
            }
            
            indexOfType = -1;
            return false;*/
            //AvailableDecalType d = UsableDecalTypes.FirstOrDefault(t => t.Available);
            //if (d != default(AvailableDecalType) /*null*/)
            AvailableDecalType d = UsableDecalTypes.First(t => t.Available);
            if (d != null)
            {
                return true;
            }
            return false;
        }

        public static int NextAvailableDecalType()
        {
            /*foreach (AvailableDecalType type in UsableDecalTypes)
            {
                if (type.Available == true)
                {
                    indexOfType = UsableDecalTypes.IndexOf(type);
                    type.Available = false;
                    return true;
                }
            }
            
            indexOfType = -1;
            return false;*/
            //AvailableDecalType d = UsableDecalTypes.FirstOrDefault(t => t.Available);
            //if (d != default(AvailableDecalType) /*null*/)
            AvailableDecalType d = UsableDecalTypes.First(t => t.Available);
            if (d != null)
            {
                return UsableDecalTypes.IndexOf(d);
            }
            return -1;
        }

        public static void SetAllUsableDecalTypesAvailable()
        {
            foreach (var dt in UsableDecalTypes)
            {
                dt.Available = true;
            }
        }

        public static void FreeAnyUnusedDecalTypes()
        {
            foreach (var type in UsableDecalTypes)
            {
                /*if (!AllGraffitiInMap.Any(g => g.Exists() && type == UsableDecalTypes[g.Graffiti.DecalTypeIndex])
                    && !LoadedVehicleOutfits.Any(o => o.DecalList.Any(d => d.Exists() && type == UsableDecalTypes[d.Graffiti.DecalTypeIndex])))
                {
                    type.Available = true;
                }*/

                if (AllAddonGraffiti.Any(g => g.DecalTypeIndex != -1 && type == UsableDecalTypes[g.DecalTypeIndex] && g.LoadedCount < 1)) // If there is any graffiti using this type, and there are no loaded decals, then make the decalType available
                {
                    type.Available = true;
                }
            }
        }

        public static void RefreshAllGraffiti()
        {
            if (GraffitiMethods.AllGraffitiInMap != null && GraffitiMethods.AllGraffitiInMap.Count > 0)
            {
                foreach (var g in AllGraffitiInMap.ToList())
                {
                    g.Remove();
                }
            }

            if (GraffitiMethods.LoadedVehicleOutfits != null && GraffitiMethods.LoadedVehicleOutfits.Count > 0)
            {
                foreach (var outfit in LoadedVehicleOutfits.ToList())
                {
                    foreach (var decal in outfit.DecalList.ToList())
                    {
                        decal.Remove();
                    }
                }
            }

            SetAllUsableDecalTypesAvailable();
        }

        /// <summary>
        /// Takes the Graffiti property in each map graffiti and vehicle outfit decal
        /// and makes them references to their matching Graffiti object in the
        /// AllAddonGraffiti list.
        /// </summary>
        public static void InitGraffitiReferences()
        {
            foreach (var existingGraf in AllGraffitiInMap)
            {
                Graffiti addonGraffiti = AllAddonGraffiti.First(g => g.TextureDictionary == existingGraf.Graffiti.TextureDictionary && g.TextureName == existingGraf.Graffiti.TextureName);
                if (addonGraffiti != null && addonGraffiti != default(Graffiti))
                {
                    existingGraf.Graffiti = addonGraffiti;
                }
            }

            foreach (var outfit in AllVehicleGraffitiOutfits)
            {
                foreach (var decal in outfit.DecalList)
                {
                    Graffiti addonGraffiti = AllAddonGraffiti.First(g => g.TextureDictionary == decal.Graffiti.TextureDictionary && g.TextureName == decal.Graffiti.TextureName);
                    if (addonGraffiti != null && addonGraffiti != default(Graffiti))
                    {
                        decal.Graffiti = addonGraffiti;
                    }
                }
            }
        }

        public static void RemoveUnusedLoadedOutfits()
        {
            if (GraffitiMethods.LoadedVehicleOutfits != null && GraffitiMethods.LoadedVehicleOutfits.Count > 0)
            {
                foreach (var outfit in LoadedVehicleOutfits.ToList())
                {
                    if (outfit.CurrentVehicle == null || !outfit.CurrentVehicle.Exists())
                    {
                        foreach (var decal in outfit.DecalList.ToList())
                        {
                            decal.Remove();
                        }
                        outfit.DecalList.Clear();
                        LoadedVehicleOutfits.Remove(outfit);
                    }
                }
            }
        }

        public enum MirrorFlip
        {
            None,
            Mirror,
            Mirror2,
            Flip,
            Flip2
        }

        public static GraffitiScriptSettings MyScriptSettings;

        public static bool IsInTaggingMode = false;

        public static bool IsInFinalDecisionMode = false;

        public static bool SeeInfoMode;

        public static Graffiti LastSelectedGraffiti;

        public static Vehicle LastHitVehicle;

        public static Vector3 LastHitLocation;

        public static Vector3 LastHitDirection;

        public static float LastHitRotationAngle;

        public static Vector2 LastHitGrafSize;

        public static int LastHitDecalTypeIndex = 0;

        public static float RotationAngle = 0f;

        public static float WidthHeightSizeCoeff = 1.0f;

        public static float ChangeUnit = 1f;

        #region VEHICLE_SPECIFIC

        private static Vehicle _currentEditVehicle;

        public static Vehicle CurrentEditVehicle
        {
            get { return _currentEditVehicle; }
            set
            {
                if (value != null /*&& value != _currentEditVehicle*/)
                {
                    // Find outfit that is assigned to this vehicle. If none exists, create a new one.
                    if (LoadedVehicleOutfits == null) { LoadedVehicleOutfits = new List<DecaledVehicleOutfit>(); }
                    DecaledVehicleOutfit outfit = LoadedVehicleOutfits.FirstOrDefault(o => o.CurrentVehicle == value);

                    if (outfit != default(DecaledVehicleOutfit))
                    {
                        CurrentEditOutfit = outfit;
                    }
                    else
                    {
                        outfit = new DecaledVehicleOutfit("Unnamed - " + DateTime.Now.ToString("MMMM dd',' yyyy hh':'mm':'ss"));
                        value.Model.Request(250);
                        outfit.SetVehicleInfo(value);
                        LoadedVehicleOutfits.Add(outfit);
                        CurrentEditOutfit = outfit;
                    }
                    CurrentEditOutfit.ApplyToVehicle(value);
                }
                _currentEditVehicle = value;
            }
        }

        public static DecaledVehicleOutfit CurrentEditOutfit;

        public static void SaveCurrentEditOutfitPermanently(out int indexOfSavedOutfit, out bool isNewOufit)
        {
            if (AllVehicleGraffitiOutfits == null) { AllVehicleGraffitiOutfits = new List<DecaledVehicleOutfit>(); }
            DecaledVehicleOutfit savedOutfit = AllVehicleGraffitiOutfits.FirstOrDefault(o => CurrentEditOutfit.OutfitName.Equals(o.OutfitName) && CurrentEditOutfit.VehicleModelHash == o.VehicleModelHash);

            // If the current edit outfit already exists in the saved outfit list..
            if (savedOutfit != default(DecaledVehicleOutfit))
            {
                savedOutfit.DecalList = CurrentEditOutfit.Clone().DecalList;

                // Also affect other loaded outfits derived from the saved outfit.
                foreach (var outfit in LoadedVehicleOutfits)
                {
                    if (CurrentEditOutfit.OutfitName.Equals(outfit.OutfitName) && CurrentEditOutfit.VehicleModelHash == outfit.VehicleModelHash)
                    {
                        foreach (var decal in outfit.DecalList)
                        {
                            decal.Remove();
                        }
                        outfit.DecalList = CurrentEditOutfit.Clone().DecalList;

                        // Makes sure the decals from this outfit point to its specific vehicle
                        // instead of the vehicle from the CurrentEditOutfit outfit.
                        outfit.ApplyToVehicle(outfit.CurrentVehicle);
                    }
                }

                indexOfSavedOutfit = AllVehicleGraffitiOutfits.IndexOf(savedOutfit);
                isNewOufit = false;
            }
            // If the current edit outfit does NOT exist in the saved outfit list..
            else
            {
                savedOutfit = CurrentEditOutfit.Clone();
                AllVehicleGraffitiOutfits.Add(savedOutfit);
                indexOfSavedOutfit = AllVehicleGraffitiOutfits.Count - 1;
                isNewOufit = true;
            }

            //SaveAllVehicleGraffitiToFile();
            SaveSingleVehicleOutfit(savedOutfit);

            UI.ShowSubtitle("Outfit saved: " + CurrentEditOutfit.OutfitName, 8000);
        }

        #endregion

        public static bool DisplayTemporaryGraffiti()
        {
            if (UsableDecalTypes.Any(d => d.Available)) //((LastHitDecalTypeIndex != -1 && UsableDecalTypes[LastHitDecalTypeIndex].Available) || NextAvailableDecalType(out LastHitDecalTypeIndex))
            {
                RaycastResult ray = World.Raycast(GameplayCamera.Position, GameplayCamera.Position + GameplayCamera.Direction * 2000, IntersectOptions.Map | IntersectOptions.Mission_Entities | IntersectOptions.Objects, Game.Player.Character);

                //int handle = TextureClasses.AddDecalTexture(ray.HitCoords, -ray.SurfaceNormal, new Vector3(unkX, unkY, unkZ).Normalized, "graffiti", "black_angel_wings", TextureClasses.DecalTypes.solidPool_oil, 3.84f, 3.555f, 255f, 255f, 255f, 255f, 0.5f);

                //Vector2 temp = MathHelper.PointOnCircle(1f, RotationAngle, new Vector2(0f, 0f));
                //Vector3 unkVec3 = Quaternion.FromToRotation(-ray.SurfaceNormal, -ray.SurfaceNormal) * new Vector3(temp.Y, temp.X, 0f);
                //Vector3 unkVec3 = Quaternion.RotationAxis(-ray.SurfaceNormal, MathUtil.DegreesToRadians(RotationAngle)) * new Vector3(1f, 1f, 1f);
                
                Vector2 grafSize = ResizedWidthHeightBasedOnNewWidth(LastSelectedGraffiti.TextureWidth, LastSelectedGraffiti.TextureHeight, 2f, WidthHeightSizeCoeff);

                LastHitLocation = ray.HitCoords;
                LastHitDirection = -ray.SurfaceNormal;
                LastHitRotationAngle = RotationAngle;
                LastHitGrafSize = grafSize;

                if (ray.DitHitEntity && GTAVFunctions.GTAFunction.EntityIsAVehicle(ray.HitEntity))
                {
                    LastHitVehicle = (Vehicle)ray.HitEntity;
                }
                else
                {
                    LastHitVehicle = null;
                }

                int handle = DecalHelper.AddDecalTexture(ray.HitCoords, -ray.SurfaceNormal, RotationAngle, LastSelectedGraffiti.TextureDictionary, LastSelectedGraffiti.TextureName, 9007, grafSize.X, grafSize.Y, 255f, 255f, 255f, 255f, 0.5f, true, LastHitVehicle);

                /*UI.ShowSubtitle(
                    "Angle: " + RotationAngle
                    + "~n~SurfaceNormal: " + -ray.SurfaceNormal
                    , 1);*/
                return true;
            }
            else
            {
                UI.ShowSubtitle("Too many decals nearby!\nYou cannot place anymore here.", 1);
                return false;
            }
        }

        public static Vector2 ResizedWidthHeightBasedOnNewWidth(float oldWidth, float oldHeight, float newWidth, float sizeCoeff = 1.0f)
        {
            float aspRatio = oldWidth / oldHeight;
            return new Vector2(newWidth, newWidth / aspRatio) * sizeCoeff;
        }

        public static Vector2 ResizedWidthHeightBasedOnNewHeight(float oldWidth, float oldHeight, float newHeight, float sizeCoeff = 1.0f)
        {
            float aspRatio = oldWidth / oldHeight;
            return new Vector2(newHeight * aspRatio, newHeight) * sizeCoeff;
        }
        
        public static void DisplayAllLoadedVehicleInfo()
        {
            if (GraffitiMethods.LoadedVehicleOutfits != null && GraffitiMethods.LoadedVehicleOutfits.Count > 0)
            {
                foreach (var outfit in LoadedVehicleOutfits.ToList())
                {
                    outfit.ShowOutfitInfo3D();
                    outfit.ShowAllDecalInfo3D();
                }
            }
        }

        public static void DisplaySingleVehicleDecalInfo(DecaledVehicleOutfit savedOutfit, SingleVehicleDecal specificDecal)
        {
            if (LoadedVehicleOutfits != null)
            {
                foreach (var loadedOutfit in LoadedVehicleOutfits.ToList().Where(lo => 
                    savedOutfit.OutfitName.Equals(lo.OutfitName) && savedOutfit.VehicleModelHash == lo.VehicleModelHash))
                {
                    foreach (var decal in loadedOutfit.DecalList.ToList().Where(ld =>
                        specificDecal.Graffiti.TextureName.Equals(ld.Graffiti.TextureName) && specificDecal.Location == ld.Location))
                    {
                        if (loadedOutfit.CurrentVehicle == null || !loadedOutfit.CurrentVehicle.Exists()) return;

                        decal.ShowInfo3D();
                    }
                }
            }
        }
    }
}
