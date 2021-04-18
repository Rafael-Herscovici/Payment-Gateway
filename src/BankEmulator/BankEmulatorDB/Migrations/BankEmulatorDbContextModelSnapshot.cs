﻿// <auto-generated />
using System;
using BankEmulatorDB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BankEmulatorDB.Migrations
{
    [DbContext(typeof(BankEmulatorDbContext))]
    partial class BankEmulatorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BankEmulatorDB.Entities.AccountEntity", b =>
                {
                    b.Property<int>("CardNumber")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("Balance")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(19,4)")
                        .HasDefaultValue(0m);

                    b.Property<DateTime>("CardExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("CardSecurityCode")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("CardNumber");

                    b.HasIndex("CardNumber", "CardExpiryDate", "CardSecurityCode");

                    b.ToTable("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
