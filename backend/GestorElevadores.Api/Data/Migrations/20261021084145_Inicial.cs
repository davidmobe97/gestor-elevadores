using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestorElevadores.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreTaller = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Cif = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    PersonaContacto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Telefono = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Direccion = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Poblacion = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Provincia = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FechaAlta = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Elevadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Marca = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Modelo = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    NumeroSerie = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    CapacidadKg = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaInstalacion = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elevadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Elevadores_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Incidencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Prioridad = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Estado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    TecnicoAsignado = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    FechaApertura = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Solucion = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ElevadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidencias_Elevadores_ElevadorId",
                        column: x => x.ElevadorId,
                        principalTable: "Elevadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Revisiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    Tecnico = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Resultado = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ElevadorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Revisiones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Revisiones_Elevadores_ElevadorId",
                        column: x => x.ElevadorId,
                        principalTable: "Elevadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Cif", "Direccion", "Email", "FechaAlta", "NombreTaller", "PersonaContacto", "Poblacion", "Provincia", "Telefono" },
                values: new object[,]
                {
                    { 1, null, null, "lavega@example.com", new DateTime(2025, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Talleres La Vega (demo)", "Antonio Ruiz", "Alcalá de Guadaíra", "Sevilla", "600000001" },
                    { 2, null, null, "aljarafe@example.com", new DateTime(2025, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Neumáticos Aljarafe (demo)", "María López", "Bormujos", "Sevilla", "600000002" },
                    { 3, null, null, "campina@example.com", new DateTime(2025, 1, 15, 9, 0, 0, 0, DateTimeKind.Utc), "AutoServicio Campiña (demo)", "José Moreno", "Carmona", "Sevilla", "600000003" }
                });

            migrationBuilder.InsertData(
                table: "Elevadores",
                columns: new[] { "Id", "CapacidadKg", "ClienteId", "FechaInstalacion", "Marca", "Modelo", "NumeroSerie", "Tipo" },
                values: new object[,]
                {
                    { 1, 4000, 1, new DateOnly(2024, 3, 10), "Königstein", "KN-2C40", "DEMO-0001", "DosColumnas" },
                    { 2, 3000, 1, new DateOnly(2025, 6, 2), "Königstein", "KN-T30", "DEMO-0002", "Tijera" },
                    { 3, 5000, 2, new DateOnly(2023, 11, 20), "Cascos", "C-4C50", "DEMO-0003", "CuatroColumnas" },
                    { 4, 2700, 3, new DateOnly(2026, 2, 14), "Cascos", "C-P27", "DEMO-0004", "Parking" }
                });

            migrationBuilder.InsertData(
                table: "Incidencias",
                columns: new[] { "Id", "Descripcion", "ElevadorId", "Estado", "FechaApertura", "FechaCierre", "Prioridad", "Solucion", "TecnicoAsignado", "Titulo" },
                values: new object[,]
                {
                    { 1, "Mancha de aceite bajo la columna 2.", 3, "Abierta", new DateTime(2026, 10, 1, 8, 30, 0, 0, DateTimeKind.Utc), null, "Alta", null, null, "Pierde aceite el cilindro" },
                    { 2, "Se queda a 10 cm del suelo.", 2, "Cerrada", new DateTime(2026, 9, 3, 10, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 9, 4, 12, 0, 0, 0, DateTimeKind.Utc), "Media", "Ajuste del final de carrera inferior.", "David", "No baja del todo" }
                });

            migrationBuilder.InsertData(
                table: "Revisiones",
                columns: new[] { "Id", "ElevadorId", "Fecha", "Observaciones", "Resultado", "Tecnico" },
                values: new object[,]
                {
                    { 1, 1, new DateOnly(2025, 3, 12), null, "Favorable", "David" },
                    { 2, 3, new DateOnly(2024, 11, 25), "Cables de seguridad con desgaste, sustituir en la próxima visita.", "ConDefectosLeves", "David" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Elevadores_ClienteId",
                table: "Elevadores",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Elevadores_Marca_NumeroSerie",
                table: "Elevadores",
                columns: new[] { "Marca", "NumeroSerie" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_ElevadorId",
                table: "Incidencias",
                column: "ElevadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Revisiones_ElevadorId",
                table: "Revisiones",
                column: "ElevadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Incidencias");

            migrationBuilder.DropTable(
                name: "Revisiones");

            migrationBuilder.DropTable(
                name: "Elevadores");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
