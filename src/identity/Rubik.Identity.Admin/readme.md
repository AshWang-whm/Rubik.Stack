code: client{scope(openid,profile,*identityscope(�ڲ�Ӧ�ã�Ĭ������û���Ϣ��),*apiscoe(����scope ��ѯ����Ҫaud��ֵ)),clientid,redirect_uri,response_type,secret}


Discovery Doc



Scope:������App֮���Scope���ã�Ĭ�Ϸ����ڲ�ȫ�����û���Ϣ����+scope mapping APP��API������aud claim
ScopeEntity
1.Identity Scope(�û���Ϣ)  2.Api Resource Scope(api��Դ)



Admin ʵ��:
1.TbApp��Ӧ�ù���
{
	Oidc����()��
	Ȩ�޹�������(Ӧ�����ƣ�����:�ͻ���/API,ResponseType:Code/Token/IdToken/TokenIdToken)
}

2.TbUser���û�����

3.TbAppRole��Ӧ�ý�ɫ����

4.TbOrganization����֯�ܹ�����

5.TbAppPermission��Ӧ��Ȩ�޹���

6.TbPostion, ��λ

7.RelationDeptPost, ����&��λ��ϵ

6.RelationRolePermission����ɫ&Ȩ�޹�ϵ

7.RelationPostUser�����Ÿ�λ&�û���ϵ

8.RelationRoleUser����ɫ&�û���ϵ

9.RelationAppPermission��Ӧ��&Ȩ�޹�ϵ

11.RelationAppRole��Ӧ��&��ɫ��ϵ

10.RelationDeptApp*


RBAC���̣�
1.��Ӳ���/��֯������û�����Ӹ�λ��
2.���ò���&�û���ϵ�����ò���&��λ��ϵ����λ&�û���ϵ

3.���Ӧ�ã����Ȩ�ޣ���ӽ�ɫ
4.����Ӧ��&��ɫ��ϵ������Ӧ��&Ȩ�޹�ϵ������Ӧ�õĽ�ɫ&Ӧ�ù�ϵ

5.�����û�&Ӧ�ý�ɫ��ϵ

6.�û���¼��ͨ��clientid��*scope(apiӦ��)����ȡclient&api��Ӧ����Ϣ���û���Ϣ���û�&Ӧ��&��ɫ��Ϣ��token��Я���û���ɫ��Ϣ

7.�˵��ӿڣ���֤token����ȡ�û�id��clientid(app) => ��ѯ���û���ɫ&�˵�Ȩ�� => ���ز˵���Ϣ

