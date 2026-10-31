namespace CineNote.Models
{
    public enum TimeOfDay
    {
        Day,
        Night,
        Morning,
        Evening
    }

    public static class TimeOfDayExtensions
    {
        public static string ToDisplayName(this TimeOfDay timeOfDay)
        {
            return timeOfDay switch
            {
                TimeOfDay.Day => "ДЕНЬ",
                TimeOfDay.Night => "НОЧЬ",
                TimeOfDay.Morning => "УТРО",
                TimeOfDay.Evening => "ВЕЧЕР",
                _ => timeOfDay.ToString()
            };
        }
    }
}