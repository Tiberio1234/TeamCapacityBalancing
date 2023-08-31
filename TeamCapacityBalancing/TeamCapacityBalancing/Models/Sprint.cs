namespace TeamCapacityBalancing.Models;

public class Sprint
{
    public string Name { get; set; }
    public float NumberOfWeeks { get; set; }
    public bool IsInShortTerm { get; set; }
    public Sprint(string name, float numberOfWeeks, bool isInShortTerm)
    {
        Name = name;
        NumberOfWeeks = numberOfWeeks;
        IsInShortTerm = isInShortTerm;
    }
}
