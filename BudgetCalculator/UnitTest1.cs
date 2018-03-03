using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq;

namespace BudgetCalculator
{
    [TestClass]
    public class UnitTest1
    {
        public Accounting target;
        private IRepository<Budget> _repository = Substitute.For<IRepository<Budget>>();

        [TestInitialize]
        public void TestInit()
        {
            target = new Accounting(_repository);
        }

        [TestMethod]
        public void no_budgets()
        {
            GivenBudgets();

            TotalAmountShouldBe(0, new DateTime(2018, 3, 1), new DateTime(2018, 3, 1));
        }

        [TestMethod]
        public void 找不到這個月的預算_拿到0()
        {
            GivenBudgets(new Budget() { YearMonth = "201801", Amount = 62 });
            TotalAmountShouldBe(0, new DateTime(2018, 2, 1), new DateTime(2018, 2, 15));
        }

        [TestMethod]
        public void 時間起訖不合法()
        {
            GivenBudgets();
            var start = new DateTime(2018, 3, 1);
            var end = new DateTime(2018, 2, 1);

            Action actual = () => target.TotalAmount(start, end);

            actual.Should().Throw<ArgumentException>();
        }

        [TestMethod]
        public void 當一月預算為62_一月一號到一月十五號_預算拿到30()
        {
            GivenBudgets(new Budget() { YearMonth = "201801", Amount = 62 });
            TotalAmountShouldBe(30, new DateTime(2018, 1, 1), new DateTime(2018, 1, 15));
        }

        [TestMethod]
        public void 當一月預算為62_一月一號到一月三十一號_預算拿到62()
        {
            GivenBudgets(new Budget() { YearMonth = "201801", Amount = 62 });
            TotalAmountShouldBe(62, new DateTime(2018, 1, 1), new DateTime(2018, 1, 31));
        }

        [TestMethod]
        public void 當一月預算為62_二月預算為0_三月預算為62_一月一號到三月十號_預算拿到82()
        {
            GivenBudgets(
                new Budget() { YearMonth = "201801", Amount = 62 },
                new Budget() { YearMonth = "201802", Amount = 0 },
                new Budget() { YearMonth = "201803", Amount = 62 }
                );

            TotalAmountShouldBe(82, new DateTime(2018, 1, 1), new DateTime(2018, 3, 10));
        }

        [TestMethod]
        public void 當一月預算為62_二月預算為280_一月一號到二月二十八號_預算拿到342()
        {
            GivenBudgets(new Budget() { YearMonth = "201801", Amount = 62 },
                new Budget() { YearMonth = "201802", Amount = 280 });
            TotalAmountShouldBe(342, new DateTime(2018, 1, 1), new DateTime(2018, 2, 28));
        }

        [TestMethod]
        public void 當一月預算為62_二月預算為280_三月預算為62_一月一號到三月十號_預算拿到362()
        {
            GivenBudgets(new Budget() { YearMonth = "201801", Amount = 62 },
                new Budget() { YearMonth = "201802", Amount = 280 },
                new Budget() { YearMonth = "201803", Amount = 62 });
            TotalAmountShouldBe(362, new DateTime(2018, 1, 1), new DateTime(2018, 3, 10));
        }

        [TestMethod]
        public void 當十二月預算為310一月預算為310_二月預算為280_三月預算為310_十二月一號到三月十號_預算拿到1000()
        {
            GivenBudgets(new Budget() { YearMonth = "201712", Amount = 310 },
                new Budget() { YearMonth = "201801", Amount = 310 },
                new Budget() { YearMonth = "201802", Amount = 280 },
                new Budget() { YearMonth = "201803", Amount = 310 });
            TotalAmountShouldBe(1000, new DateTime(2017, 12, 1), new DateTime(2018, 3, 10));
        }

        private void GivenBudgets(params Budget[] budgets)
        {
            _repository.GetAll().Returns(budgets.ToList());
        }

        private void TotalAmountShouldBe(int expected, DateTime start, DateTime end)
        {
            target.TotalAmount(start, end).Should().Be(expected);
        }
    }
}