﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BR.EzTravel.Web.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ExHolidayEntities : DbContext
    {
        public ExHolidayEntities()
            : base("name=ExHolidayEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<lnkmemberpackagetype> lnkmemberpackagetypes { get; set; }
        public virtual DbSet<lnkmemberpost> lnkmemberposts { get; set; }
        public virtual DbSet<lnkmemberpostcity> lnkmemberpostcities { get; set; }
        public virtual DbSet<lnkmemberpostcountry> lnkmemberpostcountries { get; set; }
        public virtual DbSet<lnkmemberpostimage> lnkmemberpostimages { get; set; }
        public virtual DbSet<lnkmemberpostpackageactivity> lnkmemberpostpackageactivities { get; set; }
        public virtual DbSet<lnkmemberrenewal> lnkmemberrenewals { get; set; }
        public virtual DbSet<lnkmemberreward> lnkmemberrewards { get; set; }
        public virtual DbSet<lnkrating> lnkratings { get; set; }
        public virtual DbSet<lnkrewardclaim> lnkrewardclaims { get; set; }
        public virtual DbSet<refcategory> refcategories { get; set; }
        public virtual DbSet<refcity> refcities { get; set; }
        public virtual DbSet<refcountry> refcountries { get; set; }
        public virtual DbSet<refpackage> refpackages { get; set; }
        public virtual DbSet<refpackageactivity> refpackageactivities { get; set; }
        public virtual DbSet<refpackagetype> refpackagetypes { get; set; }
        public virtual DbSet<refrewardscheme> refrewardschemes { get; set; }
        public virtual DbSet<tblmember> tblmembers { get; set; }
        public virtual DbSet<trnmemberrenewal> trnmemberrenewals { get; set; }
        public virtual DbSet<trnroi> trnrois { get; set; }
        public virtual DbSet<trnblog> trnblogs { get; set; }
        public virtual DbSet<lnkmemberpostcomment> lnkmemberpostcomments { get; set; }
        public virtual DbSet<lnkblogcomment> lnkblogcomments { get; set; }
    }
}
