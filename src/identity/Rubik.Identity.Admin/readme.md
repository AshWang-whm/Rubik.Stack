code: client{scope(openid,profile,*identityscope(内部应用，默认输出用户信息？),*apiscoe(根据scope 查询所需要aud的值)),clientid,redirect_uri,response_type,secret}


Discovery Doc



Scope:不考虑App之外的Scope配置，默认返回内部全部的用户信息内容+scope mapping APP：API，设置aud claim
ScopeEntity
1.Identity Scope(用户信息)  2.Api Resource Scope(api资源)



Admin 实体:
1.TbApp，应用管理
{
	Oidc配置()，
	权限管理配置(应用名称，类型:客户端/API,ResponseType:Code/Token/IdToken/TokenIdToken)
}

2.TbUser，用户管理

3.TbAppRole，应用角色管理

4.TbOrganization，组织架构管理

5.TbAppPermission，应用权限管理

6.TbPostion, 岗位

7.RelationDeptPost, 部门&岗位关系

6.RelationRolePermission，角色&权限关系

7.RelationPostUser，部门岗位&用户关系

8.RelationRoleUser，角色&用户关系

9.RelationAppPermission，应用&权限关系

11.RelationAppRole，应用&角色关系

10.RelationDeptApp*


RBAC流程：
1.添加部门/组织，添加用户，添加岗位，
2.设置部门&用户关系，设置部门&岗位关系，岗位&用户关系

3.添加应用，添加权限，添加角色
4.设置应用&角色关系，设置应用&权限关系，设置应用的角色&应用关系

5.设置用户&应用角色关系

6.用户登录，通过clientid，*scope(api应用)，获取client&api的应用信息，用户信息，用户&应用&角色信息，token中携带用户角色信息

7.菜单接口：验证token，获取用户id，clientid(app) => 查询到用户角色&菜单权限 => 返回菜单信息

