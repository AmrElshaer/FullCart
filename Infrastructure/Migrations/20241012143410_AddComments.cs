using System;
using EFCore.AuditExtensions.Common.Extensions;
using EFCore.AuditExtensions.Common.SharedModels;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AddComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       //     migrationBuilder.DropAuditTrigger(triggerName: "Audit__Orders_Orders_Audit");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderComments",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderComments", x => new { x.OrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_OrderComments_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_OrderId",
                table: "Comments",
                column: "OrderId");

            // migrationBuilder.AlterColumn<string>(
            //     name: "Id",
            //     table: "Orders_Audit",
            //     type: "nvarchar(max)",
            //     nullable: false,
            //     oldClrType: typeof(Guid),
            //     oldType: "uniqueidentifier");

            // migrationBuilder.CreateAuditTrigger(
            //     auditedEntityTableName: "Orders",
            //     auditTableName: "Orders_Audit",
            //     triggerName: "Audit__Orders_Orders_Audit",
            //     auditedEntityTableKey: new AuditedEntityKeyProperty[]
            //     {
            //         new(columnName: "Id", columnType: AuditColumnType.Text)
            //     },
            //     updateOptimisationThreshold: 100,
            //     noKeyChanges: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropAuditTrigger(triggerName: "Audit__Orders_Orders_Audit");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "OrderComments");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Orders_Audit",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateAuditTrigger(
                auditedEntityTableName: "Orders",
                auditTableName: "Orders_Audit",
                triggerName: "Audit__Orders_Orders_Audit",
                auditedEntityTableKey: new AuditedEntityKeyProperty[]
                {
                    new(columnName: "Id", columnType: AuditColumnType.Guid)
                },
                updateOptimisationThreshold: 100,
                noKeyChanges: false);
        }
    }
}
