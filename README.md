# Fhi.NINCheck
Bibliotek for å sjekke gyldighet av Fnr, Dnr, HNr,FHN,Duf samt i test environment Tenor og SyntPop nummere.


## Bruk

```csharp
using Fhi.NinCheck;

string nin = "some 11 digit nin number (12 for Duf)";
if (nin.ErGyldigNin())
{
    // Do something
}
```

Dersom man kjører i test-miljø sender man med en parameter i kallet.
Default er true, altså produksjonsmiljø.

```csharp
using Fhi.NinCheck;

string nin = "some 11 digit nin number (12 for Duf)";
bool isTestEnvironment = !Environment.IsProduction;
if (nin.ErGyldigNin(isTestEnvironment))
{
    // Do something
}
```

Metoden `ErGyldigNin` sjekker Fnr, Dnr, Duf, HNr og FHN, og under test i tillegg Tenor og SyntPop. 

## Enkeltkall

Det finnes også enklere kall for å sjekke om et spesifikt type nummer er gyldig.
```csharp
using Fhi.NinCheck;

nin.ErGyldigFNummer();
nin.ErGyldigDNummer();
nin.ErGyldigHNummer();
nin.ErGyldigFHNummer();
nin.ErGyldigDufNummer();
nin.ErGyldigTenorTestNummer();
nin.ErGyldigSyntetiskTestNummer();
```

## Egne kombinerte sjekker

Man kan 
også sette sammen sine egne kombinerte sjekker om man ønsker å begrense mer enn ErGyldigNin gjør.
Koden for ErGyldigNin kan anvendes som mal for dette.

```csharp
using Fhi.NinCheck;

public static bool ErGyldigNin(this string nin,bool isProduction=true)
    {
        if (nin.ErGyldigFNummer() 
            || nin.ErGyldigDNummer() 
            || nin.ErGyldigDufNummer()
            || nin.ErGyldigHNummer()
            || nin.ErGyldigFHNummer()
            )
            return true;
        if (isProduction) return false;
        return nin.ErGyldigSyntetiskTestNummer() 
               || nin.ErGyldigTenorTestNummer();
    }
```

## Feilinformasjon

Om et nummer feiler, kan man hente ut informasjon om hvorfor det feilet.

```csharp
using Fhi.NinCheck;

string feilinformasjon = Validation.LastFailedStep();
```
