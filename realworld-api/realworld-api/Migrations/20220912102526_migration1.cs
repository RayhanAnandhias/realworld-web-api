using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace realworld_api.Migrations
{
    public partial class migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tag",
                columns: table => new
                {
                    id_tag = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tag_created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    tag_updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tag", x => x.id_tag);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    user_updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id_user);
                });

            migrationBuilder.CreateTable(
                name: "article",
                columns: table => new
                {
                    id_article = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_author = table.Column<int>(type: "int", nullable: true),
                    slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    favorites_count = table.Column<int>(type: "int", nullable: false),
                    article_created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    article_updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article", x => x.id_article);
                    table.ForeignKey(
                        name: "FK_article_user_id_author",
                        column: x => x.id_author,
                        principalTable: "user",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "profile_following",
                columns: table => new
                {
                    id_user_observer = table.Column<int>(type: "int", nullable: false),
                    id_user_target = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_profile_following", x => new { x.id_user_observer, x.id_user_target });
                    table.ForeignKey(
                        name: "FK_profile_following_user_id_user_observer",
                        column: x => x.id_user_observer,
                        principalTable: "user",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_profile_following_user_id_user_target",
                        column: x => x.id_user_target,
                        principalTable: "user",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "article_tag",
                columns: table => new
                {
                    id_article = table.Column<int>(type: "int", nullable: false),
                    id_tag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article_tag", x => new { x.id_article, x.id_tag });
                    table.ForeignKey(
                        name: "FK_article_tag_article_id_article",
                        column: x => x.id_article,
                        principalTable: "article",
                        principalColumn: "id_article",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_article_tag_tag_id_tag",
                        column: x => x.id_tag,
                        principalTable: "tag",
                        principalColumn: "id_tag",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comment",
                columns: table => new
                {
                    id_comment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_article = table.Column<int>(type: "int", nullable: false),
                    id_user = table.Column<int>(type: "int", nullable: false),
                    comment_body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    comment_created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    comment_updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comment", x => x.id_comment);
                    table.ForeignKey(
                        name: "FK_comment_article_id_article",
                        column: x => x.id_article,
                        principalTable: "article",
                        principalColumn: "id_article",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comment_user_id_user",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_favorite",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "int", nullable: false),
                    id_article_favorite = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_favorite", x => new { x.id_user, x.id_article_favorite });
                    table.ForeignKey(
                        name: "FK_user_favorite_article_id_article_favorite",
                        column: x => x.id_article_favorite,
                        principalTable: "article",
                        principalColumn: "id_article",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_favorite_user_id_user",
                        column: x => x.id_user,
                        principalTable: "user",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_article_id_author",
                table: "article",
                column: "id_author");

            migrationBuilder.CreateIndex(
                name: "IX_article_tag_id_tag",
                table: "article_tag",
                column: "id_tag");

            migrationBuilder.CreateIndex(
                name: "IX_comment_id_article",
                table: "comment",
                column: "id_article");

            migrationBuilder.CreateIndex(
                name: "IX_comment_id_user",
                table: "comment",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_profile_following_id_user_target",
                table: "profile_following",
                column: "id_user_target");

            migrationBuilder.CreateIndex(
                name: "IX_user_favorite_id_article_favorite",
                table: "user_favorite",
                column: "id_article_favorite");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "article_tag");

            migrationBuilder.DropTable(
                name: "comment");

            migrationBuilder.DropTable(
                name: "profile_following");

            migrationBuilder.DropTable(
                name: "user_favorite");

            migrationBuilder.DropTable(
                name: "tag");

            migrationBuilder.DropTable(
                name: "article");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
