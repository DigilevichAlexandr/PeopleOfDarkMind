using System.Text.Json;
using PeopleOfDarkMind.Domain.Entities;

namespace PeopleOfDarkMind.Application.Services;

public static class StatEffects
{
    public static void Apply(CharacterStats stats, string effectsJson)
    {
        if (string.IsNullOrWhiteSpace(effectsJson) || effectsJson == "{}") return;

        try
        {
            var effects = JsonSerializer.Deserialize<Dictionary<string, int>>(effectsJson);
            if (effects is null) return;

            foreach (var (key, value) in effects)
            {
                switch (key.ToLowerInvariant())
                {
                    case "money": stats.Money = Clamp(stats.Money + value, 0, 999999); break;
                    case "mood": stats.Mood = Clamp(stats.Mood + value); break;
                    case "energy": stats.Energy = Clamp(stats.Energy + value); break;
                    case "health": stats.Health = Clamp(stats.Health + value); break;
                    case "reputation": stats.Reputation = Clamp(stats.Reputation + value); break;
                    case "stress": stats.Stress = Clamp(stats.Stress + value); break;
                    case "awareness": stats.Awareness = Clamp(stats.Awareness + value); break;
                    case "sensitivity": stats.Sensitivity = Clamp(stats.Sensitivity + value); break;
                    case "intuition": stats.Intuition = Clamp(stats.Intuition + value); break;
                    case "underworldbond": stats.UnderworldBond = Clamp(stats.UnderworldBond + value); break;
                    case "influenceresistance": stats.InfluenceResistance = Clamp(stats.InfluenceResistance + value); break;
                    case "willpower": stats.Willpower = Clamp(stats.Willpower + value); break;
                }
            }
        }
        catch
        {
            // ignore malformed json
        }
    }

    private static int Clamp(int value, int min = 0, int max = 100) =>
        Math.Max(min, Math.Min(max, value));
}
