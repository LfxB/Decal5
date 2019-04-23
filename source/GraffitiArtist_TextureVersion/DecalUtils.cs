using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA; // This is a reference that is needed! do not edit this
using GTA.Native; // This is a reference that is needed! do not edit this
using GTA.Math;
using System.Drawing;
using GTAMath;

namespace GraffitiArtist
{
    public static class DecalHelper
    {
        public enum DecalTypes
        {
            splatters_blood = 1010,
            splatters_blood_dir = 1015,
            splatters_blood_mist = 1017,
            splatters_mud = 1020,
            splatters_paint = 1030,
            splatters_water = 1040,
            splatters_water_hydrant = 1050,
            splatters_blood2 = 1110,
            weapImpact_metal = 4010,
            weapImpact_concrete = 4020,
            weapImpact_mattress = 4030,
            weapImpact_mud = 4032,
            weapImpact_wood = 4050,
            weapImpact_sand = 4053,
            weapImpact_cardboard = 4040,
            weapImpact_melee_glass = 4100,
            weapImpact_glass_blood = 4102,
            weapImpact_glass_blood2 = 4104,
            weapImpact_shotgun_paper = 4200,
            weapImpact_shotgun_mattress,
            weapImpact_shotgun_metal,
            weapImpact_shotgun_wood,
            weapImpact_shotgun_dirt,
            weapImpact_shotgun_tvscreen,
            weapImpact_shotgun_tvscreen2,
            weapImpact_shotgun_tvscreen3,
            weapImpact_melee_concrete = 4310,
            weapImpact_melee_wood = 4312,
            weapImpact_melee_metal = 4314,
            burn1 = 4421,
            burn2,
            burn3,
            burn4,
            burn5,
            bang_concrete_bang = 5000,
            bang_concrete_bang2,
            bang_bullet_bang,
            bang_bullet_bang2 = 5004,
            bang_glass = 5031,
            bang_glass2,
            solidPool_water = 9000,
            solidPool_blood,
            solidPool_oil,
            solidPool_petrol,
            solidPool_mud,
            porousPool_water,
            porousPool_blood,
            porousPool_oil,
            porousPool_petrol,
            porousPool_mud,
            porousPool_water_ped_drip,
            liquidTrail_water = 9050,
            texture_paint = 9118,
            texture_paint_2,
            texture_paint_follow_cam = 9120,
            texture_paint_follow_cam_2,
            texture_paint_follow_cam_3,
            texture_paint_4,
        }

        public static void RequestStreamedTextureDict(string textureDict, bool unk = true)
        {
            Function.Call(Hash.REQUEST_STREAMED_TEXTURE_DICT, textureDict, unk);
        }

        public static bool HasStreamedTextureDictLoaded(string textureDict)
        {
            return Function.Call<bool>(Hash.HAS_STREAMED_TEXTURE_DICT_LOADED, textureDict);
        }

        public static void SetStreamedTextureDictAsNoLongerNeeded(string dict)
        {
            Function.Call(Hash.SET_STREAMED_TEXTURE_DICT_AS_NO_LONGER_NEEDED, dict);
        }

        public static int AddDecal(Vector3 pos, Vector3 direction, float rotationAngle, DecalTypes decalType, float width = 1.0f, float height = 1.0f, float R = 25.5f, float G = 25.5f, float B = 25.5f, float opacity = 255, float timeout = 20.0f, bool allowOnVehicles = true, Entity onEntity = null)
        {
            //return Function.Call<int>(Hash.ADD_DECAL, (int)decalType, pos.X, pos.Y, pos.Z, direction.X, direction.Y, direction.Z, 1f, 0f, 0f, width, height, R / 255, G / 255, B / 255, opacity, timeout, 0, 0, allowOnVehicles);

            //Vector3 v3 = direction.Normalized;
            //return Function.Call<int>(Hash.ADD_DECAL, (int)decalType, pos.X, pos.Y, pos.Z, direction.X, direction.Y, direction.Z, v3.Z, v3.X, v3.Y, width, height, R / 255, G / 255, B / 255, opacity, timeout, 0, 0, allowOnVehicles);

            //Vector3 normDir = func_192(direction);
            //return Function.Call<int>(Hash.ADD_DECAL, (int)decalType, pos.X, pos.Y, pos.Z, normDir.X, normDir.Y, normDir.Z, normDir.Y, -normDir.Z, normDir.X, width, height, R / 255, G / 255, B / 255, opacity / 255, timeout, 1, 0, allowOnVehicles);

            Vector3 normDir = direction.Normalized;
            Vector3 unkVec = DecalRotationInput(direction, rotationAngle, onEntity);
            return Function.Call<int>(Hash.ADD_DECAL, (int)decalType, pos.X, pos.Y, pos.Z, normDir.X, normDir.Y, normDir.Z, unkVec.X, unkVec.Y, unkVec.Z, width, height, R / 255, G / 255, B / 255, opacity / 255, timeout, 1, 0, allowOnVehicles);
        }

        public static int AddDecal(Vector3 pos, Vector3 direction, float rotationAngle, int decalType, float width = 1.0f, float height = 1.0f, float R = 25.5f, float G = 25.5f, float B = 25.5f, float opacity = 255, float timeout = 20.0f, bool allowOnVehicles = true, Entity onEntity = null)
        {
            //return Function.Call<int>(Hash.ADD_DECAL, (int)decalType, pos.X, pos.Y, pos.Z, direction.X, direction.Y, direction.Z, 1f, 0f, 0f, width, height, R / 255, G / 255, B / 255, opacity, timeout, 0, 0, allowOnVehicles);

            //Vector3 v3 = direction.Normalized;
            //return Function.Call<int>(Hash.ADD_DECAL, (int)decalType, pos.X, pos.Y, pos.Z, direction.X, direction.Y, direction.Z, v3.Z, v3.X, v3.Y, width, height, R / 255, G / 255, B / 255, opacity, timeout, 0, 0, allowOnVehicles);

            //Vector3 normDir = func_192(direction);
            //return Function.Call<int>(Hash.ADD_DECAL, (int)decalType, pos.X, pos.Y, pos.Z, normDir.X, normDir.Y, normDir.Z, normDir.Y, -normDir.Z, normDir.X, width, height, R / 255, G / 255, B / 255, opacity / 255, timeout, 1, 0, allowOnVehicles);

            Vector3 normDir = direction.Normalized;
            Vector3 unkVec = DecalRotationInput(direction, rotationAngle, onEntity);
            return Function.Call<int>(Hash.ADD_DECAL, decalType, pos.X, pos.Y, pos.Z, normDir.X, normDir.Y, normDir.Z, unkVec.X, unkVec.Y, unkVec.Z, width, height, R / 255, G / 255, B / 255, opacity / 255, timeout, 1, 0, allowOnVehicles);
        }

        public static int AddDecalTexture(Vector3 pos, Vector3 direction, float rotationAngle, string textureDict, string textureName, DecalTypes decalType = DecalTypes.texture_paint, float width = 1.0f, float height = 1.0f, float R = 255f, float G = 255f, float B = 255f, float opacity = 255, float timeoutSeconds = 20.0f, bool allowOnVehicles = true, Entity onEntity = null)
        {
            int handle = AddDecal(pos, direction, rotationAngle, decalType, width, height, R, G, B, opacity, timeoutSeconds, allowOnVehicles, onEntity);
            AddTextureToDecalType((int)decalType, textureDict, textureName);

            return handle;
        }

        public static int AddDecalTexture(Vector3 pos, Vector3 direction, float rotationAngle, string textureDict, string textureName, int decalType = 10000, float width = 1.0f, float height = 1.0f, float R = 255f, float G = 255f, float B = 255f, float opacity = 255, float timeoutSeconds = 20.0f, bool allowOnVehicles = true, Entity onEntity = null)
        {
            int handle = AddDecal(pos, direction, rotationAngle, decalType, width, height, R, G, B, opacity, timeoutSeconds, allowOnVehicles, onEntity);
            AddTextureToDecalType(decalType, textureDict, textureName);

            return handle;
        }

        public static void AddTextureToDecalType(int decalType, string textureDict, string textureName)
        {
            Function.Call(Hash._0x8A35C742130C6080, decalType, textureDict, textureName);
        }

        private static Vector3 DecalRotationInput(Vector3 direction, float rotationAngle, Entity onEntity = null)
        {
            return (Quaternion.RotationAxis(direction, MathUtil.DegreesToRadians(rotationAngle)) * Vector3.ProjectOnPlane(onEntity == null ? new Vector3(0f, 1f, 0f) : onEntity.ForwardVector, direction)).Normalized;
        }

        public static void RemoveDecal(int handle)
        {
            Function.Call(Hash.REMOVE_DECAL, handle);
        }

        public static void RemoveDecalsInRange(Vector3 worldCoord, float range)
        {
            Function.Call(Hash.REMOVE_DECALS_IN_RANGE, worldCoord.X, worldCoord.Y, worldCoord.Z, range);
        }

        public static bool IsDecalAlive(int handle)
        {
            return Function.Call<bool>(Hash.IS_DECAL_ALIVE, handle);
        }

        static Vector3 func_192(Vector3 vector) //Normalize, it seems. fm_deathmatch_creator_c
        {
            float vectorMagnitude;
            float fVar1;

            vectorMagnitude = GetMagnitude(vector);
            if (vectorMagnitude != 0f)
            {
                fVar1 = 1f / vectorMagnitude;

                //vector = vector * Vector(fVar1, fVar1, fVar1);
                //vector = Vector3.Cross(vector, new Vector3(fVar1, fVar1, fVar1));

                vector.X *= fVar1;
                vector.Y *= fVar1;
                vector.Z *= fVar1;
            }
            else
            {
                vector.X = 0f;
                vector.Y = 0f;
                vector.Z = 0f;
            }
            return vector;
        }

        static Vector3 sub_6897(Vector3 vector) // from re_dealgonewrong.c4
        {
            float vectorMagnitude = GetMagnitude(vector);
            float v_6;

            if (vectorMagnitude != 0.0f)
            {
                v_6 = 1.0f / vectorMagnitude;
                vector = Vector3.Cross(vector, new Vector3(v_6, v_6, v_6));
            }
            else
            {
                vector.X = 0f;
                vector.Y = 0f;
                vector.Z = 0f;
            }

            return vector;
        }

        static float GetMagnitude(Vector3 vector3d)
        {
            return Function.Call<float>(Hash.VMAG2, vector3d.X, vector3d.Y, vector3d.Z); //calculated magnitude without using sqrt operations -> much faster
        }

        public static void DrawLine(Vector3 from, Vector3 to, Color col)
        {
            Function.Call(Hash.DRAW_LINE, from.X, from.Y, from.Z, to.X, to.Y, to.Z, col.R, col.G, col.B, col.A);
        }
    }
}
