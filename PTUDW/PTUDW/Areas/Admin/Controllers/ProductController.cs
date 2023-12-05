using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.DAO;
using MyClass.Model;
using PTUDW.Library;

namespace PTUDW.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        ProductsDAO productsDAO = new ProductsDAO();
        CategoriesDAO categoriesDAO = new CategoriesDAO();
        SuppliersDAO suppliersDAO = new SuppliersDAO();
        LinksDAO linksDAO = new LinksDAO();

        // GET: Admin/Product/INDEX
        public ActionResult Index()
        {
            return View(productsDAO.getList("Index"));
        }

        // GET: Admin/Product/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(products);
        }

        // GET: Admin/Product/Create
        public ActionResult Create()
        {
            //Dung de lua chon tu dan sach droplist nhu bang Categories : ParentID va Suppliers: ParentID
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//truy van bang Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");//truy van bang Suppliers
            return View();
        }

        // POST: Admin/Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Products products)
        {
            if (ModelState.IsValid)
            {
                //Xu ly thong tin tu dong cho 1 so truong
                //Xu ly tu dong: CreateAt
                products.CreateAt = DateTime.Now;
                //Xu ly tu dong: UpdateAt
                products.UpdateAt = DateTime.Now;
                //Xu ly tu dong: CreateBy:
                products.CreateBy = Convert.ToInt32(Session["UserID"]);
                //Xu ly tu dong: UpdateBy
                products.UpdateBy = Convert.ToInt32(Session["UserId"]);
                //Xu ly tu dong: Slug
                products.Slug = XString.Str_Slug(products.Name);

                //xu ly cho phan upload hình ảnh
                var img = Request.Files["img"];//lay thong tin file
                if (img.ContentLength != 0)
                {
                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;
                        //upload hinh
                        string PathDir = "~/Public/img/product/";
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //xu ly cho muc Topics
                if (productsDAO.Insert(products) == 1)//khi them du lieu thanh cong
                {
                    Links links = new Links();
                    links.Slug = products.Slug;
                    links.TableId = products.Id;
                    links.Type = "product";
                    linksDAO.Insert(links);                }

                //Thong bao thanh cong
                TempData["message"] = new XMessage("success", "Tạo mới sản phẩm thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//truy van bang Categoris
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");//truy van bang Suppliers
            return View(products);
        }
        ////////////////////////////////////////////////////////////
        // GET: Admin/Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatId - truy van tu bang Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");//sai SupplierID - truy van bang Suppliers
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Products products)
        {
            if (ModelState.IsValid)
            {
                //xu ly tu dong cho cac truong: Slug, CreateAt/By, UpdateAt/By, Oder
                //Xu ly tu dong: UpdateAt
                products.UpdateAt = DateTime.Now;
                //Xu ly tu dong: Slug
                products.Slug = XString.Str_Slug(products.Name);

                //xu ly cho phan upload hinh anh
                var img = Request.Files["img"];//lay thong tin file
                string PathDir = "~/Public/img/product";
                if (img.ContentLength != 0)
                {
                    //Xu ly cho muc xoa hinh anh
                    if (products.Img != null)
                    {
                        string DelPath = Path.Combine(Server.MapPath(PathDir), products.Img);
                        System.IO.File.Delete(DelPath);
                    }

                    string[] FileExtentions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
                    //kiem tra tap tin co hay khong
                    if (FileExtentions.Contains(img.FileName.Substring(img.FileName.LastIndexOf("."))))//lay phan mo rong cua tap tin
                    {
                        string slug = products.Slug;
                        //ten file = Slug + phan mo rong cua tap tin
                        string imgName = slug + img.FileName.Substring(img.FileName.LastIndexOf("."));
                        products.Img = imgName;
                        //upload hinh
                        string PathFile = Path.Combine(Server.MapPath(PathDir), imgName);
                        img.SaveAs(PathFile);
                    }
                }//ket thuc phan upload hinh anh

                //Cap nhat du lieu, sua them cho phan Links phuc vu cho Topics
                //Neu trung khop thong tin: Type = product va TableID = product.ID
                Links links = linksDAO.getRow(products.Id, "product");
                if (productsDAO.Update(products) == 1)
                {
                    //Cap nhat du lieu
                    links.Slug = products.Slug;
                    linksDAO.Update(links);
                }

                //thong bao tao mau tin thanh cong
                TempData["message"] = new XMessage("success", "Cập nhật sản phẩm thành công");                
                return RedirectToAction("Index");
            }
            ViewBag.ListCatID = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");//sai CatId - truy van tu bang Categories
            ViewBag.ListSupID = new SelectList(suppliersDAO.getList("Index"), "Id", "Name");//sai SupplierID - truy van bang Suppliers
            return View(products);
        }

        // GET: Admin/Product/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(products);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = productsDAO.getRow(id);
            //xu ly cho phan upload hình ảnh
            string PathDir = "~/Public/img/product/";
            if (productsDAO.Delete(products) == 1)
            {
                //Xu ly cho muc xoa hinh anh
                if (products.Img != null)
                {
                    string DelPath = Path.Combine(Server.MapPath(PathDir), products.Img);
                    System.IO.File.Delete(DelPath);
                }
                //tim thay mau tin thi xoa, cap nhat cho Links
                Links links = linksDAO.getRow(products.Id, "product");
                //Xoa luon cho Links
                linksDAO.Delete(links);
            }
            //Thong bao xoa mau tin thanh cong
            TempData["message"] = new XMessage("success", "Xóa sản phẩm thành công");
            return RedirectToAction("Trash");
        }

        //phat sinh them mot so action moi: Status, Trash, Deltrash, Recover
        //STATUS
        // GET: Admin/Category/Status/5
        public ActionResult Status(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            //Truy van Id
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 1 <- -> 2
                products.Status = (products.Status == 1) ? 2 : 1;

                //Cap nhat gia tri UpdateAt
                products.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                productsDAO.Update(products);

                //Thong bao cap nhat trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Cập nhật trạng thái thành công");

                return RedirectToAction("Index");
            }
        }
        //Deltrash
        // GET: Admin/Category/Deltrash/5
        public ActionResult DelTrash(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            //Truy van Id
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 1,2 -> 0 : không hiển thị ở Index
                products.Status = 0;

                //Cap nhat gia tri UpdateAt
                products.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                productsDAO.Update(products);

                //Thong bao cap nhat trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");

                return RedirectToAction("Index");
            }
        }

        //Trash
        // GET: Admin/Trash
        public ActionResult Trash()
        {
            return View(productsDAO.getList("Trash"));
        }

        //RECOVER       
        // GET: Admin/Category/Recover/5
        public ActionResult Recover(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            //Truy van Id
            Products products = productsDAO.getRow(id);
            if (products == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 0-->2 : Khong xuat ban
                products.Status = 2;

                //Cap nhat gia tri UpdateAt
                products.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                productsDAO.Update(products);

                //Thong bao mau tin trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Phục hồi mẫu tin thành công");

                return RedirectToAction("Index");
            }
        }
    }
}
