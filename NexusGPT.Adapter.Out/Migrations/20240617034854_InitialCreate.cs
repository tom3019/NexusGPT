using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusGPT.Adapter.Out.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageChannels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalQuestionTokenCount = table.Column<int>(type: "int", nullable: false),
                    TotalAnswerTokenCount = table.Column<int>(type: "int", nullable: false),
                    TotalTokenCount = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Question = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ChannelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionTokenCount = table.Column<int>(type: "int", nullable: false),
                    AnswerTokenCount = table.Column<int>(type: "int", nullable: false),
                    TotalTokenCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => new { x.MessageChannelId, x.Id });
                    table.ForeignKey(
                        name: "FK_Messages_MessageChannels_MessageChannelId",
                        column: x => x.MessageChannelId,
                        principalTable: "MessageChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageChannel_Id",
                table: "MessageChannels",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Message_Id",
                table: "Messages",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "MessageChannels");
        }
    }
}
