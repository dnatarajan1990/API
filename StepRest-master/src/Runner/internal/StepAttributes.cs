using System;

namespace StepRest.Runner
{
    public class GivenAttribute : Step
    {
        public GivenAttribute(string regex)
        {
        }
    }
    public class WhenAttribute : Step
    {
        public WhenAttribute(string regex)
        {
        }
    }
    public class ThenAttribute : Step
    {
        public ThenAttribute(string regex)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class Step : Attribute
    {
        
    }
}
