/**
 * Production Environment Configuration
 * Think of this as the "main gym" settings - optimized for production
 * with security, performance, and reliability as top priorities.
 */
export const environment = {
  production: true,
  apiUrl: 'https://api.fitnessvibe.com/api',
  appName: 'FitnessVibe',
  version: '1.0.0',
  
  // Feature flags for production
  features: {
    enableDevTools: false,
    enableMockData: false,
    enableOfflineMode: true,
    enablePushNotifications: true,
    enableAnalytics: true,
    enableBetaFeatures: false
  },
  
  // API configuration (production optimized)
  api: {
    timeout: 30000, // 30 seconds
    retryAttempts: 3,
    retryDelay: 2000
  },
  
  // Authentication settings (production security)
  auth: {
    tokenRefreshThreshold: 300000, // 5 minutes in milliseconds
    sessionTimeout: 28800000, // 8 hours in milliseconds
    maxLoginAttempts: 3,
    lockoutDuration: 1800000 // 30 minutes in milliseconds
  },
  
  // Logging configuration (production)
  logging: {
    level: 'error',
    enableConsoleLogging: false,
    enableRemoteLogging: true
  },
  
  // Push notification settings (production)
  pushNotifications: {
    vapidKey: 'prod-vapid-key-here',
    enableInDevelopment: false
  },
  
  // Analytics settings (production)
  analytics: {
    googleAnalyticsId: 'GA-XXXXXXXXX',
    enableTracking: true
  },
  
  // Map integration (production keys)
  maps: {
    googleMapsApiKey: 'prod-google-maps-key',
    mapboxToken: 'prod-mapbox-token'
  },
  
  // Social media integration (production)
  social: {
    facebookAppId: 'prod-facebook-app-id',
    twitterApiKey: 'prod-twitter-api-key',
    instagramClientId: 'prod-instagram-client-id'
  },
  
  // Production-specific settings
  production_config: {
    enableServiceWorker: true,
    enableCompression: true,
    enableCaching: true,
    cdnUrl: 'https://cdn.fitnessvibe.com',
    errorReportingUrl: 'https://errors.fitnessvibe.com'
  }
};
