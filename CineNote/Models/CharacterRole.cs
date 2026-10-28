namespace CineNote.Models
{
    public enum CharacterRole
    {
        Protagonist,
        Antagonist,
        Mentor,
        Sidekick,
        Supporting,
        Episodic
    }

    public static class CharacterRoleExtensions
    {
        public static string ToDisplayName(this CharacterRole role) => role switch
        {
            CharacterRole.Protagonist => "Протагонист",
            CharacterRole.Antagonist => "Антагонист",
            CharacterRole.Mentor => "Наставник",
            CharacterRole.Sidekick => "Соратник",
            CharacterRole.Supporting => "Второстепенный ключевой",
            CharacterRole.Episodic => "Эпизодический",
            _ => "Не указано"
        };
    }
}