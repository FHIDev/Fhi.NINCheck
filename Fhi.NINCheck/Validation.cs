namespace Fhi.NINCheck;

/// <summary>
///     Extention methods for validation.
/// </summary>
public static class Validation
{
    public static string LastFailedStep { get; private set; } = "";
    /// <summary>
    ///     Validates a given fødselsnummer.
    /// </summary>
    /// <param name="fnr">The fødselsnummer to validate.</param>
    /// <returns>Whether the fødselsnummer was valid or not.</returns>
    public static bool ErGyldigFødselsNummer(this string fnr) => fnr.ErGyldigFNummer();

    /// <summary>
    ///     Validates a given fødselsnummer.
    /// </summary>
    /// <param name="nin">The fødselsnummer to validate.</param>
    /// <returns>Whether the fødselsnummer was valid or not.</returns>
    public static bool ErGyldigFNummer(this string nin)
    {
        return CheckLength(nin)
               && CheckCharacters(nin)
               && CheckDateWithMonth(nin, ExtractRawMonth(nin))
               && CheckKontrollSiffre(nin);
    }

    /// <summary>
    ///     Validates a given d-nummer.
    /// </summary>
    /// <param name="dnr">D-nummer to validate.</param>
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
        return CheckLength(nin)
               && CheckCharacters(nin)
               && CheckDay(nin, 40, out var day)
               && CheckDateWithDay(nin, day)
               && CheckKontrollSiffre(nin);
    }

    /// <summary>
    ///     Validates a given h-nummer.
    /// </summary>
    /// <param name="hnr">H-nummer to validate.</param>
    /// <returns>Whther the h-nummer is valid or not.</returns>
    /// <remarks>
    ///     Et H-nummer er ellevesifret, som ordinære fødselsnummer, og består av en
    ///     modifisert sekssifret fødselsdato og et femsifret personnummer. Fødselsdatoen
    ///     modifiseres ved at det legges til 4 på det tredje sifferet: en person født 1. januar 1980
    ///     får dermed fødselsdato 014180, mens en som er født 31. januar 1980 får 314180.
    /// </remarks>
    public static bool ErGyldigHNummer(this string nin)
    {
        return CheckLength(nin)
               && CheckCharacters(nin)
               && CheckMonth(nin, 40, out var month)
               && CheckDateWithMonth(nin, month)
               && CheckKontrollSiffre(nin);
    }

    /// <summary>
    ///    Validates a given syntetisk testnummer fra SyntPop.  Reglen er mnd + 65
    /// </summary>
    /// <param name="nin"></param>
    /// <returns></returns>
    public static bool ErGyldigSyntetiskTestNummer(this string nin)
    {
        return CheckLength(nin)
            && CheckCharacters(nin)
            && CheckMonth(nin, 65, out var month)
            && CheckDateWithMonth(nin, month)
            && CheckKontrollSiffre(nin);
    }

    /// <summary>
    ///    Validates a given syntetisk testnummer fra SyntPop.  Reglen er mnd + 65
    /// </summary>
    /// <param name="nin"></param>
    /// <returns></returns>
    public static bool ErGyldigTenorTestNummer(this string nin)
    {
        return CheckLength(nin)
               && CheckCharacters(nin)
               && CheckMonth(nin, 80, out var month)
               && CheckDateWithMonth(nin, month)
               && CheckKontrollSiffre(nin);
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
        if (string.IsNullOrEmpty(fhnr) || !(fhnr.Length == 11 && fhnr.IsNumeric()))
        {
            return false;
        }

        var fhNummer = fhnr.Substring(0, 9);

        var nr = fhNummer.ToInt();
        var harRiktigTallSerie = 800000000 <= nr && nr <= 999999999;

        return harRiktigTallSerie && CheckKontrollSiffre(fhnr);
    }



    private static bool CheckDateWithMonth(string nin, int month)
    {
        try
        {
            var day = int.Parse(nin[..2]);
            var year = int.Parse(nin.Substring(4, 2));
            var individ = nin.Substring(6, 3).ToInt();
            _ = new DateOnly(FindCorrectYear(year,individ), month,day);
        }
        catch (ArgumentOutOfRangeException)
        {
            LastFailedStep = nameof(CheckDateWithMonth);
            return false;
        }

        return true;
    }

    private static bool CheckDateWithDay(string nin, int day)
    {
        try
        {
            var month = int.Parse(nin.Substring(2,2));
            var year = int.Parse(nin.Substring(4, 2));
            var individ = nin.Substring(6, 3).ToInt();
            _ = new DateOnly(FindCorrectYear(year, individ), month, day);
        }
        catch (ArgumentOutOfRangeException)
        {
            LastFailedStep = nameof(CheckDateWithDay);
            return false;
        }

        return true;
    }

    private static int FindCorrectYear(int year, int individ)
    {
        return year switch
        {
            <= 40 when individ < 500 => 1900 + year,
            <= 40 when true => 2000 + year,
            >= 54 when individ is >= 500 and <= 749 => 1800 + year,
            >= 54 when !(individ >= 500 && false) => 1900 + year,
            _ => 1900 + year
        };
    }



    private static bool CheckMonth(string nin, int lowLimit, out int month)
    {
        month = ExtractRawMonth(nin);

        if (month < lowLimit + 1 || month > lowLimit + 12)
        {
            LastFailedStep = nameof(CheckMonth);
            return false;
        }

        month -= lowLimit;
        return true;
    }

    private static int ExtractRawMonth(string nin) => int.Parse(nin.Substring(2, 2));

    private static int ExtractMonth(int rawMonth)
    {
        return rawMonth switch
        {
            >= 65 and <80 => rawMonth - 65,
            >= 80 => rawMonth - 80,
            _ => rawMonth
        };
    }

    private static bool CheckDay(string nin, int lowLimit, out int day)
    {
        int[] daysInMonth = {31,29,31,30,31,30,31,31,30,31,30,31} ;
        
        day = int.Parse(nin[..2]);
        int month = ExtractMonth(ExtractRawMonth(nin));
        if (month < 1 || month > 12)
        {
            LastFailedStep = nameof(CheckDay)+$" - Exceeded number of months {month}";
            return false;
        }
        if (day < lowLimit + 1 || day > lowLimit + daysInMonth[month-1])
        {
            LastFailedStep = nameof(CheckDay);
            return false;
        }

        day -= lowLimit;
        return true;
    }

    private static bool CheckCharacters(string hnr)
    {
        try
        {
            long.Parse(hnr);
        }
        catch (FormatException)
        {
            LastFailedStep = nameof(CheckCharacters);
            return false;
        }

        return true;
    }

    private static bool CheckLength(string nin)
    {
        if (string.IsNullOrEmpty(nin) || nin.Length != 11 || nin.Contains(' '))
        {
            LastFailedStep = nameof(CheckLength);
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

        if (k1 == 11)
        {
            k1 = 0;
        }
        else if (k1 == 10)
        {
            return false;
        }

        for (var i = 0; i < 10; i++)
        {
            s2 += Convert.ToInt16(fnr.Substring(i, 1)) * v2[i];
        }

        var r2 = s2 % 11;
        var k2 = 11 - r2;

        if (k2 == 11)
        {
            k2 = 0;
        }
        else if (k2 == 10)
        {
            return false;
        }

        if ((Convert.ToInt16(fnr.Substring(9, 1)) == k1 && Convert.ToInt16(fnr.Substring(10, 1)) == k2))
        {
            return true;
        }

        return false;
    }
}