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

Tabellen over ble erstattet av endringen i 2024.

## D-nummer: hele individnummerserien (2024)

Forskrift om endring i folkeregisterforskriften (FOR-2024-02-01-161), fastsatt av Skattedirektoratet 1. februar 2024, i kraft samme dag. Endrer folkeregisterforskriften § 2-2-2 første ledd til:

> «Ved tildeling av d-nummer anvendes individsiffer fra hele nummerserien 000–999.»

Konsekvenser:

- For d-numre tildelt fra 1.2.2024 sier individnummeret ingenting om århundre.
- Validering av d-nummer skal ikke sjekke individnummer mot fødselsår.
- Fødselsåret i et d-nummer kan være 18xx, 19xx eller 20xx. Det korrekte året finnes bare i Folkeregisteret.

Eksempel: `41693093714` er et SyntPop d-nummer (dag 41 = 1, måned 69 = april, år 30, individnummer 937). Kontrollsifrene stemmer. Etter reglene for fødselsnummer gir 937 med år 30 året 2030, men etter 2024-regelen kan personen like gjerne være født 1930.

Tidligere regler for d-nummer (Folkeregisterhåndboken § 2-2, ID-2-2.006, før 2021): 000–499 i synkende rekkefølge for personer født før 1.1.2000, og 500–999 i synkende rekkefølge for personer født etter 1.1.2000.

## Konsekvenser for Fhi.NINCheck

1. K1 fra 2032: `NinChecker.cs` og `Validation.cs` godtar bare `k1 = 11 - (sum % 11)`. Numre som tildeles fra 1.1.2032 med ny K1 blir avvist som ugyldige.
2. Århundre for d-nummer: `FindCorrectYear()` i `NinChecker.cs` bruker århundretabellen for fødselsnummer også på d-numre. Det er feil for d-numre tildelt etter 16.11.2021, og helt uten grunnlag etter 1.2.2024.
   - `Birthdate` kan gi feil århundre. Eksempel: født 2005 med individnummer 300 gir 1905.
   - Gyldige d-numre blir avvist når tabellen gir et år i fremtiden. Eksempel: `41693093714` gir 2030, og `IsValidYear` feiler. Personen kan være født 1930.
   - Riktig oppførsel: godta et d-nummer når datoen er gyldig i minst ett mulig århundre og ikke ligger i fremtiden. `Birthdate` for d-nummer kan ikke avgjøres sikkert fra nummeret.
3. Århundre fra 2032: individnummeret angir ikke lenger århundre for noen numre. `Birthdate` kan da ikke utledes sikkert fra nummeret.

## Ikke dekket

- Syntetiske numre (Tenor og SyntPop). Skatteetaten sier bare at syntetiske numre er bygget opp annerledes enn reelle numre.
- H-nummer, FH-nummer og DUF-nummer. Disse reglene kommer ikke fra Skatteetaten.
- Forskriftsteksten på Lovdata og Skatteetatens kildekode er ikke lest direkte.

## Kilder

- [Nytt fødselsnummer fra 2032 – teknisk dokumentasjon](https://skatteetaten.github.io/folkeregisteret-api-dokumentasjon/nytt-fodselsnummer-fra-2032/)
- [Folkeregisteret API-dokumentasjon – Nyheter](https://skatteetaten.github.io/folkeregisteret-api-dokumentasjon/nyheter/) (Utvidelse av D-nummerserien: 24.06.2021, 09.08.2021 og 17.09.2021)
- [FOR-2024-02-01-161 Forskrift om endring i folkeregisterforskriften](https://lovdata.no/dokument/LTI/forskrift/2024-02-01-161)
- [Folkeregisterhåndboken § 2-2, ID-2-2.006](https://www.skatteetaten.no/rettskilder/type/handboker/folkeregisterhandboken/gjeldende/kapittel2-registreringspliktige-personer-identifikasjonsnummer/ID-2-2.001/ID-2-2.006/)
- [Validering av fødselsnummer og d-nummer](https://www.skatteetaten.no/deling/folkeregisteret/pid/validering/)
- [Nye fødselsnummer og d-nummer fra 2032](https://www.skatteetaten.no/deling/opplysninger/folkeregisteropplysninger/pid/)
- [Folkeregisterhåndboken § 2-2 Identifikasjonsnummer](https://www.skatteetaten.no/rettskilder/type/handboker/folkeregisterhandboken/gjeldende/kapittel2-registreringspliktige-personer-identifikasjonsnummer/ID-2-2.001/)
