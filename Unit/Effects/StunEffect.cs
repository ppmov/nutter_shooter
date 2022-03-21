public class StunEffect : Effect
{
    protected override void Include()
    {
        target.Stun(duration);
    }
}
