﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECoupoun.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ECoupounEntities : DbContext
    {
        public ECoupounEntities()
            : base("name=ECoupounEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<APIDetail> APIDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<ProductMaster> ProductMasters { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderPriority> ProviderPriorities { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductHistory> ProductHistories { get; set; }
        public DbSet<ProductLinksHistory> ProductLinksHistories { get; set; }
        public DbSet<ProductLink> ProductLinks { get; set; }
        public DbSet<ProductPricing> ProductPricings { get; set; }
        public DbSet<ProductPricingHistory> ProductPricingHistories { get; set; }
        public DbSet<ProductViewDetail> ProductViewDetails { get; set; }
    }
}