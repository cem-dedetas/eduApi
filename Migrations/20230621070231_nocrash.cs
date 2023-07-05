using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace EduAPI.Migrations
{
    /// <inheritdoc />
    public partial class nocrash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    senderid = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "longtext", nullable: false),
                    timeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LiveLectureClassId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LiveLectures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Lecturerid = table.Column<int>(type: "int", nullable: false),
                    ChannelName = table.Column<string>(type: "longtext", nullable: false),
                    ShareUrl = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiveLectures", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Recordings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    start = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    end = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    sid = table.Column<string>(type: "longtext", nullable: true),
                    ruid = table.Column<string>(type: "longtext", nullable: true),
                    resourceId = table.Column<string>(type: "longtext", nullable: true),
                    url = table.Column<string>(type: "longtext", nullable: true),
                    LiveLectureClassId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recordings", x => x.id);
                    table.ForeignKey(
                        name: "FK_Recordings_LiveLectures_LiveLectureClassId",
                        column: x => x.LiveLectureClassId,
                        principalTable: "LiveLectures",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    tag_name = table.Column<string>(type: "longtext", nullable: false),
                    LiveLectureClassId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.id);
                    table.ForeignKey(
                        name: "FK_Tags_LiveLectures_LiveLectureClassId",
                        column: x => x.LiveLectureClassId,
                        principalTable: "LiveLectures",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "longtext", nullable: false),
                    id_number = table.Column<string>(type: "longtext", nullable: false),
                    password_hash = table.Column<byte[]>(type: "longblob", nullable: false),
                    password_salt = table.Column<byte[]>(type: "longblob", nullable: false),
                    email = table.Column<string>(type: "longtext", nullable: false),
                    token = table.Column<string>(type: "longtext", nullable: true),
                    LiveLectureClassId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_LiveLectures_LiveLectureClassId",
                        column: x => x.LiveLectureClassId,
                        principalTable: "LiveLectures",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserHistories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    userid = table.Column<int>(type: "int", nullable: false),
                    liveLectureId = table.Column<int>(type: "int", nullable: false),
                    joinDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    lastPingDate = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserHistories", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserHistories_LiveLectures_liveLectureId",
                        column: x => x.liveLectureId,
                        principalTable: "LiveLectures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserHistories_Users_userid",
                        column: x => x.userid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_LiveLectureClassId",
                table: "ChatMessages",
                column: "LiveLectureClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_senderid",
                table: "ChatMessages",
                column: "senderid");

            migrationBuilder.CreateIndex(
                name: "IX_LiveLectures_Lecturerid",
                table: "LiveLectures",
                column: "Lecturerid");

            migrationBuilder.CreateIndex(
                name: "IX_Recordings_LiveLectureClassId",
                table: "Recordings",
                column: "LiveLectureClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_LiveLectureClassId",
                table: "Tags",
                column: "LiveLectureClassId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistories_liveLectureId",
                table: "UserHistories",
                column: "liveLectureId");

            migrationBuilder.CreateIndex(
                name: "IX_UserHistories_userid",
                table: "UserHistories",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LiveLectureClassId",
                table: "Users",
                column: "LiveLectureClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_LiveLectures_LiveLectureClassId",
                table: "ChatMessages",
                column: "LiveLectureClassId",
                principalTable: "LiveLectures",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_senderid",
                table: "ChatMessages",
                column: "senderid",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LiveLectures_Users_Lecturerid",
                table: "LiveLectures",
                column: "Lecturerid",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_LiveLectures_LiveLectureClassId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Recordings");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "UserHistories");

            migrationBuilder.DropTable(
                name: "LiveLectures");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
