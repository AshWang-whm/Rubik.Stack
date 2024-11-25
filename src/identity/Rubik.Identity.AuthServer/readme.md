# TODO List
1.auth server项目 refresh token 流程. 完成但未测试
2.auth server项目 对接rsa key 和无rsa key方案的实现
2.Rubik.Infrastructure.RequestClient 对接refresh token流程
3.wasm 接入auth server
4.js 接入auth server
5.client credential 模式
6.规范oauth 异常返回信息
{
  "error": "invalid_request",
  "error_description": "Request was missing parameter."
}




刷新Refresh Token的具体步骤
‌发送POST请求‌：向Token Endpoint发送POST请求，包含以下参数：

grant_type：设置为refresh_token
refresh_token：提供当前的Refresh Token
client_id：客户端的标识符
client_secret：客户端的秘密密钥（通常在服务器端配置）
redirect_uri：授权码获取时的回调URL