using System.Threading.Tasks.Dataflow;
using FakeItEasy;
using TPLDataFlowDemo;

namespace NamesProcessor.Tests
{
    [TestClass]
    public class ValidatorsTests
    {
        private IValidatorsFactory _validatorsFactory;
        private IBlockedNameService _blockedNameService;
        [TestInitialize]
        public void TestInitialize()
        {
            _blockedNameService = A.Fake<IBlockedNameService>();
            _validatorsFactory = new ValidatorsFactory(_blockedNameService);
        }

        [TestMethod]
        public void CreateNameValidatorTest_validInput()
        {
            var nameValidator = _validatorsFactory.CreateNameValidator();
            var name = "tester";
            nameValidator.Post(name);
            var result = nameValidator.Receive();

            Assert.IsTrue(result.isValid);
            Assert.AreEqual(result.name, name);
        }

        [TestMethod]
        public void CreateNameValidatorTest_notValidInput()
        {
            var nameValidator = _validatorsFactory.CreateNameValidator();
            var name = "";
            nameValidator.Post(name);
            var result = nameValidator.Receive();

            Assert.IsFalse(result.isValid);
            Assert.AreEqual(result.name, name);
        }

        [TestMethod]
        public async Task CreateBlockedNamesValidatorTest_validInput()
        {
            var name = "tester";
            A.CallTo(() => _blockedNameService.IsBlocked(name)).Returns((false, null));
            var blockedValidator = _validatorsFactory.CreateBlockedNamesValidator();

             blockedValidator.Post((name,true));

             var result =await blockedValidator.ReceiveAsync();
             Assert.IsTrue(result.isValid);
             Assert.AreEqual(result.name, name);
        }

        [TestMethod]
        public async Task CreateBlockedNamesValidatorTest_notValidInput()
        {
            var name = "tester";
            A.CallTo(() => _blockedNameService.IsBlocked(name)).Returns((true, "blocked"));
            var blockedValidator = _validatorsFactory.CreateBlockedNamesValidator();

            blockedValidator.Post((name,true));

            var result =await blockedValidator.ReceiveAsync();
            Assert.IsFalse(result.isValid);
            Assert.AreEqual(result.name, name);
        }
    }
}