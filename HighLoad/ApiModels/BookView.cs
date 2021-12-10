using System;

namespace HighLoad.ApiModels
{
    public record BookView
    {
        public string Id { get; init; } = null!;
        public DateTime Date { get; init; }
        public string Name { get; init; } = null!;
        public string Author { get; init; } = null!;
    }
}