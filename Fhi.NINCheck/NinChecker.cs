namespace Fhi.NINCheck;

/// <summary>
/// Validating a NIN
/// </summary>
public class NinChecker
{
    private string Nin { get; }

    private int day;
    private int month;
    int year;
    public int RealYear { get; }
    public int RealDay => IsDayDnrModified ? day - DnrDayOffset : day;
    public int RealMonth => month - MonthModifier;
    public bool IsDayDnrModified { get; private set; }
    public bool IsMonthModified => MonthModifier != 0;

    /// <summary>
    /// Verifies that the number is technically valid. Length and characters.
    /// </summary>
    public bool IsTechValid { get; private set; } = true;
    public string ErrorMessage { get; private set; } = "";

    public bool IsValidFnrDnr => IsTechValid && IsValidKontrollSiffre;

    public bool IsDufNumber => Nin.Length == 12 && DufYearCheck();

    private const int DnrDayOffset = 40;
    private const int HnrMonthOffset = 40;
    private const int TenorMonthOffset = 80;
    private const int SyntPopMonthOffset = 65;

    public bool IsValidDate => IsValidDay && IsValidMonth && IsValidYear;
    public bool IsValidYear => RealYear > 1854 && RealYear <= DateTime.Now.Year;
    

    public bool IsDNummer => IsValidFnrDnr && IsValidDate && day > DnrDayOffset;
    public bool IsHNummer => IsTechValid && IsValidDate && month is > HnrMonthOffset and < HnrMonthOffset + 12;
    public bool IsSyntPopNummer => IsValidFnrDnr && IsValidDate && month is > SyntPopMonthOffset and < SyntPopMonthOffset + 12;

    public bool IsTenorNummer => IsValidFnrDnr && IsValidDate && month is > TenorMonthOffset and < TenorMonthOffset + 12;

    public bool IsFNummer => IsValidFnrDnr && !IsDNummer && !IsHNummer && !IsDufNumber && IsValidDate;

    public bool IsValidDay =>
        day switch
        {
            > 0 and < 32 => true,
            > DnrDayOffset and < DnrDayOffset + 32 => true,
            _ => false
        };

    public bool IsValidKontrollSiffre => CheckKontrollSiffre();

    public bool IsValidMonth => MonthModifier != -1;

    private int MonthModifier =>
        month switch
        {
            > 0 and < 13 => 0,
            > HnrMonthOffset and < HnrMonthOffset + 12 => HnrMonthOffset,
            > SyntPopMonthOffset and < SyntPopMonthOffset + 12 => SyntPopMonthOffset,
            > TenorMonthOffset and < TenorMonthOffset + 12 => TenorMonthOffset,
            _ => -1
        };

    /// <summary>
    /// Checking a Norwegian Identification Number (NIN) for validity.
    /// </summary>
    /// <param name="nin"></param>
    public NinChecker(string nin)
    {
        Nin = nin;
        if (CheckLength() && CheckCharacters())
        {
            Parse();
            RealYear = FindCorrectYear();
            
        }
    }

    private void Parse()
    {
        day = Nin[..2].ToInt();
        month = Nin[2..4].ToInt();
        year = Nin[4..6].ToInt();
        IsDayDnrModified = day > 40;
    }

    public bool CheckLength()
    {
        if (string.IsNullOrEmpty(Nin) || (Nin.Length != 11 && Nin.Length != 12) || Nin.Contains(' '))
        {
            ErrorMessage = "Nin must be 11 or 12 digits long with no spaces";
            IsTechValid = false;
            return false;
        }
        return true;
    }

    public bool CheckCharacters()
    {
        try
        {
            _ = long.Parse(Nin);
        }
        catch (FormatException)
        {
            ErrorMessage = $"Nin contains invalid characters";
            IsTechValid = false;
            return false;
        }
        return true;
    }

    

    private int FindCorrectYear()
    {
        var individ = Nin.Substring(6, 3).ToInt();
        return year switch
        {
            <= 40 when individ < 500 => 1900 + year,
            <= 40 when true => 2000 + year,
            >= 54 when individ is >= 500 and <= 749 => 1800 + year,
            _ => 1900 + year
        };
    }

    private bool DufYearCheck()
    {
        var dufyear = Nin.Substring(0, 4).ToInt();
        bool goodYear = dufyear >= 1854 && dufyear <= DateTime.Now.Year;
        if (!goodYear)
        {
            ErrorMessage = $"Ikke sannsynlig årstall: {year}";
            return false;
        }
        return true;
    }

    private bool CheckKontrollSiffre()
    {
        var v1 = new[] { 3, 7, 6, 1, 8, 9, 4, 5, 2 };
        var v2 = new[] { 5, 4, 3, 2, 7, 6, 5, 4, 3, 2 };

        var s1 = 0;
        var s2 = 0;

        for (var i = 0; i < 9; i++)
        {
            s1 += Convert.ToInt16(Nin.Substring(i, 1)) * v1[i];
        }

        var r1 = s1 % 11;
        var k1 = 11 - r1;

        switch (k1)
        {
            case 11:
                k1 = 0;
                break;
            case 10:
                SetError();
                return false;
        }

        for (var i = 0; i < 10; i++)
        {
            s2 += Convert.ToInt16(Nin.Substring(i, 1)) * v2[i];
        }

        var r2 = s2 % 11;
        var k2 = 11 - r2;

        switch (k2)
        {
            case 11:
                k2 = 0;
                break;
            case 10:
                SetError();
                return false;
        }
        bool isk1 = Convert.ToInt16(Nin.Substring(9, 1)) == k1;
        bool isk2 = Convert.ToInt16(Nin.Substring(10, 1)) == k2;
        var result = isk1 && isk2;
        if (!result)
        {
            SetError();
        }
        return result;

        void SetError()
        {
            ErrorMessage = $"Kontrollsiffer is not valid";
        }
    }
}