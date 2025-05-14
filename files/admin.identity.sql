/*
 Navicat Premium Dump SQL

 Source Server         : pgsql
 Source Server Type    : PostgreSQL
 Source Server Version : 160003 (160003)
 Source Host           : localhost:5432
 Source Catalog        : admin.identity
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 160003 (160003)
 File Encoding         : 65001

 Date: 14/05/2025 21:48:28
*/


-- ----------------------------
-- Sequence structure for tb_application_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_application_ID_seq";
CREATE SEQUENCE "public"."tb_application_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_application_permission_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_application_permission_ID_seq";
CREATE SEQUENCE "public"."tb_application_permission_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_application_role_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_application_role_ID_seq";
CREATE SEQUENCE "public"."tb_application_role_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_oidc_api_resource_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_oidc_api_resource_ID_seq";
CREATE SEQUENCE "public"."tb_oidc_api_resource_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_oidc_api_scope_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_oidc_api_scope_ID_seq";
CREATE SEQUENCE "public"."tb_oidc_api_scope_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_organization_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_organization_ID_seq";
CREATE SEQUENCE "public"."tb_organization_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_organization_job_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_organization_job_ID_seq";
CREATE SEQUENCE "public"."tb_organization_job_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_position_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_position_ID_seq";
CREATE SEQUENCE "public"."tb_position_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_relation_job_user_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_relation_job_user_ID_seq";
CREATE SEQUENCE "public"."tb_relation_job_user_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_relation_organization_user_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_relation_organization_user_ID_seq";
CREATE SEQUENCE "public"."tb_relation_organization_user_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_relation_position_user_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_relation_position_user_ID_seq";
CREATE SEQUENCE "public"."tb_relation_position_user_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_relation_role_permission_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_relation_role_permission_ID_seq";
CREATE SEQUENCE "public"."tb_relation_role_permission_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_relation_role_user_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_relation_role_user_ID_seq";
CREATE SEQUENCE "public"."tb_relation_role_user_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_role_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_role_ID_seq";
CREATE SEQUENCE "public"."tb_role_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Sequence structure for tb_user_ID_seq
-- ----------------------------
DROP SEQUENCE IF EXISTS "public"."tb_user_ID_seq";
CREATE SEQUENCE "public"."tb_user_ID_seq" 
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1;

-- ----------------------------
-- Table structure for tb_application
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_application";
CREATE TABLE "public"."tb_application" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "Name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Code" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "RedirectUri" varchar(255) COLLATE "pg_catalog"."default",
  "ResponseType" int2,
  "ClientSecret" varchar(255) COLLATE "pg_catalog"."default",
  "Scope" varchar(255) COLLATE "pg_catalog"."default",
  "CallbackPath" varchar(255) COLLATE "pg_catalog"."default",
  "OidcAppType" int2 NOT NULL,
  "ModifyDate" timestamp(6) NOT NULL,
  "ModifyUser" varchar(255) COLLATE "pg_catalog"."default",
  "IsDelete" bool NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default",
  "Sort" int4 NOT NULL
)
;

-- ----------------------------
-- Records of tb_application
-- ----------------------------
INSERT INTO "public"."tb_application" VALUES (2, 'OidcAuthServer', 'OidcAuthServer', NULL, NULL, NULL, NULL, '/oidc/callback', 0, '2024-10-14 21:57:43.961476', 'Test', 'f', '2024-10-14 21:57:43.961434', 'Test', 0);
INSERT INTO "public"."tb_application" VALUES (5, 'BlazorWasmTest', 'blazor_wasm_test', '/oidc/callback', 2, NULL, NULL, '/oidc/callback', 2, '2024-10-28 22:12:19.09235', 'Test', 'f', '2024-10-28 22:12:19.092321', 'Test', 0);
INSERT INTO "public"."tb_application" VALUES (6, 'ConsoleTest', 'console_password_test', NULL, 2, 'client_password_test_client_password_test', 'openid profile scope1', NULL, 1, '2024-11-25 22:17:05.691201', 'Test', 'f', '2024-11-25 22:17:05.691166', 'Test', 0);
INSERT INTO "public"."tb_application" VALUES (7, 'JavascriptTest', 'javascript_test', 'http://localhost:5201/callback.html', 1, 'javascript_test_javascript_test', 'openid profile api.test.scope1', 'http://localhost:5201/callback.html', 2, '2025-04-05 22:51:17.102824', '8024221', 'f', '2025-04-05 22:51:17.102798', '8024221', 0);
INSERT INTO "public"."tb_application" VALUES (1, '权限管理系统', 'app_admin', '/oidc/callback', 1, 'app_admin_app_admin_app_admin', 'openid profile offline_access api.test.scope1', '/oidc/callback', 0, '2024-10-13 22:58:12.576055', 'Test', 'f', '2024-08-25 21:52:26.481026', 'Test', 0);
INSERT INTO "public"."tb_application" VALUES (3, 'OidcMvcTest', 'mvc_client', '/oidc/callback', 1, 'ClientSecretClientSecretClientSecretClientSecret', 'openid profile scope1 api.test.scope1', '/oidc/callback', 0, '2024-10-14 22:02:28.251272', 'Test', 'f', '2024-10-14 21:59:24.392921', 'Test', 0);
INSERT INTO "public"."tb_application" VALUES (4, 'BlazorServerTest', 'blazor_server_test', '/oidc/callback', 1, 'ClientSecretClientSecretClientSecretClientSecret', 'openid profile offline_access scope1 role', '/oidc/callback', 0, '2024-10-27 22:18:57.533041', 'Test', 'f', '2024-10-27 22:18:32.123816', 'Test', 0);

-- ----------------------------
-- Table structure for tb_application_permission
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_application_permission";
CREATE TABLE "public"."tb_application_permission" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "Name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Code" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "PageType" int4,
  "Url" varchar(255) COLLATE "pg_catalog"."default",
  "Icon" varchar(255) COLLATE "pg_catalog"."default",
  "Description" varchar(255) COLLATE "pg_catalog"."default",
  "ParentID" int4,
  "PermissionType" int4 NOT NULL,
  "ApplicationID" int4 NOT NULL,
  "ModifyDate" timestamp(6) NOT NULL,
  "ModifyUser" varchar(255) COLLATE "pg_catalog"."default",
  "IsDelete" bool NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default",
  "Sort" int4 NOT NULL
)
;

-- ----------------------------
-- Records of tb_application_permission
-- ----------------------------

-- ----------------------------
-- Table structure for tb_application_role
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_application_role";
CREATE TABLE "public"."tb_application_role" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "Name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Code" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "ParentID" int4,
  "Description" varchar(255) COLLATE "pg_catalog"."default",
  "ApplicationID" int4 NOT NULL,
  "ModifyDate" timestamp(6) NOT NULL,
  "ModifyUser" varchar(255) COLLATE "pg_catalog"."default",
  "IsDelete" bool NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default",
  "Sort" int4 NOT NULL
)
;

-- ----------------------------
-- Records of tb_application_role
-- ----------------------------

-- ----------------------------
-- Table structure for tb_oidc_api_resource
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_oidc_api_resource";
CREATE TABLE "public"."tb_oidc_api_resource" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "Name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Code" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "ModifyDate" timestamp(6) NOT NULL,
  "ModifyUser" varchar(255) COLLATE "pg_catalog"."default",
  "IsDelete" bool NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default",
  "Sort" int4 NOT NULL
)
;

-- ----------------------------
-- Records of tb_oidc_api_resource
-- ----------------------------
INSERT INTO "public"."tb_oidc_api_resource" VALUES (1, 'Test Api', 'api.test', '2025-03-18 20:12:17.470963', '8022101', 'f', '2025-03-18 20:12:17.470883', '8022101', 0);

-- ----------------------------
-- Table structure for tb_oidc_api_scope
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_oidc_api_scope";
CREATE TABLE "public"."tb_oidc_api_scope" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "Name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Code" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "ApiID" int4 NOT NULL,
  "Claims" varchar(255) COLLATE "pg_catalog"."default",
  "ModifyDate" timestamp(6) NOT NULL,
  "ModifyUser" varchar(255) COLLATE "pg_catalog"."default",
  "IsDelete" bool NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default",
  "Sort" int4 NOT NULL
)
;

-- ----------------------------
-- Records of tb_oidc_api_scope
-- ----------------------------
INSERT INTO "public"."tb_oidc_api_scope" VALUES (1, 'api.test.scope1', 'api.test.scope1', 1, 'role job', '2025-03-18 23:00:51.916761', '8022101', 'f', '2025-03-18 20:41:04', '1', 0);
INSERT INTO "public"."tb_oidc_api_scope" VALUES (2, 'api.test.scope2', 'api.test.scope2', 1, 'dept pos', '2025-03-18 23:01:36.879365', '8022101', 'f', '2025-03-18 20:41:50', '1', 0);

-- ----------------------------
-- Table structure for tb_organization
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_organization";
CREATE TABLE "public"."tb_organization" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "Name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "ParentID" int4,
  "OrganizationType" int4 NOT NULL,
  "Description" varchar(255) COLLATE "pg_catalog"."default",
  "ModifyDate" timestamp(6) NOT NULL,
  "ModifyUser" varchar(255) COLLATE "pg_catalog"."default",
  "IsDelete" bool NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default",
  "Sort" int4 NOT NULL,
  "Code" varchar(255) COLLATE "pg_catalog"."default" NOT NULL
)
;

-- ----------------------------
-- Records of tb_organization
-- ----------------------------

-- ----------------------------
-- Table structure for tb_organization_job
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_organization_job";
CREATE TABLE "public"."tb_organization_job" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "Name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Code" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "OrganizationID" int4 NOT NULL,
  "ModifyDate" timestamp(6) NOT NULL,
  "ModifyUser" varchar(255) COLLATE "pg_catalog"."default",
  "IsDelete" bool NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default",
  "Sort" int4 NOT NULL
)
;

-- ----------------------------
-- Records of tb_organization_job
-- ----------------------------

-- ----------------------------
-- Table structure for tb_position
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_position";
CREATE TABLE "public"."tb_position" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "Name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Code" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "ModifyDate" timestamp(6) NOT NULL,
  "ModifyUser" varchar(255) COLLATE "pg_catalog"."default",
  "IsDelete" bool NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default",
  "Sort" int4 NOT NULL,
  "ParentID" int4
)
;

-- ----------------------------
-- Records of tb_position
-- ----------------------------

-- ----------------------------
-- Table structure for tb_relation_job_user
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_relation_job_user";
CREATE TABLE "public"."tb_relation_job_user" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "JobID" int4 NOT NULL,
  "UserID" int4 NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Records of tb_relation_job_user
-- ----------------------------

-- ----------------------------
-- Table structure for tb_relation_organization_user
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_relation_organization_user";
CREATE TABLE "public"."tb_relation_organization_user" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "OrganizationID" int4 NOT NULL,
  "UserID" int4 NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Records of tb_relation_organization_user
-- ----------------------------

-- ----------------------------
-- Table structure for tb_relation_position_user
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_relation_position_user";
CREATE TABLE "public"."tb_relation_position_user" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "PositionID" int4 NOT NULL,
  "UserID" int4 NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Records of tb_relation_position_user
-- ----------------------------

-- ----------------------------
-- Table structure for tb_relation_role_permission
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_relation_role_permission";
CREATE TABLE "public"."tb_relation_role_permission" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "RoleID" int4 NOT NULL,
  "PermissionID" int4 NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Records of tb_relation_role_permission
-- ----------------------------

-- ----------------------------
-- Table structure for tb_relation_role_user
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_relation_role_user";
CREATE TABLE "public"."tb_relation_role_user" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "RoleID" int4 NOT NULL,
  "UserID" int4 NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Records of tb_relation_role_user
-- ----------------------------

-- ----------------------------
-- Table structure for tb_role
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_role";
CREATE TABLE "public"."tb_role" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "Name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Code" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "ParentID" int4,
  "Description" varchar(255) COLLATE "pg_catalog"."default",
  "ModifyDate" timestamp(6) NOT NULL,
  "ModifyUser" varchar(255) COLLATE "pg_catalog"."default",
  "IsDelete" bool NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default",
  "Sort" int4 NOT NULL
)
;

-- ----------------------------
-- Records of tb_role
-- ----------------------------

-- ----------------------------
-- Table structure for tb_user
-- ----------------------------
DROP TABLE IF EXISTS "public"."tb_user";
CREATE TABLE "public"."tb_user" (
  "ID" int4 NOT NULL GENERATED BY DEFAULT AS IDENTITY (
INCREMENT 1
MINVALUE  1
MAXVALUE 2147483647
START 1
CACHE 1
),
  "Name" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Code" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Password" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "Email" varchar(255) COLLATE "pg_catalog"."default",
  "Gender" int8 NOT NULL,
  "Age" int4,
  "EntryDate" timestamp(6),
  "ModifyDate" timestamp(6) NOT NULL,
  "ModifyUser" varchar(255) COLLATE "pg_catalog"."default",
  "IsDelete" bool NOT NULL,
  "AddDate" timestamp(6) NOT NULL,
  "AddUser" varchar(255) COLLATE "pg_catalog"."default",
  "Sort" int4 NOT NULL
)
;

-- ----------------------------
-- Records of tb_user
-- ----------------------------
INSERT INTO "public"."tb_user" VALUES (9, '管理员', 'admin', '8BF7365FA1DE8F5D81536BEC2F0D78E5', NULL, 0, NULL, NULL, '2025-05-14 21:33:15.489709', NULL, 'f', '2025-05-14 21:33:15.489673', NULL, 0);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_application_ID_seq"
OWNED BY "public"."tb_application"."ID";
SELECT setval('"public"."tb_application_ID_seq"', 7, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_application_permission_ID_seq"
OWNED BY "public"."tb_application_permission"."ID";
SELECT setval('"public"."tb_application_permission_ID_seq"', 5, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_application_role_ID_seq"
OWNED BY "public"."tb_application_role"."ID";
SELECT setval('"public"."tb_application_role_ID_seq"', 4, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_oidc_api_resource_ID_seq"
OWNED BY "public"."tb_oidc_api_resource"."ID";
SELECT setval('"public"."tb_oidc_api_resource_ID_seq"', 1, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_oidc_api_scope_ID_seq"
OWNED BY "public"."tb_oidc_api_scope"."ID";
SELECT setval('"public"."tb_oidc_api_scope_ID_seq"', 1, false);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_organization_ID_seq"
OWNED BY "public"."tb_organization"."ID";
SELECT setval('"public"."tb_organization_ID_seq"', 21, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_organization_job_ID_seq"
OWNED BY "public"."tb_organization_job"."ID";
SELECT setval('"public"."tb_organization_job_ID_seq"', 7, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_position_ID_seq"
OWNED BY "public"."tb_position"."ID";
SELECT setval('"public"."tb_position_ID_seq"', 12, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_relation_job_user_ID_seq"
OWNED BY "public"."tb_relation_job_user"."ID";
SELECT setval('"public"."tb_relation_job_user_ID_seq"', 8, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_relation_organization_user_ID_seq"
OWNED BY "public"."tb_relation_organization_user"."ID";
SELECT setval('"public"."tb_relation_organization_user_ID_seq"', 21, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_relation_position_user_ID_seq"
OWNED BY "public"."tb_relation_position_user"."ID";
SELECT setval('"public"."tb_relation_position_user_ID_seq"', 15, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_relation_role_permission_ID_seq"
OWNED BY "public"."tb_relation_role_permission"."ID";
SELECT setval('"public"."tb_relation_role_permission_ID_seq"', 14, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_relation_role_user_ID_seq"
OWNED BY "public"."tb_relation_role_user"."ID";
SELECT setval('"public"."tb_relation_role_user_ID_seq"', 17, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_role_ID_seq"
OWNED BY "public"."tb_role"."ID";
SELECT setval('"public"."tb_role_ID_seq"', 2, true);

-- ----------------------------
-- Alter sequences owned by
-- ----------------------------
ALTER SEQUENCE "public"."tb_user_ID_seq"
OWNED BY "public"."tb_user"."ID";
SELECT setval('"public"."tb_user_ID_seq"', 9, true);

-- ----------------------------
-- Auto increment value for tb_application
-- ----------------------------
SELECT setval('"public"."tb_application_ID_seq"', 7, true);

-- ----------------------------
-- Indexes structure for table tb_application
-- ----------------------------
CREATE UNIQUE INDEX "uq_code" ON "public"."tb_application" USING btree (
  "Code" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table tb_application
-- ----------------------------
ALTER TABLE "public"."tb_application" ADD CONSTRAINT "public_tb_application_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_application_permission
-- ----------------------------
SELECT setval('"public"."tb_application_permission_ID_seq"', 5, true);

-- ----------------------------
-- Primary Key structure for table tb_application_permission
-- ----------------------------
ALTER TABLE "public"."tb_application_permission" ADD CONSTRAINT "public_tb_application_permission_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_application_role
-- ----------------------------
SELECT setval('"public"."tb_application_role_ID_seq"', 4, true);

-- ----------------------------
-- Primary Key structure for table tb_application_role
-- ----------------------------
ALTER TABLE "public"."tb_application_role" ADD CONSTRAINT "public_tb_application_role_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_oidc_api_resource
-- ----------------------------
SELECT setval('"public"."tb_oidc_api_resource_ID_seq"', 1, true);

-- ----------------------------
-- Primary Key structure for table tb_oidc_api_resource
-- ----------------------------
ALTER TABLE "public"."tb_oidc_api_resource" ADD CONSTRAINT "public_tb_oidc_api_resource_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_oidc_api_scope
-- ----------------------------
SELECT setval('"public"."tb_oidc_api_scope_ID_seq"', 1, false);

-- ----------------------------
-- Primary Key structure for table tb_oidc_api_scope
-- ----------------------------
ALTER TABLE "public"."tb_oidc_api_scope" ADD CONSTRAINT "public_tb_oidc_api_scope_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_organization
-- ----------------------------
SELECT setval('"public"."tb_organization_ID_seq"', 21, true);

-- ----------------------------
-- Primary Key structure for table tb_organization
-- ----------------------------
ALTER TABLE "public"."tb_organization" ADD CONSTRAINT "public_tb_organization_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_organization_job
-- ----------------------------
SELECT setval('"public"."tb_organization_job_ID_seq"', 7, true);

-- ----------------------------
-- Primary Key structure for table tb_organization_job
-- ----------------------------
ALTER TABLE "public"."tb_organization_job" ADD CONSTRAINT "public_tb_organization_job_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_position
-- ----------------------------
SELECT setval('"public"."tb_position_ID_seq"', 12, true);

-- ----------------------------
-- Primary Key structure for table tb_position
-- ----------------------------
ALTER TABLE "public"."tb_position" ADD CONSTRAINT "public_tb_position_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_relation_job_user
-- ----------------------------
SELECT setval('"public"."tb_relation_job_user_ID_seq"', 8, true);

-- ----------------------------
-- Primary Key structure for table tb_relation_job_user
-- ----------------------------
ALTER TABLE "public"."tb_relation_job_user" ADD CONSTRAINT "public_tb_relation_job_user_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_relation_organization_user
-- ----------------------------
SELECT setval('"public"."tb_relation_organization_user_ID_seq"', 21, true);

-- ----------------------------
-- Primary Key structure for table tb_relation_organization_user
-- ----------------------------
ALTER TABLE "public"."tb_relation_organization_user" ADD CONSTRAINT "public_tb_relation_organization_user_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_relation_position_user
-- ----------------------------
SELECT setval('"public"."tb_relation_position_user_ID_seq"', 15, true);

-- ----------------------------
-- Primary Key structure for table tb_relation_position_user
-- ----------------------------
ALTER TABLE "public"."tb_relation_position_user" ADD CONSTRAINT "public_tb_relation_position_user_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_relation_role_permission
-- ----------------------------
SELECT setval('"public"."tb_relation_role_permission_ID_seq"', 14, true);

-- ----------------------------
-- Indexes structure for table tb_relation_role_permission
-- ----------------------------
CREATE INDEX "idx_role_permission" ON "public"."tb_relation_role_permission" USING btree (
  "RoleID" "pg_catalog"."int4_ops" ASC NULLS LAST,
  "PermissionID" "pg_catalog"."int4_ops" ASC NULLS LAST
);

-- ----------------------------
-- Uniques structure for table tb_relation_role_permission
-- ----------------------------
ALTER TABLE "public"."tb_relation_role_permission" ADD CONSTRAINT "uq_role_permission" UNIQUE ("RoleID", "PermissionID");

-- ----------------------------
-- Primary Key structure for table tb_relation_role_permission
-- ----------------------------
ALTER TABLE "public"."tb_relation_role_permission" ADD CONSTRAINT "public_tb_relation_role_permission_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_relation_role_user
-- ----------------------------
SELECT setval('"public"."tb_relation_role_user_ID_seq"', 17, true);

-- ----------------------------
-- Primary Key structure for table tb_relation_role_user
-- ----------------------------
ALTER TABLE "public"."tb_relation_role_user" ADD CONSTRAINT "public_tb_relation_role_user_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_role
-- ----------------------------
SELECT setval('"public"."tb_role_ID_seq"', 2, true);

-- ----------------------------
-- Primary Key structure for table tb_role
-- ----------------------------
ALTER TABLE "public"."tb_role" ADD CONSTRAINT "public_tb_role_pkey" PRIMARY KEY ("ID");

-- ----------------------------
-- Auto increment value for tb_user
-- ----------------------------
SELECT setval('"public"."tb_user_ID_seq"', 9, true);

-- ----------------------------
-- Primary Key structure for table tb_user
-- ----------------------------
ALTER TABLE "public"."tb_user" ADD CONSTRAINT "public_tb_user_pkey" PRIMARY KEY ("ID");
