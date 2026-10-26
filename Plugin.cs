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
    internal static bool IsActive { get; set; }
    private void Awake() 
    {
        Log = Logger;
        Config.CompleteMissingPortraits = base.Config.Bind("Art", "Complete Missing Portraits", true, "Toggle whether to complete missing card portraits with a default texture").Value;
        harmony.PatchAll(assembly);
        CardManager.ModifyCardList += OnModifyCardList;
        StarterDeckManager.ModifyDeckList += OnModifyDeckList;
        Log.LogInfo($"Loaded {NAME} {VERSION}");
    }
    private static List<CardInfo> OnModifyCardList(List<CardInfo> list)
    {
        foreach (CardInfo item in list.Where(x => x.IsBaseGameCard()))
        {
            if (item.metaCategories.Count == 0 || item.metaCategories.Count == 1 && (item.metaCategories.Contains(CardMetaCategory.Rare) || item.metaCategories.Contains(CardMetaCategory.GBCPlayable)))
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
    private static List<StarterDeckManager.FullStarterDeck> OnModifyDeckList(List<StarterDeckManager.FullStarterDeck> list)
    {
        return list;
    }
    private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
    private static readonly AssetBundle assets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(assembly.Location), "assets"));
    private static readonly Harmony harmony = new(GUID);
}
