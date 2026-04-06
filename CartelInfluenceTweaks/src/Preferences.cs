using Fxcpds;
using MelonLoader;
using System;

namespace CartelInfluenceTweaks {
  internal struct Preferences {
    private MelonPreferences_Entry<float>[] prefs;

    public float graffitiRemoved => prefs[(int) State.GraffitiCleaned].Value;
    public float graffitiCreated => prefs[(int) State.GraffitiCreated].Value;
    public float graffitiInterrupted => prefs[(int) State.GraffitiInterrupted].Value;

    public float killedDealer => prefs[(int) State.DealerDefeated].Value;
    public float killedAmbush => prefs[(int) State.AmbushDefeated].Value;

    public float customerGained => prefs[(int) State.CustomerStolen].Value;
    public float playerDeal => prefs[(int) State.PlayerDeal].Value;

    public float cartelGraffiti => prefs[(int) State.CartelGraffiti].Value;
    public float cartelDeal => prefs[(int) State.CartelDeal].Value;

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

      prefs = new MelonPreferences_Entry<float>[Enum.GetValues(typeof(State)).Length];

      prefs[(int) State.GraffitiCleaned] = playerActions.CreateEntry(
        identifier: "graffitiRemovalModifier",
        default_value: 1f,
        display_name: "Graffiti Removal",
        description: "Adjusts how much influence the cartel loses when a" +
          " player cleans cartel graffiti.  Min = 0x, Max = 100x",
        validator: validator
      );

      prefs[(int) State.GraffitiCreated] = playerActions.CreateEntry(
        identifier: "graffitiCreationModifier",
        default_value: 1f,
        display_name: "Graffiti Creation",
        description: "Adjusts how much influence the cartel loses when a" +
          " player creates new graffiti. Min = 0x, Max = 100x",
        validator: validator
      );

      prefs[(int) State.GraffitiInterrupted] = playerActions.CreateEntry(
        identifier: "graffitiInterruptModifier",
        default_value: 1f,
        display_name: "Graffiti Interrupt",
        description: "Adjusts how much influence the cartel loses when a" +
          " player interrupts a cartel member spray painting graffiti." +
          " Min = 0x, Max = 100x",
        validator: validator
      );

      prefs[(int) State.DealerDefeated] = playerActions.CreateEntry(
        identifier: "dealerKillModifier",
        default_value: 1f,
        display_name: "Dealer Defeated",
        description: "Adjusts how much influence the cartel loses when a" +
          " cartel dealer is killed or knocked out by a player.  Min = 0x," +
          " Max = 100x",
        validator: validator
      );

      prefs[(int) State.AmbushDefeated] = playerActions.CreateEntry(
        identifier: "ambushKillModifier",
        default_value: 1f,
        display_name: "Ambush Defeated",
        description: "Adjusts how much influence the cartel loses when a" +
          " cartel ambush is defeated by a player.  Min = 0x, Max = 100x",
        validator: validator
      );

      prefs[(int) State.CustomerStolen] = playerActions.CreateEntry(
        identifier: "customerGainedModifier",
        default_value: 1f,
        display_name: "New Customer",
        description: "Adjusts how much influence the cartel loses when a" +
          " player takes one of their customers.  Min = 0x, Max = 100x",
        validator: validator
      );

      prefs[(int) State.PlayerDeal] = playerActions.CreateEntry(
        identifier: "playerDealModifier",
        default_value: 0f,
        display_name: "Deal Completed",
        description: "Percentage of influence the cartel loses when a player" +
          " makes a deal.  Min = 0x, Max = 100x",
        validator: validator
      );

      prefs[(int) State.CartelGraffiti] = cartelActions.CreateEntry(
        identifier: "cartelGraffitiModifier",
        default_value: 0f,
        display_name: "Create Graffiti",
        description: "Percentage of influence the cartel gains when a cartel" +
          " dealer makes a deal.  Min = 0x, Max = 100x",
        validator: validator
      );

      prefs[(int) State.CartelDeal] = cartelActions.CreateEntry(
        identifier: "cartelDealModifier",
        default_value: 0f,
        display_name: "Deal Completed",
        description: "Percentage of influence the cartel loses when a player" +
          " makes a deal.  Min = 0x, Max = 100x",
        validator: validator
      );
    }
  }
}