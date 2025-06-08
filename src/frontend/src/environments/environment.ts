/**
 * Development Environment Configuration
 * Think of this as the "test gym" settings - optimized for development
 * with debugging tools enabled and development-friendly configurations.
 */
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api',
  appName: 'FitnessVibe Dev',
  version: '1.0.0-dev',
  
  // Feature flags for development
  features: {
    enableDevTools: true,
    enableMockData: false,
    enableOfflineMode: true,
    enablePushNotifications: false,
    enableAnalytics: false,
    enableBetaFeatures: true
  },
  
  // API configuration
  api: {
    timeout: 10000, // 10 seconds
    retryAttempts: 3,
    retryDelay: 1000
  },
  
  // Authentication settings
  auth: {
    tokenRefreshThreshold: 300000, // 5 minutes in milliseconds
    sessionTimeout: 86400000, // 24 hours in milliseconds
    maxLoginAttempts: 5,
    lockoutDuration: 900000 // 15 minutes in milliseconds
  },
  
  // Logging configuration
  logging: {
    level: 'debug',
    enableConsoleLogging: true,
    enableRemoteLogging: false
  },
  
  // Push notification settings (development)
  pushNotifications: {
    vapidKey: 'dev-vapid-key-here',
    enableInDevelopment: false
  },
  
  // Analytics settings (disabled in dev)
  analytics: {
    googleAnalyticsId: '',
    enableTracking: false
  },
  
  // Map integration (development keys)
  maps: {
    googleMapsApiKey: 'dev-google-maps-key',
    mapboxToken: 'dev-mapbox-token'
  },
  
  // Social media integration (dev)
  social: {
    facebookAppId: 'dev-facebook-app-id',
    twitterApiKey: 'dev-twitter-api-key',
    instagramClientId: 'dev-instagram-client-id'
  },
  
  // Development-specific settings
  development: {
    enableHotReload: true,
    showPerformanceMetrics: true,
    enableDebugInfo: true,
    mockApiDelay: 500, // Simulate network delay
    autoLogin: false, // Set to true for automatic login during development
    defaultUser: {
      email: 'dev@fitnessvibe.com',
      password: 'Dev123!'
    }
  }
};
