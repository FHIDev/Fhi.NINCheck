namespace Fhi.NINCheck;

/// <summary>
///     Extention methods for validation.
/// </summary>
public static class Validation
{
    /// <summary>
    /// Last error message, indicating which step in the validation process failed.
    /// </summary>
    public static string LastFailedStep { get; private set; } = "";

    /// <summary>
    /// Validerer et fødselsnummer om det er fnr eller dnr, og dersom det kjører i et testsystem, om det er et syntetisk testnummer fra SyntPop eller Tenor
    /// </summary>
    /// <param name="nin"></param>
    /// <param name="isProduction">Default verdi true</param>
    /// <returns></returns>
    public static bool ErGyldigNin(this string nin, bool isProduction = true)
    {
        if (isProduction
            && (nin.ErGyldigSyntetiskTestNummer() || nin.ErGyldigTenorTestNummer()))
        {
            LastFailedStep = "Syntetiske testnummer er ikke gyldig i produksjon.";
            return false;
        }

        if (nin.ErGyldigFNummer()
            || nin.ErGyldigDNummer()
            || nin.ErGyldigDufNummer()
            || nin.ErGyldigHNummer()
            || nin.ErGyldigFHNummer()
            )
        {
            return true;
        }

        if (isProduction)
        {
            return false;
        }

        return nin.ErGyldigSyntetiskTestNummer() || nin.ErGyldigTenorTestNummer();
    }

    /// <summary>
    ///     Validates a given fødselsnummer.
    /// </summary>
    /// <param name="nin">The fødselsnummer to validate.</param>
    /// <returns>Whether the fødselsnummer was valid or not.</returns>
    public static bool ErGyldigFødselsNummer(this string nin) => nin.ErGyldigFNummer();

    /// <summary>
    ///     Validates a given fødselsnummer.
    /// </summary>
    /// <param name="nin">The fødselsnummer to validate.</param>
    /// <returns>Whether the fødselsnummer was valid or not.</returns>
    public static bool ErGyldigFNummer(this string nin)
    {
        var checker = new NinChecker(nin);
        if (!checker.IsTechValid)
        {
            LastFailedStep = checker.ErrorMessage;
            return false;
        }

        return checker.IsFNummer;
    }


    /// <summary>
    ///     Validates a given d-nummer.
    /// </summary>
    /// <param name="nin">D-nummer to validate.</param>
    /// <returns>Whether the provided d-nummer was valid or not</returns>
    /// <remarks>
    ///     Et D-nummer er ellevesifret, som ordinære fødselsnummer, og består av en
    ///     modifisert sekssifret fødselsdato og et femsifret personnummer.
    ///     Fødselsdatoen modifiseres ved at det legges til 4 på det første sifferet:
    ///     en person født 1. januar 1980 får dermed fødselsdato 410180, mens en som er født 31. januar
    ///     1980 får 710180.
    /// </remarks>
    public static bool ErGyldigDNummer(this string nin)
    {
        var checker = new NinChecker(nin);
        if (!checker.IsTechValid)
        {
            LastFailedStep = checker.ErrorMessage;
            return false;
        }

        return checker.IsDNummer;
    }

    /// <summary>
    ///     Validates a given h-nummer.
    /// </summary>
    /// <param name="nin">H-nummer to validate.</param>
    /// <returns>Whether the h-nummer is valid or not.</returns>
    /// <remarks>
    ///     Et H-nummer er ellevesifret, som ordinære fødselsnummer, og består av en
    ///     modifisert sekssifret fødselsdato og et femsifret personnummer. Fødselsdatoen
    ///     modifiseres ved at det legges til 4 på det tredje sifferet: en person født 1. januar 1980
    ///     får dermed fødselsdato 014180, mens en som er født 31. januar 1980 får 314180.
    /// </remarks>
    public static bool ErGyldigHNummer(this string nin)
    {
        var checker = new NinChecker(nin);
        if (!checker.IsTechValid)
        {
            LastFailedStep = checker.ErrorMessage;
            return false;
        }

        return checker.IsHNummer;
    }

    /// <summary>
    ///    Validates a given syntetisk testnummer fra SyntPop.  Reglen er mnd + 65
    /// </summary>
    /// <param name="nin"></param>
    /// <returns></returns>
    public static bool ErGyldigSyntetiskTestNummer(this string nin)
    {
        var checker = new NinChecker(nin);
        if (!checker.IsTechValid)
        {
            LastFailedStep = checker.ErrorMessage;
            return false;
        }

        return checker.IsSyntPopNummer;
    }

    /// <summary>
    ///    Validates a given testnumber from Tenor.  Reglen er mnd + 80
    /// </summary>
    /// <param name="nin"></param>
    /// <returns></returns>
    public static bool ErGyldigTenorTestNummer(this string nin)
    {
        var checker = new NinChecker(nin);
        if (!checker.IsTechValid)
        {
            LastFailedStep = checker.ErrorMessage;
            return false;
        }

        return checker.IsTenorNummer;
    }

    /// <summary>
    /// Returns the BirthDate of the given FNr, DNr or HNr
    /// Null if not correct type or invalid.
    /// </summary>
    /// <param name="nin"></param>
    /// <returns></returns>
    public static DateTime? Birthdate(this string nin)
    {
        var checker = new NinChecker(nin);
        return checker.Birthdate;
    }

    /// <summary>
    /// Returns whether a BirthDate of the given FNr, DNr or HNr exists.
    /// </summary>
    /// <param name="nin"></param>
    /// <returns></returns>
    public static bool HasBirthdate(this string nin)
    {
        var checker = new NinChecker(nin);
        return checker.HasBirthdate;
    }

    /// <summary>
    ///     Validates a given FH-nummer
    /// </summary>
    /// <param name="fhnr"></param>
    /// <remarks>
    ///     Et FH-nummer skal bestå av et ni-siffret tall mellom 800.000.000 og 999.999.999.
    ///     Siffer 10 og 11, kontrollsifrene, beregnes på samme måte som for fødselsnummer.
    /// </remarks>
    /// <returns></returns>
    public static bool ErGyldigFHNummer(this string fhnr)
    {
        if (!CheckLength(fhnr) || !CheckCharacters(fhnr))
            return false;
        var fhNummer = fhnr.Substring(0, 9);
        var nr = fhNummer.ToInt();
        var harRiktigTallSerie = nr is >= 800000000 and <= 999999999;
        if (!harRiktigTallSerie)
        {
            LastFailedStep = "Ikke riktig tallserie";
            return false;
        }
        return CheckKontrollSiffre(fhnr);
    }

    /// <summary>
    /// Validates a given DUF-nummer
    /// Et Duf-nummer skal bestå av et 12-siffret tall.
    /// Sifrene 0-3 er årstall, og resten er løpenummer
    /// </summary>
    /// <param name="nin"></param>
    /// <remarks>
    ///     
    /// </remarks>
    /// <returns></returns>
    public static bool ErGyldigDufNummer(this string nin)
    {
        var checker = new NinChecker(nin);
        if (!checker.IsTechValid)
        {
            LastFailedStep = checker.ErrorMessage;
            return false;
        }

        return checker.IsDufNumber;
    }

    /// <summary>
    /// Only use this when the month is not the be checked
    /// </summary>
    private static int ExtractMonth(int rawMonth)
    {
        return rawMonth switch
        {
            >= 65 and < 80 => rawMonth - 65,
            >= 80 => rawMonth - 80,
            _ => rawMonth
        };
    }

    private static bool CheckCharacters(string hnr)
    {
        try
        {
            _ = long.Parse(hnr);
        }
        catch (FormatException)
        {
            LastFailedStep = nameof(CheckCharacters);
            return false;
        }
        return true;
    }

    private static bool CheckLength(string nin, int length = 11)
    {
        if (string.IsNullOrEmpty(nin) || nin.Length != length || nin.Contains(' '))
        {
            LastFailedStep = $"{nameof(CheckLength)}: Got {nin?.Length}, expected {length}";
            return false;
        }

        return true;
    }

    private static bool CheckKontrollSiffre(string fnr)
    {
        LastFailedStep = nameof(CheckKontrollSiffre);
        var v1 = new[] { 3, 7, 6, 1, 8, 9, 4, 5, 2 };
        var v2 = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

        var s1 = 0;
        var s2 = 0;

        for (var i = 0; i < 9; i++)
        {
            s1 += Convert.ToInt16(fnr.Substring(i, 1)) * v1[i];
        }

        var r1 = s1 % 11;
        var k1 = 11 - r1;

        switch (k1)
        {
            case 11:
                k1 = 0;
                break;
            case 10:
                return false;
        }

        for (var i = 0; i < 10; i++)
        {
            s2 += Convert.ToInt16(fnr.Substring(i, 1)) * v2[i];
        }

        var r2 = s2 % 11;
        var k2 = 11 - r2;

        switch (k2)
        {
            case 11:
                k2 = 0;
                break;
            case 10:
                return false;
        }
        bool isk1 = Convert.ToInt16(fnr.Substring(9, 1)) == k1;
        bool isk2 = Convert.ToInt16(fnr.Substring(10, 1)) == k2;
        return isk1 && isk2;
    }

    /// <summary>
    /// Returnerer hva slags nummer det kan være
    /// </summary>
    /// <param name="nin"></param>
    /// <returns></returns>
    public static string CheckNinType(this string nin)
    {
        var basetype = nin.ErGyldigFNummer() ? "FNummer"
            : nin.ErGyldigDNummer() ? "DNummer"
            : "";
        if (basetype == "")
        {
            if (nin.ErGyldigDufNummer())
                return "DufNummer";
            if (nin.ErGyldigHNummer())
                return "HNummer";
            if (nin.ErGyldigFHNummer())
                return "FHNummer";
        }
        var testtype = nin.ErGyldigTenorTestNummer() ? "TenorTestNummer"
            : nin.ErGyldigSyntetiskTestNummer() ? "SyntPopTestNummer"
            : "";

        if (basetype == "" && testtype == "")
            return "Ukjent";
        if (testtype != "" && basetype == "")
            return $"Ulovlig testnummer, kun {testtype}";
        return testtype == "" ? basetype : $"{testtype},{basetype}";

    }
}