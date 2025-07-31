const loadingBanner = document.getElementById('loading-banner');
const form = document.getElementById('browse-form');
const urlInput = document.getElementById('browse-url');
const errorBox = document.getElementById('browse-error');
const frame = document.getElementById('viewbox');
let password;

async function encrypt(text, password) {
  const encoder = new TextEncoder();
  const iv = crypto.getRandomValues(new Uint8Array(12));
  const keyMaterial = await crypto.subtle.importKey(
    'raw',
    encoder.encode(password),
    'PBKDF2',
    false,
    ['deriveBits']
  );
  const rawKey = await crypto.subtle.deriveBits(
    {
      name: 'PBKDF2',
      salt: new Uint8Array(),
      iterations: 1,
      hash: 'SHA-256'
    },
    keyMaterial,
    256
  );
  const key = await crypto.subtle.importKey(
    'raw',
    rawKey,
    'AES-GCM',
    false,
    ['encrypt']
  );

  const encrypted = await crypto.subtle.encrypt(
    { name: 'AES-GCM', iv },
    key,
    encoder.encode(text)
  );

  const combined = new Uint8Array(iv.length + encrypted.byteLength);
  combined.set(iv, 0);
  combined.set(new Uint8Array(encrypted), iv.length);

  return btoa(String.fromCharCode(...combined));
}

form.addEventListener('submit', async e => {
  e.preventDefault();
  errorBox.textContent = '';
  errorBox.style.display = 'none';

  const url = urlInput.value?.trim();
  if (!url) {
    return;
  }

  loadingBanner.style.display = 'flex';

  try {
    if (!password) {
      passwordResponse = await fetch('/password');

      if (!passwordResponse.ok) {
        errorBox.textContent = 'Failed to fetch encryption password.';
        errorBox.style.display = 'flex';
        console.error(response);
        return;
      }

      password = await passwordResponse.text();
    }

    const urlEncrypted = await encrypt(url, password);
    const response = await fetch(`/browse?url=${encodeURIComponent(urlEncrypted)}`);
    if (!response.ok) {
      errorBox.textContent = 'Failed to fetch content.';
      errorBox.style.display = 'flex';
      console.error(response);
      return;
    }

    const html = await response.text();
    const blob = new Blob([html], { type: 'text/html' });
    const blobUrl = URL.createObjectURL(blob);

    frame.src = blobUrl;
  } catch (error) {
    errorBox.textContent = 'Failed to load content.';
    errorBox.style.display = 'flex';
    console.error(error);
  } finally {
    loadingBanner.style.display = 'none';
  }
});
