﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntityModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EDModelContainer : DbContext
    {
        public EDModelContainer()
            : base("name=EDModelContainer")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Phrase> Phrases { get; set; }
        public virtual DbSet<MeasureUnit> MeasureUnits { get; set; }
        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<AdditionalText> AdditionalTexts { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Messenger> Messengers { get; set; }
        public virtual DbSet<UserMessenger> UserMessengers { get; set; }
        public virtual DbSet<UserAchievement> UserAchievements { get; set; }
        public virtual DbSet<Achievement> Achievements { get; set; }
    }
}
