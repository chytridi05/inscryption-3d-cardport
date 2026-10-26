global using static OriginalPort.Plugin;
using BepInEx;
using BepInEx.Logging;
using DiskCardGame;
using HarmonyLib;
using InscryptionAPI.Ascension;
using InscryptionAPI.Card;
using System.Reflection;
using UnityEngine;

namespace OriginalPort;

[BepInPlugin(GUID, NAME, VERSION)]
[BepInDependency("community.inscryption.patch")]
[BepInDependency("cyantist.inscryption.api")]
public class Plugin : BaseUnityPlugin
{
    public const string GUID = "chytridi05.inscryption.cardport";
    public const string NAME = "CardPort";
    public const string VERSION = "0.0.1";
    internal static ManualLogSource Log;
    internal static new class Config
    {
        internal static bool CompleteMissingPortraits;
    }
    private void Awake() 
    {
        Log = Logger;
        Config.CompleteMissingPortraits = base.Config.Bind("Art", "Complete Missing Portraits", true, "Toggle whether to complete missing card portraits with a default texture").Value;
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        StarterDeckManager.New(GUID, "Undead", Resources.Load<Texture2D>("Art/UI/Ascension/starterdeck_icon_vanilla"), new string[]
        {
            "Gravedigger",
            "Zombie",
            "FrankNStein",
        });
        StarterDeckManager.New(GUID, "Technology", Resources.Load<Texture2D>("Art/UI/Ascension/starterdeck_icon_vanilla"), new string[]
        {
            "Shieldbot",
            "Sniper",
            "CloserBot",
        });
        StarterDeckManager.New(GUID, "Wizard", Resources.Load<Texture2D>("Art/UI/Ascension/starterdeck_icon_vanilla"), new string[]
        {
            "RubyGolem",
            "JuniorSage",
            "GemFiend",
        });
        CardManager.ModifyCardList += OnModifyCardList;
        Log.LogInfo($"Loaded {NAME} {VERSION}");
    }
    private static List<CardInfo> OnModifyCardList(List<CardInfo> list)
    {
        foreach (CardInfo item in list.Where(x => x.IsBaseGameCard()))
        {
            if (item.metaCategories.Count == 0 || (item.metaCategories.Count == 1 && item.metaCategories.Contains(CardMetaCategory.GBCPlayable)))
            {
                continue;
            }
            if (item.name == "Skeleton" || item.name == "MoxEmerald" || item.name == "MoxRuby" || item.name == "MoxSapphire")
            {
                continue;
            }
            if (item.portraitTex == null)
            {
                if (!Config.CompleteMissingPortraits)
                {
                    continue;
                }
                item.SetPortrait(Resources.Load<Sprite>("Art/Cards/Portraits/portrait_adder"));
                item.SetEmissivePortrait(Resources.Load<Sprite>("Art/Cards/Portraits/portrait_adder_emission"));
            }
            if (item.temple != CardTemple.Nature)
            {
                if (!item.HasCardMetaCategory(CardMetaCategory.Rare))
                {
                    if (!item.HasCardMetaCategory(CardMetaCategory.ChoiceNode))
                    {
                        item.AddMetaCategories(CardMetaCategory.ChoiceNode);
                    }
                    if (!item.HasCardMetaCategory(CardMetaCategory.TraderOffer))
                    {
                        item.AddMetaCategories(CardMetaCategory.TraderOffer);
                    }
                }
            }
            else
            {
                if (!item.HasCardMetaCategory(CardMetaCategory.Rare) && item.HasCardMetaCategory(CardMetaCategory.GBCPack))
                {
                    if (!item.HasCardMetaCategory(CardMetaCategory.ChoiceNode))
                    {
                        item.AddMetaCategories(CardMetaCategory.ChoiceNode);
                    }
                    if (!item.HasCardMetaCategory(CardMetaCategory.TraderOffer))
                    {
                        item.AddMetaCategories(CardMetaCategory.TraderOffer);
                    }
                }
            }
        }
        return list;
    }
    private static readonly Harmony harmony = new(GUID);
}
