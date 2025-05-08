# Rubik.Stack

##### Organizational structure management system & Oidc single sign-on service (written with reference to OAuth2.0 & Oidc official document process, not all functions are implemented，onle code&implicit&password flow implemented)
##### The project framework mainly uses Blazor App, UI: AntDesign, Orm: Freesql, Database: PgSql
## Directory structure

+ identity

+ + Rubik.Identity.Admin: Organizational structure management system, project startup initialization database
  + Rubik.Identity.AuthServer: Single sign-on service Mvc Host
  + Rubik.Identity.Oidc.Core: Single sign-on core process
  + Rubik.Identity.OidcReferenceAuthentication: Jwt component token authentication process access Oidc expansion package
  + Rubik.Identity.Share.Entity: Organizational structure database mapping entity

  
+ identity_test
+ + BlazorServerTest: Blazor App access oidc server & request JwtTest api test project
  + BlazorWasmTest: Blazor Wasm access oidc server, not implemented
  + JwtTest: JwtAuthentication Connect to oidc server for testing
  + MvcClient: AddOpenIdConnect Connect to oidc server for testing
  + infrastructure Encapsulation of some common class libraries
  + VueTsTest： vue3&ts use oidc-client-ts for testing

+ lowcode Low-code platform, no use

-------------------------------------------------------------------------------------------

##### 组织架构管理系统&Oidc单点登录服务（参考OAuth2.0&Oidc官方文档流程编写，未实现全部功能,仅实现了code,implicit,password模式）
##### 项目框架主要使用Blazor App，UI:AntDesign, Orm：Freesql，Database：PgSql
##目录结构
+ identity
+ + Rubik.Identity.Admin：组织架构管理系统，项目启动初始化数据库
  + Rubik.Identity.AuthServer: 单点登录服务Mvc Host
  + Rubik.Identity.Oidc.Core: 单点登录核心流程
  + Rubik.Identity.OidcReferenceAuthentication： Jwt组件token认证流程接入Oidc的拓展包
  + Rubik.Identity.Share.Entity：组织架构数据库映射实体
  
+ identity_test
+ + BlazorServerTest: Blazor App接入oidc server & 请求 JwtTest api  测试项目
  + BlazorWasmTest： Blazor Wasm接入oidc server，未实现
  + JwtTest：JwtAuthentication 接入oidc server测试
  + MvcClient：AddOpenIdConnect 接入oidc server测试
  + VueTsTest：vue 3使用oidc-client-ts接入oidc server code模式测试
 

+ infrastructure 一些通用类库的封装


+ lowcode 低代码平台，没搞头
