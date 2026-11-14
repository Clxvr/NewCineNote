namespace CineNote.Models
{
    public enum SceneStatus
    {
        Draft,       // Черновик
        InProgress,  // В работе
        Ready        // Готова
    }

    public static class SceneStatusExtensions
    {
        public static string ToDisplayName(this SceneStatus status)
        {
            return status switch
            {
                SceneStatus.Draft => "Черновик",
                SceneStatus.InProgress => "В работе",
                SceneStatus.Ready => "Готова",
                _ => status.ToString()
            };
        }

        public static string ToBadgeClass(this SceneStatus status)
        {
            return status switch
            {
                SceneStatus.Draft => "badge-status-draft",
                SceneStatus.InProgress => "badge-status-inprogress",
                SceneStatus.Ready => "badge-status-ready",
                _ => "bg-secondary"
            };
        }

        public static string ToIconClass(this SceneStatus status)
        {
            return status switch
            {
                SceneStatus.Draft => "bi-pencil-square",
                SceneStatus.InProgress => "bi-hourglass-split",
                SceneStatus.Ready => "bi-check-circle-fill",
                _ => "bi-circle"
            };
        }
    }
}
