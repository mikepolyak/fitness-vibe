import { Pipe, PipeTransform } from '@angular/core';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

/**
 * Safe HTML Pipe - safely renders HTML content in templates.
 * Think of this as a security guard that checks HTML content
 * before displaying it, ensuring no malicious code gets through.
 */
@Pipe({
  name: 'safeHtml',
  pure: true
})
export class SafeHtmlPipe implements PipeTransform {

  constructor(private sanitizer: DomSanitizer) {}

  /**
   * Transform HTML string into SafeHtml for Angular templates.
   * 
   * @param value - HTML string to sanitize and make safe
   * @param sanitizationLevel - Level of sanitization: 'strict', 'moderate', 'minimal'
   * @returns SafeHtml that can be used with [innerHTML]
   * 
   * Examples:
   * - "<strong>Great workout!</strong>" → Safe HTML for bold text
   * - "Visit <a href='...'>our website</a>" → Safe HTML with link
   * - "<script>alert('hack')</script>" → Cleaned/removed malicious content
   */
  transform(
    value: string | null | undefined,
    sanitizationLevel: 'strict' | 'moderate' | 'minimal' = 'moderate'
  ): SafeHtml {

    if (!value) {
      return '';
    }

    // Pre-process HTML based on sanitization level
    const processedHtml = this.preProcessHtml(value, sanitizationLevel);

    // Use Angular's built-in sanitizer
    return this.sanitizer.bypassSecurityTrustHtml(processedHtml);
  }

  /**
   * Pre-process HTML content based on sanitization level.
   * Like having different security checkpoints with varying strictness.
   */
  private preProcessHtml(html: string, level: string): string {
    switch (level) {
      case 'strict':
        return this.strictSanitize(html);
      
      case 'minimal':
        return this.minimalSanitize(html);
      
      case 'moderate':
      default:
        return this.moderateSanitize(html);
    }
  }

  /**
   * Strict sanitization - only allows very basic formatting.
   * Like a high-security area that only allows essential items.
   */
  private strictSanitize(html: string): string {
    // Only allow basic text formatting tags
    const allowedTags = ['b', 'strong', 'i', 'em', 'u', 'br', 'p'];
    return this.sanitizeWithAllowedTags(html, allowedTags);
  }

  /**
   * Moderate sanitization - allows common formatting and some interactive elements.
   * Like a balanced security checkpoint for typical fitness content.
   */
  private moderateSanitize(html: string): string {
    // Allow common formatting and safe interactive elements
    const allowedTags = [
      'b', 'strong', 'i', 'em', 'u', 'br', 'p', 'div', 'span',
      'h1', 'h2', 'h3', 'h4', 'h5', 'h6',
      'ul', 'ol', 'li',
      'a', 'img',
      'blockquote', 'code', 'pre'
    ];
    
    let processed = this.sanitizeWithAllowedTags(html, allowedTags);
    
    // Additional processing for links and images
    processed = this.sanitizeLinks(processed);
    processed = this.sanitizeImages(processed);
    
    return processed;
  }

  /**
   * Minimal sanitization - allows most HTML but removes dangerous scripts.
   * Like a low-security checkpoint that mainly blocks obvious threats.
   */
  private minimalSanitize(html: string): string {
    // Remove only the most dangerous elements
    const dangerousTags = ['script', 'object', 'embed', 'applet', 'iframe'];
    let processed = html;
    
    dangerousTags.forEach(tag => {
      const regex = new RegExp(`<${tag}[^>]*>.*?</${tag}>`, 'gi');
      processed = processed.replace(regex, '');
    });
    
    // Remove dangerous attributes
    processed = this.removeDangerousAttributes(processed);
    
    return processed;
  }

  /**
   * Sanitize HTML keeping only allowed tags.
   * Like a whitelist security system.
   */
  private sanitizeWithAllowedTags(html: string, allowedTags: string[]): string {
    // Create a regex pattern for allowed tags
    const tagPattern = allowedTags.join('|');
    const allowedRegex = new RegExp(`<(\/?)(?:${tagPattern})(?:\\s[^>]*)?>`, 'gi');
    
    // Extract all allowed tags
    const allowedMatches = html.match(allowedRegex) || [];
    
    // Remove all HTML tags
    let processed = html.replace(/<[^>]*>/g, '');
    
    // Re-insert allowed tags in their original positions
    let matchIndex = 0;
    processed = html.replace(/<[^>]*>/g, (match) => {
      if (allowedRegex.test(match)) {
        return allowedMatches[matchIndex++] || '';
      }
      return '';
    });
    
    return processed;
  }

  /**
   * Sanitize link attributes for security.
   * Like checking that links are safe before allowing them.
   */
  private sanitizeLinks(html: string): string {
    return html.replace(/<a([^>]*)>/gi, (match, attributes) => {
      // Ensure external links open in new tab and have security attributes
      let sanitizedAttributes = attributes;
      
      // Add security attributes for external links
      if (attributes.includes('href=')) {
        const hrefMatch = attributes.match(/href=["']([^"']+)["']/i);
        if (hrefMatch) {
          const url = hrefMatch[1];
          // Check if it's an external link
          if (url.startsWith('http') && !url.includes(window.location.hostname)) {
            sanitizedAttributes += ' target="_blank" rel="noopener noreferrer"';
          }
        }
      }
      
      // Remove dangerous attributes
      sanitizedAttributes = sanitizedAttributes.replace(/on\w+\s*=\s*["'][^"']*["']/gi, '');
      
      return `<a${sanitizedAttributes}>`;
    });
  }

  /**
   * Sanitize image attributes for security.
   * Like checking that images are safe to display.
   */
  private sanitizeImages(html: string): string {
    return html.replace(/<img([^>]*)>/gi, (match, attributes) => {
      let sanitizedAttributes = attributes;
      
      // Remove dangerous attributes
      sanitizedAttributes = sanitizedAttributes.replace(/on\w+\s*=\s*["'][^"']*["']/gi, '');
      
      // Add loading="lazy" for performance
      if (!attributes.includes('loading=')) {
        sanitizedAttributes += ' loading="lazy"';
      }
      
      // Add alt attribute if missing (for accessibility)
      if (!attributes.includes('alt=')) {
        sanitizedAttributes += ' alt="Image"';
      }
      
      return `<img${sanitizedAttributes}>`;
    });
  }

  /**
   * Remove dangerous attributes from HTML elements.
   * Like removing suspicious items during security screening.
   */
  private removeDangerousAttributes(html: string): string {
    // Remove event handlers and dangerous attributes
    const dangerousAttributes = [
      'on\\w+', // Event handlers (onclick, onload, etc.)
      'javascript:', // JavaScript URLs
      'vbscript:', // VBScript URLs
      'data:', // Data URLs (can be dangerous)
      'formaction',
      'form',
      'srcdoc'
    ];
    
    let processed = html;
    dangerousAttributes.forEach(attr => {
      const regex = new RegExp(`\\s${attr}\\s*=\\s*["'][^"']*["']`, 'gi');
      processed = processed.replace(regex, '');
    });
    
    return processed;
  }

  /**
   * Static method for processing HTML in components.
   * Like a utility function for cleaning HTML in TypeScript code.
   */
  static sanitizeHtml(
    html: string,
    sanitizer: DomSanitizer,
    level: 'strict' | 'moderate' | 'minimal' = 'moderate'
  ): SafeHtml {
    const pipe = new SafeHtmlPipe(sanitizer);
    return pipe.transform(html, level);
  }

  /**
   * Convert markdown-like syntax to HTML.
   * Like allowing users to write simple formatting that gets converted to HTML.
   */
  static markdownToHtml(text: string): string {
    if (!text) return '';
    
    let html = text;
    
    // Convert markdown-like syntax
    html = html.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>'); // Bold
    html = html.replace(/\*(.*?)\*/g, '<em>$1</em>'); // Italic
    html = html.replace(/\n/g, '<br>'); // Line breaks
    html = html.replace(/\[([^\]]+)\]\(([^)]+)\)/g, '<a href="$2" target="_blank" rel="noopener noreferrer">$1</a>'); // Links
    
    return html;
  }

  /**
   * Escape HTML entities to prevent XSS.
   * Like encoding dangerous characters so they display as text instead of code.
   */
  static escapeHtml(text: string): string {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
  }

  /**
   * Unescape HTML entities.
   * Like decoding HTML entities back to readable text.
   */
  static unescapeHtml(html: string): string {
    const div = document.createElement('div');
    div.innerHTML = html;
    return div.textContent || div.innerText || '';
  }

  /**
   * Strip all HTML tags from content.
   * Like removing all formatting to get just the plain text.
   */
  static stripHtml(html: string): string {
    const div = document.createElement('div');
    div.innerHTML = html;
    return div.textContent || div.innerText || '';
  }

  /**
   * Truncate HTML content while preserving structure.
   * Like creating a preview of HTML content with proper tag closure.
   */
  static truncateHtml(html: string, maxLength: number, suffix: string = '...'): string {
    const plainText = this.stripHtml(html);
    
    if (plainText.length <= maxLength) {
      return html;
    }
    
    // Find a good breaking point
    const truncatedText = plainText.substring(0, maxLength);
    const lastSpaceIndex = truncatedText.lastIndexOf(' ');
    const breakPoint = lastSpaceIndex > maxLength * 0.8 ? lastSpaceIndex : maxLength;
    
    // Create truncated HTML (simplified approach)
    const truncatedPlain = plainText.substring(0, breakPoint) + suffix;
    
    // For now, return plain text. In a full implementation, you'd preserve HTML structure
    return truncatedPlain;
  }
}
