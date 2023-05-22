﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApiEventos;

#nullable disable

namespace WebApiEventos.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230521230207_OrganizerRelationship2")]
    partial class OrganizerRelationship2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0-preview.4.23259.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApiEventos.Entities.Assistants", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("EventId");

                    b.HasIndex("UserId");

                    b.ToTable("Asistants");
                });

            modelBuilder.Entity("WebApiEventos.Entities.Comments", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OrganizersId")
                        .HasColumnType("int");

                    b.Property<int>("OrgnaizerId")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganizersId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("WebApiEventos.Entities.Events", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("Capacidad")
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Descripcion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrganizersId")
                        .HasColumnType("int");

                    b.Property<string>("Ubicacion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UsersId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganizersId");

                    b.HasIndex("UsersId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("WebApiEventos.Entities.Organizers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Organizers");
                });

            modelBuilder.Entity("WebApiEventos.Entities.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WebApiEventos.Entities.Assistants", b =>
                {
                    b.HasOne("WebApiEventos.Entities.Events", "Event")
                        .WithMany("Assistants")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApiEventos.Entities.Users", "User")
                        .WithMany("Asistants")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebApiEventos.Entities.Comments", b =>
                {
                    b.HasOne("WebApiEventos.Entities.Organizers", "Organizers")
                        .WithMany()
                        .HasForeignKey("OrganizersId");

                    b.HasOne("WebApiEventos.Entities.Users", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Organizers");

                    b.Navigation("User");
                });

            modelBuilder.Entity("WebApiEventos.Entities.Events", b =>
                {
                    b.HasOne("WebApiEventos.Entities.Organizers", "Organizers")
                        .WithMany("Events")
                        .HasForeignKey("OrganizersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApiEventos.Entities.Users", null)
                        .WithMany("Favorites")
                        .HasForeignKey("UsersId");

                    b.Navigation("Organizers");
                });

            modelBuilder.Entity("WebApiEventos.Entities.Events", b =>
                {
                    b.Navigation("Assistants");
                });

            modelBuilder.Entity("WebApiEventos.Entities.Organizers", b =>
                {
                    b.Navigation("Events");
                });

            modelBuilder.Entity("WebApiEventos.Entities.Users", b =>
                {
                    b.Navigation("Asistants");

                    b.Navigation("Comments");

                    b.Navigation("Favorites");
                });
#pragma warning restore 612, 618
        }
    }
}