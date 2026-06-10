namespace RunicVoice.Abilities;

public interface IRunicVoiceAbility
{
    string Id { get; }
    string DisplayName { get; }
    bool CanCast(Player player, out string reason);
    void Cast(Player player);
}
