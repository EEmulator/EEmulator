namespace EverybodyEdits.Game.Crews
{
    /// <summary>
    ///     Powers available to crew members.
    /// </summary>
    public enum CrewPower
    {
        /// <summary>
        ///     Automatic edit rights in unreleased crew worlds.
        /// </summary>
        AutoEdit = 0,

        /// <summary>
        ///     Editing settings of crew worlds.
        /// </summary>
        WorldSettingsAccess = 1,

        /// <summary>
        ///     Joining and editig logo world.
        /// </summary>
        LogoWorldAccess = 2,

        /// <summary>
        ///     Buying stuff in crew shop.
        /// </summary>
        ShopAccess = 3,

        /// <summary>
        ///     Management of crew worlds.
        /// </summary>
        WorldsManagement = 4,

        /// <summary>
        ///     Management of crew members.
        /// </summary>
        MembersManagement = 5,

        /// <summary>
        ///     Management of crew ranks.
        /// </summary>
        RanksManagement = 6,

        /// <summary>
        ///     Customization of crew profile.
        /// </summary>
        ProfileCustomization = 7,

        /// <summary>
        ///     Sending of crew alerts.
        /// </summary>
        AlertSending = 8,

        Count = 9
    }
}