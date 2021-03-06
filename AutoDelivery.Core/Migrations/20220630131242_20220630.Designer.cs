// <auto-generated />
using System;
using AutoDelivery.Core.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AutoDelivery.Core.Migrations
{
    [DbContext(typeof(AutoDeliveryContext))]
    [Migration("20220630131242_20220630")]
    partial class _20220630
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AutoDelivery.Domain.Mail.MailConfig", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("MAilPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MailAccount")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MailSTMPServer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MailServiceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MailConfigs");
                });

            modelBuilder.Entity("AutoDelivery.Domain.OrderDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTimeOffset?>("CreatedTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("CustomerMail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ItemQuantity")
                        .HasColumnType("int");

                    b.Property<string>("OrderDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("OrderId")
                        .HasColumnType("bigint");

                    b.Property<string>("OrderName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("UpdatedTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("UserAccountId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserAccountId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("AutoDelivery.Domain.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTimeOffset>("CreatedTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("EditTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool?>("HasActiveKey")
                        .HasColumnType("bit");

                    b.Property<bool?>("HasActiveLink")
                        .HasColumnType("bit");

                    b.Property<bool?>("HasSerialNum")
                        .HasColumnType("bit");

                    b.Property<bool?>("HasSubActiveKey")
                        .HasColumnType("bit");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("bit");

                    b.Property<string>("MainName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Maker")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProductCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("ProductCommonName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductEdition")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductSku")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductVersion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SubName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserAccountId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductCategoryId");

                    b.HasIndex("UserAccountId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("AutoDelivery.Domain.ProductCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ProductCategories");
                });

            modelBuilder.Entity("AutoDelivery.Domain.Serial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ActiveKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ActiveLink")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("CreatedTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("OrderDetailId")
                        .HasColumnType("int");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductSku")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.Property<string>("SerialNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("ShippedTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("SubActiveKey")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Used")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("OrderDetailId");

                    b.HasIndex("ProductId");

                    b.ToTable("Serials");
                });

            modelBuilder.Entity("AutoDelivery.Domain.User.OAuthState", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ShopifyShopDomain")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LoginStates");
                });

            modelBuilder.Entity("AutoDelivery.Domain.User.UserAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTimeOffset?>("BillingOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("MailconfigurationId")
                        .HasColumnType("int");

                    b.Property<string>("ShopifyAccessToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("ShopifyChargeId")
                        .HasColumnType("bigint");

                    b.Property<string>("ShopifyShopDomain")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ShopifyShopId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MailconfigurationId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AutoDelivery.Domain.OrderDetail", b =>
                {
                    b.HasOne("AutoDelivery.Domain.User.UserAccount", null)
                        .WithMany("OrderDetails")
                        .HasForeignKey("UserAccountId");
                });

            modelBuilder.Entity("AutoDelivery.Domain.Product", b =>
                {
                    b.HasOne("AutoDelivery.Domain.ProductCategory", "ProductCategory")
                        .WithMany()
                        .HasForeignKey("ProductCategoryId");

                    b.HasOne("AutoDelivery.Domain.User.UserAccount", null)
                        .WithMany("Products")
                        .HasForeignKey("UserAccountId");

                    b.Navigation("ProductCategory");
                });

            modelBuilder.Entity("AutoDelivery.Domain.Serial", b =>
                {
                    b.HasOne("AutoDelivery.Domain.OrderDetail", null)
                        .WithMany("RelatedSerials")
                        .HasForeignKey("OrderDetailId");

                    b.HasOne("AutoDelivery.Domain.Product", null)
                        .WithMany("SerialsInventory")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("AutoDelivery.Domain.User.UserAccount", b =>
                {
                    b.HasOne("AutoDelivery.Domain.Mail.MailConfig", "Mailconfiguration")
                        .WithMany()
                        .HasForeignKey("MailconfigurationId");

                    b.Navigation("Mailconfiguration");
                });

            modelBuilder.Entity("AutoDelivery.Domain.OrderDetail", b =>
                {
                    b.Navigation("RelatedSerials");
                });

            modelBuilder.Entity("AutoDelivery.Domain.Product", b =>
                {
                    b.Navigation("SerialsInventory");
                });

            modelBuilder.Entity("AutoDelivery.Domain.User.UserAccount", b =>
                {
                    b.Navigation("OrderDetails");

                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
