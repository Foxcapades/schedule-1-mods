using CartelInfluenceTweaks.state;
using Fxcpds;
using MelonLoader;
using System;

namespace CartelInfluenceTweaks {
  internal struct Preferences {
    private MelonPreferences_Entry<float>[] prefs;

    /// <summary>
    /// Cartel influence percent lost when a player cleans/removes cartel
    /// created graffiti.
    /// </summary>
    public float graffitiRemovalChange => prefs[(int) ActionType.GraffitiCleaned].Value;

    /// <summary>
    /// Cartel influence percent lost when a player creates new graffiti in a
    /// region with cartel influence.
    /// </summary>
    public float graffitiCreationChange => prefs[(int) ActionType.GraffitiCreated].Value;

    /// <summary>
    /// Cartel influence percent lost when a player interrupts a cartel thug
    /// spray-painting new graffiti.
    /// </summary>
    public float graffitiInterruptedChange => prefs[(int) ActionType.GraffitiInterrupted].Value;

    /// <summary>
    /// Cartel influence percent lost when a player kills or knocks out a cartel
    /// dealer.
    /// </summary>
    public float dealerKilledChange => prefs[(int) ActionType.DealerDefeated].Value;

    /// <summary>
    /// Cartel influence percent lost when a player kills or knocks out all
    /// cartel thugs in an ambush.
    /// </summary>
    public float ambushKilledChange => prefs[(int) ActionType.AmbushDefeated].Value;

    /// <summary>
    /// Cartel influence percent lost when a player gains a new customer in
    /// cartel territory.
    /// </summary>
    public float customerGainedChange => prefs[(int) ActionType.CustomerStolen].Value;

    /// <summary>
    /// Cartel influence percent lost when a player completes a deal in cartel
    /// territory.
    /// </summary>
    public float playerDealChange => prefs[(int) ActionType.PlayerDeal].Value;

    /// <summary>
    /// Cartel influence percent lost when a player employed dealer completes
    /// a deal in cartel territory.
    /// </summary>
    public float playerProxyDealChange => prefs[(int) ActionType.PlayerProxyDeal].Value;

    /// <summary>
    /// Cartel influence percent gained when a cartel member creates new
    /// graffiti.
    /// </summary>
    public float cartelGraffitiChange => prefs[(int) ActionType.CartelGraffiti].Value;

    /// <summary>
    /// Cartel influence percent gained when a cartel dealer completes a deal.
    /// </summary>
    public float cartelDealChange => prefs[(int) ActionType.CartelDeal].Value;

    /// <summary>
    /// Cartel influence percent gained when a cartel thug kills a player.
    /// </summary>
    public float playerKilledChange => prefs[(int) ActionType.PlayerDefeated].Value;

    public void init(Mod mod) {
      var playerActions = MelonPreferences.CreateCategory(
        identifier: Mod.MOD_NAME + " Player Actions",
        display_name: "Player Actions"
      );
      playerActions.SetFilePath(mod.ConfigPath);

      var cartelActions = MelonPreferences.CreateCategory(
        identifier: Mod.MOD_NAME + " Cartel Actions",
        display_name: "Cartel Actions"
      );
      cartelActions.SetFilePath(mod.ConfigPath);

      var validator = new NumberValidator<float>(0f, 100f);

      prefs = new MelonPreferences_Entry<float>[Enum.GetValues(typeof(ActionType)).Length];

      prefs[(int) ActionType.GraffitiCleaned] = playerActions.CreateEntry(
        identifier: "graffitiRemovalModifier",
        default_value: 5f,
        display_name: "Graffiti Removal",
        description: "Percent of cartel influence lost when a player cleans" +
          " cartel graffiti.",
        validator: validator
      );

      prefs[(int) ActionType.GraffitiCreated] = playerActions.CreateEntry(
        identifier: "graffitiCreationModifier",
        default_value: 5f,
        display_name: "Graffiti Creation",
        description: "Percent of cartel influence lost when a player creates" +
          " new graffiti.",
        validator: validator
      );

      prefs[(int) ActionType.GraffitiInterrupted] = playerActions.CreateEntry(
        identifier: "graffitiInterruptModifier",
        default_value: 10f,
        display_name: "Graffiti Interrupt",
        description: "Percent of cartel influence lost when a player" +
          " interrupts a spray-painting cartel thug.",
        validator: validator
      );

      prefs[(int) ActionType.DealerDefeated] = playerActions.CreateEntry(
        identifier: "dealerKillModifier",
        default_value: 10f,
        display_name: "Dealer Defeated",
        description: "Percent of cartel influence lost when a cartel dealer" +
          " is killed or knocked out by a player.",
        validator: validator
      );

      prefs[(int) ActionType.AmbushDefeated] = playerActions.CreateEntry(
        identifier: "ambushKillModifier",
        default_value: 10f,
        display_name: "Ambush Defeated",
        description: "Percent of cartel influence lost when a cartel ambush" +
          " is defeated by a player.",
        validator: validator
      );

      prefs[(int) ActionType.CustomerStolen] = playerActions.CreateEntry(
        identifier: "customerGainedModifier",
        default_value: 10f,
        display_name: "New Customer",
        description: "Percent of cartel influence lost when a player takes" +
          " one of their customers.",
        validator: validator
      );

      prefs[(int) ActionType.PlayerDeal] = playerActions.CreateEntry(
        identifier: "playerDealModifier",
        default_value: 0f,
        display_name: "Player Deal Completed",
        description: "Percent of cartel influence lost when a player" +
          " makes a deal.",
        validator: validator
      );

      prefs[(int) ActionType.PlayerProxyDeal] = playerActions.CreateEntry(
        identifier: "playerProxyDealModifier",
        default_value: 0f,
        display_name: "Employee Deal Completed",
        description: "Percent of cartel influence lost when a dealer" +
          " makes a deal.",
        validator: validator
      );

      prefs[(int) ActionType.CartelGraffiti] = cartelActions.CreateEntry(
        identifier: "cartelGraffitiModifier",
        default_value: 0f,
        display_name: "Create Graffiti",
        description: "Percent of cartel influence gained when the cartel" +
          " creates graffiti.",
        validator: validator
      );

      prefs[(int) ActionType.CartelDeal] = cartelActions.CreateEntry(
        identifier: "cartelDealModifier",
        default_value: 0f,
        display_name: "Deal Completed",
        description: "Percent of cartel influence gained when the cartel" +
          " makes a deal.",
        validator: validator
      );

      prefs[(int) ActionType.PlayerDefeated] = cartelActions.CreateEntry(
        identifier: "playerDefeatModifier",
        default_value: 0f,
        display_name: "Player Killed",
        description: "Percent of cartel influence gained when the cartel" +
        " kills a player.",
        validator: validator
      );
    }

    public float this[ActionType type] => prefs[(int) type].Value;
  }
}