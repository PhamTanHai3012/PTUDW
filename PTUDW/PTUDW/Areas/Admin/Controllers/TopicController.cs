using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MyClass.DAO;
using MyClass.Model;
using PTUDW.Library;

namespace PTUDW.Areas.Admin.Controllers
{
    public class TopicController : Controller
    {
        TopicsDAO topicsDAO = new TopicsDAO();
        LinksDAO linksDAO = new LinksDAO();
        //INDEX
        // GET: Admin/Category
        public ActionResult Index()
        {
            return View(topicsDAO.getList("Index"));
        }

        //DETAIL/////////////////////////////////////////////
        // GET: Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại chủ đề");
                return RedirectToAction("Index");
            }
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tồn tại chủ đề");
                return RedirectToAction("Index");
            }
            return View(topics);
        }

        //CREATE///////////////////////////////////////////////
        // GET: Admin/Category/Create
        public ActionResult Create()
        {
            ViewBag.ListTopic = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            ViewBag.OrderTopic = new SelectList(topicsDAO.getList("Index"), "Order", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Topics topics)
        {
            if (ModelState.IsValid)
            {
                //Xu ly tu dong: CreateAt
                topics.CreateAt = DateTime.Now;
                //Xu ly tu dong: UpdateAt
                topics.UpdateAt = DateTime.Now;
                //Xu ly cho muc CreateBy
                topics.CreateBy = Convert.ToInt32(Session["UserId"]);
                //Xu ly tu dong: ParentId
                if (topics.ParentId == null)
                {
                    topics.ParentId = 0;
                }
                //Xu ly tu dong: Order
                if (topics.Order == null)
                {
                    topics.Order = 1;
                }
                else
                {
                    topics.Order += 1;
                }
                //Xu ly tu dong: Slug
                topics.Slug = XString.Str_Slug(topics.Name);

                //Chen them dong cho DB
                if (topicsDAO.Insert(topics) == 1)//khi them du lieu thanh cong
                {
                    Links links = new Links();
                    links.Slug = topics.Slug;
                    links.TableId = topics.Id;
                    links.Type = "topic";
                    linksDAO.Insert(links);
                }

                //Thong bao cap nhat trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Tạo mới chủ đề thành công");
                //Tro ve trang Index
                return RedirectToAction("Index");
            }
            ViewBag.ListTopic = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            ViewBag.OrderTopic = new SelectList(topicsDAO.getList("Index"), "Order", "Name");
            return View(topics);
        }
        //EDIT
        // GET: Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy chủ đề");
                return RedirectToAction("Index");
            }
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy chủ đề");
                return RedirectToAction("Index");
            }
            ViewBag.ListTopic = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            ViewBag.OrderTopic = new SelectList(topicsDAO.getList("Index"), "Order", "Name");
            return View(topics);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Topics topics)
        {
            if (ModelState.IsValid)
            {
                //Xu ly tu dong: Slug
                topics.Slug = XString.Str_Slug(topics.Name);
                //Xu ly tu dong: ParentID
                if (topics.ParentId == null)
                {
                    topics.ParentId = 0;
                }
                //Xủ ly tu dong: Order
                if (topics.Order == null)
                {
                    topics.Order = 1;
                }
                else
                {
                    topics.Order += 1;
                }
                //Xu ly tu dong: UpdateAt
                topics.UpdateAt = DateTime.Now;

                //Xy ly cho muc UpdateBy
                topics.UpdateBy = Convert.ToInt32(Session["UserId"]);   

                //Cap nhat du lieu, sua them cho phan Links phuc vu cho Topics
                //Neu trung khop thong tin: Type = topic va TableID = topics.ID
                Links links = linksDAO.getRow(topics.Id, "topic");
                if (topicsDAO.Update(topics) == 1)
                {
                    //Cap nhat du lieu
                    links.Slug = topics.Slug;
                    linksDAO.Update(links);
                }
                //Thong bao cap nhat trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Cập nhật chủ đề thành công");
                return RedirectToAction("Index");
            }
            ViewBag.ListTopic = new SelectList(topicsDAO.getList("Index"), "Id", "Name");
            ViewBag.OrderTopic = new SelectList(topicsDAO.getList("Index"), "Order", "Name");
            return View(topics);
        }

        //DELETE
        // GET: Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Xóa chủ đề thất bại");
                return RedirectToAction("Index");
            }
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Xóa chủ để thất bại");
                return RedirectToAction("Index");
            }
            return View(topics);
        }

        // POST: Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Topics topics = topicsDAO.getRow(id);
            //tim thay mau tin thi xoa, cap nhat cho Links
            if (topicsDAO.Delete(topics) == 1)
            {
                Links links = linksDAO.getRow(topics.Id, "topic");
                //Xoa luon cho Links
                linksDAO.Delete(links);
            }

            //Thong bao thanh cong
            TempData["message"] = TempData["message"] = new XMessage("success", "Xóa chủ đề thành công");
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
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Cập nhật trạng thái thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 1 <- -> 2
                topics.Status = (topics.Status == 1) ? 2 : 1;

                //cap nhat gia tri cho UpdateAt/By
                topics.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
                topics.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                topicsDAO.Update(topics);

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
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Không tìm thấy mẫu tin");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 1,2 -> 0 : không hiển thị ở Index
                topics.Status = 0;

                //Cap nhat gia tri UpdateAt
                topics.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                topicsDAO.Update(topics);

                //Thong bao cap nhat trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Xóa mẫu tin thành công");

                return RedirectToAction("Index");
            }
        }

        //Trash
        // GET: Admin/Trash
        public ActionResult Trash()
        {
            return View(topicsDAO.getList("Trash"));
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
            Topics topics = topicsDAO.getRow(id);
            if (topics == null)
            {
                //Thong bao that bai
                TempData["message"] = new XMessage("danger", "Phục hồi mẫu tin thất bại");
                return RedirectToAction("Index");
            }
            else
            {
                //Chuyen doi trang thai cua Status 0-->2 : Khong xuat ban
                topics.Status = 2;

                //cap nhat gia tri cho UpdateAt/By
                topics.UpdateBy = Convert.ToInt32(Session["UserId"].ToString());
                topics.UpdateAt = DateTime.Now;

                //Cap nhat lai DB
                topicsDAO.Update(topics);

                //Thong bao mau tin trang thai thanh cong
                TempData["message"] = TempData["message"] = new XMessage("success", "Phục hồi mẫu tin thành công");

                return RedirectToAction("Index");
            }
        }
    }
}
