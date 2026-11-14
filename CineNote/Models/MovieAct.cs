namespace CineNote.Models
{
    public enum MovieAct
    {
        Act1, // Акт I: Экспозиция и завязка
        Act2, // Акт II: Развитие и мидпоинт
        Act3  // Акт III: Кульминация и финал
    }

    public static class MovieActExtensions
    {
        public static string ToDisplayName(this MovieAct act)
        {
            return act switch
            {
                MovieAct.Act1 => "Акт I: Экспозиция и завязка",
                MovieAct.Act2 => "Акт II: Развитие и мидпоинт",
                MovieAct.Act3 => "Акт III: Кульминация и финал",
                _ => act.ToString()
            };
        }

        public static string ToShortName(this MovieAct act)
        {
            return act switch
            {
                MovieAct.Act1 => "Акт I",
                MovieAct.Act2 => "Акт II",
                MovieAct.Act3 => "Акт III",
                _ => act.ToString()
            };
        }

        public static string ToDescription(this MovieAct act)
        {
            return act switch
            {
                MovieAct.Act1 => "Знакомство с героем, устройство мира и завязка основного конфликта",
                MovieAct.Act2 => "Преодоление препятствий, нарастание напряжения и центральный мидпоинт",
                MovieAct.Act3 => "Пик драматической борьбы, кульминация и финальная развязка",
                _ => string.Empty
            };
        }

        public static string ToIconClass(this MovieAct act)
        {
            return act switch
            {
                MovieAct.Act1 => "bi-play-circle-fill",
                MovieAct.Act2 => "bi-lightning-charge-fill",
                MovieAct.Act3 => "bi-fire",
                _ => "bi-film"
            };
        }

        public static string ToAccentColor(this MovieAct act)
        {
            return act switch
            {
                MovieAct.Act1 => "#8B5CF6", // Фиолетовый
                MovieAct.Act2 => "#F59E0B", // Янтарный
                MovieAct.Act3 => "#EF4444", // Красный / огонь
                _ => "#1F2937"
            };
        }
    }
}
