namespace CineNote.Models
{
    public enum Genre
    {
        Drama,
        Comedy,
        Action,
        Thriller,
        SciFi,
        Horror,
        Romance,
        Fantasy,
        Mystery,
        Crime,
        Animation,
        Documentary,
        Other
    }

    public static class GenreExtensions
    {
        public static string ToDisplayName(this Genre genre) => genre switch
        {
            Genre.Drama => "Драма",
            Genre.Comedy => "Комедия",
            Genre.Action => "Боевик",
            Genre.Thriller => "Триллер",
            Genre.SciFi => "Фантастика",
            Genre.Horror => "Ужасы",
            Genre.Romance => "Мелодрама",
            Genre.Fantasy => "Фэнтези",
            Genre.Mystery => "Детектив",
            Genre.Crime => "Криминал",
            Genre.Animation => "Анимация",
            Genre.Documentary => "Документальный",
            Genre.Other => "Другое",
            _ => "Другое"
        };
    }
}
