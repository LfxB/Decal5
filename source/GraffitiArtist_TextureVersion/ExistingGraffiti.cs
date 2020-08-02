using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GTA;
using GTA.Math;
using GTA.UI;
using GTAMath;

namespace GraffitiArtist
{
    [XmlInclude(typeof(ExistingGraffitiInMap))]
    [XmlInclude(typeof(SingleVehicleDecal))]
    public class ExistingGraffiti
    {
        public Graffiti Graffiti { get; set; }

        public Vector3 Location { get; set; }
        public float RotationAngle { get; set; }

        public float DisplayWidth { get; set; }

        public float DisplayHeight { get; set; }

        [XmlIgnore]
        public int Handle = -1337;

        [XmlIgnore]
        protected bool _init;

        [XmlIgnore]
        protected bool _textureExists = true;

        [XmlIgnore]
        protected int OriginalGraffitiObjectIndex = -1;

        [XmlIgnore]
        protected bool justMadeRedundant;

        [XmlIgnore]
        protected bool isCounted;

        public ExistingGraffiti() { }

        public virtual void ManageInWorld() { }

        protected virtual bool IsWithinViewingRange()
        {
            float x, y;
            return World.GetDistance(Game.Player.Character.Position, Location) <= GraffitiMethods.MyScriptSettings.MinimumLoadingRange || (GTAVFunctions.GTAFunction.GetScreenCoordFromWorldCoord(Location, out x, out y) && World.GetDistance(Game.Player.Character.Position, Location) <= GraffitiMethods.MyScriptSettings.MaximumLoadingRange);
        }

        public virtual void CreateDecal(float timeInSeconds = -1f) { }

        public bool Exists()
        {
            return DecalHelper.IsDecalAlive(Handle);
        }

        public virtual void Remove()
        {
            if (DecalTypeIndexIsValid() && isCounted)
            {
                GraffitiOG.LoadedCount--;
                if (GraffitiOG.LoadedCount < 1)
                {
                    GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].Available = true;
                    GraffitiOG.DecalTypeIndex = -1;
                }
                isCounted = false;
            }

            DecalHelper.RemoveDecal(Handle);
            DecalHelper.RemoveDecalsInRange(Location, 0.0000000000001f);

            Handle = -1337;
            justMadeRedundant = true;
        }

        protected bool DecalTypeIndexIsValid()
        {
            return _init && GraffitiOG.DecalTypeIndex != -1;
        }

        public virtual void ShowInfo3D() { }

        public bool Initialized()
        {
            if (!_init)
            {
                if (!_textureExists) return false;

                GTAVFunctions.GTAFunction.GetTextureResolution(Graffiti.TextureDictionary, Graffiti.TextureName, out _textureExists);

                if (!_textureExists) return false;

                Graffiti addonGraffiti = GraffitiMethods.AllAddonGraffiti.FirstOrDefault(g => g.TextureDictionary == Graffiti.TextureDictionary && g.TextureName == Graffiti.TextureName);
                if (addonGraffiti != null && addonGraffiti != default(Graffiti))
                {
                    OriginalGraffitiObjectIndex = GraffitiMethods.AllAddonGraffiti.IndexOf(addonGraffiti);
                }
                else
                {
                    _textureExists = false;
                    return false;
                }

                _init = true;
                return true;
            }
            return true;
        }

        public Graffiti GraffitiOG
        {
            get { return GraffitiMethods.AllAddonGraffiti[OriginalGraffitiObjectIndex]; }
        }
    }

    public class ExistingGraffitiInMap : ExistingGraffiti
    {
        public Vector3 Direction { get; set; }

        public ExistingGraffitiInMap() { }
        
        public ExistingGraffitiInMap(Graffiti graffiti, Vector3 coord, Vector3 direction, float rotationAngle, float width, float height)
        {
            Graffiti = graffiti;
            Location = coord;
            Direction = direction;
            RotationAngle = rotationAngle;
            DisplayWidth = width;
            DisplayHeight = height;
            //AvailableDecalTypeIndex = GraffitiMethods.LastHitDecalTypeIndex;
        }

        public override void ManageInWorld()
        {
            if (!Initialized()) return;

            if (IsWithinViewingRange())
            {
                if (Exists())
                {
                    // Debug
                    if (GraffitiMethods.SeeInfoMode)
                    {
                        ShowInfo3D();
                    }

                    if (!DecalTypeIndexIsValid()) return;
                    GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].Available = false;
                    DecalHelper.AddTextureToDecalType(GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].DecalTypeID, GraffitiOG.TextureDictionary, GraffitiOG.TextureName);
                }
                else
                {
                    this.CreateDecal();
                }
                justMadeRedundant = false;
            }
            else if (!IsWithinViewingRange())
            {
                if (!justMadeRedundant)
                {
                    /*if (this.Exists())
                    {
                        this.Remove();
                    }
                    else
                    {
                        if (DecalTypeIndexIsValid() && isCounted)
                        {
                            GraffitiOG.LoadedCount--;
                            if (GraffitiOG.LoadedCount < 1)
                            {
                                GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].Available = true;
                                GraffitiOG.DecalTypeIndex = -1;
                            }
                            isCounted = false;
                        }
                        justMadeRedundant = true;
                    }*/

                    this.Remove();
                }
            }
        }

        public override void CreateDecal(float timeInSeconds = -1f)
        {
            if (GraffitiOG.LoadedCount > 0 || GraffitiMethods.AvailableDecalTypeExists())
            {
                if (GraffitiOG.LoadedCount < 1)
                {
                    GraffitiOG.LoadedCount = 0; // Just in case it some how goes negative..
                    GraffitiOG.DecalTypeIndex = GraffitiMethods.NextAvailableDecalType();
                }

                if (!DecalTypeIndexIsValid()) return;

                GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].Available = false;
                Handle = DecalHelper.AddDecalTexture(Location, Direction, RotationAngle, GraffitiOG.TextureDictionary, GraffitiOG.TextureName, GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].DecalTypeID, DisplayWidth, DisplayHeight, 255f, 255f, 255f, 255f, timeInSeconds);

                if (Exists() && !isCounted)
                {
                    GraffitiOG.LoadedCount++;
                    isCounted = true;
                }
            }
            else
            { 
                Screen.ShowSubtitle("There are too many decals in this area!\nSome will not display.", 1);
            }
        }

        public override void ShowInfo3D()
        {
            TextElement info = new TextElement(
                "DecalType: " + (DecalTypeIndexIsValid()
                                  ? GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].DecalTypeID.ToString()
                                  : "None")
                              + "\nCount: " + GraffitiOG.LoadedCount
                /*"\nAvailable: " + GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].Available*/,
                Screen.WorldToScreen(Location), 0.40f, System.Drawing.Color.White, Font.ChaletComprimeCologne,
                Alignment.Center);
            info.Enabled = true;
            info.Draw();
        }
    }

    public class SingleVehicleDecal : ExistingGraffiti
    {
        [XmlIgnore]
        public Vehicle TaggedVehicle;

        public Vector3 Location_ForDirectionPurpose { get; set; }

        public GraffitiMethods.MirrorFlip MirrorFlip { get; set; }

        public float RGB_Red = 255f;
        public float RGB_Green = 255f;
        public float RGB_Blue = 255f;
        public float RGB_Alpha = 255f;

        [XmlIgnore]
        public Vector3 Direction;

        [XmlIgnore]
        public int MirroredHandle = -1337;

        public SingleVehicleDecal() { }

        public SingleVehicleDecal(Vehicle vehicle, Graffiti graffiti, Vector3 relativeCoord, Vector3 relativeCoord2_fromDirection, float rotationAngle, float width, float height)
        {
            Graffiti = graffiti;
            Location = relativeCoord;
            Location_ForDirectionPurpose = relativeCoord2_fromDirection;
            RotationAngle = rotationAngle;
            DisplayWidth = width;
            DisplayHeight = height;
            
            TaggedVehicle = vehicle;
        }

        public SingleVehicleDecal Clone()
        {
            SingleVehicleDecal clone =  new SingleVehicleDecal(this.TaggedVehicle, this.Graffiti, this.Location, this.Location_ForDirectionPurpose, this.RotationAngle, this.DisplayWidth, this.DisplayHeight);
            clone.Graffiti = this.Graffiti;
            clone.MirrorFlip = this.MirrorFlip;
            clone.RGB_Red = this.RGB_Red;
            clone.RGB_Green = this.RGB_Green;
            clone.RGB_Blue = this.RGB_Blue;
            clone.RGB_Alpha = this.RGB_Alpha;
            return clone;
        }

        public override void ManageInWorld()
        {
            if (!Initialized()) return;

            if (TaggedVehicle == null || !TaggedVehicle.Exists()) return;

            Direction = (TaggedVehicle.GetOffsetPosition(Location) - TaggedVehicle.GetOffsetPosition(Location_ForDirectionPurpose)).Normalized;

            if (IsWithinViewingRange())
            {
                if (Exists())
                {
                    // Debug
                    if (GraffitiMethods.SeeInfoMode)
                    {
                        ShowInfo3D();
                    }

                    if (!DecalTypeIndexIsValid()) return;
                    GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].Available = false;
                    DecalHelper.AddTextureToDecalType(GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].DecalTypeID, GraffitiOG.TextureDictionary, GraffitiOG.TextureName);
                }
                else
                {
                    this.CreateDecal();
                }
                justMadeRedundant = false;
            }
            else if (!IsWithinViewingRange())
            {
                if (!justMadeRedundant)
                {
                    /*if (this.Exists())
                    {
                        this.Remove();
                    }
                    else
                    {
                        if (DecalTypeIndexIsValid() && isCounted)
                        {
                            GraffitiOG.LoadedCount--;
                            if (GraffitiOG.LoadedCount < 1)
                            {
                                GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].Available = true;
                                GraffitiOG.DecalTypeIndex = -1;
                            }
                            isCounted = false;
                        }
                       justMadeRedundant = true;
                    }*/

                    this.Remove();
                }
            }
        }

        public override void CreateDecal(float timeInSeconds = -1f)
        {
            if (GraffitiOG.LoadedCount > 0 || GraffitiMethods.AvailableDecalTypeExists())
            {
                if (GraffitiOG.LoadedCount < 1)
                {
                    GraffitiOG.LoadedCount = 0; // Just in case it some how goes negative..
                    GraffitiOG.DecalTypeIndex = GraffitiMethods.NextAvailableDecalType();
                }

                if (!DecalTypeIndexIsValid()) return;

                GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].Available = false;
                Handle = DecalHelper.AddDecalTexture(TaggedVehicle.GetOffsetPosition(Location), Direction,
                    RotationAngle, GraffitiOG.TextureDictionary, GraffitiOG.TextureName,
                    GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].DecalTypeID, DisplayWidth,
                    DisplayHeight, RGB_Red, RGB_Green, RGB_Blue, RGB_Alpha, timeInSeconds, true, TaggedVehicle);

                if (MirrorFlip != GraffitiMethods.MirrorFlip.None)
                {
                    CreateMirroredDecal(timeInSeconds);
                }

                if (Exists() && !isCounted)
                {
                    GraffitiOG.LoadedCount++;
                    isCounted = true;
                }
            }
            else
            {
                Screen.ShowSubtitle("There are too many decals in this area!\nSome will not display.", 1);
            }
        }

        Vector3 GetMirroredPos(out Vector3 mirroredDir)
        {
            Vector3 pos = new Vector3(Location.X * -1f, Location.Y, Location.Z);
            Vector3 pos2 = new Vector3(Location_ForDirectionPurpose.X * -1f, Location_ForDirectionPurpose.Y, Location_ForDirectionPurpose.Z);
            mirroredDir = (TaggedVehicle.GetOffsetPosition(pos) - TaggedVehicle.GetOffsetPosition(pos2)).Normalized;
            return pos;
        }

        public void CreateMirroredDecal(float timeInSeconds = -1f)
        {
            Vector3 dir;
            float rotAngle = 0f;

            switch (MirrorFlip)
            {
                case GraffitiMethods.MirrorFlip.Mirror:
                    rotAngle = RotationAngle - ((RotationAngle - 180f) * 2f);
                    break;
                case GraffitiMethods.MirrorFlip.Mirror2:
                    rotAngle = (RotationAngle - ((RotationAngle - 180f) * 2f)) - 180f;
                    break;
                case GraffitiMethods.MirrorFlip.Flip:
                    rotAngle = RotationAngle - 180f;
                    break;
                case GraffitiMethods.MirrorFlip.Flip2:
                    rotAngle = RotationAngle;
                    break;
            }

            Vector3 pos = GetMirroredPos(out dir);
            MirroredHandle = DecalHelper.AddDecalTexture(TaggedVehicle.GetOffsetPosition(pos), dir, rotAngle,
                GraffitiOG.TextureDictionary, GraffitiOG.TextureName,
                GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].DecalTypeID, DisplayWidth, DisplayHeight,
                RGB_Red, RGB_Green, RGB_Blue, RGB_Alpha, timeInSeconds, true, TaggedVehicle);
        }

        public void DeleteMirroredDecal()
        {
            DecalHelper.RemoveDecal(MirroredHandle);

            Vector3 temp;
            if (TaggedVehicle != null && TaggedVehicle.Exists())
                DecalHelper.RemoveDecalsInRange(TaggedVehicle.GetOffsetPosition(GetMirroredPos(out temp)), 0.0000000000001f);
        }

        protected override bool IsWithinViewingRange()
        {
            float x, y;
            Vector3 worldCoord = TaggedVehicle.GetOffsetPosition(Location);
            return World.GetDistance(Game.Player.Character.Position, worldCoord) <= GraffitiMethods.MyScriptSettings.MinimumLoadingRange || (GTAVFunctions.GTAFunction.GetScreenCoordFromWorldCoord(worldCoord, out x, out y) && World.GetDistance(Game.Player.Character.Position, worldCoord) <= GraffitiMethods.MyScriptSettings.MaximumLoadingRange);
        }

        public override void Remove()
        {
            if (DecalTypeIndexIsValid() && isCounted)
            {
                GraffitiOG.LoadedCount--;
                if (GraffitiOG.LoadedCount < 1)
                {
                    GraffitiMethods.UsableDecalTypes[GraffitiOG.DecalTypeIndex].Available = true;
                    GraffitiOG.DecalTypeIndex = -1;
                }
                isCounted = false;
            }

            DecalHelper.RemoveDecal(Handle);

            Handle = -1337;
            justMadeRedundant = true;

            DeleteMirroredDecal();

            if (TaggedVehicle != null && TaggedVehicle.Exists())
                DecalHelper.RemoveDecalsInRange(TaggedVehicle.GetOffsetPosition(Location), 0.0000000000001f);
        }
        
        public override void ShowInfo3D()
        {
            TextElement info = new TextElement(Graffiti.TextureName
                                               /*+ "\nDecalType: " + GraffitiMethods.UsableDecalTypes[AvailableDecalTypeIndex].DecalTypeID 
                                               + "\nAvailable: " + GraffitiMethods.UsableDecalTypes[AvailableDecalTypeIndex].Available */
                                               + "\n" + GTAVFunctions.GTAFunction.RoundVector3D(this.Location, 3),
                Screen.WorldToScreen(TaggedVehicle.GetOffsetPosition(Location)), 0.40f,
                System.Drawing.Color.FromArgb(180, System.Drawing.Color.White), Font.ChaletComprimeCologne,
                Alignment.Center);
            info.Enabled = true;
            info.Draw();
        }
    }

    public class DecaledVehicleOutfit
    {
        public int VehicleModelHash { get; set; }

        public string FriendlyName { get; set; }

        public string DisplayName { get; set; }

        public string OutfitName { get; set; }

        public List<SingleVehicleDecal> DecalList;

        [XmlIgnore]
        public Vehicle CurrentVehicle;

        public DecaledVehicleOutfit() { }

        public DecaledVehicleOutfit(string outfitName, List<SingleVehicleDecal> decalList = null)
        {
            OutfitName = outfitName;
            DecalList = decalList != null ? decalList : new List<SingleVehicleDecal>();
        }

        public void ApplyToVehicle(Vehicle vehicle)
        {
            CurrentVehicle = vehicle;
            SetVehicleInfo(vehicle);
            DecalList.ForEach(d => d.TaggedVehicle = vehicle);
        }

        public void RemoveFromVehicle()
        {
            foreach (var decal in DecalList)
            {
                decal.Remove();
                //decal.TaggedVehicle.Wash();
                decal.TaggedVehicle = null;
            }
            CurrentVehicle = null;
        }

        public void SetVehicleInfo(Vehicle vehicle)
        {
            VehicleModelHash = vehicle.Model.Hash;
            FriendlyName = vehicle.LocalizedName;
            DisplayName = vehicle.DisplayName;
        }

        public void ManageOutfit()
        {
            DecalList.ForEach(d => d.ManageInWorld());
            if (GraffitiMethods.SeeInfoMode)
            {
                ShowOutfitInfo3D();
            }
        }

        public void ShowOutfitInfo3D()
        {
            if (CurrentVehicle == null || !CurrentVehicle.Exists()) return;

            TextElement info = new TextElement("Outfit Name: " + OutfitName +
                                               "\nVehicle Name: " + (FriendlyName == null || FriendlyName == "NULL"
                                                   ? DisplayName
                                                   : FriendlyName),
                Screen.WorldToScreen(CurrentVehicle.Position), 0.40f,
                System.Drawing.Color.FromArgb(180, System.Drawing.Color.Red), Font.ChaletComprimeCologne,
                Alignment.Center);
            info.Enabled = true;
            info.Draw();
        }

        public void ShowAllDecalInfo3D()
        {
            if (CurrentVehicle == null || !CurrentVehicle.Exists()) return;

            DecalList.ForEach(d => d.ShowInfo3D());
        }

        public DecaledVehicleOutfit Clone()
        {
            List<SingleVehicleDecal> clonedList = new List<SingleVehicleDecal>();
            DecalList.ForEach(d => clonedList.Add(d.Clone()));
            DecaledVehicleOutfit cloneOutfit = new DecaledVehicleOutfit(this.OutfitName, clonedList);
            cloneOutfit.VehicleModelHash = this.VehicleModelHash;
            cloneOutfit.FriendlyName = this.FriendlyName;
            cloneOutfit.DisplayName = this.DisplayName;
            return cloneOutfit;
        }
    }

    /*public class DecaledVehicleModel
    {
        public int VehicleModelHash { get; set; }

        public string FriendlyName { get; set; }

        public List<DecaledVehicleOutfit> Outfits;

        public DecaledVehicleModel() { }
    }*/
}
