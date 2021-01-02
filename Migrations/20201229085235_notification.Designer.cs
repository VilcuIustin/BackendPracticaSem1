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
    [Migration("20201229085235_notification")]
    partial class notification
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Backend.Entities.Models.Comment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("CommentId")
                        .HasColumnType("bigint");

                    b.Property<long?>("ImageId")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("PostId")
                        .HasColumnType("bigint");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.HasIndex("ImageId");

                    b.HasIndex("PostId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("Backend.Entities.Models.ImgURL", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ImgUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("PostId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PostId");

                    b.ToTable("ImgURL");
                });

            modelBuilder.Entity("Backend.Entities.Models.Notification", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("idReceiver")
                        .HasColumnType("bigint");

                    b.Property<long>("idSender")
                        .HasColumnType("bigint");

                    b.Property<string>("message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("status")
                        .HasColumnType("bit");

                    b.HasKey("id");

                    b.HasIndex("UserId");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("Backend.Entities.Models.Post", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DTPost")
                        .HasColumnType("datetime2");

                    b.Property<int>("IdUser")
                        .HasColumnType("int");

                    b.Property<int>("NrLikes")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("Backend.Entities.Models.PostId", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("MyPost")
                        .HasColumnType("bigint");

                    b.Property<long?>("idPost")
                        .HasColumnType("bigint");

                    b.Property<long>("postId")
                        .HasColumnType("bigint");

                    b.Property<long>("userId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("MyPost");

                    b.HasIndex("idPost");

                    b.ToTable("PostId");
                });

            modelBuilder.Entity("Backend.Entities.Models.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.Property<long?>("ProfilePicId")
                        .HasColumnType("bigint");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProfilePicId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Backend.Entities.Models.UserId", b =>
                {
                    b.Property<long>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long?>("UserId1")
                        .HasColumnType("bigint");

                    b.Property<int>("dateFollowing")
                        .HasColumnType("int");

                    b.Property<long>("followedBy")
                        .HasColumnType("bigint");

                    b.Property<long>("following")
                        .HasColumnType("bigint");

                    b.Property<bool>("status")
                        .HasColumnType("bit");

                    b.HasKey("id");

                    b.HasIndex("UserId");

                    b.HasIndex("UserId1");

                    b.ToTable("UserId");
                });

            modelBuilder.Entity("Backend.Entities.Models.Comment", b =>
                {
                    b.HasOne("Backend.Entities.Models.Comment", null)
                        .WithMany("SubComment")
                        .HasForeignKey("CommentId");

                    b.HasOne("Backend.Entities.Models.ImgURL", "Image")
                        .WithMany()
                        .HasForeignKey("ImageId");

                    b.HasOne("Backend.Entities.Models.Post", null)
                        .WithMany("PostComment")
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("Backend.Entities.Models.ImgURL", b =>
                {
                    b.HasOne("Backend.Entities.Models.Post", null)
                        .WithMany("Images")
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("Backend.Entities.Models.Notification", b =>
                {
                    b.HasOne("Backend.Entities.Models.User", null)
                        .WithMany("notifications")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Backend.Entities.Models.PostId", b =>
                {
                    b.HasOne("Backend.Entities.Models.User", null)
                        .WithMany("MyPosts")
                        .HasForeignKey("MyPost");

                    b.HasOne("Backend.Entities.Models.User", null)
                        .WithMany("PostLiked")
                        .HasForeignKey("idPost");
                });

            modelBuilder.Entity("Backend.Entities.Models.User", b =>
                {
                    b.HasOne("Backend.Entities.Models.ImgURL", "ProfilePic")
                        .WithMany()
                        .HasForeignKey("ProfilePicId");
                });

            modelBuilder.Entity("Backend.Entities.Models.UserId", b =>
                {
                    b.HasOne("Backend.Entities.Models.User", null)
                        .WithMany("Followers")
                        .HasForeignKey("UserId");

                    b.HasOne("Backend.Entities.Models.User", null)
                        .WithMany("Following")
                        .HasForeignKey("UserId1");
                });
#pragma warning restore 612, 618
        }
    }
}
