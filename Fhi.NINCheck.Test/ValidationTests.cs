namespace Fhi.NINCheck.Test;

internal class ValidationTests
{
    [TestCase("55076500565", "Ukjent")]
    [TestCase("12345678901", "Ukjent")]
    [TestCase("01112835470", "FNummer")]
    [TestCase("200112345609","DufNummer")]
    [TestCase("81212121223","FHNummer")]
    [TestCase("17454026641","HNummer")]
    [TestCase("28894698995","TenorTestNummer,FNummer")] 
    [TestCase("68126952442", "DNummer")]
    [TestCase("31739556891", "SyntPopTestNummer,FNummer")]
    public void CheckTypeOfNumber(string nin, string expected)
    {
        TestContext.WriteLine($"Testing {nin}, is type : {nin.CheckNinType()}, FeilInfo {Fhi.NINCheck.Validation.LastFailedStep}");
        Assert.That(nin.CheckNinType(), Is.EqualTo(expected));
    }

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

    [TestCase("200112345609",true)] // Duf
    [TestCase("68126952442",true)] // Dnr
    [TestCase("70090678378", true)]
    [TestCase("67016464373", true)]
    [TestCase("31739556891", false)] // Syntpop
    [TestCase("28894698995",false)] // Tenor
    [TestCase("01112835470", true)] // Fnr
    [TestCase("01032078210", true)]
    [TestCase("01104343909", true)]
    public void Check_IsValidInProduction(string nin,bool expected)
    {
        Assert.That(nin.ErGyldigNin(true),Is.EqualTo(expected), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
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
        TestContext.WriteLine($"Reacted on {Validation.LastFailedStep}");
        Assert.That(result,Is.False, $"{nin.FormatWith()} skulle vært ugyldig, failed at {Validation.LastFailedStep}");
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
        TestContext.WriteLine($"Reacted on {Validation.LastFailedStep}");
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
        TestContext.WriteLine($"Reacted on {Validation.LastFailedStep}");
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
        TestContext.WriteLine($"Reacted on {Validation.LastFailedStep}");
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
        TestContext.WriteLine($"Reacted on {Validation.LastFailedStep}");
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

    [TestCase("97454026641")]
    [TestCase("18981388020")]
    [TestCase("18181388020")]
    [TestCase("98481388020")]
    [TestCase("z7454026641")]
    public void Can_find_invalid_hnumbers(string nin)
    {
        var result = nin.ErGyldigHNummer();
        TestContext.WriteLine($"Reacted on {Validation.LastFailedStep}");
        Assert.That(result, Is.False, $"{nin.FormatWith()} skal ikke være gyldig, failed at {Validation.LastFailedStep}");

    }

    [TestCase("81212121223")]
    [TestCase("94545456561")]
    public void Can_validate_a_fhnummer(string nin)
    {
        Assert.That(nin.ErGyldigFHNummer(), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");

    }
    [TestCase("71212121229")]
    [TestCase("")]
    [TestCase("1898z388020")]
    public void Can_find_invalid_fhnumbers(string nin)
    {
        var result = nin.ErGyldigFHNummer();
        TestContext.WriteLine($"Reacted on {Validation.LastFailedStep}");
        Assert.That(result, Is.False, $"{nin.FormatWith()} skal ikke være gyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("200112345609")]
    [TestCase("201017238203")]
    [TestCase("200816832910")]
    public void Can_validate_a_dufnummer(string nin)
    {
        Assert.That(nin.ErGyldigDufNummer(), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("71212121229")]
    [TestCase("")]
    [TestCase("1898z388020")]
    [TestCase("123411234560")]
    public void Can_find_invalid_dufnumbers(string nin)
    {
        var result = nin.ErGyldigDufNummer();
        TestContext.WriteLine($"Reacted on {Validation.LastFailedStep}");
        Assert.That(result, Is.False, $"{nin.FormatWith()} skal ikke være gyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("03923248608")]
    public void CheckElementsOfNumber(string fnr)
    {
        var c = new NinChecker(fnr);
        TestContext.WriteLine($" TechValid {c.IsTechValid}");
        TestContext.WriteLine($" Fnr {c.IsFNummer}");
        TestContext.WriteLine($" Dnr {c.IsDNummer}");
    }



}