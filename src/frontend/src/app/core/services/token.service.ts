import { Injectable } from '@angular/core';

/**
 * Token Service - the secure vault for managing authentication tokens.
 * Think of this as a high-security locker system that stores and manages
 * your digital gym access cards (JWT tokens) safely.
 */
@Injectable({
  providedIn: 'root'
})
export class TokenService {
  private readonly ACCESS_TOKEN_KEY = 'fv_access_token';
  private readonly REFRESH_TOKEN_KEY = 'fv_refresh_token';
  private readonly TOKEN_EXPIRY_KEY = 'fv_token_expiry';

  constructor() {}

  /**
   * Store authentication tokens securely.
   * Like putting your access cards in a secure locker.
   */
  setTokens(accessToken: string, refreshToken: string): void {
    try {
      // Store in localStorage for persistence across browser sessions
      localStorage.setItem(this.ACCESS_TOKEN_KEY, accessToken);
      localStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
      
      // Decode and store token expiry for convenience
      const tokenPayload = this.decodeToken(accessToken);
      if (tokenPayload?.exp) {
        localStorage.setItem(this.TOKEN_EXPIRY_KEY, tokenPayload.exp.toString());
      }
    } catch (error) {
      console.error('Error storing tokens:', error);
      // Fallback to sessionStorage if localStorage fails
      this.setTokensInSessionStorage(accessToken, refreshToken);
    }
  }

  /**
   * Retrieve the access token.
   * Like getting your current access card from the locker.
   */
  getAccessToken(): string | null {
    try {
      return localStorage.getItem(this.ACCESS_TOKEN_KEY) || 
             sessionStorage.getItem(this.ACCESS_TOKEN_KEY);
    } catch (error) {
      console.error('Error retrieving access token:', error);
      return null;
    }
  }

  /**
   * Retrieve the refresh token.
   * Like getting your backup access card from the locker.
   */
  getRefreshToken(): string | null {
    try {
      return localStorage.getItem(this.REFRESH_TOKEN_KEY) || 
             sessionStorage.getItem(this.REFRESH_TOKEN_KEY);
    } catch (error) {
      console.error('Error retrieving refresh token:', error);
      return null;
    }
  }

  /**
   * Clear all stored tokens.
   * Like cleaning out your locker when you cancel your membership.
   */
  clearTokens(): void {
    try {
      localStorage.removeItem(this.ACCESS_TOKEN_KEY);
      localStorage.removeItem(this.REFRESH_TOKEN_KEY);
      localStorage.removeItem(this.TOKEN_EXPIRY_KEY);
      
      sessionStorage.removeItem(this.ACCESS_TOKEN_KEY);
      sessionStorage.removeItem(this.REFRESH_TOKEN_KEY);
      sessionStorage.removeItem(this.TOKEN_EXPIRY_KEY);
    } catch (error) {
      console.error('Error clearing tokens:', error);
    }
  }

  /**
   * Check if access token exists and is potentially valid.
   * Like checking if you have an access card in your wallet.
   */
  hasValidToken(): boolean {
    const token = this.getAccessToken();
    if (!token) return false;

    // Check if token is expired
    return !this.isTokenExpired(token);
  }

  /**
   * Check if the access token is expired.
   * Like checking if your gym membership has expired.
   */
  isTokenExpired(token?: string): boolean {
    const accessToken = token || this.getAccessToken();
    if (!accessToken) return true;

    try {
      const payload = this.decodeToken(accessToken);
      if (!payload?.exp) return true;

      // Check if token is expired (with 30 second buffer)
      const currentTime = Math.floor(Date.now() / 1000);
      return payload.exp < (currentTime + 30);
    } catch (error) {
      console.error('Error checking token expiry:', error);
      return true;
    }
  }

  /**
   * Get token expiry date.
   * Like checking when your membership expires.
   */
  getTokenExpiryDate(token?: string): Date | null {
    const accessToken = token || this.getAccessToken();
    if (!accessToken) return null;

    try {
      const payload = this.decodeToken(accessToken);
      if (!payload?.exp) return null;

      return new Date(payload.exp * 1000);
    } catch (error) {
      console.error('Error getting token expiry date:', error);
      return null;
    }
  }

  /**
   * Get user ID from token.
   * Like reading the member ID from your access card.
   */
  getUserIdFromToken(token?: string): number | null {
    const accessToken = token || this.getAccessToken();
    if (!accessToken) return null;

    try {
      const payload = this.decodeToken(accessToken);
      return payload?.sub ? parseInt(payload.sub, 10) : null;
    } catch (error) {
      console.error('Error getting user ID from token:', error);
      return null;
    }
  }

  /**
   * Get user roles from token.
   * Like checking what areas of the gym you have access to.
   */
  getUserRolesFromToken(token?: string): string[] {
    const accessToken = token || this.getAccessToken();
    if (!accessToken) return [];

    try {
      const payload = this.decodeToken(accessToken);
      return payload?.roles || [];
    } catch (error) {
      console.error('Error getting user roles from token:', error);
      return [];
    }
  }

  /**
   * Check if user has specific role.
   * Like checking if you have premium membership access.
   */
  hasRole(role: string, token?: string): boolean {
    const roles = this.getUserRolesFromToken(token);
    return roles.includes(role);
  }

  /**
   * Get time until token expires (in milliseconds).
   * Like checking how much time is left on your day pass.
   */
  getTimeUntilExpiry(token?: string): number {
    const expiryDate = this.getTokenExpiryDate(token);
    if (!expiryDate) return 0;

    return Math.max(0, expiryDate.getTime() - Date.now());
  }

  /**
   * Check if token needs refresh (within 5 minutes of expiry).
   * Like checking if you need to renew your membership soon.
   */
  needsRefresh(token?: string): boolean {
    const timeUntilExpiry = this.getTimeUntilExpiry(token);
    return timeUntilExpiry > 0 && timeUntilExpiry < (5 * 60 * 1000); // 5 minutes
  }

  /**
   * Decode JWT token payload.
   * Like reading the information stored on your access card.
   */
  private decodeToken(token: string): any {
    try {
      // JWT tokens have 3 parts separated by dots: header.payload.signature
      const parts = token.split('.');
      if (parts.length !== 3) {
        throw new Error('Invalid token format');
      }

      // Decode the payload (second part)
      const payload = parts[1];
      
      // Add padding if needed (base64 requires length to be multiple of 4)
      const paddedPayload = payload.padEnd(payload.length + (4 - payload.length % 4) % 4, '=');
      
      // Decode base64 and parse JSON
      const decodedPayload = atob(paddedPayload);
      return JSON.parse(decodedPayload);
    } catch (error) {
      console.error('Error decoding token:', error);
      return null;
    }
  }

  /**
   * Fallback method to store tokens in sessionStorage.
   * Like using a temporary locker when the main one isn't available.
   */
  private setTokensInSessionStorage(accessToken: string, refreshToken: string): void {
    try {
      sessionStorage.setItem(this.ACCESS_TOKEN_KEY, accessToken);
      sessionStorage.setItem(this.REFRESH_TOKEN_KEY, refreshToken);
      
      const tokenPayload = this.decodeToken(accessToken);
      if (tokenPayload?.exp) {
        sessionStorage.setItem(this.TOKEN_EXPIRY_KEY, tokenPayload.exp.toString());
      }
    } catch (error) {
      console.error('Error storing tokens in sessionStorage:', error);
    }
  }

  /**
   * Clear expired tokens automatically.
   * Like automatically cleaning out old access cards.
   */
  clearExpiredTokens(): void {
    if (this.isTokenExpired()) {
      this.clearTokens();
    }
  }

  /**
   * Get token information for debugging.
   * Like getting a detailed report about your access card status.
   */
  getTokenInfo(): {
    hasToken: boolean;
    isExpired: boolean;
    expiryDate: Date | null;
    userId: number | null;
    roles: string[];
    timeUntilExpiry: number;
    needsRefresh: boolean;
  } {
    const token = this.getAccessToken();
    
    return {
      hasToken: !!token,
      isExpired: this.isTokenExpired(token),
      expiryDate: this.getTokenExpiryDate(token),
      userId: this.getUserIdFromToken(token),
      roles: this.getUserRolesFromToken(token),
      timeUntilExpiry: this.getTimeUntilExpiry(token),
      needsRefresh: this.needsRefresh(token)
    };
  }
}
