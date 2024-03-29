﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Survey.Infrastructure.Migrations
{
    public partial class inits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    ParentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectCategories_ProjectCategories_ParentID",
                        column: x => x.ParentID,
                        principalTable: "ProjectCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    ProjectCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectCategories_ProjectCategoryId",
                        column: x => x.ProjectCategoryId,
                        principalTable: "ProjectCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProjectCategories",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProjectCategories", x => new { x.UserId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_UserProjectCategories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProjectCategories_ProjectCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProjectCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormTitle = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Project_Id = table.Column<int>(type: "int", nullable: false),
                    Form_Status = table.Column<int>(type: "int", nullable: false),
                    WasPublished = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Forms_Projects_Project_Id",
                        column: x => x.Project_Id,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProject",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProjectsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProject", x => new { x.UserId, x.ProjectsId });
                    table.ForeignKey(
                        name: "FK_UserProject_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProject_Projects_ProjectsId",
                        column: x => x.ProjectsId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Field_Type = table.Column<int>(type: "int", nullable: false),
                    Form_Id = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionOrder = table.Column<int>(type: "int", nullable: false),
                    SkipLogicType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Forms_Form_Id",
                        column: x => x.Form_Id,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyDownloads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DownloadType = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FormId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyDownloads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyDownloads_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyDownloads_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveySubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FormId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SubmitFromIP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CaseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveySubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveySubmissions_AspNetUsers_AgentId",
                        column: x => x.AgentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SurveySubmissions_Forms_FormId",
                        column: x => x.FormId,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question_Id = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isSelected = table.Column<bool>(type: "bit", nullable: false),
                    OrderNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptions_Questions_Question_Id",
                        column: x => x.Question_Id,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Form_Id = table.Column<int>(type: "int", nullable: true),
                    Question_Id = table.Column<int>(type: "int", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: true),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmissionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answers_Forms_Form_Id",
                        column: x => x.Form_Id,
                        principalTable: "Forms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Answers_QuestionOptions_OptionId",
                        column: x => x.OptionId,
                        principalTable: "QuestionOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Answers_Questions_Question_Id",
                        column: x => x.Question_Id,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answers_SurveySubmissions_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "SurveySubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkipLogic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Parent_Question_Id = table.Column<int>(type: "int", nullable: false),
                    Child_Question_Id = table.Column<int>(type: "int", nullable: false),
                    Condition_Option = table.Column<int>(type: "int", nullable: true),
                    Operator = table.Column<int>(type: "int", nullable: false),
                    Comparable_Value = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkipLogic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkipLogic_QuestionOptions_Condition_Option",
                        column: x => x.Condition_Option,
                        principalTable: "QuestionOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkipLogic_Questions_Child_Question_Id",
                        column: x => x.Child_Question_Id,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkipLogic_Questions_Parent_Question_Id",
                        column: x => x.Parent_Question_Id,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", "ed9e915e-7edb-4329-8d67-4bfbc42147e1", "SuperAdmin", "SUPERADMIN" },
                    { "2", "2e23a2f5-38ad-470a-81f7-92b3e936ee53", "Dean", "DEAN" },
                    { "5", "4297a5af-f04e-470a-958e-d4cd3bd9115f", "Student", "STUDENT" },
                    { "3", "5fbe9a9e-b90a-4b1e-893c-da45a1cc3188", "Professor", "PROFESSOR" },
                    { "4", "31daea34-3f15-4645-8129-6d037cc78156", "QA", "QA" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1", 0, "24bd707b-05e1-4ec4-bba3-b840efc3c1ac", "IdentityUser", "admin@riinvest.net", true, false, null, "ADMIN@RIINVEST.NET", null, "AQAAAAEAACcQAAAAEO3VV678iLtvVRJEvPpSSHzN6SThfs283ciCRsnEfJqwYul884rn2WaX9siZjUlTTQ==", null, false, "4c722e04-97ef-49a2-a0f2-e6159a787541", false, "SuperAdmin" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 3, "Permission", "Permissions.Survey.ViewAll", "1" },
                    { 31, "Permission", "Permissions.Survey.Update", "2" },
                    { 32, "Permission", "Permissions.Survey.Delete", "2" },
                    { 33, "Permission", "Permissions.Survey.Collect", "2" },
                    { 34, "Permission", "Permissions.Survey.SeeSurveyResults", "2" },
                    { 35, "Permission", "Permissions.Survey.Publish", "2" },
                    { 36, "Permission", "Permissions.User.ViewUsers", "2" },
                    { 37, "Permission", "Permissions.User.UpdateUser", "2" },
                    { 38, "Permission", "Permissions.User.DeleteUser", "2" },
                    { 39, "Permission", "Permissions.User.AddUser", "2" },
                    { 40, "Permission", "Permissions.Role.Add", "2" },
                    { 41, "Permission", "Permissions.Role.Delete", "2" },
                    { 42, "Permission", "Permissions.Role.PermissionsView", "2" },
                    { 43, "Permission", "Permissions.Role.View", "2" },
                    { 44, "Permission", "Permissions.Project.Add", "2" },
                    { 45, "Permission", "Permissions.Project.Delete", "2" },
                    { 46, "Permission", "Permissions.Project.Update", "2" },
                    { 47, "Permission", "Permissions.Project.View", "2" },
                    { 48, "Permission", "Permissions.Category.Add", "2" },
                    { 49, "Permission", "Permissions.Category.AssignUsers", "2" },
                    { 50, "Permission", "Permissions.Category.Delete", "2" },
                    { 51, "Permission", "Permissions.Category.Update", "2" },
                    { 52, "Permission", "Permissions.Category.View", "2" },
                    { 1, "Permission", "Permissions.Survey.View", "5" },
                    { 30, "Permission", "Permissions.Survey.Create", "2" },
                    { 2, "Permission", "Permissions.Survey.Collect", "5" },
                    { 29, "Permission", "Permissions.Survey.View", "2" },
                    { 27, "Permission", "Permissions.Category.View", "1" },
                    { 4, "Permission", "Permissions.Survey.View", "1" },
                    { 5, "Permission", "Permissions.Survey.Create", "1" },
                    { 6, "Permission", "Permissions.Survey.Update", "1" },
                    { 7, "Permission", "Permissions.Survey.Delete", "1" },
                    { 8, "Permission", "Permissions.Survey.Collect", "1" },
                    { 9, "Permission", "Permissions.Survey.SeeSurveyResults", "1" },
                    { 10, "Permission", "Permissions.Survey.Publish", "1" },
                    { 11, "Permission", "Permissions.User.ViewUsers", "1" },
                    { 12, "Permission", "Permissions.User.UpdateUser", "1" },
                    { 13, "Permission", "Permissions.User.DeleteUser", "1" },
                    { 14, "Permission", "Permissions.User.AddUser", "1" },
                    { 15, "Permission", "Permissions.Role.Add", "1" },
                    { 16, "Permission", "Permissions.Role.Delete", "1" },
                    { 17, "Permission", "Permissions.Role.PermissionsView", "1" }
                });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 18, "Permission", "Permissions.Role.View", "1" },
                    { 19, "Permission", "Permissions.Project.Add", "1" },
                    { 20, "Permission", "Permissions.Project.Delete", "1" },
                    { 21, "Permission", "Permissions.Project.Update", "1" },
                    { 22, "Permission", "Permissions.Project.View", "1" },
                    { 23, "Permission", "Permissions.Category.Add", "1" },
                    { 24, "Permission", "Permissions.Category.AssignUsers", "1" },
                    { 25, "Permission", "Permissions.Category.Delete", "1" },
                    { 26, "Permission", "Permissions.Category.Update", "1" },
                    { 28, "Permission", "Permissions.Survey.ViewAll", "2" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1" });

            migrationBuilder.CreateIndex(
                name: "IX_Answers_Form_Id",
                table: "Answers",
                column: "Form_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_OptionId",
                table: "Answers",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_Question_Id",
                table: "Answers",
                column: "Question_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Answers_SubmissionId",
                table: "Answers",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Forms_Project_Id",
                table: "Forms",
                column: "Project_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCategories_ParentID",
                table: "ProjectCategories",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectCategoryId",
                table: "Projects",
                column: "ProjectCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_Question_Id",
                table: "QuestionOptions",
                column: "Question_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_Form_Id",
                table: "Questions",
                column: "Form_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SkipLogic_Child_Question_Id",
                table: "SkipLogic",
                column: "Child_Question_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SkipLogic_Condition_Option",
                table: "SkipLogic",
                column: "Condition_Option");

            migrationBuilder.CreateIndex(
                name: "IX_SkipLogic_Parent_Question_Id",
                table: "SkipLogic",
                column: "Parent_Question_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDownloads_FormId",
                table: "SurveyDownloads",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyDownloads_UserId",
                table: "SurveyDownloads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveySubmissions_AgentId",
                table: "SurveySubmissions",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveySubmissions_FormId",
                table: "SurveySubmissions",
                column: "FormId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProject_ProjectsId",
                table: "UserProject",
                column: "ProjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProjectCategories_CategoryId",
                table: "UserProjectCategories",
                column: "CategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Answers");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "SkipLogic");

            migrationBuilder.DropTable(
                name: "SurveyDownloads");

            migrationBuilder.DropTable(
                name: "UserProject");

            migrationBuilder.DropTable(
                name: "UserProjectCategories");

            migrationBuilder.DropTable(
                name: "SurveySubmissions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "QuestionOptions");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectCategories");
        }
    }
}
