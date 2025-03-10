using Microsoft.EntityFrameworkCore.Migrations;

namespace Streetcode.DAL.Migrations;

public partial class AddIndexToFacts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "index",
            schema: "streetcode",
            table: "facts",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "index",
            schema: "streetcode",
            table: "facts");
    }
}
