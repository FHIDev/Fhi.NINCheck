namespace Fhi.NINCheck.Test;

internal class ValidationTests
{
    [TestCase("55076500565", "Ukjent")]
    [TestCase("12345678901", "Ukjent")]
    [TestCase("037422972082", "Ukjent")]
    [TestCase("01112835470", "FNummer")]
    [TestCase("200112345609", "DufNummer")]
    [TestCase("81212121223", "FHNummer")]
    [TestCase("17454026641", "HNummer")]
    [TestCase("28894698995", "TenorTestNummer,FNummer")]
    [TestCase("68126952442", "DNummer")]
    [TestCase("31739556891", "SyntPopTestNummer,FNummer")]
    public void CheckTypeOfNumber(string nin, string expected)
    {
        TestContext.Out.WriteLine($"Testing {nin}, is type : {nin.CheckNinType()}, FeilInfo {Fhi.NINCheck.Validation.LastFailedStep}");
        Assert.That(nin.CheckNinType(), Is.EqualTo(expected));
    }

    [TestCase("01814099829", 1940)]  // Intervallet 900-999 (Personer født mellom 1940 og 1999), Issue 24
    [TestCase("01844098762", 1940)]
    [TestCase("03014595561", 1945)] // Intervallet 900-999 (Personer født mellom 1940 og 1999)
    [TestCase("18076591411", 1965)]
    [TestCase("28049064480", 1890)] // Intervallet 500-749 (Personer født mellom 1854 og 1899)
    [TestCase("28119770779", 1897)]
    [TestCase("07020554821", 2005)] // Intervallet 500-999 (Personer født mellom 2000 og 2039)
    [TestCase("12121064195", 2010)]
    [TestCase("22067510015", 1975)]  // Intervallet 000-499 (Personer født mellom 1900 og 1999)
    [TestCase("15038512797", 1985)]
    public void Check_IsValidForIntervalsWithCorrectYear(string nin, int year)
    {
        Assert.That(nin.ErGyldigNin(false), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
        var ninchecker = new NinChecker(nin);
        Assert.That(ninchecker.RealYear, Is.EqualTo(year));
    }

    [TestCase("01814099829")]
    [TestCase("01844098762")]
    [TestCase("200112345609")] // Duf
    [TestCase("68126952442")] // Dnr
    [TestCase("70090678378")]
    [TestCase("67016464373")]
    [TestCase("31739556891")] // Syntpop
    [TestCase("28894698995")] // Tenor
    [TestCase("01112835470")] // Fnr
    [TestCase("01032078210")]
    [TestCase("01104343909")]
    public void Check_IsValidInTest(string nin)
    {
        Assert.That(nin.ErGyldigNin(false), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("200112345609", true)] // Duf
    [TestCase("68126952442", true)] // Dnr
    [TestCase("70090678378", true)]
    [TestCase("67016464373", true)]
    [TestCase("31739556891", false)] // Syntpop
    [TestCase("28894698995", false)] // Tenor
    [TestCase("01112835470", true)] // Fnr
    [TestCase("01032078210", true)]
    [TestCase("01104343909", true)]
    public void Check_IsValidInProduction(string nin, bool expected)
    {
        Assert.That(nin.ErGyldigNin(true), Is.EqualTo(expected), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("31739556891")] // Syntpop
    [TestCase("28894698995")] // Tenor
    public void Check_IsInvalidInProduction(string nin)
    {
        Assert.That(Validation.ErGyldigNin(nin, isProduction: true), Is.EqualTo(false));
        Assert.That(Validation.LastFailedStep, Is.EqualTo("Syntetiske testnummer er ikke gyldig i produksjon."));
    }

    [TestCase("037422972082")]
    [TestCase("1")]
    [TestCase("111111111111111111111111111")]
    public void Check_IsInvalidAndUknownInProduction(string nin)
    {
        Assert.That(Validation.ErGyldigNin(nin, isProduction: true), Is.EqualTo(false));
        Assert.That(Validation.LastFailedStep, Is.EqualTo("Ukjent NIN eller NHN-ID."));
    }

    [TestCase("1")]
    [TestCase("111111111111111111111111111")]
    public void Check_IsInvalidInTest(string nin)
    {
        Assert.That(Validation.ErGyldigNin(nin, isProduction: false), Is.EqualTo(false));
        Assert.That(Validation.LastFailedStep, Is.EqualTo("Nin må være 11 eller 12 siffer uten mellomrom."));
    }

    [TestCase("130112345609")] // Duf
    [TestCase("68156952442")] // Dnr
    [TestCase("90090678378")]
    [TestCase("67016464376")]
    [TestCase("32739556891")] // Syntpop
    [TestCase("28894698965")] // Tenor
    [TestCase("00112835470")] // Fnr
    [TestCase("01352078210")]
    [TestCase("01104310009")]
    public void Check_IsValidInTestForInvalidNumbers(string nin)
    {
        var result = nin.ErGyldigNin(false);
        TestContext.Out.WriteLine($"Reacted on {Validation.LastFailedStep}");
        Assert.That(result, Is.False, $"{nin.FormatWith()} skulle vært ugyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("17054026641")]
    [TestCase("22095314442")]
    [TestCase("17028338791")]
    [TestCase("18081388020")]
    [TestCase("01112835470")]
    [TestCase("01032078210")]
    [TestCase("01104343909")]
    public void Can_validate_a_fnummer(string fnr)
    {
        Assert.Multiple(() =>
        {
            Assert.That(fnr.ErGyldigFødselsNummer(),
                $"{fnr.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
            Assert.That(fnr.ErGyldigFNummer(),
                $"{fnr.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
        });
    }

    [TestCase("03923248608")]
    [TestCase("28894698995")]
    public void Can_validate_a_Tenornummer(string nin)
    {
        Assert.That(nin.ErGyldigTenorTestNummer(), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("31739556891")]
    public void Is_Invalid_Tenornummer(string nin)
    {
        var result = nin.ErGyldigTenorTestNummer();
        TestContext.Out.WriteLine($"Reacted on {Validation.LastFailedStep}");
        Assert.That(result, Is.False, $"{nin.FormatWith()} skal ikke være gyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("44722264549")]
    [TestCase("31739556891")]
    public void Can_validate_a_SyntPopTestNumber(string nin)
    {
        Assert.That(nin.ErGyldigSyntetiskTestNummer(), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("28894698995")]
    public void Is_Invalid_SyntPopnummer(string nin)
    {
        var result = nin.ErGyldigSyntetiskTestNummer();
        TestContext.Out.WriteLine($"Reacted on {Validation.LastFailedStep}");
        Assert.That(result, Is.False, $"{nin.FormatWith()} skal ikke være gyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("97054026641")]
    [TestCase("z7054026641")]
    [TestCase("18081388093")]
    [TestCase("01112835480")]
    [TestCase("01032078270")]
    [TestCase("11111111100")]
    public void Can_find_invalid_fnumbers(string nin)
    {
        var result = nin.ErGyldigFødselsNummer();
        TestContext.Out.WriteLine($"Reacted on {Validation.LastFailedStep}");
        Assert.That(result, Is.False, $"{nin.FormatWith()} skal ikke være gyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("51106297510")]
    [TestCase("45092528433")]
    [TestCase("68126952442")]
    [TestCase("70090678378")]
    [TestCase("67016464373")]
    public void Can_validate_a_dnummer(string nin)
    {
        Assert.That(nin.ErGyldigDNummer(), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("91106297510")]
    [TestCase("21106297510")]
    [TestCase("45992528433")]
    [TestCase("z1106297510")]
    public void Can_find_invalid_dnumbers(string nin)
    {
        var result = nin.ErGyldigDNummer();
        TestContext.Out.WriteLine($"Reacted on {Validation.LastFailedStep}");
        Assert.That(result, Is.False, $"{nin.FormatWith()} skal ikke være gyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("17454026641")]
    [TestCase("22495314442")]
    [TestCase("17428338791")]
    [TestCase("18481388020")]
    [TestCase("01512835470")]
    public void Can_validate_a_hnummer(string nin)
    {
        Assert.That(nin.ErGyldigHNummer(), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
    }

#pragma warning disable NUnit1001 // The individual arguments provided by a TestCaseAttribute must match the type of the corresponding parameter of the method
    [TestCase(null)]
#pragma warning restore NUnit1001 // The individual arguments provided by a TestCaseAttribute must match the type of the corresponding parameter of the method
    [TestCase("97454026641")]
    [TestCase("18981388020")]
    [TestCase("18181388020")]
    [TestCase("98481388020")]
    [TestCase("z7454026641")]
    [TestCase("204167553610")]
    public void Can_find_invalid_hnumbers(string nin)
    {
        var result = nin.ErGyldigHNummer();
        TestContext.Out.WriteLine($"Reacted on {Validation.LastFailedStep}");
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (nin != null)
            Assert.That(result, Is.False, $"{nin.FormatWith()} skal ikke være gyldig, failed at {Validation.LastFailedStep}");

    }

    [TestCase("81212121223")]
    [TestCase("94545456561")]
    public void Can_validate_a_fhnummer(string nin)
    {
        Assert.That(nin.ErGyldigFHNummer(), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");

    }

#pragma warning disable NUnit1001 // The individual arguments provided by a TestCaseAttribute must match the type of the corresponding parameter of the method
    [TestCase(null)]
#pragma warning restore NUnit1001 // The individual arguments provided by a TestCaseAttribute must match the type of the corresponding parameter of the method
    [TestCase("71212121229")]
    [TestCase("")]
    [TestCase("1898z388020")]
    public void Can_find_invalid_fhnumbers(string nin)
    {
        var result = nin.ErGyldigFHNummer();
        TestContext.Out.WriteLine($"Reacted on {Validation.LastFailedStep}");
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (nin != null)
            Assert.That(result, Is.False, $"{nin.FormatWith()} skal ikke være gyldig, failed at {Validation.LastFailedStep}");
    }

    // [TestCase("204167553610")]
    [TestCase("199201426900")]
    [TestCase("200112345609")]
    [TestCase("201017238203")]
    [TestCase("200816832910")]

    public void Can_validate_a_dufnummer(string nin)
    {
        Assert.That(nin.ErGyldigDufNummer(), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
        int year = nin[..4].ToInt();
        var ninchecker = new NinChecker(nin);
        Assert.That(ninchecker.RealYear, Is.EqualTo(year));

    }

    [TestCase("71212121229")]
    [TestCase("")]
    [TestCase("1898z388020")]
    [TestCase("123411234560")]
    [TestCase("204167553610")]
    [TestCase("500266228604")]
    public void Can_find_invalid_dufnumbers(string nin)
    {
        var result = nin.ErGyldigDufNummer();
        TestContext.Out.WriteLine($"Reacted on {Validation.LastFailedStep}");
        Assert.That(result, Is.False, $"{nin.FormatWith()} skal ikke være gyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("037422972082")]
    public void Can_find_invalid_dufnumbers_and_returns_norwegian_validation_message(string nin)
    {
        var result = nin.ErGyldigDufNummer();
        Assert.That(Validation.LastFailedStep, Is.EqualTo("Ikke sannsynlig årstall: 0374"));
    }

    [TestCase("03923248608")]
    public void CheckElementsOfNumber(string fnr)
    {
        var c = new NinChecker(fnr);
        TestContext.Out.WriteLine($" TechValid {c.IsTechValid}");
        TestContext.Out.WriteLine($" Fnr {c.IsFNummer}");
        TestContext.Out.WriteLine($" Dnr {c.IsDNummer}");
    }

    [TestCaseSource(nameof(ThatBirthDayWorksForCorrectTypesInput))]
    public void ThatBirthDayWorksForCorrectTypes(string nin, DateTime expectedBirthDate)
    {
        var c = new NinChecker(nin);
        Assert.That(c.Birthdate, Is.EqualTo(expectedBirthDate));
        Assert.That(c.HasBirthdate, Is.True);
    }

    private static object[] ThatBirthDayWorksForCorrectTypesInput =
    {
        new object[] { "17454026641", new DateTime(1940, 5, 17) }, // HNr
        new object[] { "51106297510", new DateTime(1962, 10, 11) }, // DNr
        new object[] { "17054026641", new DateTime(1940, 5, 17) }, // FNr
    };

    [TestCase("200112345609")] // Duf
    [TestCase("81212121223")]  // Fhn
    public void ThatBirthDayIgnoresInvalidTypes(string nin)
    {
        var c = new NinChecker(nin);
        Assert.That(c.Birthdate, Is.Null);
        Assert.That(c.HasBirthdate, Is.False);
    }

    [TestCase("200112345609")] // Duf
    [TestCase("81212121223")]  // Fhn
    public void ThatBirthDayIgnoresInvalidTypesUsingExtensionmethods(string nin)
    {
        Assert.That(nin.Birthdate(), Is.Null);
        Assert.That(nin.HasBirthdate, Is.False);
    }
}