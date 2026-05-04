using System;
using System.Collections.Generic;
using System.Text;

namespace SaaS.Modules.HomePage.Entities
{
    public class HomePageSection
    {
        public Guid Id { get; set; }
        public string SectionKey { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
