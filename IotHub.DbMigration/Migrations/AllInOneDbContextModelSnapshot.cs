﻿// <auto-generated />
using System;
using IotHub.DbMigration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IotHub.DbMigration.Migrations
{
    [DbContext(typeof(AllInOneDbContext))]
    partial class AllInOneDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IotHub.Core.Cqrs.CqrsEngine.CommandEventStorage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("DataJson");

                    b.Property<string>("DataType");

                    b.Property<bool>("IsCommand");

                    b.HasKey("Id");

                    b.ToTable("CommandEventStorage");
                });

            modelBuilder.Entity("IotHub.Core.Cqrs.CqrsEngine.CommandEventStorageHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CommandEventId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Message");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.ToTable("CommandEventStorageHistory");
                });

            modelBuilder.Entity("IotHub.Core.Cqrs.EventSourcingRepository.EventSourcingDescription", b =>
                {
                    b.Property<Guid>("EsdId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AggregateId");

                    b.Property<string>("AggregateType");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("EventData");

                    b.Property<string>("EventType");

                    b.Property<long>("Version");

                    b.HasKey("EsdId");

                    b.ToTable("EventSourcingDescription");
                });

            modelBuilder.Entity("IotHub.SampleDomainWithEventSourcing.DataAccess.SampleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Published");

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.ToTable("SampleEntities");
                });

            modelBuilder.Entity("IotHub.UserDomain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Actived");

                    b.Property<string>("Password");

                    b.Property<string>("TokenSession");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("IotHub.UserDomain.UserProfile", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Fullname");

                    b.HasKey("UserId");

                    b.ToTable("UserProfiles");
                });
#pragma warning restore 612, 618
        }
    }
}
