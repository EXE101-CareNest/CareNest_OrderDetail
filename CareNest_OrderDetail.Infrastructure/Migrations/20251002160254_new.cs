using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareNest_OrderDetail.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Đổi tên bảng và constraint theo điều kiện để tránh lỗi khi bảng đã là chữ thường
            migrationBuilder.Sql(@"
DO $$
BEGIN
    -- Nếu bảng đã tồn tại với tên chữ thường, bỏ qua đổi tên
    IF EXISTS (
        SELECT 1 FROM pg_catalog.pg_class c
        JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace
        WHERE n.nspname='public' AND c.relname='orderdetails'
    ) THEN
        RAISE NOTICE 'Table orderdetails already exists. Skipping rename.';
    -- Nếu bảng tồn tại với tên có chữ hoa, đổi tên về chữ thường
    ELSIF EXISTS (
        SELECT 1 FROM pg_catalog.pg_class c
        JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace
        WHERE n.nspname='public' AND c.relname='OrderDetails'
    ) THEN
        ALTER TABLE ""OrderDetails"" RENAME TO orderdetails;
    END IF;

    -- Nếu constraint PK cũ còn tồn tại, đổi tên constraint về tên mới
    IF EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'PK_OrderDetails'
    ) THEN
        ALTER TABLE orderdetails RENAME CONSTRAINT ""PK_OrderDetails"" TO ""PK_orderdetails"";
    END IF;
END $$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Hoàn tác đổi tên theo điều kiện
            migrationBuilder.Sql(@"
DO $$
BEGIN
    -- Đổi lại tên constraint nếu đang là tên mới
    IF EXISTS (
        SELECT 1 FROM pg_constraint WHERE conname = 'PK_orderdetails'
    ) THEN
        ALTER TABLE orderdetails RENAME CONSTRAINT ""PK_orderdetails"" TO ""PK_OrderDetails"";
    END IF;

    -- Nếu bảng chữ thường tồn tại và bảng chữ hoa chưa tồn tại, đổi tên ngược lại
    IF EXISTS (
        SELECT 1 FROM pg_catalog.pg_class c
        JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace
        WHERE n.nspname='public' AND c.relname='orderdetails'
    ) AND NOT EXISTS (
        SELECT 1 FROM pg_catalog.pg_class c
        JOIN pg_catalog.pg_namespace n ON n.oid=c.relnamespace
        WHERE n.nspname='public' AND c.relname='OrderDetails'
    ) THEN
        ALTER TABLE orderdetails RENAME TO ""OrderDetails"";
    END IF;
END $$;
");
        }
    }
}
