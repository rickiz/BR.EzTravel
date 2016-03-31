using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BR.EzTravel.Web.Models
{
    #region Index

    public class BlogIndexViewModel
    {
        public List<BlogDetails> Blogs { get; set; }
        public List<refcategory> Categories { get; set; }
        public List<PopularBlog> PopularBlogs { get; set; }
        public List<LatestBlogComment> LatestBlogComments { get; set; }
    }

    public class BlogDetails
    {
        public int ID { get; set; }
        public int CategoryID { get; set; }

        public string Title { get; set; }

        public string Body { get; set; }

        public DateTime LastEditedDate { get; set; }

        public string CreatedBy { get; set; }

        public int TotalComments { get; set; }

        public string ThumbnailImagePath { get; set; }
    }

    public class PopularBlog
    {
        public int ID { get; set; }
        public DateTime CreateDT { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public int NoOfComments { get; set; }
    }

    public class LatestBlogComment
    {
        public int ID { get; set; }
        public DateTime CreateDT { get; set; }
        public string Author { get; set; }
        public string Comment { get; set; }
        public string Title { get; set; }
    }

    #endregion

    #region Details

    public class BlogDetailsViewModel : BlogDetails
    {
        public List<BlogCategory> Categories { get; set; }

        public List<LatestBlogComment> Comments { get; set; }
    }

    public class BlogCategory
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public int Count { get; set; }
    }

    #endregion
}
