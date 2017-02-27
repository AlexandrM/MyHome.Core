using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using MyHome.Shared;

namespace MyHome.Core.Migrations
{
    [DbContext(typeof(DB))]
    [Migration("20170222193606_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("MyHome.Shared.ElementItemEnumModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ElementItemId");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("ElementItemId");

                    b.ToTable("ElementItemEnums");
                });

            modelBuilder.Entity("MyHome.Shared.ElementItemModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowSchedule");

                    b.Property<string>("Description");

                    b.Property<string>("ElementId");

                    b.Property<string>("ExternalId");

                    b.Property<bool>("IsEnum");

                    b.Property<string>("ModeId");

                    b.Property<string>("Name");

                    b.Property<bool>("NotCollectChanges");

                    b.Property<int>("RefreshTime");

                    b.Property<int>("Type");

                    b.HasKey("Id");

                    b.HasIndex("ElementId");

                    b.HasIndex("ModeId")
                        .IsUnique();

                    b.ToTable("ElementItems");
                });

            modelBuilder.Entity("MyHome.Shared.ElementItemValueModel", b =>
                {
                    b.Property<DateTime>("DateTime");

                    b.Property<string>("ElementItemId");

                    b.Property<string>("RawValue");

                    b.Property<DateTime>("Updated");

                    b.Property<string>("ValueId");

                    b.HasKey("DateTime", "ElementItemId");

                    b.HasIndex("ElementItemId");

                    b.HasIndex("ValueId");

                    b.ToTable("ElementItemValues");
                });

            modelBuilder.Entity("MyHome.Shared.ElementModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ExternalId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Elements");
                });

            modelBuilder.Entity("MyHome.Shared.ScheduleHourModel", b =>
                {
                    b.Property<string>("ScheduleId");

                    b.Property<int>("DayOfWeek");

                    b.Property<int>("Hour");

                    b.Property<string>("RawValue");

                    b.Property<string>("ValueId");

                    b.HasKey("ScheduleId", "DayOfWeek", "Hour");

                    b.HasIndex("ValueId");

                    b.ToTable("ScheduleHours");
                });

            modelBuilder.Entity("MyHome.Shared.ScheduleModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ElementItemId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("MyHome.Shared.SettingModel", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Group");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("MyHome.Shared.ElementItemEnumModel", b =>
                {
                    b.HasOne("MyHome.Shared.ElementItemModel", "ElementItem")
                        .WithMany("EnumValues")
                        .HasForeignKey("ElementItemId");
                });

            modelBuilder.Entity("MyHome.Shared.ElementItemModel", b =>
                {
                    b.HasOne("MyHome.Shared.ElementModel", "Element")
                        .WithMany("Items")
                        .HasForeignKey("ElementId");

                    b.HasOne("MyHome.Shared.ScheduleModel", "Mode")
                        .WithOne("ElementItem")
                        .HasForeignKey("MyHome.Shared.ElementItemModel", "ModeId");
                });

            modelBuilder.Entity("MyHome.Shared.ElementItemValueModel", b =>
                {
                    b.HasOne("MyHome.Shared.ElementItemModel", "ElementItem")
                        .WithMany("Values")
                        .HasForeignKey("ElementItemId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyHome.Shared.ElementItemEnumModel", "Value")
                        .WithMany()
                        .HasForeignKey("ValueId");
                });

            modelBuilder.Entity("MyHome.Shared.ScheduleHourModel", b =>
                {
                    b.HasOne("MyHome.Shared.ScheduleModel", "Schedule")
                        .WithMany("ScheduleHours")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyHome.Shared.ElementItemEnumModel", "Value")
                        .WithMany()
                        .HasForeignKey("ValueId");
                });
        }
    }
}
