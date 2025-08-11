# Phantom Proxy

Anonymous browser proxy inspired by [browser.lol](https://browser.lol).

## 📅 Roadmap

### URL Proxying
- [x] Proxy URLs in standard HTML attributes
- [ ] Proxy URLs in standard SVG attributes
- [ ] Proxy CSS URLs (inline styles and external files)
- [x] Proxy `srcset` attribute URLs

### Security
- [x] Encrypt target URLs in Network tab
- [x] Generate AES key client-side, encrypted with server RSA public key

### Performance
- [ ] Add performance measurement
- [ ] Cache proxied resources

### Other
- [ ] Add infrastructure-level unit tests
- [ ] Write deployment instruction
- [ ] Publish NuGet package
- [ ] Optionally remove `<script>` tags
