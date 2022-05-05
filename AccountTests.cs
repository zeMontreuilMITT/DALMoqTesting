using DALBank.BLL;
using DALBank.DAL;
using DALBank.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DALBankTests
{
    [TestClass]
    public class AccountTests
    {
        private AccountBusinessLogic accountBl;
        private List<Account> allAccounts;

        [TestInitialize]
        public void Initialize()
        {
            // setup to create a fake database that returns a fake account
            Mock<AccountRepository> mockRepo = new Mock<AccountRepository>();

            Account mockAccount1 = new Account { Id = 1, Name = "Checking", Balance = 123.45, IsActive = true, CustomerId = 1 };
            Account mockAccount2 = new Account { Id = 2, Name = "Savings", Balance = 21981789.23, IsActive = true, CustomerId = 1 };
            Account mockAccount3 = new Account { Id = 3, Name = "TFSA", Balance = -12, IsActive = true, CustomerId = 1 };

            allAccounts = new List<Account> { mockAccount1, mockAccount2, mockAccount3 };

            // testing double
            mockRepo.Setup(repo => repo.GetAllAccounts()).Returns(allAccounts);

            // replace Delete
            mockRepo.Setup(repo => repo.DeleteAccount(It.Is<Account>(a => a.CustomerId == 1))).Callback(() => allAccounts.Remove(mockAccount1));


            mockRepo.Setup(repo => repo.GetAccountById(It.Is<int>(i => i == 1))).Returns(mockAccount1);


            mockRepo.Setup(repo => repo.GetAccountById(
                    It.Is<int>(i => i == 2)
                )).Returns(mockAccount2);
            mockRepo.Setup(repo => repo.GetAccountById(
                    It.Is<int>(i => i == 3)
                )).Returns(mockAccount3);

            accountBl = new AccountBusinessLogic(mockRepo.Object);
        }

        [TestMethod]
        public void GetBalanceTest()
        {
            Assert.AreEqual(accountBl.GetBalance(1), 123.45);
        }

        [TestMethod]
        public void GetAllBalancesReturnsSumOfActiveAccountBalances()
        {
            double actualSum = allAccounts.Sum(a => a.Balance);
            Assert.AreEqual(actualSum, accountBl.GetTotalBalanceOfCustomer(1));
        }
    }
}