using System;
using System.ComponentModel.DataAnnotations;

namespace HighLoad.Entities
{
    public record Book
    {
        [Key]
        public Guid Id { get; init; } = Guid.NewGuid();
        public DateTime Date { get; init; }
        public string Name { get; init; } = null!;
        public string Author { get; init; } = null!;
    }
}