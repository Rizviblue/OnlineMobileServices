using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineMobileServices.Migrations
{
    /// <inheritdoc />
    public partial class FixFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubmittedDate",
                table: "Feedback",
                newName: "FeedbackDate");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Feedback",
                newName: "MobileNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MobileNumber",
                table: "Feedback",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "FeedbackDate",
                table: "Feedback",
                newName: "SubmittedDate");
        }
    }
}
