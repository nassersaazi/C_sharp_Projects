using System;
using Xunit;
using sampleTDDAppLibrary.Logic;

namespace sampleTDDApp.UnitTests
{
    public class PhoneValidatorTest
    {
        [Theory]
        [InlineData("077865ty3321")]
        [InlineData("t0778653321")]
        public void should_returnTrue_if_numberContainsLetters(string number)
        {
            //Arrange  
            var validator = new PhoneValidator();
            
            bool actual;
           
            //Act  
            actual = validator.NumberContainsLetters(number);
           
            //Assert  
            Assert.True(actual,$"The number {number} contains letters but test says it doesn't");
        }
    }
}
