using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsApp.Models
{
    public class Category : ICloneable
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int? ParentId { get; set; }

        public Category? Parent { get; set; }

        public int? StartAge { get; set; }

        public int? ElementsInCategory { get; set; }

        public required bool IsBlocked { get; set; } = false;

        public List<Category> ChildCategories { get; set; } = new List<Category>();

        public List<Event> Events { get; set; } = new();

        public object Clone ()
        {
            return new Category()
            {
                Id = Id,
                Name = Name,
                ParentId = ParentId,
                Parent = Parent?.Clone() as Category,
                ElementsInCategory = ElementsInCategory,
                IsBlocked = IsBlocked,
                ChildCategories = new (ChildCategories)
            };
        }

        public override bool Equals ( object? obj )
        {
            return obj is Category category && Name == category.Name;
        }

        public override int GetHashCode ()
        {
            return Name?.GetHashCode() ?? 0;
        }
    }
}
