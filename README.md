# Rubik.Stack
##### 组织架构管理系统&Oidc单点登录服务（参考OAuth2.0&Oidc官方文档流程编写，未实现全部功能）
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
 

+ infrastructure 一些通用类库的封装


+ lowcode 低代码平台，没搞头
