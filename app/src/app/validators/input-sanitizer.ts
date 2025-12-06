// Remove <script>...</script> blocks (case-insensitive).
export function stripScriptTags(input: string): string {
  return input.replace(/<\s*script[\s\S]*?>[\s\S]*?<\s*\/\s*script\s*>/gi, '');
}

// Remove any HTML tags (simple).
export function stripHtmlTags(input: string): string {
  return input.replace(/<\/?[^>]+(>|$)/g, '');
}

// Trim and collapse multiple whitespace into a single space.
export function normalizeWhitespace(input: string): string {
  return input.replace(/\s+/g, ' ').trim();
}

/* 
Primary sanitizer for plain-text fields (title, author, quote):

- Removes <script> blocks entirely  
- Removes all remaining HTML tags  
- Escapes < and > (prevents injection but preserves math-style titles)  
- Removes ASCII control characters (0–31, 127)  
- Normalizes and collapses whitespace, then trims  
- Enforces an optional maxLen limit  
*/
export function sanitizeText(
  input: string | undefined | null,
  maxLen = 200
): string {
  if (!input) return '';
  let v = String(input);

  // 1. Remove actual HTML tags (script, div, etc)
  v = v.replace(/<\s*script[\s\S]*?>[\s\S]*?<\s*\/\s*script\s*>/gi, '');
  v = v.replace(/<\/?[^>]+(>|$)/g, '');

  // 2. Escape any remaining angle brackets (safe + preserves math titles)
  v = v.replace(/</g, '&lt;').replace(/>/g, '&gt;');

  // 3. Remove ASCII control chars (0–31)
  v = v.replace(/[\x00-\x1F\x7F]/g, '');

  // 4. Normalize whitespace
  v = v.replace(/\s+/g, ' ').trim();

  // 5. Enforce max length
  if (maxLen && v.length > maxLen) {
    v = v.substring(0, maxLen).trim();
  }

  return v;
}

// Returns sanitized string if non-empty, otherwise null
export function isSanitizedNonEmpty(
  input: string | null | undefined,
  maxLen = 1000
): string | null {
  const cleaned = sanitizeText(input, maxLen);
  return cleaned.length > 0 ? cleaned : null;
}
