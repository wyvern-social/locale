# Wyvern Social - Language Localization Guide

Welcome to the **Wyvern Social Language Repository**! This document provides an overview of the supported languages, their intended purpose, and contribution guidelines for translations.

---

## Supported Languages

### Normal Languages
| Code      | Name       | Description                                         |
|-----------|------------|-----------------------------------------------------|
| `en_us`   | English    | Standard English, used as the default language.    |
| `no`      | Norwegian  | Standard Norwegian, for native speakers in Norway.|

### Joke / Playful Languages
| Code      | Name                 | Description                                                                 |
|-----------|--------------------|-----------------------------------------------------------------------------|
| `en_pt`   | Pirate Talk          | Fun, pirate-themed English variant for casual or gamified experiences.      |
| `en_p`    | Anglish              | English stripped of foreign loanwords, using only native Anglo-Saxon roots. |
| `en_ws`   | Shakespearean English| Early Modern English style, inspired by Shakespearean prose.                |
| `uwu_us`  | UwUSpeak             | Playful, cutesy language style popular in online communities.              |
| `lol_us`  | LOLCAT               | Meme-style English that mimics cat-speak, internet humor, and playful spelling. |

> **Note:** Each language file contains translations for all other languages’ names to ensure consistency across UI and user-facing messages.

---

## Contribution Guidelines

We welcome contributions for new languages, improvements, or fixes. Please follow these rules:

1. **Consistency:** Maintain the tone/style of the language. Each translation should reflect the intended voice (e.g., `lol_us` should remain playful).
2. **Formatting:** Use JSON with proper indentation (2 spaces) and valid UTF-8 characters.
3. **Naming:** Files should be named according to the language code, e.g., `en_fr.json` for French.
4. **Testing:** Ensure all keys match the structure of the default English file (`en_us.json`) to avoid breaking the application.
5. **Submission:** Send your completed translation file to the **Wyvern Contribution Team** at `contributing@wyvern.gg`.
6. **Review:** All submissions must be reviewed and approved before being added to the repository.

---

## Adding a New Language

To add a new language:

1. Duplicate `en_us.json` as a starting point.
2. Replace all strings with the translated equivalents.
3. Add the new language code and name to all other `Langs` sections in existing language files to maintain cross-language consistency.
4. Follow contribution guidelines above when submitting your translation.

---

## Best Practices

- Avoid overly literal translations; keep them idiomatic and culturally appropriate.
- For playful languages (`lol_us`, `uwu_us`, `en_pt`), maintain a **fun, consistent tone** without breaking readability.
- Use `{placeholders}` for dynamic content (e.g., `{{username}}`, `{age}`) and **do not translate the placeholder syntax**.

---

## Support

For questions regarding localization, file format, or contribution, contact the **Wyvern Localization Team** at `support@wyvern.gg`.

---

*© 2025 Wyvern Social. All rights reserved.*
