using CineNote.Data;
using CineNote.Models;
using Microsoft.EntityFrameworkCore;

namespace CineNote.Services
{
    public class BreakdownService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public BreakdownService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Scene>> GetBreakdownScenesAsync(int projectId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Scenes
                .AsNoTracking()
                .Include(s => s.SceneCharacters).ThenInclude(sc => sc.Character)
                .Include(s => s.SceneProps)
                .Where(s => s.MovieProjectId == projectId)
                .OrderBy(s => s.Act)
                .ThenBy(s => s.SceneNumber)
                .ToListAsync();
        }

        public BreakdownSummary BuildSummary(
            IEnumerable<Scene> allScenes,
            MovieAct? actFilter = null,
            TimeOfDay? timeFilter = null,
            LocationType? locationTypeFilter = null)
        {
            IEnumerable<Scene> query = allScenes;

            if (actFilter.HasValue)
                query = query.Where(s => s.Act == actFilter.Value);

            if (timeFilter.HasValue)
                query = query.Where(s => s.TimeOfDay == timeFilter.Value);

            if (locationTypeFilter.HasValue)
                query = query.Where(s => s.LocationType == locationTypeFilter.Value);

            var filtered = query.ToList();

            var locations = filtered
                .GroupBy(s => string.IsNullOrWhiteSpace(s.LocationName)
                    ? "ЛОКАЦИЯ НЕ УКАЗАНА"
                    : s.LocationName.Trim().ToUpperInvariant())
                .OrderBy(g => g.Key == "ЛОКАЦИЯ НЕ УКАЗАНА" ? 1 : 0)
                .ThenBy(g => g.Key)
                .Select(g => new LocationBreakdownItem
                {
                    LocationName = g.Key,
                    Scenes = g.OrderBy(s => s.SceneNumber).ToList(),
                    Characters = g.SelectMany(s => s.SceneCharacters)
                        .Where(sc => sc.Character != null)
                        .Select(sc => sc.Character!.Name)
                        .Distinct()
                        .OrderBy(n => n)
                        .ToList(),
                    Props = g.SelectMany(s => s.SceneProps)
                        .GroupBy(sp => new { Name = sp.Name.Trim(), sp.Category })
                        .Select(pg => new PropSummaryItem
                        {
                            Name = pg.Key.Name,
                            Category = pg.Key.Category,
                            MaxQuantity = pg.Max(x => x.Quantity)
                        })
                        .OrderBy(p => p.Category)
                        .ThenBy(p => p.Name)
                        .ToList()
                })
                .ToList();

            var props = filtered
                .SelectMany(s => s.SceneProps.Select(sp => new { Scene = s, Prop = sp }))
                .GroupBy(x => new { Name = x.Prop.Name.Trim(), x.Prop.Category })
                .Select(g => new PropSummaryItem
                {
                    Name = g.Key.Name,
                    Category = g.Key.Category,
                    MaxQuantity = g.Max(x => x.Prop.Quantity),
                    SceneNumbers = g.Select(x => x.Scene.SceneNumber).Distinct().OrderBy(n => n).ToList(),
                    Locations = g.Select(x => string.IsNullOrWhiteSpace(x.Scene.LocationName)
                            ? "Без локации"
                            : x.Scene.LocationName.Trim().ToUpperInvariant())
                        .Distinct()
                        .OrderBy(l => l)
                        .ToList()
                })
                .OrderBy(p => p.Category)
                .ThenBy(p => p.Name)
                .ToList();

            int uniqueChars = filtered
                .SelectMany(s => s.SceneCharacters)
                .Select(sc => sc.CharacterId)
                .Distinct()
                .Count();

            int uniqueProps = filtered
                .SelectMany(s => s.SceneProps)
                .Select(sp => $"{sp.Category}:{sp.Name.Trim().ToLowerInvariant()}")
                .Distinct()
                .Count();

            return new BreakdownSummary
            {
                Scenes = filtered,
                Locations = locations,
                Props = props,
                UniqueCharactersCount = uniqueChars,
                UniquePropsCount = uniqueProps
            };
        }
    }

    public class BreakdownSummary
    {
        public List<Scene> Scenes { get; set; } = new();
        public List<LocationBreakdownItem> Locations { get; set; } = new();
        public List<PropSummaryItem> Props { get; set; } = new();
        public int UniqueCharactersCount { get; set; }
        public int UniquePropsCount { get; set; }
    }

    public class LocationBreakdownItem
    {
        public string LocationName { get; set; } = string.Empty;
        public List<Scene> Scenes { get; set; } = new();
        public List<string> Characters { get; set; } = new();
        public List<PropSummaryItem> Props { get; set; } = new();
    }

    public class PropSummaryItem
    {
        public string Name { get; set; } = string.Empty;
        public PropCategory Category { get; set; }
        public int MaxQuantity { get; set; }
        public List<int> SceneNumbers { get; set; } = new();
        public List<string> Locations { get; set; } = new();
    }
}
