const loadingBanner = document.getElementById('loading-banner');
const form = document.getElementById('browse-form');
const urlInput = document.getElementById('browse-url');
const errorBox = document.getElementById('browse-error');
const frame = document.getElementById('viewbox');

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
    const urlBase64 = btoa(url);
    const response = await fetch(`/browse?url=${urlBase64}`);
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
