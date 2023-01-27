﻿// <auto-generated />
using System;
using HardwareShop.Dal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HardwareShop.WebApi.Migrations
{
    [DbContext(typeof(MainDatabaseContext))]
    partial class MainDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("HardwareShop.Dal.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("HashedPassword")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.AccountShop", b =>
                {
                    b.Property<int>("AccountId")
                        .HasColumnType("integer");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.HasKey("AccountId");

                    b.HasIndex("ShopId");

                    b.ToTable("AccountShops");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Customer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.CustomerDebt", b =>
                {
                    b.Property<int>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<double>("AmountOfDebt")
                        .HasColumnType("double precision");

                    b.HasKey("CustomerId");

                    b.ToTable("CustomerDebts");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.CustomerDebtHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("AmountOfChange")
                        .HasColumnType("double precision");

                    b.Property<int>("CustomerDebtId")
                        .HasColumnType("integer");

                    b.Property<string>("Reason")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CustomerDebtId");

                    b.ToTable("CustomerDebtHistories");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Invoice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<double>("ChangeOfDebt")
                        .HasColumnType("double precision");

                    b.Property<double>("CurrentDebt")
                        .HasColumnType("double precision");

                    b.Property<int>("CustomerId")
                        .HasColumnType("integer");

                    b.Property<double>("Deposit")
                        .HasColumnType("double precision");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("ShopId");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.InvoiceDetail", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("InvoiceId")
                        .HasColumnType("integer");

                    b.Property<double>("OriginalPrice")
                        .HasColumnType("double precision");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<double>("Profit")
                        .HasColumnType("double precision");

                    b.Property<double>("Quantity")
                        .HasColumnType("double precision");

                    b.Property<double>("TotalCost")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("InvoiceId");

                    b.HasIndex("ProductId");

                    b.ToTable("InvoiceDetails");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<double?>("Mass")
                        .HasColumnType("double precision");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double?>("PercentForCustomer")
                        .HasColumnType("double precision");

                    b.Property<double?>("PercentForFamiliarCustomer")
                        .HasColumnType("double precision");

                    b.Property<double>("PriceForCustomer")
                        .HasColumnType("double precision");

                    b.Property<double?>("PriceForFamiliarCustomer")
                        .HasColumnType("double precision");

                    b.Property<double?>("PricePerMass")
                        .HasColumnType("double precision");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.Property<int>("UnitId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.HasIndex("UnitId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Shop", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Shops");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.ShopAsset", b =>
                {
                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.Property<byte[]>("Logo")
                        .HasColumnType("bytea");

                    b.HasKey("ShopId");

                    b.ToTable("ShopAssets");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UnitCategoryId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UnitCategoryId");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.UnitCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("UnitCategories");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Warehouse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ShopId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ShopId");

                    b.ToTable("Warehouses");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.AccountShop", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.Account", "Account")
                        .WithOne("ShopAccount")
                        .HasForeignKey("HardwareShop.Dal.Models.AccountShop", "AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HardwareShop.Dal.Models.Shop", "Shop")
                        .WithMany("ShopAccounts")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Customer", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.Shop", "Shop")
                        .WithMany()
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.CustomerDebt", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.Customer", "Customer")
                        .WithOne("Debt")
                        .HasForeignKey("HardwareShop.Dal.Models.CustomerDebt", "CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.CustomerDebtHistory", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.CustomerDebt", "CustomerDebt")
                        .WithMany("Histories")
                        .HasForeignKey("CustomerDebtId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CustomerDebt");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Invoice", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.Customer", "Customer")
                        .WithMany("Invoices")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HardwareShop.Dal.Models.Shop", "Shop")
                        .WithMany("Invoices")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.InvoiceDetail", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.Invoice", "Invoice")
                        .WithMany("Details")
                        .HasForeignKey("InvoiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HardwareShop.Dal.Models.Product", "Product")
                        .WithMany("InvoiceDetails")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Invoice");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Product", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.Shop", "Shop")
                        .WithMany("Products")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HardwareShop.Dal.Models.Unit", "Unit")
                        .WithMany("Products")
                        .HasForeignKey("UnitId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shop");

                    b.Navigation("Unit");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.ProductCategory", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.Shop", "Shop")
                        .WithMany("ProductCategories")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.ShopAsset", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.Shop", "Shop")
                        .WithOne("ShopAsset")
                        .HasForeignKey("HardwareShop.Dal.Models.ShopAsset", "ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Unit", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.UnitCategory", "UnitCategory")
                        .WithMany("Units")
                        .HasForeignKey("UnitCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UnitCategory");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Warehouse", b =>
                {
                    b.HasOne("HardwareShop.Dal.Models.Shop", "Shop")
                        .WithMany("Warehouses")
                        .HasForeignKey("ShopId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Shop");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Account", b =>
                {
                    b.Navigation("ShopAccount");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Customer", b =>
                {
                    b.Navigation("Debt");

                    b.Navigation("Invoices");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.CustomerDebt", b =>
                {
                    b.Navigation("Histories");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Invoice", b =>
                {
                    b.Navigation("Details");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Product", b =>
                {
                    b.Navigation("InvoiceDetails");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Shop", b =>
                {
                    b.Navigation("Invoices");

                    b.Navigation("ProductCategories");

                    b.Navigation("Products");

                    b.Navigation("ShopAccounts");

                    b.Navigation("ShopAsset");

                    b.Navigation("Warehouses");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.Unit", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("HardwareShop.Dal.Models.UnitCategory", b =>
                {
                    b.Navigation("Units");
                });
#pragma warning restore 612, 618
        }
    }
}
