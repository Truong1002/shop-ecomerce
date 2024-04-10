import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'ShopEcommerce',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:44343/',
    redirectUri: baseUrl,
    clientId: 'ShopEcommerce_App',
    responseType: 'code',
    scope: 'offline_access ShopEcommerce',
    requireHttps: true
  },
  apis: {
    default: {
      url: 'https://localhost:44389',
      rootNamespace: 'ShopEcommerce',
    },
  },
} as Environment;
