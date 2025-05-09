using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlacklistedToken",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    black_listed_on = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlacklistedToken", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "BlogCategory",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategory", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Option",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    option_key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    option_value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Option", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Otp",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    otp_code = table.Column<int>(type: "int", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expiry_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Otp", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Plan",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    property_count = table.Column<int>(type: "int", nullable: false),
                    plan_months = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyCategory",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCategory", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyFacility",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFacility", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "PropertySituation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertySituation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Province",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Province", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    short_description = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_alt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    blog_text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    show_blog = table.Column<bool>(type: "bit", nullable: false),
                    keywords = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    blog_category_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blog", x => x.id);
                    table.ForeignKey(
                        name: "FK_Blog_BlogCategory_blog_category_id",
                        column: x => x.blog_category_id,
                        principalTable: "BlogCategory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    province_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.id);
                    table.ForeignKey(
                        name: "FK_City_Province_province_id",
                        column: x => x.province_id,
                        principalTable: "Province",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agency",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    agency_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    city_province_full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    city_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agency", x => x.id);
                    table.ForeignKey(
                        name: "FK_Agency_City_city_id",
                        column: x => x.city_id,
                        principalTable: "City",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    gallery = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    state_enum = table.Column<int>(type: "int", nullable: false),
                    address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type_enum = table.Column<int>(type: "int", nullable: false),
                    city_province_full_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    city_id = table.Column<long>(type: "bigint", nullable: false),
                    cityid = table.Column<long>(type: "bigint", nullable: false),
                    meterage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    category_id = table.Column<long>(type: "bigint", nullable: false),
                    is_for_sale = table.Column<bool>(type: "bit", nullable: false),
                    sell_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    mortgage_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    rent_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    bed_room_count = table.Column<int>(type: "int", nullable: false),
                    property_age = table.Column<int>(type: "int", nullable: false),
                    property_floor = table.Column<int>(type: "int", nullable: false),
                    expire_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    situation_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.id);
                    table.ForeignKey(
                        name: "FK_Property_City_cityid",
                        column: x => x.cityid,
                        principalTable: "City",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Property_PropertyCategory_category_id",
                        column: x => x.category_id,
                        principalTable: "PropertyCategory",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Property_PropertySituation_situation_id",
                        column: x => x.situation_id,
                        principalTable: "PropertySituation",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_mobile_confirmed = table.Column<bool>(type: "bit", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    failed_login_count = table.Column<int>(type: "int", nullable: false),
                    lock_out_end_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    last_login_date_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    security_stamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    concurrency_stamp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_locked_out = table.Column<bool>(type: "bit", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    refresh_token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    refresh_token_expiry_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    agency_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                    table.ForeignKey(
                        name: "FK_User_Agency_agency_id",
                        column: x => x.agency_id,
                        principalTable: "Agency",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyFacilityProperty",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    property_id = table.Column<long>(type: "bigint", nullable: false),
                    property_facility_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyFacilityProperty", x => x.id);
                    table.ForeignKey(
                        name: "FK_PropertyFacilityProperty_PropertyFacility_property_facility_id",
                        column: x => x.property_facility_id,
                        principalTable: "PropertyFacility",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyFacilityProperty_Property_property_id",
                        column: x => x.property_id,
                        principalTable: "Property",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    amount = table.Column<long>(type: "bigint", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    response_message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    authority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    date_paid = table.Column<DateTime>(type: "datetime2", nullable: false),
                    card_number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    userid = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.id);
                    table.ForeignKey(
                        name: "FK_Order_User_userid",
                        column: x => x.userid,
                        principalTable: "User",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    slug = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.user_id, x.role_id, x.id });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_role_id",
                        column: x => x.role_id,
                        principalTable: "Role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Option",
                columns: new[] { "id", "created_at", "option_key", "option_value", "slug", "updated_at" },
                values: new object[] { 1L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "AdminMobile", "09301724389", "AdminMobile", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "id", "created_at", "slug", "title", "updated_at" },
                values: new object[,]
                {
                    { 1L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Admin_Role", "Admin", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Customer_Role", "Customer", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "id", "agency_id", "concurrency_stamp", "created_at", "email", "failed_login_count", "is_active", "is_locked_out", "is_mobile_confirmed", "last_login_date_time", "lock_out_end_time", "mobile", "password_hash", "refresh_token", "refresh_token_expiry_time", "security_stamp", "slug", "updated_at", "user_name" },
                values: new object[] { 1L, null, "X3JO2EOCURAEBU6HHY6OBYEDD2877FXU", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "info@avatick.com", 0, true, false, false, null, null, "09309309393", "omTtMfA5EEJCzjH5t/Q67cRXK5TRwerSqN7sJSm41No=.FRLmTm9jwMcEFnjpjgivJw==", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "098NTB7E5LFFXREHBSEHDKLI0DOBIKST", "Admin-User", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "admin-user" });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "id", "role_id", "user_id", "created_at", "slug", "updated_at" },
                values: new object[] { 0L, 1L, 1L, new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), "Admin-User", new DateTime(2025, 1, 1, 12, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Agency_city_id",
                table: "Agency",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_Agency_slug",
                table: "Agency",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistedToken_slug",
                table: "BlacklistedToken",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blog_blog_category_id",
                table: "Blog",
                column: "blog_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_short_description",
                table: "Blog",
                column: "short_description");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_slug",
                table: "Blog",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategory_slug",
                table: "BlogCategory",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_City_province_id",
                table: "City",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "IX_City_slug",
                table: "City",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contact_slug",
                table: "Contact",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Option_slug",
                table: "Option",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_slug",
                table: "Order",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Order_userid",
                table: "Order",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_Otp_slug",
                table: "Otp",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plan_slug",
                table: "Plan",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Property_category_id",
                table: "Property",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Property_cityid",
                table: "Property",
                column: "cityid");

            migrationBuilder.CreateIndex(
                name: "IX_Property_situation_id",
                table: "Property",
                column: "situation_id");

            migrationBuilder.CreateIndex(
                name: "IX_Property_slug",
                table: "Property",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyCategory_slug",
                table: "PropertyCategory",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFacility_slug",
                table: "PropertyFacility",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFacilityProperty_property_facility_id",
                table: "PropertyFacilityProperty",
                column: "property_facility_id");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFacilityProperty_property_id",
                table: "PropertyFacilityProperty",
                column: "property_id");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyFacilityProperty_slug",
                table: "PropertyFacilityProperty",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertySituation_slug",
                table: "PropertySituation",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Province_slug",
                table: "Province",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_slug",
                table: "Role",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_agency_id",
                table: "User",
                column: "agency_id",
                unique: true,
                filter: "[agency_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_User_slug",
                table: "User",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_user_name",
                table: "User",
                column: "user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_role_id",
                table: "UserRole",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_slug",
                table: "UserRole",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlacklistedToken");

            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.DropTable(
                name: "Option");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Otp");

            migrationBuilder.DropTable(
                name: "Plan");

            migrationBuilder.DropTable(
                name: "PropertyFacilityProperty");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "BlogCategory");

            migrationBuilder.DropTable(
                name: "PropertyFacility");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "PropertyCategory");

            migrationBuilder.DropTable(
                name: "PropertySituation");

            migrationBuilder.DropTable(
                name: "Agency");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "Province");
        }
    }
}
