using FakeItEasy;
using TPLDataFlowDemo;

namespace NamesProcessor.Tests
{
    [TestClass]
    public class NamesProcessorTests
    {
        private IValidatorsFactory _validatorsFactory;
        private IBlockedNameService _blockedNameService;
        private INamesDb _namesDb;
        private INamesProcessor _namesProcessor;

        [TestInitialize]
        public void TestInitialize()
        {
            _blockedNameService = A.Fake<IBlockedNameService>();
            _validatorsFactory = new ValidatorsFactory(_blockedNameService);
            _namesDb = A.Fake<INamesDb>();
            _namesProcessor = new TPLDataFlowDemo.NamesProcessor(_validatorsFactory, _namesDb);
        }

        [TestMethod]
        public async Task ProcessName_validInput()
        {
            var name = "tester";
            A.CallTo(() => _blockedNameService.IsBlocked(name)).Returns((false, null));
            var result = await _namesProcessor.ProcessName(name);

            Assert.IsTrue(result > 0);
            A.CallTo(() => _namesDb.SaveName(name)).MustHaveHappened();
        }

        [TestMethod]
        public async Task ProcessName_notValidInput()
        {
            var name = "";
            A.CallTo(() => _blockedNameService.IsBlocked(name)).Returns((false, null));

            try
            {
                await _namesProcessor.ProcessName(name);
            }
            catch (Exception e)
            {
                Assert.AreEqual(e.Message, $"The name {name} is not valid!");
            }
            
            A.CallTo(() => _namesDb.SaveName(name)).MustNotHaveHappened();
        }
    }
}
