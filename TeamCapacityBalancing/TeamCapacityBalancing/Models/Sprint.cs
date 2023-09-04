using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic;
using System;

namespace TeamCapacityBalancing.Models;

public partial class Sprint:ObservableObject
{
    public string Name { get; set; }
    public float NumberOfWeeks { get; set; }
    [ObservableProperty]
    public bool _isInShortTerm;           
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }

    public Sprint(string name, float numberOfWeeks, bool isInShortTerm)
    {
        Name = name;
        NumberOfWeeks = numberOfWeeks;
        IsInShortTerm = isInShortTerm;
    }

    public Sprint(string name, float numberOfWeeks, string? startDate, string? endDate)
    {
        Name = name;
        NumberOfWeeks = numberOfWeeks;
        StartDate = startDate;
        EndDate = endDate;
    }
}