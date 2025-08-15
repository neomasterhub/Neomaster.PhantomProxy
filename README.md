# Phantom Proxy

Anonymous browser proxy inspired by [browser.lol](https://browser.lol).

## 📅 Roadmap

### URL Proxying
- [x] Proxy URLs in standard HTML attributes
- [ ] Proxy URLs in standard SVG attributes
- [ ] Proxy URLs inside `url()` functions
- [x] Proxy URLs inside `srcset` attribute

### Security
- [x] Encrypt target URLs in Network tab
- [x] Generate AES key client-side, encrypted with server RSA public key
- [x] AES encryptor
- [x] AES encryptor unit tests
- [x] RSA encryptor
- [x] RSA encryptor unit tests
- [x] Implement temporary RSA keys for sessions with automatic generation and periodic refresh
- [ ] Limit length of URLs

### Performance
- [ ] Add performance measurement
- [ ] Cache proxied resources

### Other
- [ ] Add infrastructure-level unit tests
- [ ] Write deployment instruction
- [ ] Publish NuGet package
- [ ] Optionally remove `<script>` tags
