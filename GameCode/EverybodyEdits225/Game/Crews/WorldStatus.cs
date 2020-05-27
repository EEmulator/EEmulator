namespace EverybodyEdits.Game.Crews
{
    /// <summary>
    ///     Status for world from crew.
    /// </summary>
    public enum WorldStatus
    {
        /// <summary>
        ///     World is not part of the crew.
        /// </summary>
        NonCrew = -1,

        /// <summary>
        ///     World is under construction. Only crew members are allowed to join.
        /// </summary>
        Wip = 0,

        /// <summary>
        ///     World is accessible by everyone and crew members with required rank can still save it.
        /// </summary>
        Open = 1,

        /// <summary>
        ///     World is accessible and only world hoster can save the world.
        /// </summary>
        Released = 2
    }
}