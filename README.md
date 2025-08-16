# Phantom Proxy

Anonymous browser proxy inspired by [browser.lol](https://browser.lol).

## 📅 MVP Roadmap

### URL Proxying
- [x] Proxy URLs in standard HTML attributes
- [ ] Proxy URLs in standard SVG attributes
- [x] Proxy URLs inside `url()` functions
- [x] Proxy URLs inside `srcset` attribute

### Security
- [x] Encrypt target URLs in Network tab
- [x] Generate AES key client-side, encrypted with server RSA public key
- [x] AES encryptor
- [x] AES encryptor unit tests
- [x] RSA encryptor
- [x] RSA encryptor unit tests
- [x] Implement temporary RSA keys for sessions with automatic generation and periodic refresh
- [ ] Implement secure session keys handling
- [ ] Limit length of URLs

## Content
- [ ] Handle content with charset
- [ ] Optionally remove `<script>` tags

### Performance
- [ ] Add performance measurement
- [ ] Cache proxied resources

### Documentation
- [ ] Create deployment instruction
- [ ] Create user-friendly explanation of application workflow
