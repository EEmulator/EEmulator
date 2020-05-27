using System;

namespace EverybodyEdits.Game
{
    public class Effect : IEquatable<Effect>
    {
        public bool CanExpire;
        public readonly int Duration;
        public readonly EffectId Id;
        private DateTime activated;

        public Effect(int id, int duration = 0)
        {
            this.Id = (EffectId) id;
            this.Duration = duration;
            this.CanExpire = duration > 0;
        }

        public double TimeLeft
        {
            get { return (this.activated.AddSeconds(this.Duration) - DateTime.Now).TotalSeconds; }
        }

        public bool Expired
        {
            get { return this.CanExpire && this.TimeLeft <= 0; }
        }

        public bool Equals(Effect other)
        {
            return this.Id == other.Id;
        }

        public void Activate()
        {
            this.activated = DateTime.Now;
        }
    }
}