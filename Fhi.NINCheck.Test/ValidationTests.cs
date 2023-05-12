using System.Net;

namespace Fhi.NINCheck.Test;

internal class ValidationTests
{
    #region Test data - generated numbers to exercise the entire validation logic
    
    private readonly List<string> validFHnrs = new()
    {
        "81212121223",
        "94545456561",
    };


    private readonly List<string> validDUFnrs = new()
    {
        "200112345609",
        "201017238203",
        "200816832910",
    };


    private readonly List<string> invalidFHnrs = new()
    {
        "71212121229",
        "",
        "1898z388020",
    };




    #endregion
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

    [TestCase("28894698995")]
    public void Can_validate_a_Tenornummer(string nin)
    {
        Assert.That(nin.ErGyldigTenorTestNummer(), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
    }

    [TestCase("31739556891")]
    public void Can_validate_a_SyntPopTestNumber(string nin)
    {
        Assert.That(nin.ErGyldigSyntetiskTestNummer(), $"{nin.FormatWith()} var ugyldig, failed at {Validation.LastFailedStep}");
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

    [Test]
    public void Can_validate_a_fhnummer()
    {
        validFHnrs.ForEach(fhnr =>
            Assert.That(fhnr.ErGyldigFHNummer(),
                "{0} var ugyldig".FormatWith(fhnr))
        );
    }

    [Test]
    public void Can_find_invalid_fhnumbers()
    {
        invalidFHnrs.ForEach(fhnr =>
            Assert.That(fhnr.ErGyldigFHNummer(), Is.False,
                "{0} skal ikke være gyldig".FormatWith(fhnr))
        );
    }

}