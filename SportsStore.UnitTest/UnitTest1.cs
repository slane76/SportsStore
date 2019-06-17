using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.HtmlHelpers;
namespace SportsStore.UnitTests
{
    [TestClass]
    public class AdminTests
    {
        [TestMethod]
        public void Can_Save_Valid_Changes()
        {
            // Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Arrange - create a product
            Product product = new Product { Name = "Test" };
            // Act - try to save the product
            ActionResult result = target.Edit(product);
            // Assert - check that the repository was called
            mock.Verify(m => m.SaveProduct(product));
            // Assert - check the method result type
            Assert.IsNotInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Arrange - create a product
            Product product = new Product { Name = "Test" };
            // Arrange - add an error to the model state
            target.ModelState.AddModelError("error", "error");
            // Act - try to save the product
            ActionResult result = target.Edit(product);
            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveProduct(It.IsAny<Product>()), Times.Never());
            // Assert - check the method result type
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }
        [TestMethod]
        public void Can_Edit_Product()
        {
            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
new Product {ProductID = 1, Name = "P1"},
new Product {ProductID = 2, Name = "P2"},
new Product {ProductID = 3, Name = "P3"},
});
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Act
            Product p1 = target.Edit(1).ViewData.Model as Product;
            Product p2 = target.Edit(2).ViewData.Model as Product;
            Product p3 = target.Edit(3).ViewData.Model as Product;
            // Assert
            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
            Assert.AreEqual(3, p3.ProductID);
        }
        [TestMethod]
        public void Cannot_Edit_Nonexistent_Product()
        {
            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
new Product {ProductID = 1, Name = "P1"},
new Product {ProductID = 2, Name = "P2"},
new Product {ProductID = 3, Name = "P3"},
});
            // Arrange - create the controller
            AdminController target = new AdminController(mock.Object);
            // Act
            Product result = (Product)target.Edit(4).ViewData.Model;
            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Index_Contains_All_Products()
        {
            // Arrange - create the mock repository
            Mock<IProductRepository> mock = new Mock<IProductRepository>();
            mock.Setup(m => m.Products).Returns(new Product[] {
                new Product {ProductID = 1, Name = "P1"},
new Product {ProductID = 2, Name = "P2"},
new Product {ProductID = 3, Name = "P3"},
});
            // Arrange - create a controller
            AdminController target = new AdminController(mock.Object);
            // Action
            Product[] result = ((IEnumerable<Product>)target.Index().
            ViewData.Model).ToArray();
            // Assert
            Assert.AreEqual(result.Length, 3);
            Assert.AreEqual("P1", result[0].Name);
            Assert.AreEqual("P2", result[1].Name);
            Assert.AreEqual("P3", result[2].Name);
        }
    }
}

public class UnitTest1
{


    [TestMethod]
    public void Can_Send_Pagination_View_Model()
    {
        // Arrange
        Mock<IProductRepository> mock = new Mock<IProductRepository>();
        mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1"},
            new Product {ProductID = 2, Name = "P2"},
            new Product {ProductID = 3, Name = "P3"},
            new Product {ProductID = 4, Name = "P4"},
            new Product {ProductID = 5, Name = "P5"}
            });
        // Arrange
        ProductController controller = new ProductController(mock.Object);
        controller.PageSize = 3;
        // Act
        ProductsListViewModel result = (ProductsListViewModel)controller.List(null, 2).Model;
        // Assert
        PagingInfo pageInfo = result.PagingInfo;
        Assert.AreEqual(pageInfo.CurrentPage, 2);
        Assert.AreEqual(pageInfo.ItemsPerPage, 3);
        Assert.AreEqual(pageInfo.TotalItems, 5);
        Assert.AreEqual(pageInfo.TotalPages, 2);
    }
    [TestMethod]
    public void Can_Paginate()
    {
        // Arrange
        Mock<IProductRepository> mock = new Mock<IProductRepository>();
        mock.Setup(m => m.Products).Returns(new Product[] {
            new Product {ProductID = 1, Name = "P1"},
            new Product {ProductID = 2, Name = "P2"},
            new Product {ProductID = 3, Name = "P3"},
            new Product {ProductID = 4, Name = "P4"},
            new Product {ProductID = 5, Name = "P5"}
            });
        // Arrange
        ProductController controller = new ProductController(mock.Object);
        controller.PageSize = 3;
        // Act
        ProductsListViewModel result
        = (ProductsListViewModel)controller.List(null, 2).Model;
        // Assert
        PagingInfo pageInfo = result.PagingInfo;
        Assert.AreEqual(pageInfo.CurrentPage, 2);
        Assert.AreEqual(pageInfo.ItemsPerPage, 3);
        Assert.AreEqual(pageInfo.TotalItems, 5);
        Assert.AreEqual(pageInfo.TotalPages, 2);
    }
    [TestMethod]
    public void Can_Generate_Page_Links()
    {
        // Arrange - define an HTML helper - we need to do this
        // in order to apply the extension method
        HtmlHelper myHelper = null;
        // Arrange - create PagingInfo data
        PagingInfo pagingInfo = new PagingInfo
        {
            CurrentPage = 2,
            TotalItems = 28,
            ItemsPerPage = 10
        };
        // Arrange - set up the delegate using a lambda expression
        Func<int, string> pageUrlDelegate = i => "Page" + i;
        // Act
        MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);

        // Assert
        Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
        + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
        + @"<a class=""btn btn-default"" href=""Page3"">3</a>",
        result.ToString());
    }

    [TestClass]
    public class CartTests
    {
        [TestMethod]
        public void Can_Add_New_Lines()
        {
            // Arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" };
            // Arrange - create a new cart
            Cart target = new Cart();
            // Act
            target.AddItem(p1, 1);
            target.AddItem(p2, 1);
            CartLine[] results = target.Lines.ToArray();
            // Assert
            Assert.AreEqual(results.Length, 2);
            Assert.AreEqual(results[0].Product, p1);
            Assert.AreEqual(results[1].Product, p2);
        }
    }

    [TestMethod]
    public void Can_Add_Quantity_For_Existing_Lines()
    {
        // Arrange - create some test products
        Product p1 = new Product { ProductID = 1, Name = "P1" };
        Product p2 = new Product { ProductID = 2, Name = "P2" };
        // Arrange - create a new cart
        Cart target = new Cart();
        // Act
        target.AddItem(p1, 1);
        target.AddItem(p2, 1);
        target.AddItem(p1, 10);
        CartLine[] results = target.Lines.OrderBy(c => c.Product.ProductID).ToArray();
        // Assert
        Assert.AreEqual(results.Length, 2);
        Assert.AreEqual(results[0].Quantity, 11);
        Assert.AreEqual(results[1].Quantity, 1);
    }

    [TestMethod]
    public void Can_Remove_Line()
    {
        // Arrange - create some test products
        Product p1 = new Product { ProductID = 1, Name = "P1" };
        Product p2 = new Product { ProductID = 2, Name = "P2" };
        Product p3 = new Product { ProductID = 3, Name = "P3" };
        // Arrange - create a new cart
        Cart target = new Cart();
        // Arrange - add some products to the cart
        target.AddItem(p1, 1);
        target.AddItem(p2, 3);
        target.AddItem(p3, 5);
        target.AddItem(p2, 1);
        // Act
        target.RemoveLine(p2);
        // Assert
        Assert.AreEqual(target.Lines.Where(c => c.Product == p2).Count(), 0);
        Assert.AreEqual(target.Lines.Count(), 2);
    }
}

