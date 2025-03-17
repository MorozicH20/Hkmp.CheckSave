using Hkmp.Networking.Packet;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hkmp.CheckSave.Models
{
    public class AllowedSave
    {
        [JsonProperty("maxHealth")]
        public int maxHealth;

        [JsonProperty("maxMP")]
        public int maxMP;

        [JsonProperty("geo")]
        public int geo;

        [JsonProperty("allowedCharms")]
        public Charm[] AllowedCharms { get; set; }

        [JsonProperty("allowedSkills")]
        public Skill[] AllowedSkills { get; set; }

        [JsonProperty("bannedCharms")]
        public Charm[] BannedCharms { get; set; }

        [JsonProperty("bannedSkills")]
        public Skill[] BannedSkills { get; set; }


        /// <inheritdoc cref="IPacketData" />
        public void WriteData(IPacket packet)
        {
            var length = (byte)AllowedCharms.Length;
            packet.Write(length);

            for (var i = 0; i < length; i++)
            {
                packet.Write((byte)AllowedCharms[i]);
            }

            length = (byte)BannedCharms.Length;
            packet.Write(length);

            for (var i = 0; i < length; i++)
            {
                packet.Write((byte)BannedCharms[i]);
            }

            length = (byte)AllowedSkills.Length;
            packet.Write(length);

            for (var i = 0; i < length; i++)
            {
                packet.Write((byte)AllowedSkills[i]);
            }

            length = (byte)BannedSkills.Length;
            packet.Write(length);

            for (var i = 0; i < length; i++)
            {
                packet.Write((byte)BannedSkills[i]);
            }
        }

        /// <inheritdoc cref="IPacketData" />
        public void ReadData(IPacket packet)
        {
            var length = packet.ReadByte();
            AllowedCharms = new Charm[length];

            for (var i = 0; i < length; i++)
            {
                AllowedCharms[i] = (Charm)packet.ReadByte();
            }

            length = packet.ReadByte();
            BannedCharms = new Charm[length];

            for (var i = 0; i < length; i++)
            {
                BannedCharms[i] = (Charm)packet.ReadByte();
            }

            length = packet.ReadByte();
            AllowedSkills = new Skill[length];

            for (var i = 0; i < length; i++)
            {
                AllowedSkills[i] = (Skill)packet.ReadByte();
            }


            length = packet.ReadByte();
            BannedSkills = new Skill[length];

            for (var i = 0; i < length; i++)
            {
                BannedSkills[i] = (Skill)packet.ReadByte();
            }
        }


        [JsonConverter(typeof(StringEnumConverter))]
        public enum Charm
        {
            GatheringSwarm = 1,
            WaywardCompass,
            GrubSong,
            StalwartShell,
            BaldurShell,
            FuryOfTheFallen,
            QuickFocus,
            LifebloodHeart,
            LifebloodCore,
            DefendersCrest,
            Flukenest,
            ThornsOfAgony,
            MarkOfPride,
            SteadyBody,
            HeavyBlow,
            SharpShadow,
            SporeShroom,
            LongNail,
            ShamanStone,
            SoulCatcher,
            SoulEater,
            GlowingWomb,
            UnbreakableHeart,
            UnbreakableGreed,
            UnbreakableStrength,
            NailmastersGlory,
            JonisBlessing,
            ShapeOfUnn,
            Hiveblood,
            Dreamwielder,
            Dashmaster,
            QuickSlash,
            SpellTwister,
            DeepFocus,
            GrubberflysElegy,
            Kingsoul,
            Sprintmaster,
            Dreamshield,
            Weaversong,
            Grimmchild,
            CarefreeMelody
        }

        /// <summary>
        /// Enumeration of all skills that can be configured for a loadout.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Skill
        {
            VengefulSpirit = 0,
            DesolateDive,
            HowlingWraiths,
            ShadeSoul,
            DescendingDark,
            AbyssShriek,
            MothwingCloak,
            MantisClaw,
            CrystalHeart,
            MonarchWings,
            IsmasTear,
            ShadeCloak,
            DreamNail,
            CycloneSlash,
            DashSlash,
            GreatSlash,
        }
    }
}
