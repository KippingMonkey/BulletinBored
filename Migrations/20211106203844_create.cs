using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BulletinBored.Migrations
{
    public partial class create : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    PassWord = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostHeading = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PostContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Post_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoryPost",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    PostsID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryPost", x => new { x.CategoriesId, x.PostsID });
                    table.ForeignKey(
                        name: "FK_CategoryPost_Category_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryPost_Post_PostsID",
                        column: x => x.PostsID,
                        principalTable: "Post",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Like",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Like", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Like_Post_PostID",
                        column: x => x.PostID,
                        principalTable: "Post",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Like_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPost_PostsID",
                table: "CategoryPost",
                column: "PostsID");

            migrationBuilder.CreateIndex(
                name: "IX_Like_PostID",
                table: "Like",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_Like_UserID",
                table: "Like",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Post_UserID",
                table: "Post",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryPost");

            migrationBuilder.DropTable(
                name: "Like");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
