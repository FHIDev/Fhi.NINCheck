# Skatteetatens regler for fødselsnummer og d-nummer

Oppsummering av de nasjonale reglene fra Skatteetaten (Folkeregisteret) for oppbygging og validering av fødselsnummer og d-nummer, og hva de betyr for Fhi.NINCheck.

Hentet 2026-11-09. Se [Kilder](#kilder).

## Rettsgrunnlag

Folkeregisterloven § 2-2 (Folkeregisterhåndboken § 2-2):

- Fødselsnummeret har elleve siffer.
- De seks første sifrene er fødselsdato (DDMMÅÅ).
- De fem siste sifrene er individsiffer (3) og kontrollsiffer (2).
- D-nummer har samme oppbygging, men første siffer er økt med 4 (dag + 40).
- Departementet gir regler om individ- og kontrollsiffer i forskrift til folkeregisterloven (nr. 1201 av 14.07.2017), kapittel 2.

## Kontrollsiffer

Sifrene i nummeret er `d1 d2 m1 m2 å1 å2 i1 i2 i3 k1 k2`.

| Kontrollsiffer | Vekter                          | Over sifrene                  |
|----------------|---------------------------------|-------------------------------|
| K1             | `3 7 6 1 8 9 4 5 2`             | d1 d2 m1 m2 å1 å2 i1 i2 i3    |
| K2             | `5 4 3 2 7 6 5 4 3 2`           | d1 d2 m1 m2 å1 å2 i1 i2 i3 k1 |

### Dagens regel (numre tildelt før 1.1.2032)

- `k1 = 11 - (sum % 11)`. Resultatet 11 gir 0. Resultatet 10 betyr ugyldig nummer.
- `k2 = 11 - (sum % 11)`. Resultatet 11 gir 0. Resultatet 10 betyr ugyldig nummer.

Tilsvarende: `(vektet sum av d1..i3 + k1) % 11` må være 0.

### Ny regel for K1 (numre tildelt fra 1.1.2032)

- `k1 = {11, 12, 13, 14} - (sum % 11)`. Hvis resultatet er større enn 10, trekk fra 11. Resultatet 10 forkastes.
- Hver kombinasjon av dato og individnummer kan derfor ha opptil fire gyldige k1.
- Tilsvarende: `(vektet sum av d1..i3 + k1) % 11` må være 0, 1, 2 eller 3.
- K2 er uendret.

Systemer må godta både dagens og ny beregning parallelt. Ingen eksisterende numre endres.

### Eksempler fra Skatteetaten

| Nummer        | Beskrivelse                                                     |
|---------------|-----------------------------------------------------------------|
| `02013299997` | Født 02.01.2032, individnummer 999 (k1 = 9, k2 = 7)            |
| `30108299920` | Født 30.10.1982, individnummer 999, dagens K1 (k1 = 2)         |
| `30108299939` | Samme person, ny K1 (k1 = 3, k2 = 9). Gyldig fra 2032          |

Eksemplene er regnet ut på nytt og stemmer med reglene over.

## Endringer fra 1.1.2032

- Numrene har fortsatt elleve siffer.
- Ny K1-beregning gjelder alle nye tildelinger, også for personer født før 2032.
- Individnummeret angir ikke lenger århundre. Fødselsår må hentes fra eget felt i Folkeregisteret.
- Numrene blir kjønnsnøytrale. I dag betyr partall i i3 kvinne og oddetall mann. Fra 2032 skal kjønn ikke utledes fra nummeret.

Skatteetaten har en validator og kildekode i Java og JavaScript for den nye beregningen. Validatoren sjekker bare oppbygging, ikke om nummeret finnes i Folkeregisteret.

## Utvidelse av D-nummerserien (2021)

Varslet 24.06.2021. Satt i produksjon tirsdag 16.11.2021.

Skatteetaten økte kapasiteten i individnumrene for d-nummer, fordi serien ellers ville gått tom i 2029. Nye d-numre bruker ubenyttede nummerserier fra 1859–1899 og 1900–1939. Tabellen er gjengitt fra nyheten:

| Første siffer i individnummer | Født      | Individnummer    | Kapasitet per dag | Kommentar                                                    |
|-------------------------------|-----------|------------------|-------------------|--------------------------------------------------------------|
| 5                             | 1858–1899 | 500–599          | 82                | Svært få d-numre. Serien 600–999 flyttes til 1900-tallet.    |
| 0, 1                          | 1900–1939 | 000–199          | 164               | Få d-numre. Serien 200–499 flyttes til 2000-tallet.          |
| 0, 1, 2, 3, 4, 6, 7, 8, 9     | 1940–1999 | 000–499, 600–999 | 738               | Utvides med serien 600–999 fra 1800-tallet.                  |
| 2, 3, 4, 5, 6, 7, 8, 9        | 2000–2039 | 200–999          | 656               | Utvides med serien 200–499 fra 1900-tallet.                  |

Skatteetaten: «Konsumenter som benytter individnummeret til å utlede korrekt fødselsår for D-nummerpersoner må endre sine systemløsninger.»

Oppbyggingen (dag + 40) og kontrollsifrene er ikke endret.

Spørsmål og svar hos Skatteetaten (hentet 2026-11-16) viser samme tabell, og sier at endringen kom i konsumenttestmiljøet 20.10.2021. Testpersoner med d-nummer fra utvidelsen. I testmiljøene er måned + 80, det er det ikke i produksjon:

| Født | D-nummer                                    |
|------|---------------------------------------------|
| 1940 | `41814061033`, `41814065640`, `41814075603` |
| 2000 | `41810021827`, `41810025091`, `41810034422` |

Tabellen gjelder fortsatt etter vår tolkning. Se 2024-forskriften under.

## D-nummer: hele individnummerserien (2024)

Forskrift om endring i folkeregisterforskriften (FOR-2024-02-01-161), fastsatt av Skattedirektoratet 1. februar 2024, i kraft samme dag. Endrer folkeregisterforskriften § 2-2-2 første ledd til:

> «Ved tildeling av d-nummer anvendes individsiffer fra hele nummerserien 000–999.»

Endringen har ingen merknader. Teksten kan leses på to måter:

1. Alle individnumre kan brukes for alle fødselsår, så århundret kan ikke utledes fra nummeret.
2. Endringen gir 2021-tabellen rettslig grunnlag. Tabellen bruker allerede hele serien 000–999.

Vår tolkning er 2, men den er ikke bekreftet av Skatteetaten (spørsmål sendt 2026-11-16, ikke besvart). Grunner:

- Spørsmål og svar viser fortsatt 2021-tabellen og ber konsumenter følge den.
- Skatteetaten varslet 2021-endringen flere ganger, men varslet ingenting om d-nummer i 2024.
- For 2032 sier Skatteetaten uttrykkelig at individnummeret ikke lenger angir århundre. Noe tilsvarende er ikke sagt for d-nummer før 2032.

Eksempel: `41693093714` er et SyntPop d-nummer (dag 41 = 1, måned 69 = april, år 30, individnummer 937), meldt inn som hendelse fordi det ble avvist. Kontrollsifrene stemmer. Etter 2021-tabellen gir 937 med år 30 året 2030, altså i fremtiden. Etter tolkning 1 kan personen være født 1930.

Tidligere regler for d-nummer (Folkeregisterhåndboken § 2-2, ID-2-2.006, før 2021): 000–499 i synkende rekkefølge for personer født før 1.1.2000, og 500–999 i synkende rekkefølge for personer født etter 1.1.2000.

## Konsekvenser for Fhi.NINCheck

1. K1 fra 2032: `NinChecker.cs` og `Validation.cs` godtar bare `k1 = 11 - (sum % 11)`. Numre som tildeles fra 1.1.2032 med ny K1 blir avvist som ugyldige.
2. Århundre for d-nummer: `FindCorrectYear()` i `NinChecker.cs` bruker 2021-tabellen for d-numre.
   - Gir tabellen et år i fremtiden, trekkes 100 år fra, så nummeret ikke avvises. Det dekker tolkning 1. Etter tolkning 2 finnes ikke slike numre i produksjon. `41693093714` gir derfor 1930.
   - D-numre tildelt før 16.11.2021 til personer født 1900–1939 med individnummer 200–499 får 20xx, så lenge 20xx ikke ligger i fremtiden. Det kan ikke skilles fra nummeret. D-numre inaktiveres fem år etter tildeling (§ 2-2-2), så disse er i hovedsak inaktive fra 16.11.2026.

   Sikringen (100 år tilbake) slår bare inn for år 00–39 med individnummer 200 eller høyere, når 20xx ligger i fremtiden. I 2026 gjelder det år 27–39:

   | Numre                                                          | Uten sikringen     | Med sikringen        |
   |----------------------------------------------------------------|--------------------|----------------------|
   | Reelle d-numre etter tolkning 2 (2021-tabellen gjelder)        | Finnes ikke        | Ingen effekt         |
   | Reelle d-numre etter tolkning 1 (alle individnumre, alle år)   | Avvist             | Godtatt som 19xx     |
   | SyntPop-hendelsen `41693093714`                                | Avvist (2030)      | Godtatt som 1930     |
   | D-numre fra før 2021, født 1927–1939, individnummer 200–499    | Avvist (2027–2039) | Riktig, 19xx         |

   Ulempen er at resultatet avhenger av året i dag. År 27 med individnummer 300 gir 1927 i 2026 og 2027 fra 2027.
3. Århundre fra 2032: individnummeret angir ikke lenger århundre for noen numre. `Birthdate` kan da ikke utledes sikkert fra nummeret.
4. Anbefaling: `Birthdate` og `RealYear` er estimert fra nummeret. For d-nummer og numre tildelt fra 2032 er Folkeregisteret eneste sikre kilde. For d-nummer kan også dag og måned avvike fra reell fødselsdato (Spørsmål og svar: «færre personer vil få avvikende fødselsdato i D-nummeret med reell fødselsdato»).

## Ikke dekket

- Syntetiske numre (Tenor og SyntPop). Skatteetaten sier bare at syntetiske numre er bygget opp annerledes enn reelle numre.
- H-nummer, FH-nummer og DUF-nummer. Disse reglene kommer ikke fra Skatteetaten.
- Skatteetatens kildekode er ikke lest direkte. Ordlyden i § 2-2-2 før 2024 er ikke funnet.

## Kilder

- [Nytt fødselsnummer fra 2032 – teknisk dokumentasjon](https://skatteetaten.github.io/folkeregisteret-api-dokumentasjon/nytt-fodselsnummer-fra-2032/)
- [Folkeregisteret API-dokumentasjon – Spørsmål og svar](https://skatteetaten.github.io/folkeregisteret-api-dokumentasjon/sporsmal-og-svar/) (Utvidelse av D-nummerserien)
- [Folkeregisteret API-dokumentasjon – Nyheter](https://skatteetaten.github.io/folkeregisteret-api-dokumentasjon/nyheter/) (Utvidelse av D-nummerserien: 24.06.2021, 09.08.2021 og 17.09.2021)
- [FOR-2024-02-01-161 Forskrift om endring i folkeregisterforskriften](https://lovdata.no/dokument/LTI/forskrift/2024-02-01-161)
- [Folkeregisterforskriften kapittel 2](https://lovdata.no/dokument/SF/forskrift/2017-07-14-1201/KAPITTEL_2)
- [Folkeregisterhåndboken § 2-2, ID-2-2.006](https://www.skatteetaten.no/rettskilder/type/handboker/folkeregisterhandboken/gjeldende/kapittel2-registreringspliktige-personer-identifikasjonsnummer/ID-2-2.001/ID-2-2.006/)
- [Validering av fødselsnummer og d-nummer](https://www.skatteetaten.no/deling/folkeregisteret/pid/validering/)
- [Nye fødselsnummer og d-nummer fra 2032](https://www.skatteetaten.no/deling/opplysninger/folkeregisteropplysninger/pid/)
- [Folkeregisterhåndboken § 2-2 Identifikasjonsnummer](https://www.skatteetaten.no/rettskilder/type/handboker/folkeregisterhandboken/gjeldende/kapittel2-registreringspliktige-personer-identifikasjonsnummer/ID-2-2.001/)
