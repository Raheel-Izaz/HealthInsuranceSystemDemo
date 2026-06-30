using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthInsuranceSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddingInstallmentPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstallmentPlanId",
                table: "Policies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InstallmentPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstallmentPlans", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "InstallmentPlans",
                columns: new[] { "Id", "Count", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, 1, true, "Lump Sum / Annual" },
                    { 2, 2, true, "Semi-Annual" },
                    { 3, 4, true, "Quarterly" },
                    { 4, 12, true, "Monthly" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Policies_InstallmentPlanId",
                table: "Policies",
                column: "InstallmentPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_Policies_InstallmentPlans_InstallmentPlanId",
                table: "Policies",
                column: "InstallmentPlanId",
                principalTable: "InstallmentPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Policies_InstallmentPlans_InstallmentPlanId",
                table: "Policies");

            migrationBuilder.DropTable(
                name: "InstallmentPlans");

            migrationBuilder.DropIndex(
                name: "IX_Policies_InstallmentPlanId",
                table: "Policies");

            migrationBuilder.DropColumn(
                name: "InstallmentPlanId",
                table: "Policies");
        }
    }
}
