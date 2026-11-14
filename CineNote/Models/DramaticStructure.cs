namespace CineNote.Models
{
    public enum DramaticStructure
    {
        None,        // Обычная сцена
        Exposition,  // Завязка
        Midpoint,    // Мидпоинт
        Climax,      // Кульминация
        Resolution   // Развязка
    }

    public static class DramaticStructureExtensions
    {
        public static string ToDisplayName(this DramaticStructure marker)
        {
            return marker switch
            {
                DramaticStructure.None => "Обычная сцена",
                DramaticStructure.Exposition => "Завязка",
                DramaticStructure.Midpoint => "Мидпоинт",
                DramaticStructure.Climax => "Кульминация",
                DramaticStructure.Resolution => "Развязка",
                _ => marker.ToString()
            };
        }

        public static string ToBadgeClass(this DramaticStructure marker)
        {
            return marker switch
            {
                DramaticStructure.Exposition => "badge-dramatic-exposition",
                DramaticStructure.Midpoint => "badge-dramatic-midpoint",
                DramaticStructure.Climax => "badge-dramatic-climax",
                DramaticStructure.Resolution => "badge-dramatic-resolution",
                _ => ""
            };
        }

        public static string ToIconClass(this DramaticStructure marker)
        {
            return marker switch
            {
                DramaticStructure.Exposition => "bi-flag-fill",
                DramaticStructure.Midpoint => "bi-lightning-charge-fill",
                DramaticStructure.Climax => "bi-fire",
                DramaticStructure.Resolution => "bi-trophy-fill",
                _ => ""
            };
        }
    }
}
