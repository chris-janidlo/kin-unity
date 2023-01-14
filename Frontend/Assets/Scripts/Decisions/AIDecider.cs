namespace Decisions
{
    public abstract class AIDecider : ADecider
    {
        public virtual bool Deciding { get; protected set; }

        public float Progress { get; protected set; }
    }
}
