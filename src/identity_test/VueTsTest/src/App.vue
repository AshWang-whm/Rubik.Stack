<template>
  <div>
    <button @click="login" v-if="!isLoggedIn">Login with OpenID Connect</button>
    <div v-else>
      <p>Welcome, {{ userInfo?.profile?.name }}</p>
      <button @click="logout">Logout</button>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue';
  import { UserManager } from 'oidc-client-ts';

  const isLoggedIn = ref(false);
  const userInfo = ref(null);

  const settings = {
    authority: 'http://localhost:5000', // 替换为实际的授权服务器地址
    client_id: 'javascript_test', // 替换为实际的客户端 ID
    redirect_uri: 'http://localhost:5201/callback', // 替换为实际的回调地址
    response_type: 'code',
    scope: 'openid profile api.test.scope1',
    post_logout_redirect_uri: 'http://localhost:5201/' // 替换为实际的登出后重定向地址
  };

  const userManager = new UserManager(settings);

  const login = async () => {
    await userManager.signinRedirect();
  };

  const logout = async () => {
    await userManager.signoutRedirect();
    isLoggedIn.value = false;
    userInfo.value = null;
  };

  onMounted(async () => {
    try {
      const user = await userManager.getUser();
      console.log(user);
      if (user) {
        isLoggedIn.value = true;
        userInfo.value = user;
      }

      const url = window.location.href;
      if (url.includes(settings.redirect_uri) && url.includes('code=')) {
        const user = await userManager.signinRedirectCallback();
        isLoggedIn.value = true;
        userInfo.value = user;
      }
    } catch (error) {
      console.error('Error during OIDC process:', error);
    }
  });
</script>    
