using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;

namespace PrivacyPlus
{
    public class PrivacyPlus : NeosMod {
        public override string Name => "PrivacyPlus";
        public override string Author => "Psychpsyo";
        public override string Version => "1.0.1";
        public override string Link => "https://github.com/Psychpsyo/PrivacyPlus";

        // config keys
        [AutoRegisterConfigKey]
        public static readonly ModConfigurationKey<string> UniqueDeviceIdentifierHash = new ModConfigurationKey<string>("Hashed UniqueDeviceIdentifier", "Sets your UniqueDeviceIdentifier Hash for sending to other users.", GenerateFakeUniqueDeviceIdentifierHash);

        private static ModConfiguration config;
        public override void OnEngineInit()
        {
            config = GetConfiguration();

            Harmony harmony = new Harmony("Psychpsyo.PrivacyPlus");
            harmony.PatchAll();
        }

        private static string GenerateFakeUniqueDeviceIdentifierHash()
        {
            var csprng = new RNGCryptoServiceProvider();
            var bytes = new byte[32];

            csprng.GetNonZeroBytes(bytes);
            return String.Concat(Array.ConvertAll(bytes, x => x.ToString("X2")));
        }


        [HarmonyPatch(typeof(FrooxEngine.PlatformInterface), "GetExtraUserIds")]
        class PrivacyPlusExIds
        {
            public static Dictionary<string, string> Postfix(Dictionary<string, string> originalExIds)
            {
                // return a new dict that only has our fake UniqueDeviceIdentifier hash
                return new Dictionary<string, string>() { { "UniqueDeviceIdentifier", config.GetValue(UniqueDeviceIdentifierHash) } };
            }
        }
    }
}
