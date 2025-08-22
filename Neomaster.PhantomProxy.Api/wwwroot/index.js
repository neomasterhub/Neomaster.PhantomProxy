const loadingBanner = document.getElementById('loading-banner');
const form = document.getElementById('browse-form');
const urlInput = document.getElementById('browse-url');
const errorBox = document.getElementById('browse-error');
const frame = document.getElementById('viewbox');
let sessionInfo;

function toBase64(bytes) {
  return btoa(String.fromCharCode(...new Uint8Array(bytes)));
}

function createCookie(key, value, lifetimeMs) {
  document.cookie = `${key}=${value};Max-Age=${lifetimeMs};SameSite=Lax;Path=/`;
}

function getPemKeyBytes(pem) {
  const keyBase64 = pem.replace(/-----.*?-----/g, '').replace(/\s+/g, '');
  const keyText = atob(keyBase64);
  const keyBytes = new Uint8Array(keyText.length);

  for (let i = 0; i < keyText.length; i++) {
    keyBytes[i] = keyText.charCodeAt(i);
  }

  return keyBytes.buffer;
}

async function startSessionAsync() {
  loadingBanner.style.display = 'flex';

  try {
    const response = await fetch('/start-session');

    if (!response.ok) {
      throw new Error(`Failed to start session: ${response.status}`);
    }

    sessionInfo = await response.json();
    console.log('New session started:', sessionInfo);
  } catch (error) {
    errorBox.textContent = 'Failed to start session.';
    errorBox.style.display = 'flex';
    console.error(error);
  } finally {
    loadingBanner.style.display = 'none';
    setTimeout(() => location.reload(), sessionInfo.lifetimeMs);
  }
}

async function encryptAsync(text, pem) {
  const encoder = new TextEncoder();

  const spki = getPemKeyBytes(pem);

  const rsaKey = await crypto.subtle.importKey(
    'spki',
    spki,
    { name: 'RSA-OAEP', hash: 'SHA-256' },
    false,
    ['encrypt']
  );

  const aesKey = await crypto.subtle.generateKey(
    { name: 'AES-GCM', length: 256 },
    true,
    ['encrypt']
  );

  const iv = crypto.getRandomValues(new Uint8Array(12));

  const encrypted = await crypto.subtle.encrypt(
    { name: 'AES-GCM', iv },
    aesKey,
    encoder.encode(text)
  );

  const rawAesKey = await crypto.subtle.exportKey('raw', aesKey);

  const encryptedRawAesKey = await crypto.subtle.encrypt(
    { name: 'RSA-OAEP' },
    rsaKey,
    rawAesKey
  );

  return {
    key: encryptedRawAesKey,
    iv: iv.buffer,
    encrypted,
  };
}

startSessionAsync();

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
    const encrypted = await encryptAsync(url, sessionInfo.pem);
    const encryptedUrl = encodeURIComponent(toBase64(encrypted.encrypted));
    const key = encodeURIComponent(toBase64(encrypted.key));
    const iv = encodeURIComponent(toBase64(encrypted.iv));

    createCookie('key', key, sessionInfo.lifetimeMs);
    createCookie('iv', iv, sessionInfo.lifetimeMs);
    createCookie('session-id', sessionInfo.id, sessionInfo.lifetimeMs);

    const response = await fetch(`/browse?url=${encryptedUrl}`);
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
