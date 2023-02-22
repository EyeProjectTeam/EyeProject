﻿// <auto-generated />
using System;
using EyeProtect.Fetch.Service.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Volo.Abp.EntityFrameworkCore;

#nullable disable

namespace EyeProtect.Service.Migrations
{
    [DbContext(typeof(EyeProtectDbContext))]
    [Migration("20230222061231_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("_Abp_DatabaseProvider", EfCoreDatabaseProvider.MySql)
                .HasAnnotation("ProductVersion", "6.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("EyeProtect.Domain.Members.Member", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Account")
                        .HasMaxLength(12)
                        .HasColumnType("varchar(12)");

                    b.Property<int?>("AccountType")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("CreationTime");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("DeletionTime");

                    b.Property<string>("Email")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false)
                        .HasColumnName("IsDeleted");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("LastModificationTime");

                    b.Property<string>("Name")
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Password")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Phone")
                        .HasMaxLength(11)
                        .HasColumnType("varchar(11)");

                    b.HasKey("Id");

                    b.ToTable("Members");
                });

            modelBuilder.Entity("EyeProtect.Members.OperationRecord", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Account")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("CreationTime");

                    b.Property<DateTime?>("DeletionTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("DeletionTime");

                    b.Property<string>("Ip")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint(1)")
                        .HasDefaultValue(false)
                        .HasColumnName("IsDeleted");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("LastModificationTime");

                    b.Property<long>("MemberId")
                        .HasColumnType("bigint");

                    b.Property<string>("MemberName")
                        .HasColumnType("longtext");

                    b.Property<int>("OperrationType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("MemberId");

                    b.ToTable("OperationRecords");
                });

            modelBuilder.Entity("EyeProtect.Members.OperationRecord", b =>
                {
                    b.HasOne("EyeProtect.Domain.Members.Member", null)
                        .WithMany("OperationRecords")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EyeProtect.Domain.Members.Member", b =>
                {
                    b.Navigation("OperationRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
