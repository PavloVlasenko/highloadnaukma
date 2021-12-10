using System;

namespace HighLoad.ApiModels
{
    public record BookCreateData
    {
        public DateTime Date { get; init; }
        public string Name { get; init; } = null!;
        public string Author { get; init; } = null!;
    }
}