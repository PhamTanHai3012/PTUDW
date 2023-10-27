using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.Model;
using MyClass.DAO;
using PTUDW.Library;

namespace PTUDW.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        CategoriesDAO categoriesDAO = new CategoriesDAO();
        //INDEX
        // GET: Admin/Category
        public ActionResult Index()
        {
            return View(categoriesDAO.getList("Index"));
        }

        //DETAIL/////////////////////////////////////////////
        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại loại sản phẩm");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại loại sản phẩm");
                return RedirectToAction("Index");
            }
            return View(categories);
        }

        //CREATE///////////////////////////////////////////////
        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Categories categories)
        {
            if (ModelState.IsValid)
            {
                //Xu ly tu dong: CreateAt
                categories.CreateAt = DateTime.Now;
                //Xu ly tu dong: UpdateAt
                categories.UpdateAt = DateTime.Now;
                //Xu ly tu dong: ParentId
                if(categories.ParentID == null)
                {
                    categories.ParentID = 0;
                }
                //Xu ly tu dong: Order
                if(categories.Order == null)
                {
                    categories.Order = 1;
                }
                else
                {
                    categories.Order += 1;  
                }
                //Xu ly tu dong: Slug
                categories.Slug = XString.Str_Slug(categories.Name);

                //Chen them dong cho DB
                categoriesDAO.Insert(categories);
                //Thong bao cap nhat trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Tạo mới loại sản phẩm thành công");
                //Tro ve trang Index
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }

        //EDIT
        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Categories categories)
        {
            if (ModelState.IsValid)
            {
                //Xu ly tu dong: Slug
                categories.Slug = XString.Str_Slug(categories.Name);
                //Xu ly tu dong: ParentID
                if(categories.ParentID == null)
                {
                    categories.ParentID = 0;
                }
                //Xủ ly tu dong: Order
                if(categories.Order == null)
                {
                    categories.Order = 1;
                }
                else
                {
                    categories.Order += 1;
                }
                //Xu ly tu dong: UpdateAt
                categories.UpdateAt = DateTime.Now;
                
                //Cap nhat mau tin
                categoriesDAO.Update(categories);

                //Thong bao cap nhat trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Cập nhật mẫu tin thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListCat = new SelectList(categoriesDAO.getList("Index"), "Id", "Name");
            ViewBag.ListOrder = new SelectList(categoriesDAO.getList("Index"), "Order", "Name");
            return View(categories);
        }

        //DELETE
        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            Categories categories = categoriesDAO.getRow(id);
            if (categories == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Xóa mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            return View(categories);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Categories categories = categoriesDAO.getRow(id);
            categoriesDAO.Delete(categories);

            //Thong bao thanh cong
            TempData["message"] = TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");
            return RedirectToAction("Trash");
        }


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
            Categories categories = categoriesDAO.getRow(id);
            if (categoriesDAO == null) 
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 1 <- -> 2
                categories.Status = (categories.Status == 1) ? 2 : 1;

                //Cap nhat gia tri UpdateAt
                categories.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                categoriesDAO.Update(categories);

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
            Categories categories = categoriesDAO.getRow(id);
            if (categoriesDAO == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 1,2 -> 0 : không hiển thị ở Index
                categories.Status = 0;

                //Cap nhat gia tri UpdateAt
                categories.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                categoriesDAO.Update(categories);

                //Thong bao cap nhat trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");

                return RedirectToAction("Index");
            }
        }

        //Trash
        // GET: Admin/Trash
        public ActionResult Trash()
        {
            return View(categoriesDAO.getList("Trash"));
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
            Categories categories = categoriesDAO.getRow(id);
            if (categoriesDAO == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 0-->2 : Khong xuat ban
                categories.Status = 2;

                //Cap nhat gia tri UpdateAt
                categories.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                categoriesDAO.Update(categories);

                //Thong bao mau tin trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Phục hồi mẫu tin thành công");

                return RedirectToAction("Index");
            }
        }
    }
}
