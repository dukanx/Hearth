# Hearth — TODO

## 0. Osnovne stvari koje fale (pre novih funkcionalnosti)

- [ ] **Izbacivanje člana iz domaćinstva** — Adult može da ukloni člana; brišu se njegove pretplate/notifikacije, zadaci mu se oslobode (unassign).
- [ ] **Napuštanje domaćinstva** — član sam izlazi; ako izlazi poslednji Adult, odluka: prenos vlasništva ili raspuštanje domaćinstva.
- [ ] **Regeneracija kodova za pozivanje** — Adult generiše nov kod (stari prestaje da važi), za slučaj da kod "procuri".
- [ ] **Preimenovanje domaćinstva** — Adult menja naziv.
- [ ] **Profil korisnika** — promena imena za prikaz i lozinke.
- [ ] **"Ostani prijavljen" (refresh token)** — JWT traje 60 min, pa PWA na telefonu često izbaci na login; refresh token + tiha obnova sesije.
- [ ] **Brisanje naloga** — GDPR-friendly: briše korisnika, pretplate, notifikacije; zadatke ostavlja bez izvršioca.
- [ ] **Potvrda pre brisanja** (zadatak/stavka) — mali confirm dijalog, da ne ode slučajno.

## 1. Ponavljajući zadaci

"Iznesi đubre svakog utorka" — recurrence pravilo (dnevno / nedeljno / mesečno + dan)
na zadatku; pozadinski servis (već postoji `PushSenderService` šablon) generiše
sledeću instancu kad se prethodna završi ili kad prođe rok.

## 2. Podsetnici / rokovi za zadatke

Rok (`DueAt`) na zadatku + zakazani push ("zadatak X ističe danas u 18h").
Pozadinski servis periodično proverava rokove i šalje kroz postojeći push red.

## 3. Poeni / gamifikacija za decu

Završen zadatak = poeni (težina zadatka = broj poena). Rang lista na dashboardu,
opciono nedeljni "pobednik". Koristi postojeću Child rolu.

## 4. Troškovi domaćinstva

Cena na kupljenoj stavci → mesečni pregled: ukupno, po članu, po kategoriji.
Kasnije: podela troškova ("ko je dužan kome").

## 5. Predlozi pri kupovini

Na osnovu istorije: "često kupuješ mleko — dodaj opet?" Autocomplete iz ranije
kupovanih artikala + brzo ponovno dodavanje kupljenih.

## 6. Badge na ikoni aplikacije

Broj nepročitanih obaveštenja na PWA ikoni (Badging API, radi na iOS 16.4+).
Setuje se iz service workera pri push-u i iz aplikacije pri čitanju.
