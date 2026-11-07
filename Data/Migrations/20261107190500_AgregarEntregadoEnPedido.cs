using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YaVa.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEntregadoEnPedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EntregadoEn",
                table: "Pedidos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE Pedidos SET EntregadoEn = CreadoEn WHERE Estado = 3 AND EntregadoEn IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntregadoEn",
                table: "Pedidos");
        }
    }
}
