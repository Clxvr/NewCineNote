namespace CineNote.Models
{
    public enum LocationType
    {
        INT,
        EXT
    }

    public static class LocationTypeExtensions
    {
        public static string ToDisplayName(this LocationType locationType)
        {
            return locationType switch
            {
                LocationType.INT => "ИНТ.",
                LocationType.EXT => "НАТ.",
                _ => locationType.ToString()
            };
        }
    }
}