using Microsoft.EntityFrameworkCore.Migrations;

namespace Streetcode.DAL.Persistence.Migrations;

public partial class AddIndexToFacts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Index",
            schema: "streetcode",
            table: "facts",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Index",
            schema: "streetcode",
            table: "facts");
    }
}