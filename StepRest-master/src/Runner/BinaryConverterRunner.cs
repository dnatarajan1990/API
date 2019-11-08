using RA.Exceptions;
using StepRest.lib.Extnsions;

namespace StepRest.Runner
{
    class BinaryConverterRunner : DefaultWebRunner
    {
        protected override IRunner Refresh()
            => new BinaryConverterRunner();


        [Given(regex: @"^a (?<browser>chrome|firefox) browser$")]
        public new void GivenBrowser(string browser) => base.GivenBrowser(browser);


        [When(regex: @"^i go to binary to text page$")]
        public void GoToBinToText()
            => GoToUrl("https://www.rapidtables.com/convert/number/binary-to-ascii.html");

        [When(regex: @"^i enter ""(?<input>[^""]+)"" in binary textbox$")]
        public void EnterIntoBinaryTextbox(string input)
            => FindBy("id", "bin").ScrollTo(Driver).SendKeys(input);

        [When(regex: @"^i click convert$")]
        public void ClickConvert()
            => FindBy("xpath", @"//button[starts-with(@title, 'Conver')]").ScrollTo(Driver).Click();

        [Then(regex: @"^the (?<op>binary|text) box shows ""(?<expected>[^""]+)""$")]
        public void OutputIs(string op,string expected)
        {
            string actual = op switch
            {
                "binary" => FindBy("id", "bin").GetAttribute("value"),
                "text" => FindBy("id", "txt").GetAttribute("value")
            };
            if (!actual.Equals(expected))
                throw new AssertException(string.Format("Expected:\"{0}\", Actual:\"{1}\"", expected, actual));
        }
        [Then(regex: @"^the (?<op>binary|text) box shows """"$")]
        public void OutputIsEmpty(string op)
        {
            string actual = op switch
            {
                "binary" => FindBy("id", "bin").GetAttribute("value"),
                "text" => FindBy("id", "txt").GetAttribute("value")
            };
            if (!actual.Equals(""))
                throw new AssertException(string.Format("Expected:\"\", Actual:{0}", actual));
        }

        [Then(regex: @"^close browser$")]
        public new void Close() => base.Close();

        [When(regex: @"^i go to text to binary page$")]
        public void GoToTextToBin()
            => GoToUrl("https://www.rapidtables.com/convert/number/ascii-to-binary.html");

        

        [When(regex: @"^i enter ""(?<input>[^""]+)"" in text textbox$")]
        public void EnterIntoTextTextbox(string input)
            => FindBy("id", "txt").ScrollTo(Driver).SendKeys(input);

    }
}
