﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BankEmulatorDB.Migrations
{
    public partial class InitialSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    CardNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CardExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CardSecurityCode = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(19,4)", nullable: false, defaultValue: 0m),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.CardNumber);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CardNumber_CardExpiryDate_CardSecurityCode",
                table: "Accounts",
                columns: new[] { "CardNumber", "CardExpiryDate", "CardSecurityCode" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}