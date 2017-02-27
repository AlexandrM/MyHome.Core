using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace MyHome.Shared
{
    public class DB: DbContext
    {
        public DB()
            : base()
        {
        }

        public DB(DbContextOptions<DB> options)
            : base(options)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var options = optionsBuilder.Options.FindExtension<Microsoft.EntityFrameworkCore.Infrastructure.Internal.SqliteOptionsExtension>();

            if (options == null)
            {
                optionsBuilder.UseSqlite("Data Source=DataBase.db3;",
                    sqlOptions => sqlOptions
                    .MigrationsAssembly("MyHome.Core"));
            }
            else
            {
                optionsBuilder.UseSqlite(
                    options.ConnectionString,
                    sqlOptions => sqlOptions
                        .MigrationsAssembly("MyHome.Core")
                        .MigrationsHistoryTable("_Migrations"));

            }
            optionsBuilder.ReplaceService<SqliteMigrationsSqlGenerator, SqliteMigrationsSqlGeneratorExt>();
        }
        public DbSet<ElementModel> Elements { get; set; }

        public DbSet<ElementItemModel> ElementItems { get; set; }

        public DbSet<ElementItemValueModel> ElementItemValues { get; set; }

        public DbSet<ElementItemEnumModel> ElementItemEnums { get; set; }

        public DbSet<ScheduleModel> Schedules { get; set; }

        public DbSet<ScheduleHourModel> ScheduleHours { get; set; }

        public DbSet<SettingModel> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ElementItemValueModel>()
                .HasKey(x => new { x.DateTime, x.ElementItemId });

            modelBuilder.Entity<ScheduleHourModel>()
                .HasKey(x => new {  x.ScheduleId, x.DayOfWeek, x.Hour });

            modelBuilder.Entity<ScheduleModel>()
                .HasOne(x => x.ElementItem)
                .WithOne(x => x.Mode)
                .HasForeignKey<ElementItemModel>(x => x.ModeId);
        }
    }

    public class SqliteMigrationsSqlGeneratorExt: SqliteMigrationsSqlGenerator
    {
        public SqliteMigrationsSqlGeneratorExt(
            IRelationalCommandBuilderFactory commandBuilderFactory,
            ISqlGenerationHelper sqlGenerationHelper,
            IRelationalTypeMapper typeMapper,
            IRelationalAnnotationProvider annotations)
            : base(commandBuilderFactory, sqlGenerationHelper, typeMapper, annotations)
        {
        }

        public override IReadOnlyList<MigrationCommand> Generate(IReadOnlyList<MigrationOperation> operations, IModel model = null)
        {
            return base.Generate(LiftForeignKeyOperations(operations), model);
        }

        private static IReadOnlyList<MigrationOperation> LiftForeignKeyOperations(IReadOnlyList<MigrationOperation> migrationOperations)
        {
            var operations = new List<MigrationOperation>();
            foreach (var operation in migrationOperations)
            {
                var foreignKeyOperation = operation as AddForeignKeyOperation;
                if (foreignKeyOperation != null)
                {
                    var table = migrationOperations
                        .OfType<CreateTableOperation>()
                        .FirstOrDefault(o => o.Name == foreignKeyOperation.Table);

                    if (table != null)
                    {
                        table.ForeignKeys.Add(foreignKeyOperation);
                        //do not add to fk operation migration
                        continue;
                    }
                }

                var dropOperation = operation as DropColumnOperation;
                if (dropOperation != null)
                {
                    continue;
                }

                operations.Add(operation);
            }
            return operations.AsReadOnly();
        }
    }
}
