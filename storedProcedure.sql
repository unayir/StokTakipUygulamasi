--------------------------------------
---------Category Procedure-----------
Go
Create Procedure pr_Categories_List
as
Select * From Categories

--------------------------------------
Go
Create Procedure pr_Categories_Insert
(
	@CategoryID int,
	@CategoryName nvarchar(15),
	@Description nvarchar(MAX),
	@Picture image = null,
	@PrimaryColumn nvarchar
)as
If(@Picture is not null)
Begin
Insert Into Categories
Values
(@CategoryName, @Description, @Picture)
End
Else
Begin
Insert Into Categories
(CategoryName, Description)
Values
(@CategoryName, @Description)
End

----------------------------------------------
Go
Create Procedure pr_Categories_Update
(
	@CategoryID int,
	@CategoryName nvarchar(15),
	@Description nvarchar(MAX),
	@Picture image = null,
	@PrimaryColumn nvarchar
)as
If(@picture is not null)
Begin
Update Categories
Set CategoryName = @CategoryName,
	Description = @Description,
	Picture = @Picture
Where CategoryID = @CategoryID
End
Else
Begin
Update Categories
Set CategoryName = @CategoryName,
	Description = @Description
Where CategoryID = @CategoryID
End

-------------------------------------------
Go
Create Procedure pr_Categories_Delete
(
	@CategoryID int
)as
IF not exists (Select * From Products Where CategoryID = @CategoryID)
Begin
Delete From Categories
Where CategoryID = @CategoryID
End

--------------------------------------------
Go
Create Procedure pr_ProductsOfCategory_Delete
(
	@CategoryID int
)as
IF exists (Select * From Products Where CategoryID = @categoryID)
Begin
Delete From Products
Where CategoryID = @CategoryID
End
--------------------------------
------------Products------------
Go
Create Procedure pr_Products_List
as
Select * From Products

--------------------------------
Go
Create Proc pr_Products_Insert
(
	@ProductID int,
	@ProductName nvarchar(40),
	@CategoryID int,
	@QuantityPerUnit nvarchar(20),
	@UnitPrice money,
	@UnitsInStock int,
	@PrimaryColumn nvarchar
)as
Begin
Insert Into Products
(ProductName, CategoryID, QuantityPerUnit, UnitPrice, UnitsInStock)
Values
(@ProductName, @CategoryID, @QuantityPerUnit, @UnitPrice, @UnitsInStock)
End

---------------------------------
Go
Create Proc pr_Products_Update
(
	@ProductID int,
	@ProductName nvarchar(40),
	@CategoryID int,
	@QuantityPerUnit nvarchar(20),
	@UnitPrice money,
	@UnitsInStock int,
	@PrimaryColumn nvarchar
)as
Begin
Update Products
Set ProductName = @ProductName,
	CategoryID = @CategoryID,
	QuantityPerUnit = @QuantityPerUnit,
	UnitPrice = @UnitPrice,
	UnitsInStock = @UnitsInStock
Where ProductID = @ProductID
End
------------------------------------
Go
Create Proc pr_Products_Delete
(
	@ProductID int
)as
IF @ProductID is not null
Begin
Delete From Products
Where ProductID = @ProductID
End

--------------------------------------
Go
Create Procedure pr_ProductInCategory_List
(
	@CategoryID int 
)as
if(@CategoryID > 0)
Begin
Select P.ProductID, P.ProductName, C.CategoryName, P.QuantityPerUnit, P.UnitPrice, P.UnitsInStock From Products P join Categories C
on P.CategoryID = C.CategoryID 
Where P.CategoryID = @CategoryID
End

---------------------------------
Go
Create Procedure pr_Product_Search
(
	@CategoryID int,
	@text nvarchar(MAX)
)as
if(@CategoryID > 0)
Begin
Select P.ProductID, P.ProductName, C.CategoryName, P.QuantityPerUnit, P.UnitPrice, P.UnitsInStock From Products P join Categories C
on P.CategoryID = C.CategoryID 
Where P.CategoryID = @CategoryID And P.ProductName Like '%' + @text + '%'
End

---------------------------------

Go
Create Procedure pr_ProductStockDecrease
(
	@ProductID int,
	@ProductQuantity int
)as
Begin
Update Products
Set UnitsInStock = UnitsInStock - @ProductQuantity
Where ProductID = @ProductID
End
