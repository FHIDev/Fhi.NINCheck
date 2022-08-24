using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhi.NINCheck.Test
{
    internal class ValidationTests
    {
        #region Test data - generated numbers to exercise the entire validation logic



        private readonly List<string> validFnrs = new List<string>()
        {
            "17054026641",
            "22095314442",
            "17028338791",
            "18081388020",
            "01112835470",
            "01032078210",
            "01104343909",
        };

        private readonly List<string> validDnrs = new List<string>()
        {
            "51106297510",
            "45092528433",
            "68126952442",
            "70090678378",
            "67016464373",
        };

        private readonly List<string> validHnrs = new List<string>()
        {
            "17454026641",
            "22495314442",
            "17428338791",
            "18481388020",
            "01512835470",
        };

        private readonly List<string> validFHnrs = new List<string>()
        {
            "81212121223",
            "94545456561",
        };


        private readonly List<string> validDUFnrs = new List<string>()
        {
            "200112345609",
            "201017238203",
            "200816832910",
        };


        private readonly List<string> invalidFnrs = new List<string>()
        {
            "97054026641",
            "z7054026641",
            "18081388093",
            "01112835480",
            "01032078270",
            "11111111100",
        };

        private readonly List<string> invalidDnrs = new List<string>()
        {
            "91106297510",
            "21106297510",
            "45992528433",
            "z1106297510",
        };

        private readonly List<string> invalidHnrs = new List<string>()
        {
            "97454026641",
            "18981388020",
            "18181388020",
            "98481388020",
            "z7454026641",
        };

        private readonly List<string> invalidFHnrs = new List<string>()
        {
            "71212121229",
            "",
            "1898z388020",
        };


       

        #endregion

        public void can_validate_a_fnummer()
        {
            validFnrs.ForEach(fnr =>
                Assert.That(fnr.ErGyldigFødselsNummer(),
                    "{0} var ugyldig".FormatWith(fnr))
            );

            validFnrs.ForEach(fnr =>
                Assert.That(fnr.ErGyldigFNummer(),
                    "{0} var ugyldig".FormatWith(fnr))
            );
        }


        [Test]
        public void can_find_invalid_fnumbers()
        {
            invalidFnrs.ForEach(fnr =>
                Assert.That(fnr.ErGyldigFødselsNummer(), Is.False,
                    "{0} skal ikke være gyldig".FormatWith(fnr))
            );
        }

        [Test]
        public void can_validate_a_dnummer()
        {
            validDnrs.ForEach(dnr =>
                Assert.That(dnr.ErGyldigDNummer(),
                    "{0} var ugyldig".FormatWith(dnr))
            );
        }

        [Test]
        public void can_find_invalid_dnumbers()
        {
            invalidDnrs.ForEach(dnr =>
                Assert.That(dnr.ErGyldigDNummer(), Is.False,
                    "{0} skal ikke være gyldig".FormatWith(dnr))
            );
        }

        [Test]
        public void can_validate_a_hnummer()
        {
            validHnrs.ForEach(hnr =>
                Assert.That(hnr.ErGyldigHNummer(),
                    "{0} var ugyldig".FormatWith(hnr))
            );
        }

        [Test]
        public void can_find_invalid_hnumbers()
        {
            invalidHnrs.ForEach(hnr =>
                Assert.That(hnr.ErGyldigHNummer(), Is.False,
                    "{0} skal ikke være gyldig".FormatWith(hnr))
            );
        }

        [Test]
        public void can_validate_a_fhnummer()
        {
            validFHnrs.ForEach(fhnr =>
                Assert.That(fhnr.ErGyldigFHNummer(),
                    "{0} var ugyldig".FormatWith(fhnr))
            );
        }

        [Test]
        public void can_find_invalid_fhnumbers()
        {
            invalidFHnrs.ForEach(fhnr =>
                Assert.That(fhnr.ErGyldigFHNummer(), Is.False,
                    "{0} skal ikke være gyldig".FormatWith(fhnr))
            );
        }

    }
}
