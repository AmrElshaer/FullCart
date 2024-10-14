using System;
using EFCore.AuditExtensions.Common.Extensions;
using EFCore.AuditExtensions.Common.SharedModels;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class CommentMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropAuditTrigger(triggerName: "Audit__Orders_Orders_Audit");
            //
            // migrationBuilder.DropTable(
            //     name: "Orders_Audit");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders_Audit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationType = table.Column<string>(type: "nvarchar(6)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    User = table.Column<string>(type: "nvarchar(255)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Audit_Id",
                table: "Orders_Audit",
                column: "Id");

            migrationBuilder.CreateAuditTrigger(
                auditedEntityTableName: "Orders",
                auditTableName: "Orders_Audit",
                triggerName: "Audit__Orders_Orders_Audit",
                auditedEntityTableKey: new AuditedEntityKeyProperty[]
                {
                    new(columnName: "Id", columnType: AuditColumnType.Text)
                },
                updateOptimisationThreshold: 100,
                noKeyChanges: false);
        }
    }
}
