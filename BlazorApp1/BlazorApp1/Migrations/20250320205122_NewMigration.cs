using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ESII2025d2.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriaTalento",
                columns: table => new
                {
                    cod = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaTalento", x => x.cod);
                });

            migrationBuilder.CreateTable(
                name: "Skill",
                columns: table => new
                {
                    cod = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false),
                    area = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skill", x => x.cod);
                });

            migrationBuilder.CreateTable(
                name: "Utilizador",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    palavra_passe = table.Column<string>(type: "text", nullable: false),
                    datanascimento = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizador", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    empresa = table.Column<string>(type: "text", nullable: false),
                    numerotelefone = table.Column<string>(type: "text", nullable: false),
                    idutilizador = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.id);
                    table.ForeignKey(
                        name: "FK_Cliente_Utilizador_idutilizador",
                        column: x => x.idutilizador,
                        principalTable: "Utilizador",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Talento",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "text", nullable: false),
                    pais = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    precohora = table.Column<decimal>(type: "numeric", nullable: false),
                    codcategoriatalento = table.Column<int>(type: "integer", nullable: true),
                    idutilizador = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talento", x => x.id);
                    table.ForeignKey(
                        name: "FK_Talento_CategoriaTalento_codcategoriatalento",
                        column: x => x.codcategoriatalento,
                        principalTable: "CategoriaTalento",
                        principalColumn: "cod");
                    table.ForeignKey(
                        name: "FK_Talento_Utilizador_idutilizador",
                        column: x => x.idutilizador,
                        principalTable: "Utilizador",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostaTrabalho",
                columns: table => new
                {
                    cod = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codskill = table.Column<int>(type: "integer", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    numtotalhoras = table.Column<int>(type: "integer", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: true),
                    cliente_id = table.Column<int>(type: "integer", nullable: true),
                    cattalento_cod = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostaTrabalho", x => x.cod);
                    table.ForeignKey(
                        name: "FK_PropostaTrabalho_CategoriaTalento_cattalento_cod",
                        column: x => x.cattalento_cod,
                        principalTable: "CategoriaTalento",
                        principalColumn: "cod");
                    table.ForeignKey(
                        name: "FK_PropostaTrabalho_Cliente_cliente_id",
                        column: x => x.cliente_id,
                        principalTable: "Cliente",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_PropostaTrabalho_Skill_codskill",
                        column: x => x.codskill,
                        principalTable: "Skill",
                        principalColumn: "cod",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Experiencia",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    titulo = table.Column<string>(type: "text", nullable: false),
                    nomeempresa = table.Column<string>(type: "text", nullable: false),
                    anocomeco = table.Column<int>(type: "integer", nullable: false),
                    anofim = table.Column<int>(type: "integer", nullable: true),
                    idtalento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiencia", x => x.id);
                    table.ForeignKey(
                        name: "FK_Experiencia_Talento_idtalento",
                        column: x => x.idtalento,
                        principalTable: "Talento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TalentoSkill",
                columns: table => new
                {
                    codskill = table.Column<int>(type: "integer", nullable: false),
                    idtalento = table.Column<int>(type: "integer", nullable: false),
                    anosexperiencia = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalentoSkill", x => x.codskill);
                    table.ForeignKey(
                        name: "FK_TalentoSkill_Skill_codskill",
                        column: x => x.codskill,
                        principalTable: "Skill",
                        principalColumn: "cod",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TalentoSkill_Talento_idtalento",
                        column: x => x.idtalento,
                        principalTable: "Talento",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cliente_idutilizador",
                table: "Cliente",
                column: "idutilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Experiencia_idtalento",
                table: "Experiencia",
                column: "idtalento");

            migrationBuilder.CreateIndex(
                name: "IX_PropostaTrabalho_cattalento_cod",
                table: "PropostaTrabalho",
                column: "cattalento_cod");

            migrationBuilder.CreateIndex(
                name: "IX_PropostaTrabalho_cliente_id",
                table: "PropostaTrabalho",
                column: "cliente_id");

            migrationBuilder.CreateIndex(
                name: "IX_PropostaTrabalho_codskill",
                table: "PropostaTrabalho",
                column: "codskill");

            migrationBuilder.CreateIndex(
                name: "IX_Talento_codcategoriatalento",
                table: "Talento",
                column: "codcategoriatalento");

            migrationBuilder.CreateIndex(
                name: "IX_Talento_idutilizador",
                table: "Talento",
                column: "idutilizador");

            migrationBuilder.CreateIndex(
                name: "IX_TalentoSkill_idtalento",
                table: "TalentoSkill",
                column: "idtalento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Experiencia");

            migrationBuilder.DropTable(
                name: "PropostaTrabalho");

            migrationBuilder.DropTable(
                name: "TalentoSkill");

            migrationBuilder.DropTable(
                name: "Cliente");

            migrationBuilder.DropTable(
                name: "Skill");

            migrationBuilder.DropTable(
                name: "Talento");

            migrationBuilder.DropTable(
                name: "CategoriaTalento");

            migrationBuilder.DropTable(
                name: "Utilizador");
        }
    }
}
