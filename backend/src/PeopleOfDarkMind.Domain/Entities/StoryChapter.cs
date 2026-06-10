namespace PeopleOfDarkMind.Domain.Entities;

public class StoryChapter
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IntroText { get; set; } = string.Empty;
    public int RequiredProgress { get; set; } = 5;
}
