using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Verse;
using RimWorld;

namespace MadmanSleepTrainer
{

    [StaticConstructorOnStartup]
    static internal class Textures
    {
        public static Dictionary<string, Texture2D> TexturesByFilename;
        static Textures()
        {
            TexturesByFilename = new Dictionary<string, Texture2D>();
            var modDirectory = ModLister.GetActiveModWithIdentifier("M666.cryptosleep.neurotraining").RootDir.FullName;
            var modUITexturesDirectory = Path.Combine(modDirectory, "Textures", "UI");
            var pathsToModUITextures = Directory.GetFiles(modUITexturesDirectory);

            foreach (string pathToModUITexture in pathsToModUITextures)
            {
                var textureFilename = Path.GetFileNameWithoutExtension(pathToModUITexture);
                TexturesByFilename.Add(textureFilename, ContentFinder<Texture2D>.Get("UI/" + textureFilename));
                
            }

        }
    }
    [DefOf]
    static internal class ThisMod_ThingDefof
    {
        static ThisMod_ThingDefof()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }
        public static ThingDef MM_CryptosleepNeurotrainer;
    }


}

