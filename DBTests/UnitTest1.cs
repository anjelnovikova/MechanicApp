using DataBaseControllerLibrary;
using Microsoft.Data.SqlClient;

namespace DBTests
{
    [TestClass]
    public class DB_HandlerTests
    {
        private DB_Handler dbHandler;

        [TestInitialize]
        public void Setup()
        {
            dbHandler = new DB_Handler();
        }

        [TestMethod]
        public void ExecLogin_ValidLogin_ReturnsSuccess()
        {
            // Arrange
            string login = "login1";
            string password = "pass1";

            // Mock data for successful login
            var expectedUser = new User("Белов Александр Давидович", 1, "89999999999", "Менеджер");

            // Act
            var result = dbHandler.ExecLogin(login, password);

            // Assert
            Assert.AreEqual("Авторизация менеджера успешно пройдена!", result.Item1);
            Assert.AreEqual("Успешно", result.Item2);
            Assert.IsNotNull(result.Item3);
            Assert.AreEqual(expectedUser.FName, result.Item3.FName);
            Assert.AreEqual(expectedUser.SName, result.Item3.SName);
            Assert.AreEqual(expectedUser.TName, result.Item3.TName);
        }

        [TestMethod]
        public void ExecLogin_InvalidLogin_ReturnsError()
        {
            // Arrange
            string login = "invalidUser";
            string password = "invalidPass";

            // Act
            var result = dbHandler.ExecLogin(login, password);

            // Assert
            Assert.AreEqual("Неправильное имя пользователя или пароль!", result.Item1);
            Assert.AreEqual("Ошибка", result.Item2);
            Assert.IsNull(result.Item3);
        }

        [TestMethod]
        public void ExecuteReadQuery_ValidQuery_ReturnsResults()
        {
            // Arrange
            string sqlQuery = "SELECT * FROM Users";

            // Act
            var result = dbHandler.ExecuteReadQuery(sqlQuery);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [TestMethod]
        public void ExecuteReadQuery_InvalidQuery_ReturnsEmptyResults()
        {
            // Arrange
            string sqlQuery = "SELECT * FROM InvalidTable";

            // Act
            var result = dbHandler.ExecuteReadQuery(sqlQuery);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void ExecuteWriteQuery_InvalidQuery_ReturnsFalse()
        {
            // Arrange
            string sqlQuery = "INSERT INTO InvalidTable (invalidColumn) VALUES ('invalidValue')";
            SqlParameter[] parameters = new SqlParameter[] { };

            // Act
            var result = dbHandler.ExecuteWriteQuery(sqlQuery, parameters);

            // Assert
            Assert.IsFalse(result);
        }
    }
}