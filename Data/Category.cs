using System.Collections.Generic;

namespace MvcCore.Data
{
    public partial class Category
    {
        public Category()
        {
            Blog = new HashSet<Blog>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Blog> Blog { get; set; }
    }
}
