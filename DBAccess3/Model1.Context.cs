﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DBAccess3
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DB_133455_bcxbteambldrEntities : DbContext
    {
        public DB_133455_bcxbteambldrEntities()
            : base("name=DB_133455_bcxbteambldrEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<MlbLeague> MlbLeagues { get; set; }
        public virtual DbSet<MlbPlayer> MlbPlayers { get; set; }
        public virtual DbSet<UserPlayer> UserPlayers { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserTeam> UserTeams { get; set; }
    }
}
