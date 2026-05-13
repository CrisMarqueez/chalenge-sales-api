using System;
using Ambev.DeveloperEvaluation.ORM;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ambev.DeveloperEvaluation.ORM.Migrations;

/// <inheritdoc />
[DbContext(typeof(DefaultContext))]
[Migration("20260513190000_AddSaleOwnerUserId")]
public partial class AddSaleOwnerUserId : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "OwnerUserId",
            table: "Sales",
            type: "uuid",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "OwnerUserId",
            table: "Sales");
    }
}
