namespace Fhi.NINCheck.Test;

public class Tests
{
    [Test]
    public void can_convert_to_an_integer()
    {
        Assert.Multiple(() =>
        {
            Assert.That("unf".ToInt(), Is.EqualTo(0));
            Assert.That(((string)null!).ToInt(), Is.EqualTo(0));
            Assert.That("".ToInt(), Is.EqualTo(0));
            Assert.That("123 45".ToInt(), Is.EqualTo(0));
            Assert.That("12345".ToInt(), Is.EqualTo(12345));
        });
    }

    [Test]
    public void do_not_exist_without_content()
    {
        Assert.Multiple(() =>
        {
            Assert.That("".Exists(), Is.False);
            Assert.That(" ".Exists(), Is.False);
            Assert.That("\t".Exists(), Is.False);
            Assert.That(new string(new char[0]).Exists(), Is.False);
            Assert.That("\0".Exists(), Is.False);
            Assert.That(Convert.ToChar(0x0).ToString().Exists(), Is.False);
        });
    }

    [Test]
    public void are_not_numeric_when_composed_of_anything_but_numbers()
    {
        Assert.That(" 3".IsNumeric(), Is.False);
        Assert.That("text".IsNumeric(), Is.False);
    }
}