namespace CartelInfluenceTweaks.state {

  internal readonly struct InfluenceAction {

    /// <summary>
    /// Player cleans cartel graffiti.
    /// </summary>
    public static readonly InfluenceAction GraffitiCleaned = new InfluenceAction(-1f, ActionType.GraffitiCleaned);

    /// <summary>
    /// Player creates graffiti.
    /// </summary>
    public static readonly InfluenceAction GraffitiCreated = new InfluenceAction(-1f, ActionType.GraffitiCreated);

    /// <summary>
    /// Player interrupts cartel graffiti.
    /// </summary>
    public static readonly InfluenceAction GraffitiInterrupted = new InfluenceAction(-1f, ActionType.GraffitiInterrupted);

    /// <summary>
    /// Cartel dealer defeated.
    /// </summary>
    public static readonly InfluenceAction DealerDefeated = new InfluenceAction(-1f, ActionType.DealerDefeated);

    /// <summary>
    /// Cartel ambush defeated.
    /// </summary>
    public static readonly InfluenceAction AmbushDefeated = new InfluenceAction(-1f, ActionType.AmbushDefeated);

    /// <summary>
    /// Customer gained.
    /// </summary>
    public static readonly InfluenceAction CustomerStolen = new InfluenceAction(-1f, ActionType.CustomerStolen);

    /// <summary>
    /// Player completes deal.
    /// </summary>
    public static readonly InfluenceAction PlayerDeal = new InfluenceAction(-1f, ActionType.PlayerDeal);

    /// <summary>
    /// Player hired dealer completes deal.
    /// </summary>
    public static readonly InfluenceAction PlayerProxyDeal = new InfluenceAction(-1f, ActionType.PlayerProxyDeal);

    /// <summary>
    /// Cartel completes graffiti.
    /// </summary>
    public static readonly InfluenceAction CartelGraffiti = new InfluenceAction(1f, ActionType.CartelGraffiti);

    /// <summary>
    /// Cartel dealer completes deal.
    /// </summary>
    public static readonly InfluenceAction CartelDeal = new InfluenceAction(1f, ActionType.CartelDeal);

    /// <summary>
    /// Player defeated by cartel thug.
    /// </summary>
    public static readonly InfluenceAction PlayerDefeated = new InfluenceAction(1f, ActionType.PlayerDefeated);

    public readonly float sign;
    public readonly ActionType type;

    internal InfluenceAction(float f, ActionType type) {
      this.sign = f;
      this.type = type;
    }

    public override string ToString() => type.ToString();
  }
}