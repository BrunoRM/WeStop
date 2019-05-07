﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeStop.Domain;

namespace WeStop.Infra.Database.SqlServer.Maps
{
    public class PlayerMap : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.ToTable("player");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(35)
                .IsRequired();

            builder.Property(x => x.UserName)
                .HasColumnName("username")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Password)
                .HasColumnName("password")
                .HasColumnType("varchar(255)")
                .IsRequired();
        }
    }
}
