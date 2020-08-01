using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA; // This is a reference that is needed! do not edit this
using GTA.Native; // This is a reference that is needed! do not edit this
using GTA.Math;
using GTAMath;

namespace GTAVFunctions
{
    public static class GTAFunction
    {
        public enum RagdollType
        {
            Normal = 0,
            StiffBody = 1,
            NarrowLegStumble = 2,
            WideLegStuble = 3
        }

        public static float GetEntitySpeed_MPS(Entity ent)
        {
            return Function.Call<float>(Hash.GET_ENTITY_SPEED, ent);
        }

        public static float GetEntitySpeed_MPH(Entity ent)
        {
            return Function.Call<float>(Hash.GET_ENTITY_SPEED, ent) * 2.236936f;
        }

        public static float GetEntitySpeed_KMPH(Entity ent)
        {
            return Function.Call<float>(Hash.GET_ENTITY_SPEED, ent) * 3.6f;
        }

        public static void SetEntityMaxSpeed_KMPH(Entity ent, float speed)
        {
            Function.Call(Hash.SET_ENTITY_MAX_SPEED, ent, speed / 3.6f);
        }

        public static void EnablePedAmbientAnimations(Ped ped)
        {
            Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, ped, false);
            Function.Call(Hash.SET_PED_CAN_PLAY_GESTURE_ANIMS, ped, true);
            Function.Call(Hash.SET_PED_CAN_PLAY_VISEME_ANIMS, ped, true, 0);
            Function.Call(Hash.SET_PED_CAN_PLAY_AMBIENT_ANIMS, ped, true);
            Function.Call(Hash.SET_PED_CAN_PLAY_AMBIENT_BASE_ANIMS, ped, true);
        }

        public static void DisablePedAmbientAnimations(Ped ped)
        {
            Function.Call(Hash.SET_BLOCKING_OF_NON_TEMPORARY_EVENTS, ped, true);
            Function.Call(Hash.SET_PED_CAN_PLAY_GESTURE_ANIMS, ped, false);
            Function.Call(Hash.SET_PED_CAN_PLAY_VISEME_ANIMS, ped, false, 0);
            Function.Call(Hash.SET_PED_CAN_PLAY_AMBIENT_ANIMS, ped, false);
            Function.Call(Hash.SET_PED_CAN_PLAY_AMBIENT_BASE_ANIMS, ped, false);
        }

        public static void ToggleInjuredAnimations(Ped ped, bool on)
        {
            Function.Call(Hash._SET_PED_CAN_PLAY_INJURED_ANIMS, ped, on);
        }

        public static void SetPedRagdoll(Ped p, int ms, RagdollType type)
        {
            Function.Call(Hash.SET_PED_CAN_RAGDOLL, p, true);
            Function.Call(Hash.SET_PED_TO_RAGDOLL, p, ms, ms, (int)type, 1, 1, 0);
        }

        public static void PlayPedPainSound(Ped p)
        {
            Function.Call(Hash.PLAY_PAIN, p, 33, 0, 0);
        }

        public static void SetPedCollisionCapsule(Ped ped, float amount)
        {
            Function.Call(Hash.SET_PED_CAPSULE, ped, amount);
        }

        public static void SetCollision(Entity entity, bool toggle, bool keepPhysics)
        {
            Function.Call(Hash.SET_ENTITY_COLLISION, entity, toggle, keepPhysics);
        }

        public static void AttachToEntity(this Entity e1, Entity e2, int boneIndexE2, Vector3 offsetPos, Vector3 rotation, bool useSoftPinning = false, bool collisionBetweenEnts = false, bool entOneIsPed = false, int vertexIndex = 2, bool fixedRot = true)
        {
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, e1, e2, boneIndexE2, offsetPos.X, offsetPos.Y, offsetPos.Z, rotation.X, rotation.Y, rotation.Z, false, useSoftPinning, collisionBetweenEnts, entOneIsPed, vertexIndex, fixedRot);
        }

        /// <summary>
        /// Attaches entities together so that collision on either entity will affect both entites.
        /// </summary>
        /// <param name="entityToAttachTo">The entity to be attached to. AKA the Parent entity.</param>
        /// <param name="attachBoneIndex">Index of the bone of the Child Entity that wil be attached.</param>
        /// <param name="attachBoneIndex_EntityToAttachTo">Index of the bone of the Parent entity that wil be attached to.</param>
        /// <param name="offsetPos1">Offset position. Setting this one to a non-zero Vector3 seems to mess with the positioning of the Parent entity. Better to use offsetPos2.</param>
        /// <param name="offsetPos2">Offset position.</param>
        /// <param name="rotation">Rotation of child entity.</param>
        /// <param name="fixedRot">Whether rotation should be fixed to the Parent entity. If false, both entities will not share the same rotation.</param>
        /// <param name="p15">Not known. Seems to enable the Rotation parameter</param>
        /// <param name="collision">Whether the two entites should have collision with each other.</param>
        /// <param name="loosePositioning">If set to false, both entities are attached in a rope-like fashion. If true, positioning is completely static.</param>
        /// <param name="breakForce">Pounds(?) of force required to break the attachment.</param>
        /// <param name="p18">Not known. Seems to be 2 in R* scripts. 0 works too.</param>
        public static void AttachToPhysically(this Entity entityToAttach, Entity entityToAttachTo, int attachBoneIndex, int attachBoneIndex_EntityToAttachTo, Vector3 offsetPos1, Vector3 offsetPos2, Vector3 rotation, bool fixedRot = true, bool p15 = false,  bool collision = false, bool loosePositioning = false, float breakForce = 10000000.0f, int p18 = 2)
        {
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY_PHYSICALLY, entityToAttach, entityToAttachTo, attachBoneIndex, attachBoneIndex_EntityToAttachTo, offsetPos1.X, offsetPos1.Y, offsetPos1.Z, offsetPos2.X, offsetPos2.Y, offsetPos2.Z, rotation.X, rotation.Y, rotation.Z, breakForce, fixedRot, p15, collision, loosePositioning, p18);
        }

        public static void AttachToPhysically_Relative(this Entity entityToAttach, Entity entityToAttachTo, int attachBoneIndex, int attachBoneIndex_EntityToAttachTo, Vector3 offsetPos2, Vector3 offsetRotation, bool fixedRot = true, bool p15 = true, bool collision = false, bool loosePositioning = false, float breakForce = 10000000.0f, int p18 = 2)
        {
            entityToAttach.Detach();
            AttachToPhysically(entityToAttach, entityToAttachTo, attachBoneIndex, attachBoneIndex_EntityToAttachTo, Vector3.Zero, offsetPos2, entityToAttachTo.Rotation + offsetRotation, fixedRot, p15, collision, loosePositioning, breakForce, p18);
        }

        public static int boneIndexByName(this Entity e, string b)
        {
            return Function.Call<int>(Hash.GET_ENTITY_BONE_INDEX_BY_NAME, e, b);
        }

        public static int boneIndexByID(this Ped p, int ID)
        {
            return Function.Call<int>(Hash.GET_PED_BONE_INDEX, p, ID);
        }

        public static Vector3 RoundVector3D(Vector3 vector3d, int decimals)
        {
            return new Vector3((float)Math.Round(vector3d.X, decimals), (float)Math.Round(vector3d.Y, decimals), (float)Math.Round(vector3d.Z, decimals));
        }

        public static void Teleport(Entity entity, Vector3 position, bool clearArea = true)
        {
            Function.Call(Hash.SET_ENTITY_COORDS, entity, position.X, position.Y, position.Z, 0, 0, 0, clearArea);
        }

        public static void TeleportToGround(Entity entity, Vector3 position, Action methodToRunWhileTeleporting)
        {
            bool groundFound = false;
            //float[] groundCheckHeight = 
            //    {
            //        100.0f, 150.0f, 50.0f, 0.0f, 200.0f, 250.0f, 300.0f, 350.0f, 400.0f,
            //        450.0f, 500.0f, 550.0f, 600.0f, 650.0f, 700.0f, 750.0f, 800.0f
            //    };
            float[] groundCheckHeight =
                {
                    800.0f, 750.0f, 700.0f, 650.0f, 600.0f, 550.0f, 500.0f,
                    450.0f, 400.0f, 350.0f, 300.0f, 250.0f, 200.0f, 150.0f, 100.0f, 50.0f
                };
            for (int i = 0; i < groundCheckHeight.Length; i++)
            {
                Function.Call(Hash.SET_ENTITY_COORDS_NO_OFFSET, entity, position.X, position.Y, groundCheckHeight[i], 0, 0, 1);
                methodToRunWhileTeleporting();
                Script.Wait(100);
                float groundZ = GetGroundZ(new Vector3(position.X, position.Y, groundCheckHeight[i]), out groundFound);
                if (groundFound)
                {
                    position.Z = groundZ + 3.0f;
                    break;
                }
            }
            if (!groundFound)
            {
                position.Z = 1000.0f;
            }
            entity.PositionNoOffset = position;
            /*Teleport(entity, position, false);
            Script.Wait(100);
            RaycastResult ray = World.Raycast(position + Vector3.WorldUp * 200f, position + Vector3.WorldUp * -2f, IntersectOptions.Map);
            if (ray.DitHitAnything)
            {
                //entity.Position = ray.HitCoords;
                Teleport(entity, ray.HitCoords, false);
            }*/
        }

        public static float GetGroundZ(Vector3 pos, out bool groundFound) //thanks Jitnaught!
        {
            OutputArgument outArg = new OutputArgument();
            groundFound = Function.Call<bool>(Hash.GET_GROUND_Z_FOR_3D_COORD, pos.X, pos.Y, pos.Z, outArg, false);

            return outArg.GetResult<float>();
        }

        public static Vector3 GetGroundPosition(Vector3 pos) //thanks Jitnaught!
        {
            bool temp;
            return new Vector3(pos.X, pos.Y, GetGroundZ(pos, out temp));
        }

        public static bool PositionIsAboveGroundZHeight(Vector3 pos, float height)
        {
            bool groundIsFound;
            float gz = GetGroundZ(pos, out groundIsFound);
            //UI.ShowSubtitle(groundIsFound && pos.Z > gz + height ? "~r~" + (pos.Z - (gz + height)) : "false");
            return groundIsFound && pos.Z > gz + height ? true : false;
        }

        public static bool IsWithinThisHeightAboveGround(this Vector3 position, float height)
        {
            return World.Raycast(position, Vector3.WorldDown, height, IntersectFlags.Map | IntersectFlags.MissionEntities).DidHit;
        }

        public static void DamagePed(Ped attacker, Ped victim, int damage, RagdollType type, int ragdollMS, Vector3 forceDirection = default(Vector3), float forceDirectionMultiplier = 1f, Vector3 forceRotation = default(Vector3), float forceRotationMultiplier = 1f)
        {
            PlayPedPainSound(victim);
            SetPedRagdoll(victim, ragdollMS, type);

            victim.ApplyDamage(damage);
            victim.ApplyForce(forceDirection * forceDirectionMultiplier, forceRotation * forceRotationMultiplier);

            Function.Call(Hash.APPLY_PED_DAMAGE_PACK, victim, "BigRunOverByVehicle", 1.0, 1.0);
        }

        public static void DamageVehicle(Vector3 attackPos, Vehicle victim, int damageAmount, float visualDamageAmount = 800f, float radiusOfDamage = 1600f, Vector3 forceDirection = default(Vector3), float forceDirectionMultiplier = 1f, Vector3 forceRotation = default(Vector3), float forceRotationMultiplier = 1f)
        {
            SetVehicleDamage(victim, damageAmount);
            SetVehicleVisualDamage(victim, attackPos, visualDamageAmount, radiusOfDamage);
            victim.ApplyForce(forceDirection * forceDirectionMultiplier, forceRotation * forceRotationMultiplier);
            if (victim.Model.IsBike || victim.Model.IsBicycle) { if (victim.Driver != null && victim.Driver.Exists()) { Function.Call(Hash.KNOCK_PED_OFF_VEHICLE, victim.Driver); } }
        }

        public static void SetVehicleVisualDamage(Vehicle v, Vector3 worldCoord, float visualDamageAmount = 200f, float radiusOfDamage = 250f, bool p6 = true)
        {
            Vector3 offset = v.GetOffsetPosition(worldCoord);
            Function.Call(Hash.SET_VEHICLE_DAMAGE, v, offset.X, offset.Y, offset.Z, visualDamageAmount, radiusOfDamage, p6);
        }

        public static void SetVehicleDamage(Vehicle v, int damageAmount)
        {
            v.Health -= damageAmount;
            v.BodyHealth -= damageAmount;
            v.EngineHealth -= damageAmount;
        }

        public static void SetEntityProofs(Entity ent, bool bulletProof, bool fireProof, bool explosionProof, bool collisionProof, bool meleeProof, bool drownProof, bool p6 = true, bool p7 = true)
        {
            Function.Call(Hash.SET_ENTITY_PROOFS, ent, bulletProof, fireProof, explosionProof, collisionProof, meleeProof, p6, p7, drownProof);
        }

        public static bool EntityIsAnObject(this Entity e)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_AN_OBJECT, e);
        }

        public static bool EntityIsAPed(this Entity e)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_A_PED, e);
        }

        public static bool EntityIsAVehicle(this Entity e)
        {
            return Function.Call<bool>(Hash.IS_ENTITY_A_VEHICLE, e);
        }

        public static bool PedIsInStealthMode(Ped p)
        {
            return Function.Call<bool>(Hash.GET_PED_STEALTH_MOVEMENT, p);
        }

        public static bool HasCheatStringJustBeenEntered(string cheat)
        {
            return Function.Call<bool>(Hash._HAS_CHEAT_STRING_JUST_BEEN_ENTERED, Game.GenerateHash(cheat));
        }

        public static float CalculateRelativeValue(float input, float inputMin, float inputMax, float outputMin, float outputMax)
        {
            //http://stackoverflow.com/questions/22083199/method-for-calculating-a-value-relative-to-min-max-values
            //Making sure bounderies arent broken...
            if (input > inputMax)
            {
                input = inputMax;
            }
            if (input < inputMin)
            {
                input = inputMin;
            }
            //Return value in relation to min og max

            double position = (double)(input - inputMin) / (inputMax - inputMin);

            float relativeValue = (float)(position * (outputMax - outputMin)) + outputMin;

            return relativeValue;
        }

        public static float IncreaseNumber(this float input, float increment, float inputMax)
        {
            return (input + increment) < inputMax ? input + increment : inputMax;
        }

        public static float DecreaseNumber(this float input, float decrement, float inputMin)
        {
            return (input - decrement) > inputMin ? input - decrement : inputMin;
        }

        public static void SetTimecycleModifier(string modifierName)
        {
            Function.Call(Hash.SET_TIMECYCLE_MODIFIER, modifierName);
        }

        public static void SetTimecycleModifierStrength(float strength)
        {
            Function.Call(Hash.SET_TIMECYCLE_MODIFIER_STRENGTH, strength);
        }

        public static void ClearTimecycleModifier()
        {
            Function.Call(Hash.CLEAR_TIMECYCLE_MODIFIER);
        }

        public static void DrawLine(Vector3 from, Vector3 to, System.Drawing.Color col)
        {
            Function.Call(Hash.DRAW_LINE, from.X, from.Y, from.Z, to.X, to.Y, to.Z, col.R, col.G, col.B, col.A);
        }

        /// <summary>
        /// https://pastebin.com/nqNYWMSB
        /// </summary>
        /// <param name="text"></param>
        /// <param name="loopOnly"></param>
        /// <param name="beep"></param>
        /// <param name="shape"></param>
        public static void DisplayHelpTextThisFrame(string text, bool foreverUntilNextHelpText = false, bool beep = true, int shape = -1)
        {
            Function.Call(Hash.BEGIN_TEXT_COMMAND_DISPLAY_HELP, "CELL_EMAIL_BCON"); // jamyfafi
            //Function.Call(Hash._ADD_TEXT_COMPONENT_STRING, text);
            AddLongString(text);
            Function.Call(Hash.END_TEXT_COMMAND_DISPLAY_HELP, 0, foreverUntilNextHelpText, beep, shape);
        }

        private static void AddLongString(string str)
        {
            const int strLen = 99;
            for (int i = 0; i < str.Length; i += strLen)
            {
                string substr = str.Substring(i, Math.Min(strLen, str.Length - i));
                Function.Call(Hash.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME, substr);
            }
        }

        public static void ClearAllHelpMessages()
        {
            Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
        }

        public enum ControlString
        {
            INPUT_NEXT_CAMERA = 0,
            INPUT_LOOK_LR = 1,
            INPUT_LOOK_UD = 2,
            INPUT_LOOK_UP_ONLY = 3,
            INPUT_LOOK_DOWN_ONLY = 4,
            INPUT_LOOK_LEFT_ONLY = 5,
            INPUT_LOOK_RIGHT_ONLY = 6,
            INPUT_CINEMATIC_SLOWMO = 7,
            INPUT_SCRIPTED_FLY_UD = 8,
            INPUT_SCRIPTED_FLY_LR = 9,
            INPUT_SCRIPTED_FLY_ZUP = 10,
            INPUT_SCRIPTED_FLY_ZDOWN = 11,
            INPUT_WEAPON_WHEEL_UD = 12,
            INPUT_WEAPON_WHEEL_LR = 13,
            INPUT_WEAPON_WHEEL_NEXT = 14,
            INPUT_WEAPON_WHEEL_PREV = 15,
            INPUT_SELECT_NEXT_WEAPON = 16,
            INPUT_SELECT_PREV_WEAPON = 17,
            INPUT_SKIP_CUTSCENE = 18,
            INPUT_CHARACTER_WHEEL = 19,
            INPUT_MULTIPLAYER_INFO = 20,
            INPUT_SPRINT = 21,
            INPUT_JUMP = 22,
            INPUT_ENTER = 23,
            INPUT_ATTACK = 24,
            INPUT_AIM = 25,
            INPUT_LOOK_BEHIND = 26,
            INPUT_PHONE = 27,
            INPUT_SPECIAL_ABILITY = 28,
            INPUT_SPECIAL_ABILITY_SECONDARY = 29,
            INPUT_MOVE_LR = 30,
            INPUT_MOVE_UD = 31,
            INPUT_MOVE_UP_ONLY = 32,
            INPUT_MOVE_DOWN_ONLY = 33,
            INPUT_MOVE_LEFT_ONLY = 34,
            INPUT_MOVE_RIGHT_ONLY = 35,
            INPUT_DUCK = 36,
            INPUT_SELECT_WEAPON = 37,
            INPUT_PICKUP = 38,
            INPUT_SNIPER_ZOOM = 39,
            INPUT_SNIPER_ZOOM_IN_ONLY = 40,
            INPUT_SNIPER_ZOOM_OUT_ONLY = 41,
            INPUT_SNIPER_ZOOM_IN_SECONDARY = 42,
            INPUT_SNIPER_ZOOM_OUT_SECONDARY = 43,
            INPUT_COVER = 44,
            INPUT_RELOAD = 45,
            INPUT_TALK = 46,
            INPUT_DETONATE = 47,
            INPUT_HUD_SPECIAL = 48,
            INPUT_ARREST = 49,
            INPUT_ACCURATE_AIM = 50,
            INPUT_CONTEXT = 51,
            INPUT_CONTEXT_SECONDARY = 52,
            INPUT_WEAPON_SPECIAL = 53,
            INPUT_WEAPON_SPECIAL_TWO = 54,
            INPUT_DIVE = 55,
            INPUT_DROP_WEAPON = 56,
            INPUT_DROP_AMMO = 57,
            INPUT_THROW_GRENADE = 58,
            INPUT_VEH_MOVE_LR = 59,
            INPUT_VEH_MOVE_UD = 60,
            INPUT_VEH_MOVE_UP_ONLY = 61,
            INPUT_VEH_MOVE_DOWN_ONLY = 62,
            INPUT_VEH_MOVE_LEFT_ONLY = 63,
            INPUT_VEH_MOVE_RIGHT_ONLY = 64,
            INPUT_VEH_SPECIAL = 65,
            INPUT_VEH_GUN_LR = 66,
            INPUT_VEH_GUN_UD = 67,
            INPUT_VEH_AIM = 68,
            INPUT_VEH_ATTACK = 69,
            INPUT_VEH_ATTACK2 = 70,
            INPUT_VEH_ACCELERATE = 71,
            INPUT_VEH_BRAKE = 72,
            INPUT_VEH_DUCK = 73,
            INPUT_VEH_HEADLIGHT = 74,
            INPUT_VEH_EXIT = 75,
            INPUT_VEH_HANDBRAKE = 76,
            INPUT_VEH_HOTWIRE_LEFT = 77,
            INPUT_VEH_HOTWIRE_RIGHT = 78,
            INPUT_VEH_LOOK_BEHIND = 79,
            INPUT_VEH_CIN_CAM = 80,
            INPUT_VEH_NEXT_RADIO = 81,
            INPUT_VEH_PREV_RADIO = 82,
            INPUT_VEH_NEXT_RADIO_TRACK = 83,
            INPUT_VEH_PREV_RADIO_TRACK = 84,
            INPUT_VEH_RADIO_WHEEL = 85,
            INPUT_VEH_HORN = 86,
            INPUT_VEH_FLY_THROTTLE_UP = 87,
            INPUT_VEH_FLY_THROTTLE_DOWN = 88,
            INPUT_VEH_FLY_YAW_LEFT = 89,
            INPUT_VEH_FLY_YAW_RIGHT = 90,
            INPUT_VEH_PASSENGER_AIM = 91,
            INPUT_VEH_PASSENGER_ATTACK = 92,
            INPUT_VEH_SPECIAL_ABILITY_FRANKLIN = 93,
            INPUT_VEH_STUNT_UD = 94,
            INPUT_VEH_CINEMATIC_UD = 95,
            INPUT_VEH_CINEMATIC_UP_ONLY = 96,
            INPUT_VEH_CINEMATIC_DOWN_ONLY = 97,
            INPUT_VEH_CINEMATIC_LR = 98,
            INPUT_VEH_SELECT_NEXT_WEAPON = 99,
            INPUT_VEH_SELECT_PREV_WEAPON = 100,
            INPUT_VEH_ROOF = 101,
            INPUT_VEH_JUMP = 102,
            INPUT_VEH_GRAPPLING_HOOK = 103,
            INPUT_VEH_SHUFFLE = 104,
            INPUT_VEH_DROP_PROJECTILE = 105,
            INPUT_VEH_MOUSE_CONTROL_OVERRIDE = 106,
            INPUT_VEH_FLY_ROLL_LR = 107,
            INPUT_VEH_FLY_ROLL_LEFT_ONLY = 108,
            INPUT_VEH_FLY_ROLL_RIGHT_ONLY = 109,
            INPUT_VEH_FLY_PITCH_UD = 110,
            INPUT_VEH_FLY_PITCH_UP_ONLY = 111,
            INPUT_VEH_FLY_PITCH_DOWN_ONLY = 112,
            INPUT_VEH_FLY_UNDERCARRIAGE = 113,
            INPUT_VEH_FLY_ATTACK = 114,
            INPUT_VEH_FLY_SELECT_NEXT_WEAPON = 115,
            INPUT_VEH_FLY_SELECT_PREV_WEAPON = 116,
            INPUT_VEH_FLY_SELECT_TARGET_LEFT = 117,
            INPUT_VEH_FLY_SELECT_TARGET_RIGHT = 118,
            INPUT_VEH_FLY_VERTICAL_FLIGHT_MODE = 119,
            INPUT_VEH_FLY_DUCK = 120,
            INPUT_VEH_FLY_ATTACK_CAMERA = 121,
            INPUT_VEH_FLY_MOUSE_CONTROL_OVERRIDE = 122,
            INPUT_VEH_SUB_TURN_LR = 123,
            INPUT_VEH_SUB_TURN_LEFT_ONLY = 124,
            INPUT_VEH_SUB_TURN_RIGHT_ONLY = 125,
            INPUT_VEH_SUB_PITCH_UD = 126,
            INPUT_VEH_SUB_PITCH_UP_ONLY = 127,
            INPUT_VEH_SUB_PITCH_DOWN_ONLY = 128,
            INPUT_VEH_SUB_THROTTLE_UP = 129,
            INPUT_VEH_SUB_THROTTLE_DOWN = 130,
            INPUT_VEH_SUB_ASCEND = 131,
            INPUT_VEH_SUB_DESCEND = 132,
            INPUT_VEH_SUB_TURN_HARD_LEFT = 133,
            INPUT_VEH_SUB_TURN_HARD_RIGHT = 134,
            INPUT_VEH_SUB_MOUSE_CONTROL_OVERRIDE = 135,
            INPUT_VEH_PUSHBIKE_PEDAL = 136,
            INPUT_VEH_PUSHBIKE_SPRINT = 137,
            INPUT_VEH_PUSHBIKE_FRONT_BRAKE = 138,
            INPUT_VEH_PUSHBIKE_REAR_BRAKE = 139,
            INPUT_MELEE_ATTACK_LIGHT = 140,
            INPUT_MELEE_ATTACK_HEAVY = 141,
            INPUT_MELEE_ATTACK_ALTERNATE = 142,
            INPUT_MELEE_BLOCK = 143,
            INPUT_PARACHUTE_DEPLOY = 144,
            INPUT_PARACHUTE_DETACH = 145,
            INPUT_PARACHUTE_TURN_LR = 146,
            INPUT_PARACHUTE_TURN_LEFT_ONLY = 147,
            INPUT_PARACHUTE_TURN_RIGHT_ONLY = 148,
            INPUT_PARACHUTE_PITCH_UD = 149,
            INPUT_PARACHUTE_PITCH_UP_ONLY = 150,
            INPUT_PARACHUTE_PITCH_DOWN_ONLY = 151,
            INPUT_PARACHUTE_BRAKE_LEFT = 152,
            INPUT_PARACHUTE_BRAKE_RIGHT = 153,
            INPUT_PARACHUTE_SMOKE = 154,
            INPUT_PARACHUTE_PRECISION_LANDING = 155,
            INPUT_MAP = 156,
            INPUT_SELECT_WEAPON_UNARMED = 157,
            INPUT_SELECT_WEAPON_MELEE = 158,
            INPUT_SELECT_WEAPON_HANDGUN = 159,
            INPUT_SELECT_WEAPON_SHOTGUN = 160,
            INPUT_SELECT_WEAPON_SMG = 161,
            INPUT_SELECT_WEAPON_AUTO_RIFLE = 162,
            INPUT_SELECT_WEAPON_SNIPER = 163,
            INPUT_SELECT_WEAPON_HEAVY = 164,
            INPUT_SELECT_WEAPON_SPECIAL = 165,
            INPUT_SELECT_CHARACTER_MICHAEL = 166,
            INPUT_SELECT_CHARACTER_FRANKLIN = 167,
            INPUT_SELECT_CHARACTER_TREVOR = 168,
            INPUT_SELECT_CHARACTER_MULTIPLAYER = 169,
            INPUT_SAVE_REPLAY_CLIP = 170,
            INPUT_SPECIAL_ABILITY_PC = 171,
            INPUT_CELLPHONE_UP = 172,
            INPUT_CELLPHONE_DOWN = 173,
            INPUT_CELLPHONE_LEFT = 174,
            INPUT_CELLPHONE_RIGHT = 175,
            INPUT_CELLPHONE_SELECT = 176,
            INPUT_CELLPHONE_CANCEL = 177,
            INPUT_CELLPHONE_OPTION = 178,
            INPUT_CELLPHONE_EXTRA_OPTION = 179,
            INPUT_CELLPHONE_SCROLL_FORWARD = 180,
            INPUT_CELLPHONE_SCROLL_BACKWARD = 181,
            INPUT_CELLPHONE_CAMERA_FOCUS_LOCK = 182,
            INPUT_CELLPHONE_CAMERA_GRID = 183,
            INPUT_CELLPHONE_CAMERA_SELFIE = 184,
            INPUT_CELLPHONE_CAMERA_DOF = 185,
            INPUT_CELLPHONE_CAMERA_EXPRESSION = 186,
            INPUT_FRONTEND_DOWN = 187,
            INPUT_FRONTEND_UP = 188,
            INPUT_FRONTEND_LEFT = 189,
            INPUT_FRONTEND_RIGHT = 190,
            INPUT_FRONTEND_RDOWN = 191,
            INPUT_FRONTEND_RUP = 192,
            INPUT_FRONTEND_RLEFT = 193,
            INPUT_FRONTEND_RRIGHT = 194,
            INPUT_FRONTEND_AXIS_X = 195,
            INPUT_FRONTEND_AXIS_Y = 196,
            INPUT_FRONTEND_RIGHT_AXIS_X = 197,
            INPUT_FRONTEND_RIGHT_AXIS_Y = 198,
            INPUT_FRONTEND_PAUSE = 199,
            INPUT_FRONTEND_PAUSE_ALTERNATE = 200,
            INPUT_FRONTEND_ACCEPT = 201,
            INPUT_FRONTEND_CANCEL = 202,
            INPUT_FRONTEND_X = 203,
            INPUT_FRONTEND_Y = 204,
            INPUT_FRONTEND_LB = 205,
            INPUT_FRONTEND_RB = 206,
            INPUT_FRONTEND_LT = 207,
            INPUT_FRONTEND_RT = 208,
            INPUT_FRONTEND_LS = 209,
            INPUT_FRONTEND_RS = 210,
            INPUT_FRONTEND_LEADERBOARD = 211,
            INPUT_FRONTEND_SOCIAL_CLUB = 212,
            INPUT_FRONTEND_SOCIAL_CLUB_SECONDARY = 213,
            INPUT_FRONTEND_DELETE = 214,
            INPUT_FRONTEND_ENDSCREEN_ACCEPT = 215,
            INPUT_FRONTEND_ENDSCREEN_EXPAND = 216,
            INPUT_FRONTEND_SELECT = 217,
            INPUT_SCRIPT_LEFT_AXIS_X = 218,
            INPUT_SCRIPT_LEFT_AXIS_Y = 219,
            INPUT_SCRIPT_RIGHT_AXIS_X = 220,
            INPUT_SCRIPT_RIGHT_AXIS_Y = 221,
            INPUT_SCRIPT_RUP = 222,
            INPUT_SCRIPT_RDOWN = 223,
            INPUT_SCRIPT_RLEFT = 224,
            INPUT_SCRIPT_RRIGHT = 225,
            INPUT_SCRIPT_LB = 226,
            INPUT_SCRIPT_RB = 227,
            INPUT_SCRIPT_LT = 228,
            INPUT_SCRIPT_RT = 229,
            INPUT_SCRIPT_LS = 230,
            INPUT_SCRIPT_RS = 231,
            INPUT_SCRIPT_PAD_UP = 232,
            INPUT_SCRIPT_PAD_DOWN = 233,
            INPUT_SCRIPT_PAD_LEFT = 234,
            INPUT_SCRIPT_PAD_RIGHT = 235,
            INPUT_SCRIPT_SELECT = 236,
            INPUT_CURSOR_ACCEPT = 237,
            INPUT_CURSOR_CANCEL = 238,
            INPUT_CURSOR_X = 239,
            INPUT_CURSOR_Y = 240,
            INPUT_CURSOR_SCROLL_UP = 241,
            INPUT_CURSOR_SCROLL_DOWN = 242,
            INPUT_ENTER_CHEAT_CODE = 243,
            INPUT_INTERACTION_MENU = 244,
            INPUT_MP_TEXT_CHAT_ALL = 245,
            INPUT_MP_TEXT_CHAT_TEAM = 246,
            INPUT_MP_TEXT_CHAT_FRIENDS = 247,
            INPUT_MP_TEXT_CHAT_CREW = 248,
            INPUT_PUSH_TO_TALK = 249,
            INPUT_CREATOR_LS = 250,
            INPUT_CREATOR_RS = 251,
            INPUT_CREATOR_LT = 252,
            INPUT_CREATOR_RT = 253,
            INPUT_CREATOR_MENU_TOGGLE = 254,
            INPUT_CREATOR_ACCEPT = 255,
            INPUT_CREATOR_DELETE = 256,
            INPUT_ATTACK2 = 257,
            INPUT_RAPPEL_JUMP = 258,
            INPUT_RAPPEL_LONG_JUMP = 259,
            INPUT_RAPPEL_SMASH_WINDOW = 260,
            INPUT_PREV_WEAPON = 261,
            INPUT_NEXT_WEAPON = 262,
            INPUT_MELEE_ATTACK1 = 263,
            INPUT_MELEE_ATTACK2 = 264,
            INPUT_WHISTLE = 265,
            INPUT_MOVE_LEFT = 266,
            INPUT_MOVE_RIGHT = 267,
            INPUT_MOVE_UP = 268,
            INPUT_MOVE_DOWN = 269,
            INPUT_LOOK_LEFT = 270,
            INPUT_LOOK_RIGHT = 271,
            INPUT_LOOK_UP = 272,
            INPUT_LOOK_DOWN = 273,
            INPUT_SNIPER_ZOOM_IN = 274,
            INPUT_SNIPER_ZOOM_OUT = 275,
            INPUT_SNIPER_ZOOM_IN_ALTERNATE = 276,
            INPUT_SNIPER_ZOOM_OUT_ALTERNATE = 277,
            INPUT_VEH_MOVE_LEFT = 278,
            INPUT_VEH_MOVE_RIGHT = 279,
            INPUT_VEH_MOVE_UP = 280,
            INPUT_VEH_MOVE_DOWN = 281,
            INPUT_VEH_GUN_LEFT = 282,
            INPUT_VEH_GUN_RIGHT = 283,
            INPUT_VEH_GUN_UP = 284,
            INPUT_VEH_GUN_DOWN = 285,
            INPUT_VEH_LOOK_LEFT = 286,
            INPUT_VEH_LOOK_RIGHT = 287,
            INPUT_REPLAY_START_STOP_RECORDING = 288,
            INPUT_REPLAY_START_STOP_RECORDING_SECONDARY = 289,
            INPUT_SCALED_LOOK_LR = 290,
            INPUT_SCALED_LOOK_UD = 291,
            INPUT_SCALED_LOOK_UP_ONLY = 292,
            INPUT_SCALED_LOOK_DOWN_ONLY = 293,
            INPUT_SCALED_LOOK_LEFT_ONLY = 294,
            INPUT_SCALED_LOOK_RIGHT_ONLY = 295,
            INPUT_REPLAY_MARKER_DELETE = 296,
            INPUT_REPLAY_CLIP_DELETE = 297,
            INPUT_REPLAY_PAUSE = 298,
            INPUT_REPLAY_REWIND = 299,
            INPUT_REPLAY_FFWD = 300,
            INPUT_REPLAY_NEWMARKER = 301,
            INPUT_REPLAY_RECORD = 302,
            INPUT_REPLAY_SCREENSHOT = 303,
            INPUT_REPLAY_HIDEHUD = 304,
            INPUT_REPLAY_STARTPOINT = 305,
            INPUT_REPLAY_ENDPOINT = 306,
            INPUT_REPLAY_ADVANCE = 307,
            INPUT_REPLAY_BACK = 308,
            INPUT_REPLAY_TOOLS = 309,
            INPUT_REPLAY_RESTART = 310,
            INPUT_REPLAY_SHOWHOTKEY = 311,
            INPUT_REPLAY_CYCLEMARKERLEFT = 312,
            INPUT_REPLAY_CYCLEMARKERRIGHT = 313,
            INPUT_REPLAY_FOVINCREASE = 314,
            INPUT_REPLAY_FOVDECREASE = 315,
            INPUT_REPLAY_CAMERAUP = 316,
            INPUT_REPLAY_CAMERADOWN = 317,
            INPUT_REPLAY_SAVE = 318,
            INPUT_REPLAY_TOGGLETIME = 319,
            INPUT_REPLAY_TOGGLETIPS = 320,
            INPUT_REPLAY_PREVIEW = 321,
            INPUT_REPLAY_TOGGLE_TIMELINE = 322,
            INPUT_REPLAY_TIMELINE_PICKUP_CLIP = 323,
            INPUT_REPLAY_TIMELINE_DUPLICATE_CLIP = 324,
            INPUT_REPLAY_TIMELINE_PLACE_CLIP = 325,
            INPUT_REPLAY_CTRL = 326,
            INPUT_REPLAY_TIMELINE_SAVE = 327,
            INPUT_REPLAY_PREVIEW_AUDIO = 328,
            INPUT_VEH_DRIVE_LOOK = 329,
            INPUT_VEH_DRIVE_LOOK2 = 330,
            INPUT_VEH_FLY_ATTACK2 = 331,
            INPUT_RADIO_WHEEL_UD = 332,
            INPUT_RADIO_WHEEL_LR = 333,
            INPUT_VEH_SLOWMO_UD = 334,
            INPUT_VEH_SLOWMO_UP_ONLY = 335,
            INPUT_VEH_SLOWMO_DOWN_ONLY = 336,
            INPUT_MAP_POI = 337
        }

        public static string InputString(GTA.Control control, int controlInt = -1)
        {
            int controlID = controlInt == -1 ? (int)control : controlInt;
            return "~" + (ControlString)controlID + "~";
        }

        public static string InputString(System.Windows.Forms.Keys key, int controlInt = -1)
        {
            return key.ToString();
        }

        public static string InputString(string keyboardKey, GTA.Control control, int controlInt = -1)
        {
            if (!UsingGamepad())
            {
                return keyboardKey;
            }
            else
            {
                int controlID = controlInt == -1 ? (int)control : controlInt;
                return "~" + (ControlString)controlID + "~";
            }
        }

        public static string InputString(System.Windows.Forms.Keys key, GTA.Control control, int controlInt = -1)
        {
            if (!UsingGamepad())
            {
                return key.ToString();
            }
            else
            {
                int controlID = controlInt == -1 ? (int)control : controlInt;
                return "~" + (ControlString)controlID + "~";
            }
        }

        public static bool UsingGamepad()
        {
            return Game.LastInputMethod == InputMethod.GamePad;
        }

        public static bool GetScreenCoordFromWorldCoord(Vector3 worldCoord, out float screenX, out float screenY)
        {
            OutputArgument x = new OutputArgument();
            OutputArgument y = new OutputArgument();
            bool worldCoordIsNotOnScreen = Function.Call<bool>(Hash._GET_SCREEN_COORD_FROM_WORLD_COORD, worldCoord.X, worldCoord.Y, worldCoord.Z, x, y);
            screenX = x.GetResult<float>();
            screenY = y.GetResult<float>();
            return !worldCoordIsNotOnScreen;
        }

        public static Vector2 GetTextureResolution(string dict, string name, out bool textureExists)
        {
            Vector3 WH = Function.Call<Vector3>(Hash.GET_TEXTURE_RESOLUTION, dict, name);
            if (WH != new Vector3(4f, 4f, 0f))
            {
                textureExists = true;
            }
            else
            {
                textureExists = false;
            }
            return new Vector2(WH.X, WH.Y);
        }
    }
}
