using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareNest_OrderDetail.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class recreate_orderdetails_if_missing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_catalog.pg_class c
        JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace
        WHERE n.nspname='public' AND c.relname='orderdetails'
    ) THEN
        CREATE TABLE orderdetails (
            ""Id"" text NOT NULL,
            ""ProductDetailId"" text NULL,
            ""OrderId"" text NULL,
            ""Quantity"" integer NOT NULL,
            ""TotalAmount"" double precision NOT NULL,
            ""CreatedBy"" text NULL,
            ""UpdatedBy"" text NULL,
            ""DeletedBy"" text NULL,
            ""CreatedAt"" timestamp with time zone NULL,
            ""UpdatedAt"" timestamp with time zone NULL,
            ""DeleteAt"" timestamp with time zone NULL,
            CONSTRAINT ""PK_orderdetails"" PRIMARY KEY (""Id"")
        );
    END IF;
END $$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_catalog.pg_class c
        JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace
        WHERE n.nspname='public' AND c.relname='orderdetails'
    ) THEN
        DROP TABLE orderdetails;
    END IF;
END $$;
");
        }
    }
}


