﻿// <auto-generated />
using System;
using Backend.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Backend.Migrations
{
    [DbContext(typeof(BackendContext))]
    [Migration("20201126193416_test")]
    partial class test
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("Backend.Entities.Models.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<byte[]>("Photo")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Backend.Entities.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("IdUser")
                        .HasColumnType("int");

                    b.Property<int?>("ImgId")
                        .HasColumnType("int");

                    b.Property<int>("NrLikes")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ImgId");

                    b.HasIndex("UserId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Backend.Entities.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Backend.Entities.Models.Post", b =>
                {
                    b.HasOne("Backend.Entities.Models.Image", "Img")
                        .WithMany()
                        .HasForeignKey("ImgId");

                    b.HasOne("Backend.Entities.Models.User", null)
                        .WithMany("MyPosts")
                        .HasForeignKey("UserId");

                    b.Navigation("Img");
                });

            modelBuilder.Entity("Backend.Entities.Models.User", b =>
                {
                    b.HasOne("Backend.Entities.Models.User", null)
                        .WithMany("Fallow")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Backend.Entities.Models.User", b =>
                {
                    b.Navigation("Fallow");

                    b.Navigation("MyPosts");
                });
#pragma warning restore 612, 618
        }
    }
}
