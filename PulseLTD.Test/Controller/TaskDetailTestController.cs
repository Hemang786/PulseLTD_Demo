using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PulseLTD.Controllers;
using PulseLTD.Models;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PulseLTD.Test
{
    [TestClass]
    public class TaskDetailTestController
    {

        [TestMethod]
        public void SaveTask()
        {
            // Arrange
            TaskDetailController controller = new TaskDetailController();


            string rootPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            string filePath = Path.Combine(rootPath, "Image", "TestImage.jpg");

            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var file = new Mock<HttpPostedFileBase>();
            context.Setup(x => x.Request).Returns(request.Object);

            var _stream = new FileStream(filePath,FileMode.Open);
            // The required properties from my Controller side
            file.Setup(x => x.InputStream).Returns(_stream);
            file.Setup(x => x.ContentLength).Returns((int)_stream.Length);
            file.Setup(x => x.FileName).Returns(_stream.Name);

            controller.ControllerContext = new ControllerContext(
                                     context.Object, new RouteData(), controller);

            var model = new TaskDetail()
            {
                Title = "TestDemo9",
                ImageName = "TestImage"
            };
            // Act
            ActionResult result = controller.Create(model, file.Object);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DownloadZipFile()
        {
            // Arrange
            TaskDetailController controller = new TaskDetailController();

            // Act
            ActionResult result = controller.DownloadZipFile(string.Empty);

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
