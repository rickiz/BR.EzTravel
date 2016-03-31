﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BR.EzTravel.Web.Models;
using BR.EzTravel.Web.Helpers;
using System.IO;
using BR.EzTravel.Web.Properties;

namespace BR.EzTravel.Web.Areas.EN.Controllers
{
    public class BlogController : BaseEnController
    {
        public ActionResult Index(int categoryID = 0)
        {
            var viewModel = new BlogIndexViewModel();

            viewModel.Blogs =
                db.trnblogs
                    .Where(a => !a.CancelDT.HasValue && a.Language == language.ToString() &&
                    (a.CategoryID == categoryID || categoryID == 0))
                    .Select(a => new BlogDetails
                    {
                        ID = a.ID,
                        CategoryID = a.CategoryID,
                        Title = a.Title,
                        Body = a.Body,
                        LastEditedDate = a.UpdateDT.HasValue ? a.UpdateDT.Value : a.CreateDT,
                        CreatedBy = "Tester", // TODO: Link to tblMember
                        TotalComments = db.lnkblogcomments.Where(b => b.BlogID == a.ID && !b.CancelDT.HasValue).Count()
                    })
                    .Take(Settings.Default.MaxListPerPage)
                    .ToList();

            viewModel.PopularBlogs =
                db.trnblogs
                    .GroupJoin(db.tblmembers,a=>a.MemberID, b=>b.ID, (a,b)=> new { Blog = a, Member = b })
                    .GroupJoin(db.lnkblogcomments, a => a.Blog.ID, b => b.BlogID, (a, b) => new
                    {
                        Blog = a.Blog,
                        Member = a.Member.FirstOrDefault(),
                        BlogComments = b
                    })
                    .Where(a => !a.Blog.CancelDT.HasValue && a.Blog.Language == language.ToString())
                    .Select(a => new PopularBlog
                    {
                        ID = a.Blog.ID,
                        Author = a.Member == null? "EMMA STONE" : a.Member.PICName,
                        CreateDT = a.Blog.CreateDT,
                        NoOfComments = a.BlogComments.Count(),
                        Title = a.Blog.Title
                    })
                    .OrderByDescending(a => a.NoOfComments)
                    .Take(5)
                    .ToList();

            viewModel.LatestBlogComments =
                db.lnkblogcomments
                    .GroupJoin(db.tblmembers, a => a.MemberID, b => b.ID, (a, b) => new { BlogComment = a, Member = b })
                    .GroupJoin(db.trnblogs, a => a.BlogComment.BlogID, b => b.ID, (a, b) => new
                    {
                        BlogComment = a.BlogComment,
                        Member = a.Member.FirstOrDefault(),
                        Blog = b.FirstOrDefault()
                    })
                    .Where(a => !a.Blog.CancelDT.HasValue && a.Blog.Language == language.ToString())
                    .Select(a => new LatestBlogComment
                    {
                        ID = a.Blog == null ? 0: a.Blog.ID,
                        //Author = a.Member == null ? "EMMA STONE" : a.Member.PICName,
                        CreateDT = a.BlogComment.CreateDT,
                        Comment = a.BlogComment.Comments,
                        Title = a.Blog.Title
                    })
                    .OrderByDescending(a => a.CreateDT)
                    .Take(5)
                    .ToList();

            viewModel.Categories = db.refcategories
                                        .GroupJoin(db.trnblogs, a => a.ID, b => b.CategoryID, (a, b) => new { Category = a, Blogs = b })
                                        .Where(a => a.Category.Active)
                                        .OrderBy(a => a.Category.Name)
                                        .Select(a => new BlogCategory()
                                        {
                                            ID = a.Category.ID,
                                            Name = a.Category.Name,
                                            Count = a.Blogs.Count()
                                        }).ToList();

            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            var viewModel =
                (from a in db.trnblogs
                 join b in db.tblmembers on a.MemberID equals b.ID into tempB
                 from bb in tempB.DefaultIfEmpty()
                 where a.ID == id
                 select new BlogDetailsViewModel
                 {
                     ID = a.ID,
                     CategoryID = a.CategoryID,
                     Title = a.Title,
                     Body = a.Body,
                     LastEditedDate = a.UpdateDT.HasValue ? a.UpdateDT.Value : a.CreateDT,
                     CreatedBy = bb == null ? "EMMA STONE" : bb.PICName,
                     TotalComments = db.lnkblogcomments.Where(b => b.BlogID == a.ID && !b.CancelDT.HasValue).Count(),
                     ThumbnailImagePath = a.ThumbnailImagePath
                 }).Single();

            var comments =
                (from a in db.lnkblogcomments
                 join b in db.tblmembers on a.MemberID equals b.ID into tempb
                 from bb in tempb.DefaultIfEmpty()
                 where a.BlogID == id && !a.CancelDT.HasValue
                 select new LatestBlogComment
                 {
                     Author = bb == null ? "" : bb.PICName,
                     Comment = a.Comments,
                     CreateDT = a.CreateDT,
                     ID = a.ID
                 }).ToList();

            var lang = language.ToString();
            viewModel.Categories = db.refcategories
                                        .GroupJoin(db.trnblogs, a => a.ID, b => b.CategoryID, (a, b) => new { Category = a, Blogs = b })
                                        .Where(a => a.Category.Active)
                                        .OrderBy(a => a.Category.Name)
                                        .Select(a => new BlogCategory()
                                        {
                                            ID = a.Category.ID,
                                            Name = a.Category.Name,
                                            Count = a.Blogs.Count()
                                        }).ToList();
            
            viewModel.Comments = comments;
            //viewModel.Categories = categories;

            viewModel.PopularBlogs =
                db.trnblogs
                    .GroupJoin(db.tblmembers, a => a.MemberID, b => b.ID, (a, b) => new { Blog = a, Member = b })
                    .GroupJoin(db.lnkblogcomments, a => a.Blog.ID, b => b.BlogID, (a, b) => new
                    {
                        Blog = a.Blog,
                        Member = a.Member.FirstOrDefault(),
                        BlogComments = b
                    })
                    .Where(a => !a.Blog.CancelDT.HasValue && a.Blog.Language == language.ToString())
                    .Select(a => new PopularBlog
                    {
                        ID = a.Blog.ID,
                        Author = a.Member == null ? "EMMA STONE" : a.Member.PICName,
                        CreateDT = a.Blog.CreateDT,
                        NoOfComments = a.BlogComments.Count(),
                        Title = a.Blog.Title
                    })
                    .OrderByDescending(a => a.NoOfComments)
                    .Take(5)
                    .ToList();

            viewModel.LatestBlogComments =
                db.lnkblogcomments
                    .GroupJoin(db.tblmembers, a => a.MemberID, b => b.ID, (a, b) => new { BlogComment = a, Member = b })
                    .GroupJoin(db.trnblogs, a => a.BlogComment.BlogID, b => b.ID, (a, b) => new
                    {
                        BlogComment = a.BlogComment,
                        Member = a.Member.FirstOrDefault(),
                        Blog = b.FirstOrDefault()
                    })
                    .Where(a => !a.Blog.CancelDT.HasValue && a.Blog.Language == language.ToString())
                    .Select(a => new LatestBlogComment
                    {
                        ID = a.Blog == null ? 0 : a.Blog.ID,
                        //Author = a.Member == null ? "EMMA STONE" : a.Member.PICName,
                        CreateDT = a.BlogComment.CreateDT,
                        Comment = a.BlogComment.Comments,
                        Title = a.Blog.Title
                    })
                    .OrderByDescending(a => a.CreateDT)
                    .Take(5)
                    .ToList();

            viewModel.RelatedBlogs =
                db.trnblogs
                    .Where(a => !a.CancelDT.HasValue && a.Language == language.ToString() &&
                    (a.CategoryID == viewModel.CategoryID))
                    .GroupJoin(db.tblmembers, a => a.MemberID, b => b.ID, (a, b) => new { Blog = a, Member = b.FirstOrDefault() })
                    .Select(a => new BlogDetails
                    {
                        ID = a.Blog.ID,
                        CategoryID = a.Blog.CategoryID,
                        Title = a.Blog.Title,
                        Body = a.Blog.Body,
                        LastEditedDate = a.Blog.UpdateDT.HasValue ? a.Blog.UpdateDT.Value : a.Blog.CreateDT,
                        CreatedBy = a.Member == null ? "EMMA STONE" : a.Member.PICName,
                        TotalComments = db.lnkblogcomments.Where(b => b.BlogID == a.Blog.ID && !b.CancelDT.HasValue).Count()
                    })
                    .OrderByDescending(a => a.TotalComments)
                    .Take(5)
                    .ToList();

            return View(viewModel);
        }
    }
}