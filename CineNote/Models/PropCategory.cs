namespace CineNote.Models
{
    public enum PropCategory
    {
        Props,
        Transport,
        Wardrobe,
        Equipment,
        SpecialEffects
    }

    public static class PropCategoryExtensions
    {
        public static string ToDisplayName(this PropCategory category) => category switch
        {
            PropCategory.Props => "Реквизит",
            PropCategory.Transport => "Транспорт",
            PropCategory.Wardrobe => "Костюм и грим",
            PropCategory.Equipment => "Техника и свет",
            PropCategory.SpecialEffects => "Спецэффекты",
            _ => "Прочее"
        };

        public static string ToIconClass(this PropCategory category) => category switch
        {
            PropCategory.Props => "bi-box-seam",
            PropCategory.Transport => "bi-car-front-fill",
            PropCategory.Wardrobe => "bi-person-bounding-box",
            PropCategory.Equipment => "bi-camera-reels",
            PropCategory.SpecialEffects => "bi-stars",
            _ => "bi-tag"
        };

        public static string ToBadgeClass(this PropCategory category) => category switch
        {
            PropCategory.Props => "bg-warning-subtle text-dark border border-warning-subtle",
            PropCategory.Transport => "bg-primary-subtle text-primary border border-primary-subtle",
            PropCategory.Wardrobe => "bg-info-subtle text-dark border border-info-subtle",
            PropCategory.Equipment => "bg-secondary-subtle text-dark border border-secondary-subtle",
            PropCategory.SpecialEffects => "bg-danger-subtle text-danger border border-danger-subtle",
            _ => "bg-light text-dark border"
        };
    }
}
