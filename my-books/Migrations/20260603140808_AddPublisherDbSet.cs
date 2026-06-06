using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace my_books.Migrations
{
    /// <inheritdoc />
    public partial class AddPublisherDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publisher_PublisherId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Publisher",
                table: "Publisher");

            migrationBuilder.RenameTable(
                name: "Publisher",
                newName: "Publishers");

            migrationBuilder.AlterColumn<int>(
                name: "PublisherId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Publishers",
                table: "Publishers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Publishers",
                table: "Publishers");

            migrationBuilder.RenameTable(
                name: "Publishers",
                newName: "Publisher");

            migrationBuilder.AlterColumn<int>(
                name: "PublisherId",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Publisher",
                table: "Publisher",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publisher_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publisher",
                principalColumn: "Id");
        }
    }
}
