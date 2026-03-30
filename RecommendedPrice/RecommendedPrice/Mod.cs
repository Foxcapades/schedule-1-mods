using Fxcpds;
using HarmonyLib;
using MelonLoader;
using UnityEngine;
#if IL2CPP
using Il2CppInterop.Runtime;
using Il2CppScheduleOne.Product;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
#elif MONO
using ScheduleOne.Product;
using System;
using System.Collections.Generic;
#endif

[assembly: MelonInfo(typeof(RecommendedPrice.Mod), RecommendedPrice.Mod.MOD_NAME, "1.1.2", "Foxcapades")]
[assembly: MelonGame("TVGS", "Schedule I")]

#nullable enable
namespace RecommendedPrice {
  [HarmonyPatch]
  public class Mod: FxMod {
    public const string MOD_NAME = "Recommended Price";

    private static MelonPreferences_Category? preferences;
    private static MelonPreferences_Entry<float>? weedModifier;
    private static MelonPreferences_Entry<float>? cokeModifier;
    private static MelonPreferences_Entry<float>? methModifier;
    private static MelonPreferences_Entry<float>? shrmModifier;

    private static Action<ProductDefinition>? onProductAction;

    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
    private static readonly Dictionary<string, float> originalProductPrices = new Dictionary<string, float>(12);

    public override void OnInitializeMelon() {
      preferences = MelonPreferences.CreateCategory("Recommended Price", "Recommended Price");
      weedModifier = preferences.CreateEntry("weedModifier", 1f, "Weed Price Multiplier");
      cokeModifier = preferences.CreateEntry("cocaineModifier", 1f, "Cocaine Price Multiplier");
      methModifier = preferences.CreateEntry("methModifier", 1f, "Meth Price Multiplier");
      shrmModifier = preferences.CreateEntry("shoomModifier", 1f, "Shroom Price Multiplier");
    }

    protected override void onModPreferencesLoaded() {
      applyInMainOnly();
    }

    protected override void onModPreferencesSaved() {
      applyInMainOnly();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ProductManager), nameof(ProductManager.OnStartServer))]
    static void PreStartServer(ProductManager __instance) {
      #if IL2CPP
      onProductAction = DelegateSupport.ConvertDelegate<Action<ProductDefinition>>(onProduct);
      #elif MONO
      onProductAction = onProduct;
      #endif

      __instance.onProductDiscovered += onProductAction;
      __instance.onNewProductCreated += onProductAction;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ProductManager), "Clean")]
    static void PostClean(ProductManager __instance) {
      unapplyModifiers(__instance);

      if (onProductAction == null)
        return;

      __instance.onProductDiscovered -= onProductAction;
      __instance.onNewProductCreated -= onProductAction;
      onProductAction = null;
    }

    private static void applyInMainOnly() {
      if (!Instance.InMainScene)
        return;

      var logger = Melon<Mod>.Instance.LoggerInstance;
      var manager = ProductManager.Instance;

      logger.Msg("attempting to apply recommended price modifiers");

      var prices = getProductPrices();

      if (prices == null) {
        logger.Error("failed to get product prices, cannot modify price values");
        return;
      }

      foreach (var product in manager.AllProducts)
        apply(prices, product);
    }

    private static void onProduct(ProductDefinition product) {
      apply(getProductPrices(), product);
    }

    private static void apply(Dictionary<ProductDefinition, float>? prices, ProductDefinition product) {
      var mktValue = product.MarketValue;
      originalProductPrices.TryAdd(product.ID, product.MarketValue);
      product.MarketValue = safeMultiply(originalProductPrices[product.ID], product.DrugType);

      if (prices == null || !prices.TryGetValue(product, out var price))
        return;

      if (price == 0 || Mathf.Approximately(price, product.BasePrice) || Mathf.Approximately(price, mktValue))
        prices[product] = product.MarketValue;
    }

    private static void unapplyModifiers(ProductManager manager) {
      var prices = getProductPrices();
      var logger = Melon<Mod>.Instance.LoggerInstance;

      logger.Msg("attempting to revert recommended price modifiers");

      if (prices == null) {
        logger.Error("failed to get product prices, cannot revert price values");
        return;
      }

      foreach (var product in manager.AllProducts) {
        if (!originalProductPrices.ContainsKey(product.ID)) {
          logger.Warning("unrecognized product id {0}", product.ID);
          continue;
        }

        if (!prices.ContainsKey(product)) {
          logger.Warning("product id {0} not found in product price index", product.ID);
          continue;
        }

        prices[product] = originalProductPrices[product.ID];
      }
    }


    private static float safeMultiply(float price, EDrugType type) {
      var mult = type switch {
        EDrugType.Marijuana => weedModifier!.Value,
        EDrugType.Methamphetamine => methModifier!.Value,
        EDrugType.Cocaine => cokeModifier!.Value,
        EDrugType.Shrooms => shrmModifier!.Value,
        _ => 1
      };

      mult = mult < 0.01f ? 0.01f : mult;

      return (float) Math.Round(price * mult, MidpointRounding.AwayFromZero);
    }

    private static Dictionary<ProductDefinition, float>? getProductPrices() {
      var prop = AccessTools.DeclaredProperty(typeof(ProductManager), "ProductPrices");

      if (prop == null)
        return null;

      return (Dictionary<ProductDefinition, float>?) prop.GetValue(ProductManager.Instance);
    }
  }
}
